---
authors: kszicsillag,tibitoth
---

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

- felhasználói felület (kliens) készítése nem elvárás, de enélkül is tudni kell demonstrálni a működést, például Postman klienssel hívva a REST API-t. Az esetleges felületet, klienst nem értékeljük, pontozzuk
- A minimum elvárásokat teljesítő rendszer **24** pontot ér

!!! tip
    Külső (pl. Microsoft-os) demók, mintaalkalmazások (elemei) felhasználhatók, de ezt külön jelezni kell bemutatáskor. A nem jelzett, de átvett részletek plágiumnak számítanak. A demókból összefércelt egymáshoz nem kapcsolódó funkciókupacokat nem díjazzuk.
    
!!! tip
    Az órai demókban vagy mintaalkalmazásban megvalósított funkciók átvételéért pont nem adható, de azok tovább átdolgozhatóak saját implementációnak.

### Kötelező leadandók

A házi feladatot a moodle-ben publikált GitHub Classroom meghívóval generált repository-ban szükséges beadni a kis házi feladatokhoz hasonlóan. Viszont ezt szóban is meg kell védeni a laborvezetőnél előre egyeztetett időpontban a vizsgaidőszakban.

A beadott repository struktúrája:

- `src` mappa: forráskód (saját)
- `architecture.png`: "dokumentáció" leadása a házi feladattal együtt, min egy darab architektúra ábrából ([példa](https://learn.microsoft.com/en-us/samples/azure-samples/serverless-microservices-reference-architecture/serverless-microservices-reference-architecture/)) áll. Az ábrán látszanak a mikroszolgáltatások és a köztük lévő kapcsolatok.
    - Ha több platformra is elkészül a házi, akkor annyi architektúra ábrát kell készeíteni.
- `points.csv`: [CSV](https://en.wikipedia.org/wiki/Comma-separated_values) a megszerzendő jogcímekről és a pontszámukról és azok szummája. Elég a jogcímek rövidített neve. Példa:

    ```
    MIN;24
    IaC;10
    ...
    Sum;82
    ```
    
- `azure-learn.csv`: Opcionális: Ha [Azure tananyagot dolgoztál fel](./mslearning.md), [CSV](https://en.wikipedia.org/wiki/Comma-separated_values) az elvégzett képzési tervekről és a hosszukról **percben**. A végén külön sorban összegezd a perceket. Példa:

    ```
    Microsoft Azure Fundamentals: Describe Azure architecture and services;205
    AZ-400: Development for enterprise DevOps;405
    Összesen;610
    ```

## Platformválasztás vs. Azure költségek

A tárgy által oktatott, preferált architektúrák felhős költségei tipikusan magasabbak, mint amire a hallgatói Azure előfizetés kreditszámát (100$ / 1 év) méretezték. A költségeket lehet ugyan kontrollálni, de ez folyamatos odafigyelést igényel. Nem akartuk elvenni a lehetőséget és a motivációt a felhős szolgáltatások kipróbálásától, de a felhős alapplatformok választását a nagyházi keretében (különösen AKS) alapestben **nem ajánljuk!**

Ha mégis ezeket választanád, néhány tipp:

- Érdemes olyan platformokat választani, ahol könnyű elérni a 0 terhelés = kb. 0 költség állapotot. Az AKS-sel ezt nehéz, ACA és Azure Functions esetében könnyebb
- Figyeld a költségeket, de vedd figyelembe, hogy az elszámolás laggolhat (később jelentkeznek a költségek)
- Legnagyobb költség okozók: adatbázisok, futó virtuális gépek és node pool-ok, nagy mennyiségű adat mozgatása régiók között
- Próbáld egyszerűsíteni a felhős részt, két független telepítés esetén nem kell, hogy minden nem-üzleti funkció (pl. authentikáció) ugyanolyan komplex legyen a két telepítésben (lásd a pontrendszerben a két-független-platformos jogcímet)


!!! tip "Ajánlott viszont"
    - saját gépen futó (on-premise) K8S architektúra kiegészítése egy-egy Azure szolgáltatással, pl. konténerek letöltése Azure Container Registry-ből
    - IaC eszközökkel (pl. terraform) egy-egy parancssori paranccsal lebontani, majd szükség esetén visszaépíteni az architektúra költséges részeit


!!! danger "FAGYVESZÉLY!"
    Ha a hallgatói előfizetésen a kredit elfogy, az előfizetés befagyasztásra kerül. Vannak erőforrástípusok, melyeknél a leállítás csökkenti vagy megszünteti a költséget (pl. virtuális gép), ugyanakkor vannak, melyeket nem lehet költségsprólás miatt "kikapcsolni" (tipikusan a tárolást végző erőforrások, adatbázisok). A kredit elfogyása megakadályozhatja az Azure-os pontok megszerzését!

### Azure tananyagok elvégzésének kötségei

!!! warning
    Vannak olyan képzési tervek, amik saját előfizetésen elvégzendő, kreditbe kerülő műveleteket írnak elő. Ezek is csökkentik a más Azure-os pontok megszerzésére fordítható keretet, így érdemes az emiatt létrehozott erőforrásokat a lehető leghamarabb törölni.

!!! tip "Microsoft Learn Sandbox"
    Vannak olyan képzési tervek, ahol lehetőség van *Microsoft Learn Sandbox* használatára (pl. [ebben a modulban](https://learn.microsoft.com/en-us/training/modules/chain-azure-functions-data-using-bindings/3-explore-input-and-output-binding-types-portal-lab?pivots=javascript) - ez egy olyan Azure környezet, amihez nem kell előfizetés. A Microsoft Learn Sandbox-ról bővebben [itt](https://learn.microsoft.com/en-us/learn/support/faq?pivots=sandbox).

## Pontrendszer

Az elkészített rendszer egyes képességeire az alábbiak szerint pontok kaphatóak. A végső jegy az összpontszámból adódik.

| Jegy  | Pont   |
|-------|--------|
| 5     | 80-100 |
| 4     | 67-79  |
| 3     | 54-66  |
| 2     | 40-53  |
| 1     | 0-39   |

További szabályok:

 - ahol egy pontszám van feltüntetve, ott részpontszám alapesetben nem kapható. Tól-ig-es megadásnál, ahol alesetekre van bontva a jogcím, ott az alesetekre részpontszám nem kapható. Ahol csak maximum van feltüntetve, ott részpontszám kapható. 
 - egy jogcímen csak egyszer szerezhető pont, kivéve ahol ezt külön jelezzük

### Általános és cross-technológia

- **{PLAT2}** A szolgáltatás két különféle orkesztrációs/platformon fut, egymástól függetlenül, tehát a teljes szolgáltatás két egymástól független telepítéssel rendelkezik (pl. on-premise K8S és AKS). Nem kell, hogy minden nem-üzleti funkció (pl. authentikáció) ugyanolyan komplex legyen a két telepítésben, csak az üzleti funkciók képességei egyezzenek. Legalább az egyik platformnak Azure-ban kell futnia: **20** pont

- **{IaC}** IaC (terraform, Bicep - *ez Azure-only!*, stb.) eszközzel legalább az üzleti funkciókat futtató architektúrarész (K8S / AKS / Azure Function App platform) felépítése és lebontása. A visszaépítés végén az alkalmazásnak működnie kell, nem elég például egy üres AKS-t visszaépíteni. Védésen nem kell demózni (sokáig tartana), de platformonként egy felépítésről és egy lebontásról egy-egy kimeneti naplót be kell tudni mutatni. **7**-**10** pont

    - egyik platform telepítés (pl. Azure-os) felépítése-lebontása 7 pont
    - mindkét platform felépítése-lebontása 10 pont

- **{LANG2}** Több implementációs nyelv használata. A backend szolgáltatások legalább két különböző programozási nyelven készültek. (A frontend ebbe nem számít bele!): **5** pont

- **{GRPC}** gRPC alapú kommunikáció használata legalább egy mikroszolgáltatás esetében: **7** pont

- **{NOERR}** Hibatűrést növelő kommunikációs minták alkalmazása külső komponensek segítségével (pl. [Polly](https://www.pollydocs.org/), [Resilience4j](https://github.com/resilience4j/resilience4j), [Tenacity](https://github.com/jd/tenacity)). Saját minta implementációért nem jár pont. Ha az API gateway valósítja meg, szintén nem jár pont. **5** pont

- **{APIGW}** Saját telepítésű API gateway használata. Saját implementációért nem jár pont.: **5-10** pont

    - Traefik használata útvonalválasztásra: 5 pont
    - Más, saját telepítésű API gateway használata: 10 pont

- **{APIGW+AUTH}** A szolgáltatás authentikációjának kiszervezése API gateway-be  (csak az [itt felsorolt](https://gateway-api.sigs.k8s.io/implementations/) API gateway implementációk számítanak) forward authentikáció használatával: **7-12** pont

    - OAuth proxy használatával valamilyen elterjedt, külső vagy saját telepítésű OAuth IDP (pl. KeyCloak, Entra) felé továbbítva: 12 pont
    - Egyéb egyszerű saját dummy authentikációs szolgáltatás felé továbbítva: 7 pont

- **{ASYNCCOMM}** Aszinkron, üzenetsor alapú kommunikáció mikroszolgáltatások között saját telepítésű (pl. RabbitMQ konténer) üzenetsor, üzenetkezelő (messaging) szolgáltatással: **5-15** pont

    - Integrációs esemény eventualy consistency adatkezeléshez: 5 pont
    - Transactional Outbox pattern alkalmazása: 5 pont
    - Idempotens megvalósítás pl. deduplication elven: 5 pont

- **{SAGA}** Saga minta implementálása legalább egy folyamat esetén, hibakezeléssel és kompenzáció megvalósításával **15** pont

- **{EVENTSOURCING}** Event Sourcing minta alkalmazása **15 pont**

- **{CQRS}** és mediátor minta alkalmazása legalább egy szolgáltatás megvalósítása során. **5 pont**

- **{DDD}** tervezési elvek demonstrálása Event Storming alapú üzleti folyamattervezéssel. A pontot extra ábra(k) elkészítésével szükséges demonstrálni, amin az Event Storming során megtervezett folyamat kerül dokumentálásra. A védésen bemutatni szükséges a tervek hatását az architektúrára, implementációra: **5 pont**

- **{ACTOR}** Actor minta alkalmazása legalább egy állapottal rendelkező szolgáltatás esetében magas szintű keretrendszerek segítéségével pl.: Microsoft Orleans, AKKA.NET: **15 pont**

- **{CACHE}** Saját telepítésű (pl. Redis konténer) használata kifejezetten cache-elésre legalább egy művelet esetén: **5** pont

- **{HELM}** A szolgáltatás kubernetes-en belül futó része Helm chart-on keresztül telepíthető. Szükséges demonstrálni a rendszer frissítését a chart segítségével: **10** pont

- **{ACRBUILD}** Legalább egy saját konténer build-elése Azure Container Registry-ben (jelenleg hallgatói előfizetéssel nem teljesíthető): **7-12** pont

    - ad-hoc build saját gépről feltöltött context alapján: **7** pont
    - build valamilyen triggerre (pl. commit egy adott git ágra): **12** pont

- **{K8SJOB}** Kubernetes Job objektum használata, lefuttatása védéskor: **5** pont

- **{K8SCRONJOB}** Kubernetes CronJob objektum használata, korábbi lefutás demonstrálása védéskor: **5** pont

- **{K8SCMAP}** Kubernetes ConfigMap objektum használata valamely konfigurációs beállítás tárolására: **5** pont

- **{K8SSECRET}** Kubernetes Secret objektum használata titok tárolására: **5-7** pont
    
    - titok közvetlen feltöltése Secret objektumba: **5** pont
    - titkok leképezése saját Azure Key Vault-ból Akv2k8s-re építve: **7** pont

- **{OT}** OpenTelemetry alkalmazása különféle célokra: struktúrált naplózásra, metrikák monitorozásra, elosztott nyomkövetésre. Open Telemetry Collector komponens és valamilyen aggregátor felület használata kötelező a klaszteren belül (pl. Jaeger, Grafana, Azure Monitor), amin védéskor a naplókat, metrikákat, elosztott nyomkövetést be kell tudni mutatni: **5-20** pont

    - egyfajta célra **5** pont 
    - kétfajta célra **7** pont
    - mindhárom célra **10** pont
    - [exportálás *Azure Monitor*-ba](https://github.com/open-telemetry/opentelemetry-collector-contrib/blob/main/exporter/azuremonitorexporter/README.md) **+5** pont

- **{HSC}** Horizontális skálázás podok szintjén. Védésen a (vissza)skálázást demonstrálni kell **5-10** pont

    - Horizontal Pod AutoScaler alapú: **5** pont
    - [KEDA](https://keda.sh/) alapú, valamilyen adat, esemény alapján: **10** pont

- **{K8SNS}** Több példány (verzió) telepítése ugyanabba a környezetbe K8S namespace-ek vagy Azure Function [deployemnet slot](https://learn.microsoft.com/en-us/azure/azure-functions/functions-deployment-slots?tabs=azure-portal)-ok használatával. *Azure Container Apps* platform esetén külön *Container Apps* példány használható: **7** pont

- **{CICD}** CI/CD folyamat implementálása valamely elterjedt DevOps eszközre építve (GitHub Actions, Azure DevOps). Git push-ra a backend új verziója elkészül és kitelepül: **10-20** pont

    - egy platformra telepít: **10** pont 
    - két platformra telepít **15** pont

- **{CHAOS}** Chaos engineering eszköz alkalmazása (pl. [chaos mesh](https://chaos-mesh.org/docs/)). Védésen szemléltetés káosz teszt futtatással: **7** pont

### On-premise futó rendszerekhez

- **{OPDB2}** Legalább kétfajta on-premise adatbázis használata. Két eltérő technológiájú adatbázis használata perzisztenciára. Memória adatbázis, cache adatbázis (Redis) nem számít be: **10** pont

- **{OPACR}** Konténerek vagy helm chart(ok) letöltése on-premise klaszterbe saját Azure Container Registry-ből: **5-10** pont

    - anonim eléréssel: **5** pont
    - authentikációval pl. image pull secret-tel: **10** pont

- **{OPARC}** On-premise Kubernetes bekötése Azure Arc-ba: **10-20** pont

    - A szolgáltatás on-premise [Azure Function engine](https://learn.microsoft.com/en-us/azure/app-service/overview-arc-integration)-en fut (**preview!**) **+10** pont

- **{OPSTR}** Tartós tár, pl. lokális mappa csatolása klaszterbe. **5** pont

### Azure alapon futó rendszerekhez

- **{AZDB2}** Legalább kétfajta Azure-os adatbázisplatform használata (Azure SQL, Azure Database for PostgreSQL - Flexible Server, CosmosDB). Két eltérő technológiájú adatbázis használata perzisztenciára. Memória adatbázis, cache adatbázis (Azure Redis) nem számít be, egyéb NoSQL igen: **10** pont

!!! tip
    
    30 napig ingyenes (többször is aktiválható!) [Cosmos DB](https://cosmos.azure.com/try/)

- **{AZRED}** [Azure Redis](https://azure.microsoft.com/en-us/products/cache) szolgáltatás használata kifejezetten cache-elésre saját telepítésű cache helyett, legalább egy művelet esetén: **5** pont

- **{AZING}** Valamely [hivatalosan támogatott AKS ingress opció](https://learn.microsoft.com/en-us/azure/aks/concepts-network-ingress#compare-ingress-options) használata saját telepítésű ingress / api gateway helyett: **7** pont

- **{AZAPIMGW}** *Azure API Management* használata gateway-ként. A kliensről jövő kérés előbb az API Management gateway-be fut be, ami az Azure-os klaszterben futó ingress vagy [Azure Function-ök](https://azure.microsoft.com/en-us/blog/benefits-of-using-azure-api-management-with-microservices/) felé továbbít: **7** pont

- **{AZAPIMAUTH}** Authentikáció kiszervezése *Azure API Management* szolgáltatásba ([példa](https://learn.microsoft.com/en-us/azure/api-management/api-management-howto-protect-backend-with-aad)): **8** pont

- **{AZACR}** Konténerek vagy helm chart(ok) letöltése Azure-beli klaszterbe vagy *Azure Function*-be saját *Azure Container Registry*-ből: **3-7** pont

    - ACR admin felhasználó nevében: **3** pont
    - managed identity alapú hozzáféréssel: **7** pont

- **{AZKVAU}** Titkok lekérése saját *Azure Key Vault*-ból managed identity alapú hozzáféréssel: **7** pont

- **{AZMSG}** A mikroszolgáltatások közötti kommunikáció kiszervezése valamely [Azure üzenetkezelő szolgáltatásba](https://learn.microsoft.com/en-us/azure/service-bus-messaging/compare-messaging-services#comparison-of-services) (pl. Service Bus) managed identity alapú hozzáféréssel: **5** pont

- **{AZACA}** *Azure Container Apps* skálázása szabály alapján: **5** pont

- **{AZTEL}** Különféle telemetriák - naplók, metrikák, elosztott nyomkövetés bekötése egy Azure szolgáltatásokba (pl. *Azure Managed Grafana*, *Azure Monitor*, *Application Insights*). Védésen szemléltetés a telemetriának megfelelő vizualizációkon, vagy naplók esetén lekérdezésken, keresztül: **5-10** pont

    - Egyféle telemetria: **5** pont
    - Kétféle telemetria: **7** pont
    - Háromféle telemetria: **10** pont

- **{AZCS}** [*Azure Chaos Studio*](https://learn.microsoft.com/en-us/azure/chaos-studio/) használata káosz teszt futtatására: **7** pont

- **{AZSTR}** Tartós tár, például Azure Disk, Azure Files csatolása AKS, ACA klaszterbe vagy [Azure Function-be](https://learn.microsoft.com/en-us/azure/azure-functions/storage-considerations?tabs=azure-cli#mount-file-shares): **5** pont

- **{AZFDF}** [*Durable Functions*](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview) használata mikroszolgáltatások orkesztrációjára Azure Functions platformon: **5** pont

### Egyéb

- **{BASE}** A minimum elvárásokat teljesítő rendszer: **24** pont
- **{ALLHF}** Minden félévközi házi feladat (6 db.) teljesítése. Nem arányosítható - csak akkor adható, ha *minden* házi teljesített: **6** pont
- **{MSLEARN}** Azure tananyagok elsajátítása, kizárólag a [külön leírt követelmények](mslearning.md) szerint: max. **24** pont
- **{CONTRIB}** Visszacsatolás. A véglegesített pontrendszer vagy tananyag javítása, bővítése, módosítása pull request-tel. Helyesírási hiba is lehet, de az oktatók döntenek, hogy pontot ér-e a módosítás. Többször is megszerezhető. **0-2** pont, összesen max. **6** pont.

## Ponthatárok

## A szabályrendszer változása

Véglegesítés után is fenntartjuk a jogot

- a pontszerzési jogcímek halmazának bővítésére
- a pontszámok növelésére és egyéb hallgatóknak előnyös változtatásokra
- pontosításra, helyesírási hibák javítására
- egyéb változtatásra egyetemi szabályok változása miatt (pl. járványhelyzet miatt)

A pontrendszer, véglegesítés után is, általatok is módosítható/bővíthető. Ezt pull request formájában egy megfelelő indoklással nyújthatjátok be, de a PR benyújtása nem jelenti annak az automatikus elfogadását, arról minden esetben a tárgy oktatói döntenek.

A változásokat a [github history-ban](https://github.com/bmeviaumb11/skalazhato/commits/master/docs/homework/pontrendszer.md) követhetitek.
