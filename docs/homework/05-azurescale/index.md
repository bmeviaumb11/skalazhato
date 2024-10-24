# 05 - Skálázás Azure-ban

## Cél

A labor célja megismerni az AKS skálázási módjait, illetve Azure Load Testing szolgáltatással tesztelni is az automatikus skálázás működését.

## Előkövetelmények

A laborleírás cross-platform eszközöket használ. A labor linuxon (kubuntu v24.04) lett kidolgozva.

- Azure [hallgatói előfizetés](https://azure.microsoft.com/en-us/free/students)
- Azure [kubelogin és kubectl](https://azure.github.io/kubelogin/install.html)
    - felülírhatja a korábban telepített `kubectl` binárist 
- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) v2.61 vagy újabb
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet)
    - opcionálisan .NET 8 alap fejlesztő környezet (Visual Studio, Visual Studio Code, Rider), ha kell kódkiegészítés, pl. IntelliSense
- JRE v8 vagy későbbi  (pl. [Adoptium](https://adoptium.net/)), `java` parancs legyen a PATH-ban

## Előkészület

A feladatok megoldása során ne felejtsd el követni a feladat beadás folyamatát [GitHub](../../information/GitHub.md).

!!! danger "PR név"
    :exclamation: Beadásnál a pull request neve legyen: *hf5* :exclamation:

### Git repository létrehozása és letöltése

1. Moodle-ben keresd meg a laborhoz tartozó meghívó URL-jét és annak segítségével hozd létre a saját repository-dat.
2. Várd meg, míg elkészül a repository, majd checkout-old ki.
3. Hozz létre egy új ágat `megoldas` néven, és ezen az ágon dolgozz.
4. A `neptun.txt` fájlba írd bele a Neptun kódodat. A fájlban semmi más ne szerepeljen, csak egyetlen sorban a Neptun kód 6 karaktere.

!!! danger "NEPTUN"
    :exclamation: A feladatokban a `neptun` kifejezés helyett a saját neptunkódunkat helyettesítsük be minden esetben :exclamation:

## 0. Feladat

### AKS workload telepítése

Ha megvan az alkalmazásokkal feltöltött AKS, az AKS háziból, akkor nincs teendő. Egyébként az **AKS házi 1. feladata alapján** hozz létre és telepítsd a  mintaalkalmazást egy saját AKS-be.

## 1. Feladat

### 1.1 Container Insights bekapcsolása, replikaszám monitorozása

Kapcsoljuk be a Container Insights szolgáltatást az AKS-ünkön [az útmutató](https://learn.microsoft.com/en-us/azure/azure-monitor/containers/kubernetes-monitoring-enable?tabs=cli#existing-cluster-prometheus-container-insights-and-grafana) szerint. Prometheus és Grafana nem szükséges. A többi beállítás maradhat alapértelmezett. A bekapcsolás létrehoz egy *Log Analytics Workspace*-t (LAWS) és instrumentálja az AKS-t, hogy a monitorozási adatokat a LAWS-ba küldje.

!!! danger "Monitorozási költségek"
    Nagyobb klaszterek monitorozási költsége a keletkező monitor adatok mennyisége miatt ijesztő lehet, akár magának a klaszternek az alap működési költségével összemérhető. Hallgató előfizetésen az idő dimenzió is fontos, tehát, hogy csak rövid ideig keletkezzenek monitor adatok. Ezért is, amint lehet, a házi végén töröld az AKS-t! Kevésbé fontos, de a LAWS-t is érdemes, hogy tárolási költséget is csökkentsük. AKS törlésekor a LAWS **nem** törlődik automatikusan. 

Miután minden szükséges dolog települt, pár percben érdemes felfedezni a portál AKS monitorozással kapcsolatos főbb menüpontjait:

- Insights - előre elkészített összesítő nézetek
- Metrics - metrikákat figyelhetünk meg grafikonos nézetben
- Logs - saját lekérdezéseket készíthetünk és az eredményeket grafikonon is megjeleníthetjük. Lényegében minden monitorozott adathoz hozzáférünk és lekérdezéseket írhatunk rájuk. A lekérdezések elmenthetők, megoszthatók másokkal, akik hozzáférnek ehhez a felülethez.
- Workbooks - ha nem elég jók a beépített nézetek, kimutatások (pl. a Logs menüben összeraktunk egy jó saját lekérdezést), akkor saját dashboardokat, kimutatásokat készíthetünk

Készíts saját [KQL nyelvű](https://learn.microsoft.com/en-us/kusto/query) lekérdezést és abból grafikont (a *Results* alfül mellett a *Charts* alfülön) a *Logs* menüpontban, ami a *store-front* deployment (mintaalkalmazás része) replikaszámát mutatja az idő függvényében. Az idő dimenzió felbontása 2-5 perc közötti érték legyen. A [*KubePodInventory*](https://learn.microsoft.com/en-us/azure/azure-monitor/reference/tables/kubepodinventory) táblából érdemes kiindulni. Mentsd el a lekérdezést vagy ne navigálj el a menüpontból.

!!! warning "Névegyezés"
    Ha olyan K8S erőforrás nevére szűrsz, aminek a neve dinamikusan változik, akkor ne pontos egyezéssel szűrj, hanem részlegessel, pl. `startsWith`.

### 1.2 Manuális skálázás és HPA kipróbálása

Skálázd a store-front deploymentet HPA-val a [hivatalos útmutatót](https://learn.microsoft.com/en-us/azure/aks/tutorial-kubernetes-scale?tabs=azure-cli#autoscale-pods) követve. Várd meg amíg a hatás látszik a lekérdezés eredményében.

Az útmutató szerint a replikaszámnak vissza kellene állnia háromra, de ez az AKS-be választott VM CPU erősségétől függően nem biztos, hogy bekövetkezik. A podok terhelését a HPA az igényelt CPU-hoz viszonyítja (0,5 CPU az igényelt és 0,5 CPU-t használ = 100% terhelés). Ha rossz az igényelt CPU érték beállítás, akkor nem a várt viselkedést kaphatjuk. Ellenőrizd [a store-front által igényelt CPU-t](https://github.com/Azure-Samples/aks-store-demo/blob/abc38d094c09d421f6bb6ec6b900651992a7da14/aks-store-quickstart.yaml#L249) és ha szükséges, módosítsd, hogy bejövő kérés hiányában a minimum értékre álljon vissza.

!!! tip "HPA monitorozása"
    Egyszerűen monitorozhatjuk a HPA tevékenységét a `kubectl describe hpa $hpa_name` [paranccsal](https://kubernetes.io/docs/tasks/run-application/horizontal-pod-autoscale/#support-for-horizontalpodautoscaler-in-kubectl).


!!! example "BEADANDÓ"
    Készíts egy képernyőképet az Azure portálról (`f1.1.png`) és commitold azt be a házi feladat repó gyökerébe, amin látszik a replikaszámos vonalgrafikon (Chart Type: Line) és a különböző skálázások, illetve a HPA hatása, ahogy visszaáll a minimum értékre. A kép jobb felső sarkában látszódjon a belépett felhasználó, a bal felső sarka környékén az AKS neve.
    
    Készíts egy képernyőképet (`f1.2.png`) és commitold azt be a házi feladat repó gyökerébe, amin `kubectl describe hpa` paranccsal igazolod, hogy a HPA (és nem például manuális skálázás) csökkentette minimumra a replikaszámot.

## 2. Feladat

Terheléses tesztet írunk .NET-es tesztprojektként, mely felhős terhelésesteszt-motorokat (load test engine) használ a szükséges terhelés előállítására. 

### 2.1 HPA érzékenyítése

A k8s YAML fájlok átírásával és érvényesítésével (`kubectl apply`) állítsd át a HPA minimum replikaszám értékét 1-re, a store-front CPU igényét pedig egy kis értékre (pl. *10m*), de annyira ne legyen kicsi, hogy terhelés nélkül egynél több replika kelljen. A replikaszám grafikonon validáld, hogy a replikaszám terhelés nélkül beáll-e egyre.

### 2.2 Azure Load Testing

Hozz létre egy Azure Load Testing erőforrást (például az [útmutatót](https://learn.microsoft.com/en-us/azure/load-testing/quickstart-create-and-run-load-test?tabs=portal#create-an-azure-load-testing-resource) követve) az alábbi beállításokkal:

- Name: *lt* előtag + neptun kód (pl.*npt123* neptun kód esetén *ltnpt123*)
- Resource group: az alapértelmezett Azure erőforráscsoport (lásd AKS házi)
- Location: közös Azure régió (lásd AKS házi)

Tesztet nem kell létrehozni, csak az erőforrás legyen meg.

### 2.3 Service Principal

A tesztet programból fogjuk létrehozni és indítani, ehhez kell egy Entra technikai felhasználó, akit a programunk megszemélyesít és ennek a felhasználónak a nevében jön létre és fut le a teszt. Technikai felhasználó létrehozása több lépéses folyamat általában, de az alapesetre van egy egysoros kényelmi [Azure CLI parancs](https://learn.microsoft.com/en-us/cli/azure/azure-cli-sp-tutorial-1?tabs=bash#create-a-service-principal). Helyettesítsd be a neptun kódodat:

```bash
az ad sp create-for-rbac -n loadtest_automation_neptun
```

!!!warning "Service prinicipal neve"
    Bár lehetséges több technikai felhasználó ugyanazon névvel, a könnyebb kereshetőség miatt jobb, ha a nevek egyedibbek.

A válasz JSON-t mindenképp mentsd el, szükség lesz az adatokra belőle. A benne lévő titok utólag már nem kérdezhető le.

!!!tip "Service prinicipal parancsok"
    Az `az ad sp` [parancsokkal](https://learn.microsoft.com/en-us/cli/azure/ad/sp) a technikai felhasználókhoz kapcsolódó legtöbb gyakori tevékenység elvégezhető. Például az általunk létrehozott technikai felhasználók listázása: `az ad sp list --show-mine`.

!!!warning "Entra felületek BME tenantban"
    A BME tenantban az Azure portálos Entra kezelőfelületek le vannak tiltva, kizárólag API-t hívva, például Azure CLI-ből, kezelhetjük a technikai felhasználóinkat. 

Az Azure RBAC felületek viszont nincsenek tiltva, az [útmutató](https://learn.microsoft.com/en-us/azure/role-based-access-control/role-assignments-portal) alapján osszunk jogokat a technikai felhasználónak (két hozzárendelés): 

1. legyen *Reader* szerepköre (role) az alapértelmezett Azure erőforráscsoporton (scope), amiben a Load Testing szolgáltatást is létrehoztuk és 
2. legyen [*Load Test Contributor*](https://learn.microsoft.com/en-us/azure/load-testing/how-to-assign-roles#load-test-contributor) (role) a Load Testing erőforráson (scope)

!!!tip
    A technikai felhasználó az alanykeresőben a sima ember felhasználókkal egy kategóriában van, nem a managed identity-k között.

### 2.4 Terhelésteszt NUnit unit teszt projektként

Lépjünk be a klónozott kiinduló repó *azloadtest* könyvtárába, majd hozzunk létre egy [NUnit](https://nunit.org/) projektet a .NET CLI segítségével. Adjuk hozzá az Azure Load Test automatizálását megvalósító [.NET könyvtárat](https://github.com/abstracta/jmeter-dotnet-dsl) is.

```bash
dotnet new nunit
dotnet add package Abstracta.JmeterDsl.Azure --version 0.6
```

!!!tip "Abstracta.JmeterDsl"
    Az *Abstracta.JmeterDsl* könyvtárak lehetővé teszik, hogy az Azure Load Testing tesztjeinket [JMeter](https://learn.microsoft.com/en-us/azure/load-testing/how-to-create-and-run-load-test-with-jmeter-script?tabs=portal) XML szerkesztő eszközök helyett imperatív nyelven ([Java](https://abstracta.github.io/jmeter-java-dsl/) vagy [C#](https://abstracta.github.io/jmeter-dotnet-dsl/)), fluent jellegű API-val írjuk meg egy tesztprojektben. A csomag ebből a kód alapján egyrészt generálja a szükséges JMeter artifaktokat, majd ezekkel inicializálja a teszt végrehajtó motort (esetünkben Azure Load Test), elindítja a tesztet, végül begyűjti a lefutási statisztikákat, amikre assert feltételeket fogalmazhatunk meg.

!!!warning "Java"
    Az *Abstracta* könyvtár működéséhez szükséges Java futtatókörnyezet, lásd fent az *Előkövetelmények* részt.

Az alábbi minta alapján írd meg a saját teszteseted a generált *.cs* fájlba. A kommentek alapján cseréld le a különböző paramétereket. Ezeket kell megadni:

- mintaalkalmazás URL-je
- létrehozandó teszt neve, `store-front-neptun-test` a neptun kódot behelyettesítve
- előfizetés azonosítója (GUID)
- a korábban létrehozott Azure Load Testing szolgáltatás neve (*ltneptun*)

```csharp title="Load Test függvény minta"
//using-ok a .cs fájl tetejére
using Abstracta.JmeterDsl.Azure;
using static Abstracta.JmeterDsl.JmeterDsl;

[Test]
public void LoadTest()
{
    var stats = TestPlan(
        ThreadGroup(threads: 100, iterations: 100,
            HttpSampler(url: "http://48.48.48.48") // cseréld le: mintaalkalmazás főoldalának URL-je
        )
    ).RunIn(new AzureEngine(Environment.GetEnvironmentVariable("AZURE_CREDS"))
        .TestName("store-front-neptun-test") // cseréld le
        .SubscriptionId("b593cc31-e0b8-e0b8-e0b8-b593cc31") // cseréld le
        .ResourceGroupName("viaumb11") 
        .TestResourceName("ltneptun") //cseréld le: Azure Load Testing erőforrás neve
        .Engines(2) 
        .TestTimeout(TimeSpan.FromMinutes(30)));
    Assert.That(stats.Overall.SampleTimePercentile99, Is.LessThan(TimeSpan.FromSeconds(5)));
}
```

!!! warning "Load Testing erőforrás azonosítók"
    Fontos, hogy a kódban pontosan szerepeljenek a Load Testing erőforrás azonosító adatai (előfizetés, erőforráscsoport neve, erőforrás neve), mert ha nem találja meg ezek alapján az erőforrást, akkor megpróbálja létrehozni, de arra nem adtunk neki jogot.

Állítsd össze az `AZURE_CREDS` környezeti változó értékét a technikai felhasználó kapcsán elmentett JSON alapján: kettősponttal elválasztva a BME tenant azonosítója (*tenant*), a technikai felhasználó azonosítója (*appId*) és a titok (*password*). [Indítsd](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test) a tesztet:

```bash
dotnet test -e AZURE_CREDS="00000000-0000-0000-0000-000000000000:00000000-0000-0000-0000-000000000000:xyxyyxy.xyyxyxyx"
```

!!!danger "Titok vs. git"
    Az AZURE_CREDS értékét ne égesd be a kódba, ne kerüljön be git-be!

A tesztnek automatikusan létre kell jönnie az Azure Load Testing szolgáltatáson belül és el kell indulnia. A lefutást kövesd az Azure portálon: Load Testing erőforrás *Tests* menüpontja, ott a teszt, majd azon belül tesztlefutás (*Test runs* rész).

!!! example "BEADANDÓ"
    Készíts egy képernyőképet az Azure portálról (`f2.1.png`) és commitold azt be a házi feladat repó gyökerébe, amin látszik a replikaszámos vonalgrafikon (Chart Type: Line) és a HPA hatása, ahogy a terhelésteszt miatt megnöveli, majd visszacsökkenti a replikaszámot a minimum értékre. A kép jobb felső sarkában látszódjon a belépett felhasználó, a bal felső sarka környékén az AKS neve. Ha a hatás nem elég látványos, növeld a terhelést a `ThreadGroup` `threads` és `iterations` paramétereivel.

    Készíts egy képernyőképet (`f2.2.png`) és commitold azt be a házi feladat repó gyökerébe, amin `kubectl describe hpa` paranccsal szemlélteted a HPA skálázási eseményeit.

    Készíts egy képernyőképet (`f2.3.png`) és commitold azt be a házi feladat repó gyökerébe, amin az Azure portálon látszik a tesztlefutások (*Test Runs*) listája. A kép jobb felső sarkában látszódjon a belépett felhasználó, a bal felső sarkában a terhelés teszt neve.

!!!tip "Kitekintés - egyéb skálázási lehetőségek"
    Érdemes lett volna kipróbálni a  [klaszter skálázást](https://learn.microsoft.com/en-us/azure/aks/cluster-autoscaler?tabs=azure-cli), de a hallgatói előfizetés szigorú vCPU korlátai miatt ez nagyon nehézkes lett volna. Alternatívaként a [*virtual nodes* opciót](https://learn.microsoft.com/en-us/azure/aks/virtual-nodes) még így is használhatnánk, amivel a Azure Container Instances szolgáltatás virtuális K8S node-ként jelennének meg. Azonban ez a mód számos [dokumentált](https://learn.microsoft.com/en-us/azure/aks/virtual-nodes#limitations) és nem annyira jól dokumentált korlátozással rendelkezik, így a házi keretében szintén nehézkes lenne a használata, de egy kis példán [kipróbálható](https://learn.microsoft.com/en-us/azure/aks/virtual-nodes-portal).

## 3. Feladat - talán a legfontosabb

!!! danger "Erőforrások törlése"
    Beadás után törölj minden erőforráscsoportot az előfizetéseden belül. (Kivéve esetleg ami nem a kisházikhoz kellett.) A technikai felhasználót is érdemes [törölni](https://learn.microsoft.com/en-us/cli/azure/ad/sp?view=azure-cli-latest#az-ad-sp-delete) Entra-ból, ne hagyjunk "szemetet" magunk után.
