---
authors: tibitoth
---

# 03 - Kommunikációs megoldások

## Cél

A házi feladat célja az elosztott alkalmazások fejlesztése során felmerülő megoldások alapszintű gyakorlása.

## Előkövetelmények

* REST webszolgáltatások készítésének ismerete .NET platformon
    * A házi külön nem tér ki a REST szerű webszolgáltatások készítésének módszereire, arra a BSc szakirányos képzés [Adatvezérelt rendszerek](https://bmeviauac01.github.io/datadriven/hu/) és a [Szoftverfejlesztés .NET platformra](https://bmeviauav23.github.io/aspnetcorebook/) című választható tárgy anyagai az ajánlott irodalom.
* Docker Desktop
* [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (tesztelve: v8.0.414)
* Visual Studio 2022
    * tesztelve: v17.14.15
    * ASP.NET and web development workload
* Házi repóban található kiinduló projekt

## Kiinduló projekt

A kiinduló projekt egy .NET 8 alapú példa alkalmazás két mikroszolgáltatást tartalmaz:

* **Catalog**: egy egyszerű REST webszolgáltatás, ami termékeket listáz egy memóriában tárolt listából, de szándékosan véletlenszerűen 503-as hibakóddal tér vissza.
* **Order**: egy egyszerű REST webszolgáltatás, ami használja a Catalog szolgáltatást
    * A REST-es végpont elérését a hívó oldalon OpenAPI leíróból generált kliens oldali osztályokon keresztül érjük el. (lásd VS / Order projekt / Connected Services / Manage Connected Services / swagger Client).
    * Ez a kliens oldali kód `HttpClient` segítségével hívja meg a Catalog szolgáltatást.
* **AppHost**: .NET Aspire alapú AppHost projekt, ami megkönnyíti az elosztott alkalmazások futtatását fejlesztés során.
  Koncepciójában a Docker Compose-hoz hasonló, de YAML helyett C# nyelven írható, és nem csak .NET alkalmazásokat támogat.
    * biztosítja a komponensek megfigyelhetőségét OpenTelemetry segítségével (strukturált naplózás, elosztott nyomkövetés, metrikák)
    * és a Service Discovery-t konfigurációk kezelésén keresztül
    * Ezen felül széleskörű integrációt biztosít különböző külső komponensekkel, és egy dashboardot is kapunk a komponensek állapotának nyomon követésére.
    * Bővebben az Aspire-ről: <https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview>

## 0. Feladat - Előkészületek

1. Vedd fel a gyökérben lévő neptun.txt-be a NEPTUN azonosítódat.
2. Írd át az _AppHost_ projekt `Program.cs` fájljában a sztringekben található `NEPTUN` kifejezést a saját neptun kódodra.
3. Írd át az _Order_ projektben a `Program.cs` fájljában a sztringekben található `NEPTUN` kifejezést a saját neptun kódodra.
4. **Próbáljuk ki** az alkalmazásokat az _AppHost_ projektet futtatva.
    1. Megjelenik-e a .NET Aspire dashboard?
    2. Működnek-e a szolgáltatások külön külön a swagger felületükről?

## 1. Feladat - Hibatűrő kommunikáció

A hibatűrő kommunikáció kapcsán tárgyalt tervezési minták nem csak a kommunikáció implementációja során hasznosak, hanem bármilyen olyan komponens hívása során is, ami nem várt tranziens hibajelenséget produkálhat.
Tény, hogy leggyakrabban egy távoli hívás kommunikációja során történhet ilyen, így ott mindenképpen érdemes a hibatűrést valamilyen módon megvalósítani.

A laborfeladat során két ASP.NET Core mikroszolgáltatás közötti REST-es kommunikációt szeretnénk hibatűrőbbé tenni.
Ehhez .NET 8-ban már beépített lehetőségek vannak, amik a [Polly](https://github.com/App-vNext/Polly) osztálykönyvtárra támaszkodik, ami a leggyakoribb hibatűrő mintákat valósítja meg, nekünk csak felkonfigurálnunk kell.
Az egyszerűség kedvéért most a Retry mintát valósítsuk meg.

### Retry

Az Order szolgáltatásból hívjuk a Catalog szolgáltatást, hogy lekérjük a termékek listáját.
A Catalog szolgáltatás azonban (szándékosan) véletlenszerűen 503 HTTP hibakóddal tér vissza.
Célunk, hogy a hívásokat újrapróbálkozással kezeljük, és a hívó számára transzparens legyen az újrapróbálkozás.

**Próbáljuk ki** a jelenlegi működést az Order szolgáltatás swagger oldaláról, és figyeljük meg a hibát, amit a Catalog szolgáltatás ad vissza.

.NET-ben a HTTP kéréseket a `HttpClient` osztály segítségével végezzük. Az `IHttpClientFactory` segítségével tudjuk konfigurálni a `HttpClient`-ünket, ahol már az alap beállításokon felül hibatűrést megvalósító handlert is konfigurálni tudunk.

Az _Order_ szolgáltatás `Program` osztályában konfiguráljuk be az `HttpClient`-ünket a `AddResilienceHandler` függvény segítségével, és adjuk hozzá a retry konfigurációt az alábbi követelmények szerint:

* Az 503-as hibakód esetén próbálkozzon újra
* Maximum 5-ször próbálkozzon újra
* Újrapróbálkozás előtt várjon 1 másodpercet
* Többszöri újrapróbálkozási idő exponenciálisan növekvő időköz legyen

**Próbáljuk ki!** Tapasztalatunk szerint szinte megszűntek a hibák.

Ez a Policy gyakorlatilag beépül a `HttpClient` Handler Pipeline-jába, így a hívó számára transzparens lesz az újrapróbálkozási logika. Viszont érdemes odafigyelni a `HttpClient` `Timeout`jára is, mert az újrapróbálkozások során így az nem indul újra.

??? tip "Beépített alapértelmezett konfiguráció"
    .NET 8 óta kapunk egy akapértelmezett konfigurációt is a leggyakoribb esetekre, amit az `AddStandardResilienceHandler` metódussal tudunk hozzáadni a HttpClientFactory-hoz. Sőt ez már alapértelmezetten az Aspire segédprojektben be is volt regisztrálva, de a mai példában ezt szándékosan nem használtuk, hogy lássuk hogyan kellene ezt kézzel konfigurálni.

    ```csharp hl_lines="3-4" title="Skalazhato.HF3.ServiceDefaults/Extensions.cs"
    builder.Services.ConfigureHttpClientDefaults(http =>
    {
        // Turn on resilience by default
        // http.AddStandardResilienceHandler();

        // Turn on service discovery by default
        http.AddServiceDiscovery();
    });
    ```

    Bővebben: <https://devblogs.microsoft.com/dotnet/building-resilient-cloud-services-with-dotnet-8/>

!!! note "Polly általánosan"
    A Polly-t nem csak `HttpClient`-tel lehet használni, hanem tetszőleges kódban: össze lehet rakni egy Policy láncot, és abba beburkolni a kívánt függvényhívást. A Policy-ket akár karbantarthatjuk a DI konténerben is.

Most laboron nem nézünk példát több Policy használatára, de szóba jöhetne még a Timeout, a Circuit breaker, Cache vagy akár a Fallback policy [is](https://github.com/App-vNext/Polly/blob/master/README.md).
Az előadás anyagban találtok egy összetettebb szekvencia diagrammot, az ajánlott összetételről.

!!! example "BEADANDÓ"
    A feladathoz tartozó forráskódot commitold be és készíts egy `f1.png`-t, amiben látszik a .NET Aspire dashboardon a Trace fülön a retry policy hatása a hívásokra. A hívások között legyenek sikeres és sikertelen hívások is, és látszódjon, hogy a sikertelen hívások újrapróbálkozásokkal sikeresekké válnak.

## 2. Feladat - Aszinkron kommunikáció RabbitMQ-val

A feladat egy esemény alapú aszinkron kommunikáció megvalósítása.
Az _Order_ szolgáltatás fog publikálni egy `OrderCreated` integrációs eseményt, amit egy üzenetsorba rak.
Erre tetszőleges szolgáltatás feliratkozhat és reagálhat rá.
Esetünkben a _Catalog_ szolgáltatás fogja a megrendelt termék raktárkészletét csökkenteni.
Most az egyszerűség kedvéért ne foglalkozzunk az idempotens megvalósítással.

Az aszinkron kommunikációt RabbitMQ-n keresztül valósítjuk meg, és a MassTransit osztálykönyvtár magas szintű absztrakciójával fedjük el az alacsonyszintű kommunikációs részleteket.

### RabbitMQ beüzemelése

A .NET Aspire segítségével _AppHost_ projektbe vegyünk fel egy RabbitMQ erőforrást.

Ehhez az alábbi NuGet csomagot kell felvenni.

```xml
<PackageReference Include="Aspire.Hosting.RabbitMQ" Version="9.4.2" />
```

Vegyük fel a RabbitMQ-t az _AppHost_ projektbe, és a függőségeket erre az új erőforrásra.
   
```csharp hl_lines="1-2 5 9"
var messaging = builder.AddRabbitMQ("hf3-NEPTUN-messaging")
    .WithManagementPlugin();

var catalog = builder.AddProject<Projects.Skalazhato_HF3_Services_Catalog>("hf3-NEPTUN-catalog")
    .WithReference(messaging);

var order = builder.AddProject<Projects.Skalazhato_HF3_Services_Order>("hf3-NEPTUN-order")
    .WithReference(catalog)
    .WithReference(messaging);
```

Ez a megoldás a RabbitMQ-t egy Docker konténerben fogja futtatni.
A RabbitMQ-hoz kapunk így egy menedzsment felületet is, amit a `guest` userrel, és egy generált jelszóval tudunk elérni.
A jelszó megtalálható az Aspire Dashboardon a RabbitMQ erőforrás részletes nézeténél.

### Integrációs esemény

Létre kell hozzuk az eseményt reprezentáló üzenetet, ami lényegében a kommunikáció szerződése lesz.

1. Hozzunk létre egy új .NET 8 Class Library projektet `Skalazhato.HF3.Events` néven.

1. Ebbe vegyünk fel egy interfészt `IOrderCreatedEvent` néven az alábbi tartalommal, ami azt üzenetsorban lévő adatot reprezentálja.

    * Megrendelt termék azonosítója
    * Megrendelés ideje
    * Megrendelt mennyiség

1. Az _Order_ és _Catalog_ szolgáltatásokban vegyük fel a `Skalazhato.HF3.Events` projekt referenciáját, és implementáljuk az interfészt az _Order_ szolgáltatásban, egy új `IntegrationEvents` mappában.

Semmi bonyolultra nem kell eddig gondolni, ezek csak egyszerű DTO osztályok.

### MassTransit használata

Az üzeneteket a MassTransit könyvtár segítségével fogjuk elküldeni és fogadni.

#### Küldő oldal

Kezdjük a a küldő oldallal.

1. Vegyük fel az _Order_ szolgáltatásba a következő NuGet csomagot.

    ```xml
    <PackageReference Include="MassTransit.RabbitMQ" Version="8.5.2" />
    ```

2. Konfiguráljuk be a `Program` osztályban a MassTransit-ot, hogy RabbitMQ-t használjon.

    ```csharp
    builder.Services.AddMassTransit(x =>
    {
        x.UsingRabbitMq((ctx, config) =>
        {
            config.Host(ctx.GetRequiredService<IConfiguration>()
                .GetConnectionString("hf3-NEPTUN-messaging"));
        });
    });
    ```

3. Süssük el az eseményt a `TestController`-ben, egy újonnan létrehozott, HTTP POST-tal hívható `CreateOrder` actionben.

    * Az action-t nem fontos parametrizálni, dolgozhat beégetett értékekkel is.
    * A küldést a `IPublishEndpoint` objektum segítségével végezzük el. 
      MassTransit esetében a `Publish` süti el a broadcast szerű eseményeket, míg a `Send` inkább a command típusú üzenetekre van kihegyezve. A `Publish`-nak explicit adjuk meg típusparaméterként az `IOrderCreatedEvent` interfészt.
    * Naplózzuk **strukturáltan** az `ILogger` segítségével infó szinten, hogy melyik terméket rendeltük meg mekkora mennyiségben.
    * Az action végén adjunk vissza egy `Ok` státuszkódot, és egy üzenetet a válaszban, hogy sikeres volt a megrendelés.

#### Fogadó oldal

Térjünk át a fogadó oldalra.

1. A _Catalog_ szolgáltatás projektbe vegyük fel szintén az alábbi NuGet csomagot.

    ```xml
    <PackageReference Include="MassTransit.RabbitMQ" Version="8.5.2" />
    ```

2. Szükségünk lesz egy az eseményt lekezelő osztályra is, aminek a MassTransit-os `IConsumer<IOrderCreatedEvent>` interfészt kell megvalósítania.

    * Vegyünk fel a Catalog projektbe egy `IntegrationEventHandlers` mappát, majd abba hozzunk létre egy új osztályt `OrderCreatedEventHandler` néven.
    * Itt a kapott adatok alapján frissítsük az adatainkat: a mi Móricka példánkban a `ProductController`-ben lévő statikus listán dolgozunk.
      Nem kell most törődni az idempotens megvalósítással, azaz azzal, hogy többszöri üzenetküldés esetén ne legyen dupla módosítás.
    * Naplózzunk **strukturáltan** az `ILogger` segítségével infó szinten, hogy melyik termék raktárkészletét csökkentettük, és mennyi lett az új.

3. Konfiguráljuk be a `Program.cs`-ben a MassTransit-ot, hogy RabbitMQ-t használjon, és az `IOrderCreatedEvent` eseményünket melyik `IConsumer` megvalósítás kezelje le, illetve a figyelendő végpontokat is konfiguráljuk be alapértelmezett módon.

    ```csharp
    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<OrderCreatedEventHandler>();
        x.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(ctx.GetRequiredService<IConfiguration>()
                .GetConnectionString("hf3-NEPTUN-messaging"));
            cfg.ConfigureEndpoints(ctx);
        });
    });
    ```

4. **Próbáljuk ki!**

    * Kérjük le a termékeket
    * Süssünk el swaggerből a `POST /api/Test/CreateOrder` kérést
    * Nézzük meg, hogy frissült-e a termék raktárkészlete.

    !!! tip "Hibakeresés"
        Ha nem frissült, akkor a logokból vagy a rabbitmq menedzsment felületéről lehet nyomozni.

        * Ha azt tapasztaljuk hogy nem tud csatlakozni valamelyik szolgáltatás, akkor ellenőrizzük az Aspire AppHost projektet és a Connection Stringeket.
        * Ha azt tapasztaljuk, hogy `skipped` üzenetsorba kerülnek az üzenetek, akkor a küldő oldal rendben működött, de valamiért a fogadó oldal nem tudott a megadott üzenettípusra egyszer sem feliratkozni helyesen.

5. Kössük be az OpenTelemetry-t a MassTransit-ba, hogy a küldött és fogadott üzenetek is megjelenjenek a nyomkövetési adatok között. Az alábbi kódrészletet vegyük fel mindkét szolgáltatás `Program.cs` fájljába.

    ```csharp title="Skalazhato.HF3.Services.Order/Program.cs"
    builder.Services.AddOpenTelemetry()
        .WithMetrics(b => b.AddMeter(DiagnosticHeaders.DefaultListenerName))
        .WithTracing(providerBuilder =>
        {
            providerBuilder.AddSource(DiagnosticHeaders.DefaultListenerName);
        });
    ```

    ??? tip "Aspire community toolkit segédfüggvények"
        Az Aspire community toolkit tartalmaz segédfüggvényeket a MassTransit és RabbitMQ konfigurálására, ami kicsit egyszerűsíti a konfigurációt, és automatikusan beállítja az OpenTelemetry integrációt is. Bővebben: <https://github.com/CommunityToolkit/Aspire/tree/main/src/CommunityToolkit.Aspire.MassTransit.RabbitMQ>

!!! example "BEADANDÓ"
    A feladathoz tartozó forráskódot commitold be és készíts egy `f2.1.png`-t, amiben látszik a .NET Aspire dashboardon a _Structured_ fülön a küldő és fogadó oldali logbejegyzések.

    Illetve készíts egy `f2.2.png`-t, amiben látszik a Trace fülön az üzenetküldés és fogadás.

??? note "Kitekintés"

    1. A fenti példában nem törődtünk az idempotens megvalósítással, ez mindig külön tervezést igényel, az üzleti logikánk függvényében, de mindenképpen érdemes a tervezés során figyelni erre.

    1. Mi most broadcast jellegű integrációs eseményt sütöttünk el.
       Ne feledjünk van ennek egy másik variánsa is, amikor command szerű üzenetet küldünk egy másik szolgáltatásnak, és ott elvárjuk az esemény lefutását.
       Integrációs esemény során a fogadó félre van bízva, hogy mit kezd a kapott információval.

    1. A MassTransit egy nagyon sokoldalú eszköz, ami nem csak RabbitMQ-val használható, hanem más üzenetsorokkal is, mint például Azure Service Bus, vagy akár In-Memory üzenetsorral is tesztelhető. Illetve támogat több összetett kommunikációs mintát is, mint például a Saga, Outbox pattern, stb.

    1. Viszont a MassTransit a 9-es verziótól kezdve (2026Q1-től) már licenszköteles (fizetős) lesz, de a 8-as verzió még ingyenesen használható.
       A MassTransit helyett használható másik nyílt forráskódú eszközök is:
          - [Rebus](https://github.com/rebus-org/Rebus)
          - [NServiceBus](https://particular.net/nservicebus) (szintén licenszköteles)
          - [Brighter](https://github.com/BrighterCommand/Brighter)
          - [Wolverine](https://wolverinefx.net/)
          - [CAP](https://github.com/dotnetcore/CAP)
          - [EasyNetQ](https://github.com/EasyNetQ/EasyNetQ) (RabbitMQ specifikus)
          - stb.
