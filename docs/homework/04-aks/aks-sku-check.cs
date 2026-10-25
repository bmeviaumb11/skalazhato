// AKS system node pool VM size checker.
//
// For every region allowed by the subscription's "allowed locations" policy that offers AKS
// (Microsoft.ContainerService/managedClusters), lists the VM sizes that
//   - meet the AKS system node pool minimum that AKS enforces (2 vCPUs, 4 GB RAM; tested 2026-10,
//     the docs' stricter "4 vCPUs, no B-series" rule is not enforced),
//   - are offered to this subscription in the region (not restricted),
//   - fit into the remaining family and regional vCPU quota for the requested node count,
// sorted by Linux pay-as-you-go price.
//
// Requires the .NET 10 SDK and a logged-in Azure CLI (az login).
// Usage: dotnet run aks-sku-check.cs -- [options]
//   --subscription <id>     default: az account show
//   --regions a,b,c         skip the policy lookup and check these regions;
//                           "europe" = every physical region in Azure's "Europe" and "UK" geography groups
//                           (EU countries, Norway, Switzerland and the UK)
//   --nodes <n>             nodes in the pool, used for the quota check (default 1)
//   --min-vcpus <n>         default 2 (use 4 to follow the AKS docs for system node pools)
//   --max-vcpus <n>         default = min-vcpus
//   --min-memory <gb>       default 4
//   --burstable             include B-series sizes (AKS accepts them, but they are throttled under sustained load)
//   --currency <code>       price currency, e.g. EUR (default USD)
//   --top <n>               rows per region (default 10)
//   --show-all              also list sizes that are offered but lack quota
//   --timings               log the duration of every az call and price API request to stderr

#:property Nullable=enable

using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

var opt = Options.Parse(args);
Timing.Enabled = opt.Timings;
CultureInfo.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

string sub = opt.Subscription ?? (string)(await Az("account", "show"))["id"]!;
string[] regions = opt.Regions is [var only] && only.Equals("europe", StringComparison.OrdinalIgnoreCase)
    ? await EuropeanRegions(sub)
    : opt.Regions ?? await AllowedRegionsFromPolicy(sub);
Console.WriteLine($"Subscription: {sub}");
Console.WriteLine($"Regions:      {string.Join(", ", regions)}");
Console.WriteLine($"Criteria:     {opt.MinVcpus}-{opt.MaxVcpus} vCPU, >= {opt.MinMemory} GB, " +
                  $"{(opt.AllowBurstable ? "B-series allowed" : "no B-series")}, quota for {opt.Nodes} node(s)");

// AKS region availability from the resource provider. Locations are display names ("Poland Central").
var provider = await Az("provider", "show", "-n", "Microsoft.ContainerService", "--subscription", sub);
string registration = (string?)provider["registrationState"] ?? "?";
var aksRegions = provider["resourceTypes"]!.AsArray()
    .FirstOrDefault(t => (string?)t!["resourceType"] == "managedClusters")?["locations"]?.AsArray()
    .Select(l => ((string)l!).Replace(" ", "").ToLowerInvariant()).ToHashSet() ?? [];
Console.WriteLine($"AKS provider: Microsoft.ContainerService {registration}" +
                  (registration == "Registered" ? "" : " (register: az provider register -n Microsoft.ContainerService)"));

using var http = new HttpClient();
var armToken = new Lazy<Task<string>>(async () =>
    (string)(await Az("account", "get-access-token", "--subscription", sub, "--resource", "https://management.azure.com/"))["accessToken"]!);
var results = await Task.WhenAll(regions.Select(r => aksRegions.Contains(r.ToLowerInvariant())
    ? CheckRegion(r)
    : Task.FromResult(new RegionResult(r, 0, 0, [], "AKS (managedClusters) is not offered in this region"))));

foreach (var r in results)
{
    Console.WriteLine();
    if (r.Error != null) { Console.WriteLine($"=== {r.Region}: ERROR {r.Error}"); continue; }
    Console.WriteLine($"=== {r.Region}  (regional vCPUs: {r.CoresUsed}/{r.CoresLimit} used)");

    var rows = r.Candidates.Where(c => opt.ShowAll || c.Status == "OK")
        .OrderBy(c => c.Status != "OK").ThenBy(c => c.Price ?? double.MaxValue).ThenBy(c => c.Name)
        .Take(opt.Top).ToList();
    if (rows.Count == 0)
    {
        Console.WriteLine("  No usable size. " + (r.Candidates.Count > 0
            ? $"{r.Candidates.Count} matching size(s) are offered, but none fits the quota (use --show-all)."
            : "No matching size is offered to this subscription."));
        continue;
    }

    string cur = opt.Currency;
    Console.WriteLine($"  {"Size",-24} {"vCPU",4} {"GB",5} {"Arch",-6} {"Family quota",-28} {"Zones",-6} {cur + "/h",8} {cur + "/month",10} {"Retires",-10}  Status");
    foreach (var c in rows)
    {
        string price = c.Price is double p ? p.ToString("0.0000") : "?";
        string month = c.Price is double pm ? (pm * 730 * opt.Nodes).ToString("0.00") : "?";
        string quota = $"{c.Family} {c.FamilyFree}/{c.FamilyLimit}";
        Console.WriteLine($"  {c.Name,-24} {c.Vcpus,4} {c.MemoryGb,5:0.#} {c.Arch,-6} {quota,-28} {c.Zones,-6} {price,8} {month,10} {c.Retires ?? "-",-10}  {c.Status}");
    }
}
Console.WriteLine();
Console.WriteLine("Family quota = free/limit vCPUs. Zones = zones where the size can be deployed (\"-\" = region has no zones).");
Console.WriteLine("Prices: Linux pay-as-you-go retail price per node, month = 730 h x nodes.");
Console.WriteLine("Retires = announced retirement date of the size (the portal's \"End of Life\" group); avoid these.");
Timing.PrintSummary();

async Task<RegionResult> CheckRegion(string region)
{
    var regionClock = Stopwatch.StartNew();
    try
    {
        var usageTask = Az("vm", "list-usage", "-l", region, "--subscription", sub);
        // Calling the Resource SKUs API directly with a location filter takes seconds; "az vm list-skus -l"
        // takes ~2 minutes per region because it downloads the SKUs of every region (~280 MB) and filters locally.
        var skusTask = ArmGetAll($"/subscriptions/{sub}/providers/Microsoft.Compute/skus?api-version=2021-07-01" +
                                 $"&$filter={Uri.EscapeDataString($"location eq '{region}'")}", $"ARM skus {region}");

        var usage = (await usageTask).AsArray()
            .ToDictionary(u => (string)u!["name"]!["value"]!, u => (Used: Num(u!["currentValue"]), Limit: Num(u!["limit"])),
                          StringComparer.OrdinalIgnoreCase);
        var cores = usage.GetValueOrDefault("cores");
        long need(long vcpus) => vcpus * opt.Nodes;

        var candidates = new List<Candidate>();
        foreach (var sku in (await skusTask).Where(s => (string?)s!["resourceType"] == "virtualMachines"))
        {
            string name = (string)sku!["name"]!;
            string family = (string?)sku["family"] ?? "";
            var caps = sku["capabilities"]?.AsArray()
                .ToDictionary(c => (string)c!["name"]!, c => (string)c!["value"]!) ?? new Dictionary<string, string>();

            // vCPUsAvailable differs from vCPUs for constrained-core sizes (e.g. E4-2ds_v4); quota counts vCPUs.
            long vcpus = long.Parse(caps.GetValueOrDefault("vCPUs", "0"));
            long usable = long.Parse(caps.GetValueOrDefault("vCPUsAvailable", vcpus.ToString()));
            double mem = double.Parse(caps.GetValueOrDefault("MemoryGB", "0"));
            if (usable < opt.MinVcpus || usable > opt.MaxVcpus || mem < opt.MinMemory) continue;
            if (!opt.AllowBurstable && Regex.IsMatch(name, @"^Standard_B\d", RegexOptions.IgnoreCase)) continue;
            if (name.StartsWith("Basic_", StringComparison.OrdinalIgnoreCase)) continue;

            var restrictions = sku["restrictions"]?.AsArray() ?? [];
            if (restrictions.Any(x => (string?)x!["type"] == "Location")) continue;

            var zones = sku["locationInfo"]?[0]?["zones"]?.AsArray().Select(z => (string)z!).ToHashSet() ?? [];
            foreach (var zr in restrictions.Where(x => (string?)x!["type"] == "Zone"))
                foreach (var z in zr!["restrictionInfo"]?["zones"]?.AsArray() ?? [])
                    zones.Remove((string)z!);

            var fam = usage.GetValueOrDefault(family);
            long famFree = fam.Limit - fam.Used;
            string status = famFree < need(vcpus) ? "no family quota"
                          : cores.Limit - cores.Used < need(vcpus) ? "no regional quota"
                          : "OK";

            candidates.Add(new Candidate(name, family, usable, mem, caps.GetValueOrDefault("CpuArchitectureType", "?"),
                famFree, fam.Limit, zones.Count == 0 ? "-" : string.Join(",", zones.Order()), status, RetirementDate(caps)));
        }

        var shown = candidates.Where(c => opt.ShowAll || c.Status == "OK").Select(c => c.Name).ToList();
        var prices = await Prices(region, shown);
        candidates = candidates.Select(c => c with { Price = prices.TryGetValue(c.Name, out var p) ? p : null }).ToList();
        Timing.Log($"{region}: done in {regionClock.Elapsed.TotalSeconds:0.00}s ({shown.Count} sizes priced)");
        return new RegionResult(region, cores.Used, cores.Limit, candidates, null);
    }
    catch (Exception e)
    {
        // Restricted-access regions (e.g. germanynorth) reject the Compute APIs with a very long message.
        string error = e.Message.Contains("NoRegisteredProviderFound")
            ? "Compute APIs are not available in this region for this subscription (restricted-access region)"
            : e.Message.Trim().Split('\n')[0];
        return new RegionResult(region, 0, 0, [], error);
    }
}

async Task<Dictionary<string, double>> Prices(string region, IEnumerable<string> skus)
{
    var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
    foreach (var chunk in skus.Distinct().Chunk(15))
    {
        string filter = $"serviceName eq 'Virtual Machines' and priceType eq 'Consumption' and armRegionName eq '{region}' and (" +
                        string.Join(" or ", chunk.Select(s => $"armSkuName eq '{s}'")) + ")";
        string? url = $"https://prices.azure.com/api/retail/prices?currencyCode='{opt.Currency}'&$filter={Uri.EscapeDataString(filter)}";
        for (int pageNo = 1; url != null; pageNo++)
        {
            var page = await GetPricePage(url, $"{region} chunk of {chunk.Length} page {pageNo}");
            foreach (var item in page["Items"]!.AsArray())
            {
                string product = (string)item!["productName"]!, meter = (string)item["meterName"]!;
                if (product.Contains("Windows") || meter.Contains("Spot") || meter.Contains("Low Priority")) continue;
                string sku = (string)item["armSkuName"]!;
                double price = (double)item["retailPrice"]!;
                // Some regions list several Linux meters for one size; keep the cheapest.
                if (!result.TryGetValue(sku, out var old) || price < old) result[sku] = price;
            }
            url = (string?)page["NextPageLink"];
        }
    }
    return result;
}

// GET an ARM list endpoint with the Azure CLI's token, following nextLink.
async Task<List<JsonNode?>> ArmGetAll(string pathAndQuery, string label)
{
    string token = await armToken.Value;
    var items = new List<JsonNode?>();
    string? url = "https://management.azure.com" + pathAndQuery;
    for (int pageNo = 1; url != null; pageNo++)
    {
        var sw = Stopwatch.StartNew();
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new("Bearer", token);
        using var response = await http.SendAsync(request);
        string body = await response.Content.ReadAsStringAsync();
        Timing.Record("ARM request", sw.Elapsed, $"{label} page {pageNo}: HTTP {(int)response.StatusCode}, {body.Length / 1024} KB");
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException($"{label}: HTTP {(int)response.StatusCode} {body}");

        var page = JsonNode.Parse(body)!;
        items.AddRange(page["value"]!.AsArray());
        url = (string?)page["nextLink"];
    }
    return items;
}

async Task<JsonNode> GetPricePage(string url, string label)
{
    for (int attempt = 1; ; attempt++)
    {
        var sw = Stopwatch.StartNew();
        using var response = await http.GetAsync(url);
        string body = await response.Content.ReadAsStringAsync();
        var rateHeaders = response.Headers
            .Where(h => h.Key.Contains("ratelimit", StringComparison.OrdinalIgnoreCase) || h.Key == "Retry-After")
            .Select(h => $"{h.Key}={string.Join(",", h.Value)}");
        Timing.Record("price API request", sw.Elapsed,
            $"{label}: HTTP {(int)response.StatusCode}, {body.Length / 1024} KB {string.Join(" ", rateHeaders)}");

        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && attempt < 5)
        {
            // The retail prices API sends its own retry header (seconds) instead of the standard Retry-After.
            var wait = response.Headers.RetryAfter?.Delta
                ?? (response.Headers.TryGetValues("x-ms-ratelimit-retailPrices-retry-after", out var v) && int.TryParse(v.First(), out var secs)
                    ? TimeSpan.FromSeconds(secs) : TimeSpan.FromSeconds(10 * attempt));
            Timing.Record("price API 429 wait", wait, $"{label}: rate limited, waiting {wait.TotalSeconds:0.#}s");
            await Task.Delay(wait);
            continue;
        }
        response.EnsureSuccessStatusCode();
        return JsonNode.Parse(body)!;
    }
}

static async Task<string[]> AllowedRegionsFromPolicy(string sub)
{
    // Includes assignments inherited from management groups. If several assignments restrict regions,
    // a region must be allowed by all of them.
    var assignments = (await Az("policy", "assignment", "list", "--subscription", sub, "--disable-scope-strict-match")).AsArray();
    HashSet<string>? allowed = null;
    foreach (var a in assignments)
    {
        if ((string?)a!["enforcementMode"] == "DoNotEnforce") continue;
        var list = a["parameters"]?["listOfAllowedLocations"]?["value"]?.AsArray();
        if (list == null) continue;
        var set = list.Select(l => ((string)l!).ToLowerInvariant()).ToHashSet();
        Console.WriteLine($"Policy:       {a["displayName"]} ({a["name"]})");
        allowed = allowed == null ? set : allowed.Intersect(set).ToHashSet();
    }
    if (allowed == null)
        throw new InvalidOperationException("No policy assignment with a 'listOfAllowedLocations' parameter found; pass --regions.");
    return allowed.Where(r => r != "global").ToArray();
}

static async Task<string[]> EuropeanRegions(string sub)
{
    // "az account list-locations" has no --subscription option, so call the ARM endpoint directly.
    var locations = (await Az("rest", "--url", $"https://management.azure.com/subscriptions/{sub}/locations?api-version=2022-12-01"))["value"]!.AsArray();
    return locations
        .Where(l => (string?)l!["metadata"]?["regionType"] == "Physical" && (string?)l!["metadata"]?["geographyGroup"] is "Europe" or "UK")
        .Select(l => (string)l!["name"]!).Order().ToArray();
}

static async Task<JsonNode> Az(params string[] arguments)
{
    // Each az call is a Python process; limit how many run at once when many regions are checked.
    await Throttle.Az.WaitAsync();
    try { return await AzUnthrottled(arguments); }
    finally { Throttle.Az.Release(); }
}

static async Task<JsonNode> AzUnthrottled(string[] arguments)
{
    // On Windows az is a .cmd script, which Process can only start through cmd.exe.
    var psi = OperatingSystem.IsWindows() ? new ProcessStartInfo("cmd.exe") { ArgumentList = { "/c", "az" } } : new ProcessStartInfo("az");
    foreach (var a in arguments) psi.ArgumentList.Add(a);
    psi.ArgumentList.Add("-o");
    psi.ArgumentList.Add("json");
    psi.RedirectStandardOutput = psi.RedirectStandardError = true;

    // Label: the az subcommand plus the region, e.g. "az vm list-skus polandcentral".
    int l = Array.IndexOf(arguments, "-l");
    string label = "az " + string.Join(' ', arguments.TakeWhile(a => !a.StartsWith('-')));
    var sw = Stopwatch.StartNew();

    using var p = Process.Start(psi)!;
    var stdout = p.StandardOutput.ReadToEndAsync();
    var stderr = p.StandardError.ReadToEndAsync();
    await p.WaitForExitAsync();
    string output = await stdout;
    Timing.Record(label, sw.Elapsed, $"{label}{(l >= 0 ? " " + arguments[l + 1] : "")}: exit {p.ExitCode}, {output.Length / 1024} KB");

    if (p.ExitCode != 0) throw new InvalidOperationException($"az {string.Join(' ', arguments)} failed: {await stderr}");
    var parseClock = Stopwatch.StartNew();
    var json = JsonNode.Parse(output)!;
    Timing.Record("JSON parse", parseClock.Elapsed);
    return json;
}

// RetirementDateUtc is reported as M/d/yyyy for sizes whose retirement has been announced.
static string? RetirementDate(Dictionary<string, string> caps) =>
    caps.TryGetValue("RetirementDateUtc", out var d) && DateTime.TryParseExact(d, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
        ? date.ToString("yyyy-MM-dd") : caps.GetValueOrDefault("RetirementDateUtc");

static long Num(JsonNode? n) => n == null ? 0 : long.Parse(n.ToString());

record Candidate(string Name, string Family, long Vcpus, double MemoryGb, string Arch,
                 long FamilyFree, long FamilyLimit, string Zones, string Status, string? Retires, double? Price = null);

static class Throttle
{
    public static readonly SemaphoreSlim Az = new(6);
}

record RegionResult(string Region, long CoresUsed, long CoresLimit, List<Candidate> Candidates, string? Error);

record Options(string? Subscription, string[]? Regions, int Nodes, int MinVcpus, int MaxVcpus, double MinMemory,
               bool AllowBurstable, string Currency, int Top, bool ShowAll, bool Timings)
{
    public static Options Parse(string[] args)
    {
        var o = new Options(null, null, 1, 2, 0, 4, false, "USD", 10, false, false);
        for (int i = 0; i < args.Length; i++)
        {
            string Next() => i + 1 < args.Length ? args[++i] : throw new ArgumentException($"{args[i]} needs a value");
            o = args[i] switch
            {
                "--subscription" => o with { Subscription = Next() },
                "--regions" => o with { Regions = Next().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) },
                "--nodes" => o with { Nodes = int.Parse(Next()) },
                "--min-vcpus" => o with { MinVcpus = int.Parse(Next()) },
                "--max-vcpus" => o with { MaxVcpus = int.Parse(Next()) },
                "--min-memory" => o with { MinMemory = double.Parse(Next(), CultureInfo.InvariantCulture) },
                "--burstable" => o with { AllowBurstable = true },
                "--currency" => o with { Currency = Next().ToUpperInvariant() },
                "--top" => o with { Top = int.Parse(Next()) },
                "--show-all" => o with { ShowAll = true },
                "--timings" => o with { Timings = true },
                _ => throw new ArgumentException($"Unknown option: {args[i]}")
            };
        }
        return o.MaxVcpus == 0 ? o with { MaxVcpus = o.MinVcpus } : o;
    }
}

static class Timing
{
    public static bool Enabled;
    static readonly Stopwatch Clock = Stopwatch.StartNew();
    static readonly System.Collections.Concurrent.ConcurrentBag<(string Category, TimeSpan Duration)> Samples = [];

    public static void Log(string message)
    {
        if (Enabled) Console.Error.WriteLine($"[{Clock.Elapsed.TotalSeconds,7:0.00}s] {message}");
    }

    public static void Record(string category, TimeSpan duration, string? message = null)
    {
        Samples.Add((category, duration));
        if (message != null) Log($"{message} ({duration.TotalSeconds:0.00}s)");
    }

    // Durations overlap because regions run in parallel, so "sum" can exceed the wall clock time.
    public static void PrintSummary()
    {
        if (!Enabled) return;
        Console.Error.WriteLine();
        Console.Error.WriteLine($"Wall clock: {Clock.Elapsed.TotalSeconds:0.00}s");
        Console.Error.WriteLine($"  {"Category",-34} {"Count",5} {"Sum",8} {"Avg",7} {"Max",7}");
        foreach (var g in Samples.GroupBy(s => s.Category).OrderByDescending(g => g.Sum(s => s.Duration.TotalSeconds)))
        {
            var secs = g.Select(s => s.Duration.TotalSeconds).ToList();
            Console.Error.WriteLine($"  {g.Key,-34} {secs.Count,5} {secs.Sum(),7:0.00}s {secs.Average(),6:0.00}s {secs.Max(),6:0.00}s");
        }
    }
}
