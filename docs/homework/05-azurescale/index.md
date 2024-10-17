# 05 - Skálázás Azure-ban

## Cél

A labor célja megismerni az AKS skálázási módjait, illetve Azure Load Testing szolgáltatással tesztelni is az automatikus skálázás működését.

## Előkövetelmények

A laborleírás cross-platform eszközöket használ. A labor linuxon (kubuntu v24.04) lett kidolgozva.

- Azure [hallgatói előfizetés](https://azure.microsoft.com/en-us/free/students)
- Azure [kubelogin és kubectl](https://azure.github.io/kubelogin/install.html)
    - felülírhatja a korábban telepített `kubectl` binárist 
- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) v2.61 vagy újabb
- .NET 8 SDK
    - opcionálisan .NET 8 alap fejlesztő környezet (Visual Studio, Visual Studio Code, Rider), ha kell kódkiegészítés, pl. IntelliSense
- JRE v8 vagy későbbi, `java` parancs legyen a PATH-ban

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

Ha megvan az alkalmazásokkal feltöltött AKS, az AKS háziból, akkor nincs teendő. Egyébként az AKS házi alapján hozz létre és telepíts alkalmazásokat egy saját AKS-be.

## 1. Feladat

### 1.1 Container Insights bekapcsolása replikaszám monitorozása

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

Skálázd a store-front deploymentet manuálisan a [hivatalos útmutatót](https://learn.microsoft.com/en-us/azure/aks/tutorial-kubernetes-scale?tabs=azure-cli#manually-scale-pods)követve. Várd meg amíg a hatás látszik a lekérdezés eredményében.


Skálázd a store-front deploymentet HPA-val a [hivatalos útmutatót](https://learn.microsoft.com/en-us/azure/aks/tutorial-kubernetes-scale?tabs=azure-cli#autoscale-pods)követve. Várd meg amíg a hatás látszik a lekérdezés eredményében.

Az útmutató szerint a replikaszámnak vissza kellene állnia háromra, de ez az AKS-be választott VM CPU erősségétől függően nem biztos, hogy bekövetkezik. A podok terhelését a HPA az igényelt CPU-hoz viszonyítja (0,5 CPU az igényelt és 0,5 CPU-t használ = 100% terhelés). Ha rossz az igényelt CPU érték beállítás, akkor nem a várt viselkedést kaphatjuk. Ellenőrizd [a store-front által igényelt CPU-t](https://github.com/Azure-Samples/aks-store-demo/blob/abc38d094c09d421f6bb6ec6b900651992a7da14/aks-store-quickstart.yaml#L249) és ha szükséges, módosítsd, hogy bejövő kérés hiányában a minimum értékre álljon vissza.

!!! tip "HPA monitorozása"
    Egyszerűen monitorozhatjuk a HPA tevékenységét a `kubectl describe hpa $hpa_name` paranccsal.


!!! example "BEADANDÓ"
    Készíts egy képernyőképet az Azure portálról (`f1.2.1.png`) és commitold azt be a házi feladat repó gyökerébe, amin látszik a replikaszámos vonalgrafikon (Chart Type: Line) és a különböző skálázások, illetve a HPA hatása, ahogy visszaáll a minimum értékre. A kép jobb felső sarkában látszódjon a belépett felhasználó, a bal felső sarka környékén az AKS neve.
    
    Készíts egy képernyőképet (`f1.2.2.png`) és commitold azt be a házi feladat repó gyökerébe, amin igazolod, hogy a HPA csökkentette minimumra a replikaszámot (pl. `kubectl describe hpa` paranccsal).

## 2. Feladat

Terheléses tesztet írunk .NET-es tesztprojektként, mely felhős terhelésesteszt-motorokat (load test engine) használ szükséges terhelés előállítására. 

### 2.1 HPA érzékenyítése

A k8s YAML fájlok átírásával és érvényesítésével (`kubectl apply`) állítsd át a HPA minimum replikaszám értékét 1-re, a store-front CPU igényét pedig egy kis értékre (pl. *10m*), de annyira ne legyen kicsi, hogy terhelés nélkül egynél több replika kelljen. A replikaszám grafikonon validáld, hogy a replikaszám terhelés nélkül beáll-e egyre.


### 2.2 Azure Load Testing

Hozz létre egy Azure Load Testing erőforrást (például az [útmutatót](https://learn.microsoft.com/en-us/azure/load-testing/quickstart-create-and-run-load-test?tabs=portal#create-an-azure-load-testing-resource) követve) az alábbi beállításokkal:

- Name: *lt* előtag + neptun kód (pl.*npt123* neptun kód esetén *ltnpt123*)
- Resource group: az alapértelmezett Azure erőforráscsoport (lásd AKS házi)
- Location: közös Azure régió (lásd AKS házi)

Tesztet nem kell létrehozni, csak az erőforrás legyen meg.

### 2.3 Service Principal




### 2.4 Terhelésteszt NUnit unit teszt projektként

```bash
dotnet new nunit
dotnet add package Abstracta.JmeterDsl.Azure --version 0.6
```

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
                                            // AZURE_CREDS=tenantId:clientId:secretId
        .TestName("store-front-neptun-test") // cseréld le
        .SubscriptionId("b593cc31-e0b8-e0b8-e0b8-b593cc31") // cseréld le
        .ResourceGroupName("viaumb11") 
        .TestResourceName("ltneptun") //cseréld le: Azure Load Testing erőforrás neve
        .Engines(2) 
        .TestTimeout(TimeSpan.FromMinutes(20)));
    Assert.That(stats.Overall.SampleTimePercentile99, Is.LessThan(TimeSpan.FromSeconds(5)));
}
```

```bash
dotnet test -e AZURE_CREDS="00000000-0000-0000-0000-000000000000:00000000-0000-0000-0000-000000000000:xyxyyxy.xyyxyxyx"
```


!!! example "BEADANDÓ"
    Készíts egy képernyőképet (`f2.1.png`) és commitold azt be a házi feladat repó gyökerébe, ahol az alkalmazás futása látszik egy saját neptun kódot tartalmazó todoval. Látszódjon a weboldal címe is.

    Készíts egy másik képernyőképet (`f2.2.png`) és commitold azt be ezt is a házi feladat repó gyökerébe, ahol az Azure portálon látszik az AKS infrastruktúra erőforráscsoportjának (MC_ kezdetű) áttekintő nézete (*Overview*). Látszódjon a portálra belépett felhasználó azonosítója a jobb felső sarokban.
    
    Készíts egy másik képernyőképet (`f2.3.png`) és commitold azt be ezt is a házi feladat repó gyökerébe, ahol a végállapotban látszik parancssorban mindkét alkalmazás k8s *deployment* erőforrásai a lemezképek azonosítóival együtt (`kubectl get deployment -o wide` és `kubectl get deployment -n todoapp -o wide`)

## 3. Feladat - talán a legfontosabb

!!! danger "Erőforrások törlése"
    Beadás után törölj minden erőforráscsoportot az előfizetéseden belül. (Kivéve esetleg ami nem a kisházikhoz kellett.)