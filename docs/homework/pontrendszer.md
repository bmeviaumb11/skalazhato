# Pontrendszer
*Még nincs véglegesítve 2024. őszi félévre*

A házi feladat otthon, önállóan elkészítendő mikroszolgáltatások architektúrára épülő és konténertechnológiát használó szoftverrendszer elkészítése és működőképes állapotban való bemutatása.

## Minimum elvárások

- a rendszer kifelé egy jól körülhatárolható funkcióhalmazzal rendelkező (pl. könyvtári nyilvántartás) egységes szolgáltatást (backend) valósít meg,
- de belül több részre (mikroszolgáltatás) van darabolva. A mikroszolgáltatások külön-külön API-val rendelkeznek, mely hálózaton keresztül (pl. más mikroszolgáltatásokból) hívható.
- a szolgáltatás minden része valamely orkesztrációs vagy serverless platformon fut. Választható platformok: 

    - saját gépen futó (on-premise) Kubernetes (K8S)
    - saját gépen futó (on-premise) docker compose
    - Azure Kubernetes Services (AKS)
    - Azure Functions
    - Azure Container Apps (ACA)

- "dokumentáció" leadása a házi feladattal együtt, ami megcélzott platformonként egy darab architektúra ábrából ([példa](https://learn.microsoft.com/en-us/samples/azure-samples/serverless-microservices-reference-architecture/serverless-microservices-reference-architecture/)) áll. Az ábrán látszanak a mikroszolgáltatások és a köztük lévő kapcsolatok.
- felhasználói felület (kliens) készítése nem elvárás, de enélkül is tudni kell demonstrálni a működést, például Postman klienssel hívva a REST API-t. Az esetleges felületet, klienst nem értékeljük, pontozzuk.

!!! tip

    Külső (pl. Microsoft-os) demók, mintaalkalmazások (elemei) felhasználhatók, de ezt külön jelezni kell bemutatáskor. A nem jelzett, de átvett részletek plágiumnak számítanak. A demókból összefércelt egymáshoz nem kapcsolódó funkciókupacokat nem díjazzuk.
    
!!! tip

    Az órai demókban vagy mintaalkalmazásban megvalósított funkciók átvételéért pont nem adható, de azok tovább átdolgozhatóak saját implementációnak.

## Pontrendszer

Az elkészített rendszer egyes képességeire az alábbiak szerint pontok kaphatóak. A végső jegy az összpontszámból adódik.

|   |        |
|---|--------|
| 5 | 80-100 |
| 4 | 67-79  |
| 3 | 54-66  |
| 2 | 40-53  |
| 1 | 0-39   |

További szabályok:

 - ahol egy pontszám van feltüntetve, ott részpontszám alapesetben nem kapható. Tól-ig-es megadásnál, ahol alesetekre van bontva a jogcím, ott az alesetekre részpontszám nem kapható. Ahol csak maximum van feltüntetve, ott részpontszám kapható. 
 - egy jogcímen csak egyszer szerezhető pont, kivéve ahol ezt külön jelezzük

### Általános és cross-technológia

- A szolgáltatás két különféle orkesztrációs/platformon fut, egymástól függetlenül, tehát a teljes szolgáltatás két egymástól független telepítéssel rendelkezik (pl. on-premise K8S és AKS). Nem kell, hogy minden nem-üzleti funkció (pl. authentikáció) ugyanolyan komplex legyen a két telepítésben, csak az üzleti funkciók képességei egyezzennek. Legalább az egyik platformnak Azure-ban kell futnia: **X** pont

- Több implementációs nyelv használata. A backend szolgáltatások legalább két különböző programozási nyelven készültek. (A frontend ebbe nem számít bele!): **X** pont

- gRPC alapú kommunikáció használata legalább egy mikroszolgáltatás esetében: **X** pont

- Hibatűrést növelő kommunikációs minták alkalmazása külső komponensek segítségével (pl. [Polly](https://www.pollydocs.org/), [Resilience4j](https://github.com/resilience4j/resilience4j), [Tenacity](https://github.com/jd/tenacity)). Saját minta implementációért nem jár pont. Ha az API gateway valósítja meg, szintén nem jár pont. **X** pont

- Saját telepítésű API gateway használata: **X-Y** pont

    - Traefik használata útvonalválasztásra: X pont
    - Más, saját telepítésű API gateway használata: Z pont

- A szolgáltatás authentikációjának kiszervezése az API gateway-be forward authentikáció használatával: **X-Y** pont

    - OAuth proxy használatával valamilyen elterjedt, külső vagy saját telepítésű OAuth IDP (KeyCloak, Entra) felé továbbítva: X pont
    - Egyéb egyszerű saját dummy authentikációs szolgáltatás felé továbbítva: Z pont

- Üzenetsor alapú kommunikáció mikroszolgáltatások között saját telepítésű (pl. RabbitMQ konténer) üzenetsor, üzenetkezelő (messaging) szolgáltatással

- Saját telepítésű (pl. Redis konténer) használata kifejezetten cache-elésre legalább egy művelet esetén: **X** pont

- A szolgáltatás kubernetes-en belül futó része Helm chart-on keresztül telepíthető. Szükséges demonstrálni a rendszer frissítését a chart segítségével: **X** pont

- Legalább egy saját konténer build-elése Azure Container Registry-ben: **X-Y** pont

    - ad-hoc build saját gépről feltöltött context alapján: **X** pont
    - build valamilyen triggerre (pl. commit egy adott git ágra): **Y** pont

- Kubernetes Job objektum használata, lefuttatása védéskor: **X** pont

- Kubernetes CronJob objektum használata, korábbi lefutás demonstrálása védéskor: **X** pont

- Kubernetes ConfigMap objektum használata valamely konfigurációs beállítás tárolására: **X** pont

- Kubernetes Secret objektum használata titok tárolására: **X-Y** pont
    
    - titok közvetlen feltöltése Secret objektumba: **X** pont
    - titkok leképezése saját Azure Key Vault-ból Akv2k8s-re építve: **Y** pont

- OpenTelemetry alkalmazása különféle célokra: naplózásra, metrikák monitorozásra, elosztott nyomkövetésre. Open Telemetry Collector komponens és valamilyen aggregátor felület használata kötelező (pl. Jaeger, Grafana, Azure Monitor), amin védéskor a naplókat, metrikákat, elosztott nyomkövetést be kell tudni mutatni: **X-Z** pont

    - egyfajta célra **X** pont 
    - kétfajta célra **Y** pont
    - mindhárom célra **Z** pont
    - [exportálás *Azure Monitor*-ba](https://github.com/open-telemetry/opentelemetry-collector-contrib/blob/main/exporter/azuremonitorexporter/README.md) **W** pont

- Horizontális skálázás podok szintjén. Védésen a (vissza)skálázást demonstrálni kell **X-Y** pont

    - Horizontal Pod AutoScaler alapú: **X** pont 
    - [KEDA](https://keda.sh/) alapú, valamilyen adat, esemény alapján: **Y** pont

- Több példány (verzió) telepítése ugyanabba a környezetbe K8S namespace-ek vagy Azure Function [deployemnet slot](https://learn.microsoft.com/en-us/azure/azure-functions/functions-deployment-slots?tabs=azure-portal)-ok használatával. *Azure Container Apps* platform esetén külön *Container Apps* példány használható: **X** pont

- CI/CD folyamat implementálása valamely elterjedt DevOps eszközre építve (GitHub Actions, Azure DevOps). Git push-ra a backend új verziója elkészül és kitelepül: **X-Y** pont

    - egy platformra telepít: **X** pont 
    - két platformra telepít **Y** pont
  
- Saga minta implementálása legalább egy folyamat esetén **X** pont

- Chaos engineering eszköz alkalmazása (pl. [chaos mesh](https://chaos-mesh.org/docs/)). Védésen szemléltetés káosz teszt futtatással: **X** pont

- Event Sourcing, CQRS?

- Környezetkezelés?

### On-premise futó rendszerekhez

- Legalább két fajta on-premise adatbázis használata. Két eltérő technológiájú adatbázis használata perzisztenciára. Memória adatbázis, cache adatbázis (Redis) nem számít be: **X** pont

- Konténerek vagy helm chart(ok) letöltése on-premise klaszterbe saját Azure Container Registry-ből: **X-Y** pont

    - anonim eléréssel: **X** pont
    - image pull secret-tel: **Y** pont

- On-premise Kubernetes bekötése Azure Arc-ba: **X-Y** pont

    - A szolgáltatás on-premise [Azure Function engine](https://learn.microsoft.com/en-us/azure/app-service/overview-arc-integration)-en fut (**preview!**) **+Y** pont

- Tartós tár, pl. lokális mappa csatolása klaszterbe. **X** pont

### Azure alapon futó rendszerekhez

- Legalább két fajta Azure-os adatbázisplatform használata (Azure SQL, Azure Database for PostgreSQL - Flexible Server, CosmosDB). Két eltérő technológiájú adatbázis használata perzisztenciára. Memória adatbázis, cache adatbázis (Azure Redis) nem számít be, egyéb NoSQL igen: **X** pont

!!! tip
    
    30 napig ingyenes (többször is aktiválható!) [Cosmos DB](https://cosmos.azure.com/try/)

- [Azure Redis](https://azure.microsoft.com/en-us/products/cache) szolgáltatás használata kifejezetten cache-elésre saját telepítésű cache helyett, legalább egy művelet esetén: **X** pont

- Valamely [hivatalosan támogatott AKS ingress opció](https://learn.microsoft.com/en-us/azure/aks/concepts-network-ingress#compare-ingress-options) használata saját telepítésű ingress / api gateway helyett: **X** pont

- *Azure API Management* használata gateway-ként. A kliensről jövő kérés előbb az API Management gateway-be fut be, aki az Azure-os klaszterben futó ingress vagy [Azure Function-ök](https://azure.microsoft.com/en-us/blog/benefits-of-using-azure-api-management-with-microservices/) felé továbbít: **X** pont

- Authentikáció kiszervezése *Azure API Management* szolgáltatásba ([példa](https://learn.microsoft.com/en-us/azure/api-management/api-management-howto-protect-backend-with-aad)): **X** pont

- Konténerek vagy helm chart(ok) letöltése Azure-beli klaszterbe vagy *Azure Function*-be saját *Azure Container Registry*-ből: **X-Y** pont

    - ACR admin felhasználó nevében: **X** pont
    - managed identity alapú hozzáféréssel: **Y** pont

- Titkok lekérése saját *Azure Key Vault*-ból managed identity alapú hozzáféréssel: **X** pont

- A mikroszolgáltatások közötti kommunikáció kiszervezése valamely [Azure üzenetkezelő szolgáltatásba](https://learn.microsoft.com/en-us/azure/service-bus-messaging/compare-messaging-services#comparison-of-services) (pl. Service Bus) managed identity alapú hozzáféréssel: **X** pont

- *Azure Container Apps* skálázása szabály alapján: **X** pont

- Metrikák bekötése *Azure Managed Prometheus*-ba: **X** pont

- Naplók bekötése *Azure Application Insights*-ba: **X** pont

- Különféle telemetriák - naplók, metrikák, elosztott nyomkövetés bekötése *Azure Managed Grafana*-ba vagy *Azure Monitor (Application Insights-ba)*. Védésen szemléltetés a telemetriának megfelelő vizualizációkon, vagy naplók esetén lekérdezésken, keresztül: **X-Z** pont

    - Egyféle telemetria: **X** pont
    - Kétféle telemetria: **Y** pont
    - Háromféle telemetria: **Z** pont

- [*Azure Chaos Studio*](https://learn.microsoft.com/en-us/azure/chaos-studio/) használata káosz teszt futtatására: **X** pont

- Tartós tár, például Azure Disk, Azure Files csatolása AKS, ACA klaszterbe vagy [Azure Function-be](https://learn.microsoft.com/en-us/azure/azure-functions/storage-considerations?tabs=azure-cli#mount-file-shares): **X** pont

- [*Durable Functions*](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview) használata mikroszolgáltatások orkesztrációjára Azure Functions platformon: **X** pont

### Egyéb

- Minden félévközi házi feladat (6 db.) teljesítése. Nem arányosítható - csak akkor adható, ha *minden* házi teljesített: **6** pont

- Azure tananyagok elsajátítása, kizárólag a [külön leírt követelmények](mslearning.md) szerint: max. **X** pont

- Visszacsatolás. A véglegesített pontrendszer vagy tananyag javítása, bővítése, módosítása pull request-tel. Helyesírási hiba is lehet, de az oktatók döntenek, hogy pontot ér-e a módosítás. Többször is megszerezhető. **X-Y** pont, összesen max. **Z** pont

## Ponthatárok

## A szabályrendszer változása

Véglegesítés után is fenntartjuk a jogot

- a pontszerzési jogcímek halmazának bővítésére
- a pontszámok növelésére és egyéb hallgatóknak előnyös változtatásokra
- pontosításra, helyesírási hibák javítására
- egyéb változtatásra egyetemi szabályok változása miatt (pl. járványhelyzet miatt)

A pontrendszer, véglegesítés után is, általatok is módosítható/bővíthető. Ezt pull request formájában egy megfelelő indoklással nyújthatjátok be, de a PR benyújtása nem jelenti annak az automatikus elfogadását, arról minden esetben a tárgy oktatói döntenek.

A változásokat a [github history-ban](https://github.com/bmeviaumb11/skalazhato/commits/master/docs/homework/pontrendszer.md) követhetitek.
