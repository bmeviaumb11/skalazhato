---
authors: kszicsillag,tibitoth
---

# Nagy házi feladat
*Nincs véglegesítve 2026. őszi félévre*

<!-- *Véglegesítve 2025. őszi félévre 2025. november 14-én* -->

A házi feladat otthon, önállóan elkészítendő, mikroszolgáltatás-architektúrára épülő és konténertechnológiát használó szoftverrendszer elkészítése és működőképes állapotban való bemutatása.

A nagy házi feladat két részből áll:

1. a rendszer követelményspecifikációjának elkészítése és az architektúrájának megtervezése a szorgalmi időszakban. [Példaspecifikáció és architektúraábra](sample-spec.md).
2. a rendszer implementálása, amit szóban is meg kell védeni az utolsó vizsgáig.

## Minimum elvárások

- elfogadott specifikáció
- a rendszer kifelé egy jól körülhatárolható funkcióhalmazzal rendelkező (pl. könyvtári nyilvántartás) egységes szolgáltatást (backend) valósít meg.
    - **A specifikációban felsorolásszerűen meg kell határozni a szolgáltatás funkcióit** (pl. könyvek hozzáadása, kölcsönzés, visszavitel, felhasználók kezelése)
- az architektúra több komponensre (mikroszolgáltatás) van darabolva. A mikroszolgáltatások külön-külön API-val rendelkeznek, amelyek hálózaton keresztül (pl. más mikroszolgáltatásokból szinkron vagy aszinkron, API GW-en keresztül kliensekből) hívhatók.
    - **A specifikációban meg kell határozni a mikroszolgáltatásokat/komponenseket rövid magyarázattal, illetve azok technológiáit. Az architektúraábrán fel kell tüntetni a komponensek közötti főbb kapcsolatokat is.**
- a szolgáltatás minden része valamely orkesztrációs vagy serverless platformon fut. Választható platformok:

    - saját gépen futó (on-premise) Kubernetes (K8S)
    - saját gépen futó (on-premise) docker compose
    - Azure Kubernetes Service (AKS)
    - Azure Functions (AZF)
    - Azure Container Apps (ACA)

- felhasználói felület (kliens) készítése nem elvárás, de enélkül is tudni kell demonstrálni a működést, például Postman klienssel hívva a REST API-t. Az esetleges felületet, klienst nem értékeljük, nem pontozzuk.
- a minimum elvárásokat teljesítő rendszer **30** pontot ér.
- a minimum elvárásokat teljesítő rendszer szükséges, de nem elégséges feltétele a jegyzerzésnek - lásd alább a _Pontrendszer_ részt.

!!! tip
    Külső (pl. Microsoft-os) demók, mintaalkalmazások (elemei) felhasználhatók, de ezt külön jelezni kell bemutatáskor. A nem jelzett, de átvett részletek plágiumnak számítanak. A demókból összefércelt, egymáshoz nem kapcsolódó funkciókupacokat nem díjazzuk.

!!! tip
    Az órai demókban vagy mintaalkalmazásban megvalósított funkciók átvételéért pont nem adható, de azok tovább átdolgozhatók saját implementációnak.

### Kötelező leadandók

A házi feladatot a Moodle-ben publikált GitHub Classroom meghívóval generált repository-ban szükséges beadni. Mind a két mérföldkő (spec, implementáció) leadása azonos repository-ban történik két külön Pull Request formájában.

A beadott repository struktúrája:

- `src` mappa: forráskód (saját)
- `README.md`: specifikáció és esetleg egyéb dokumentáció markdown formátumban
- `architecture.png`: specifikáció részeként elkészített architektúraábra PNG formátumban. Az ábrának tartalmaznia kell a mikroszolgáltatásokat, azok főbb kapcsolatait, valamint az alkalmazott technológiákat.
    - Az implementáció végén aktualizálni kell az ábrát! Ha több platformra is elkészül a házi, akkor platformonként külön architektúraábrát kell készíteni.
    - Példaábra [itt](sample-spec.md) vagy [itt](https://learn.microsoft.com/en-us/azure/architecture/reference-architectures/containers/aks-microservices/aks-microservices#architecture)
- `points.csv`: [CSV](https://en.wikipedia.org/wiki/Comma-separated_values) a megszerzendő jogcímekről, a pontszámukról és azok összegéről. Elég a jogcímek rövidített neve. Példa:

    ```
    BASE;30
    IaC;10
    ...
    Sum;88
    ```

- `azure-learn.csv`: Opcionális: Ha [Azure tananyagot dolgoztál fel](./mslearning.md), [CSV](https://en.wikipedia.org/wiki/Comma-separated_values) az elvégzett képzési tervekről és a hosszukról **percben**. A végén külön sorban összegezd a perceket. Példa:

    ```
    Microsoft Azure Fundamentals: Describe Azure architecture and services;205
    AZ-400: Development for enterprise DevOps;405
    Összesen;610
    ```

## Platformválasztás vs. Azure költségek

A tárgy által oktatott, preferált architektúrák felhős költségei tipikusan magasabbak, mint amire a hallgatói Azure előfizetés kreditszámát (100$ / 1 év) méretezték. A költségeket lehet ugyan kontrollálni, de ez folyamatos odafigyelést igényel. Nem akartuk elvenni a lehetőséget és a motivációt a felhős szolgáltatások kipróbálásától, de a felhős alapplatformok választását a nagyházi keretében (különösen AKS) alapesetben **nem ajánljuk!**

Ha mégis ezeket választanád, néhány tipp:

- Érdemes olyan platformokat választani, ahol könnyű elérni a 0 terhelés = kb. 0 költség állapotot. ACA és Azure Functions esetében könnyebb, AKS-nél figyelni kell, hogy használaton kívül leálljon a klaszter.
- Figyeld a költségeket, de vedd figyelembe, hogy az elszámolás laggolhat (később jelentkeznek a költségek)
- Legnagyobb költségokozók: adatbázisok, futó virtuális gépek és node pool-ok, nagy mennyiségű adat mozgatása régiók között
- Próbáld egyszerűsíteni a felhős részt, két független telepítés esetén nem kell, hogy minden nem-üzleti funkció (pl. autentikáció) ugyanolyan komplex legyen a két telepítésben (lásd a pontrendszerben a két-független-platformos jogcímet)

!!! tip "Ajánlott viszont"
    - saját gépen futó (on-premise) K8S architektúra kiegészítése egy-egy Azure szolgáltatással, pl. konténerek letöltése Azure Container Registry-ből
    - IaC eszközökkel (pl. terraform) egy-egy parancssori paranccsal lebontani, majd szükség esetén visszaépíteni az architektúra költséges részeit

!!! danger "FAGYVESZÉLY!"
    Ha a hallgatói előfizetésen a kredit elfogy, az előfizetés befagyasztásra kerül. Vannak erőforrástípusok, amelyeknél a leállítás csökkenti vagy megszünteti a költséget (pl. virtuális gép), ugyanakkor vannak, amelyeket nem lehet költségspórolás miatt "kikapcsolni" (tipikusan a tárolást végző erőforrások, adatbázisok). A kredit elfogyása megakadályozhatja az Azure-os pontok megszerzését!

### Azure tananyagok elvégzésének költségei

!!! warning
    Vannak olyan képzési tervek, amik saját előfizetésen elvégzendő, kreditbe kerülő műveleteket írnak elő. Ezek is csökkentik a más Azure-os pontok megszerzésére fordítható keretet, így érdemes az emiatt létrehozott erőforrásokat a lehető leghamarabb törölni.

## Pontrendszer

Az elkészített rendszer egyes képességeire az alábbiak szerint pontok kaphatók. A végső jegy az összpontszámból adódik. A közepes szintet el nem érő megoldások nem kaphatnak megajánlott jegyet.

| Jegy  | Pont   |
|-------|--------|
| 5     | 80-100 |
| 4     | 67-79  |
| 3     | 54-66  |

További szabályok:

 - ahol egy pontszám van feltüntetve, ott részpontszám alapesetben nem kapható. Tól-ig-es megadásnál, ahol alesetekre van bontva a jogcím, ott az alesetekre részpontszám nem kapható. Ahol csak maximum van feltüntetve, ott részpontszám kapható.
 - egy jogcímen csak egyszer szerezhető pont, kivéve ahol ezt külön jelezzük

### Általános és cross-technológia

- **{PLAT2}** A szolgáltatás két különféle orkesztrációs platformon fut, egymástól függetlenül, tehát a teljes szolgáltatás két egymástól független telepítéssel rendelkezik (pl. on-premise K8S és AKS). Nem kell, hogy minden nem-üzleti funkció (pl. autentikáció) ugyanolyan komplex legyen a két telepítésben, csak az üzleti funkciók képességei egyezzenek. Legalább az egyik platformnak Azure-ban kell futnia: **12** pont

- **{IaC}** IaC (terraform, Bicep - *ez Azure-only!*, stb.) eszközzel legalább az üzleti funkciókat futtató architektúrarész (K8S / AKS / Azure Function App platform) felépítése és lebontása. A visszaépítés végén az alkalmazásnak működnie kell, nem elég például egy üres AKS-t visszaépíteni. Védésen nem kell demózni (sokáig tartana), de platformonként egy felépítésről és egy lebontásról egy-egy kimeneti naplót be kell tudni mutatni. Nem kell a teljes felépítés-lebontás folyamatot az IaC eszközzel megvalósítani, ki lehet egészíteni egyéb automatizációval, szkripttel. On-premise platform esetén csak akkor jár pont, ha maga a klaszter is IaC eszközzel épül fel és bomlik le (pl. kind/k3d klaszter Terraform/OpenTofu-val, k3s virtuális gépeken Vagrant/Ansible segítségével). Meglévő klaszterre (pl. Docker Desktop beépített K8S) alkalmazott K8S YAML leírók, Kustomize, Helm chartok vagy Docker Compose fájlok önmagukban nem számítanak IaC-nek ennél a jogcímnél: **10-12** pont

    - egyik platform telepítés (pl. Azure-os) felépítése-lebontása: **10** pont
    - mindkét platform felépítése-lebontása: **12** pont

- **{LANG2}** Több programozási nyelv használata. A backend szolgáltatások legalább két különböző programozási nyelven készültek. (A frontend ebbe nem számít bele!): **7** pont

- **{GRPC}** gRPC alapú kommunikáció használata legalább egy mikroszolgáltatás esetében: **7** pont

- **{NOERR}** Hibatűrést növelő kommunikációs minták alkalmazása külső komponensek segítségével (pl. [Polly](https://www.pollydocs.org/), [Resilience4j](https://github.com/resilience4j/resilience4j), [Tenacity](https://github.com/jd/tenacity)). A mintákat a használati esetnek megfelelően kell konfigurálni (pl. újrapróbálkozások száma, időtúllépés, circuit breaker küszöbértékei). Védésen hibainjektálással (pl. a hívott szolgáltatás leállításával, késleltetésével vagy hibás válaszokkal) be kell mutatni a minta működését, pl. a naplókban vagy az elosztott nyomkövetésben látható újrapróbálkozásokkal: **3** pont

    !!! danger
        Saját mintaimplementációért nem jár pont. Ha az (API/App) gateway valósítja meg, szintén nem jár pont. A keretrendszer vagy projektsablon által alapértelmezetten bekapcsolt, változtatás nélkül használt hibatűrésért (pl. a .NET Aspire ServiceDefaults `AddStandardResilienceHandler()` hívása) szintén nem jár pont.

- **{APIGW+AUTH}** A szolgáltatás autentikációjának kiszervezése (API/App) gateway-be: **5-15** pont

    - Egyszerű saját dummy autentikációs szolgáltatás felé továbbítva, forward autentikációval: **5** pont
    - Self-hosted vagy felhőben futó [Azure API Management gateway](https://learn.microsoft.com/en-us/azure/api-management/api-management-howto-protect-backend-with-aad) használatával, token validációs policy-vel (pl. `validate-jwt`, `validate-azure-ad-token`): a tokent a kliens szerzi be valamilyen elterjedt OAuth/OIDC IDP-től (pl. Entra, Keycloak), a gateway csak ellenőrzi: **7** pont
    - OAuth proxy (pl. oauth2-proxy) használatával, forward autentikációval: a bejelentkezési folyamatot (authorization code flow) a gateway kezeli, valamilyen elterjedt OAuth/OIDC IDP (pl. Entra, Keycloak) felé továbbítva: **10** pont
    - Az OAuth/OIDC IDP saját telepítésű (pl. Keycloak konténer), nem külső szolgáltatás (pl. Entra): **+5** pont

    !!! warning
        Az {AZAPIMGMT} jogcímhez szükséges policy nem lehet az autentikációt megvalósító policy.

- **{AZAPIMGMT}** *Azure API Management* használata. A kliensről jövő minden API kérés áthalad az API Management gateway-en mielőtt eljutna a klaszterbe vagy az [Azure Function-ökhöz](https://azure.microsoft.com/en-us/blog/benefits-of-using-azure-api-management-with-microservices/). Legalább egy [policy-t](https://learn.microsoft.com/en-us/azure/api-management/api-management-policies) érvényesít: **7-16** pont

    - Egy platformon: **7** pont
    - Két platformon, ebből az egyik on-premise (self-hosted mód): **+5** pont
    - Több API verzió támogatása API Management szolgáltatás segítségével: **+2** pont
    - Mock válaszok legalább egy API művelethez, API Management szolgáltatás segítségével: **+2** pont

    !!! warning
        A policy-követelményt nem teljesíti az {APIGW+AUTH} jogcímhez használt autentikációs policy (pl. `validate-jwt`).

- **{ASYNCCOMM}** Aszinkron, üzenetsor alapú kommunikáció mikroszolgáltatások között saját telepítésű (pl. RabbitMQ konténer) üzenetsor, üzenetkezelő (messaging) szolgáltatással: **5-13** pont

    - Integrációs esemény eventual consistency adatkezeléshez: **5** pont
    - Transactional Outbox pattern alkalmazása: **+5** pont
    - Idempotens megvalósítás pl. deduplikációs elven: **+3** pont

- **{SAGA}** Saga minta implementálása legalább egy folyamat esetén, hibakezeléssel és kompenzáció megvalósításával: **20** pont

- **{EVENTSOURCING}** Event Sourcing minta alkalmazása: **12** pont

- **{CQRS}** CQRS és mediátor minta alkalmazása legalább egy szolgáltatás megvalósítása során: **5** pont

- **{DDD}** DDD tervezési elvek demonstrálása Event Storming alapú üzleti folyamattervezéssel. A pontot extra ábra(k) elkészítésével szükséges demonstrálni, amin az Event Storming során megtervezett folyamat kerül dokumentálásra. A védésen bemutatni szükséges a tervek hatását az architektúrára, implementációra: **5** pont

- **{ACTOR}** Aktor minta alkalmazása legalább egy állapottal rendelkező szolgáltatás esetében magas szintű keretrendszerek segítségével pl.: Akka, Microsoft Orleans, Akka.NET: **20** pont

- **{CACHE}** Saját telepítésű cache (pl. Redis konténer) használata kifejezetten gyorsítótárazásra legalább egy művelet esetén: **3** pont

    !!! warning
        A {CACHE} és az {AZRED} közül csak az egyik számít be.

- **{HELM}** A szolgáltatás Kubernetesen belül futó része Helm chart-on keresztül telepíthető. Szükséges demonstrálni a rendszer frissítését a chart segítségével: **10** pont

- **{ACRBUILD}** Legalább egy saját konténer build-elése Azure Container Registry-ben: **2-3** pont

    - ad-hoc build saját gépről feltöltött context alapján: **2** pont
    - build valamilyen triggerre (pl. commit egy adott git ágra): **3** pont

    !!! warning
        Jelenleg hallgatói előfizetéssel nem szerezhető meg, mert a szükséges szolgáltatás ilyen típusú előfizetéseken nem vehető igénybe.

- **{K8SJOB}** Kubernetes Job objektum használata, lefuttatása védéskor. *Azure Container Apps* esetén manuálisan vagy eseményre indított [Container Apps Job](https://learn.microsoft.com/en-us/azure/container-apps/jobs) is elfogadott: **2** pont

- **{K8SCRONJOB}** Kubernetes CronJob objektum használata, korábbi lefutás demonstrálása védéskor. *Azure Container Apps* esetén ütemezett [Container Apps Job](https://learn.microsoft.com/en-us/azure/container-apps/jobs) is elfogadott: **2** pont

- **{K8SCMAP}** Kubernetes ConfigMap objektum használata valamely konfigurációs beállítás tárolására: **2** pont

- **{K8SSECRET}** Kubernetes Secret objektum használata titok tárolására. *Azure Container Apps* esetén [Container Apps secret](https://learn.microsoft.com/en-us/azure/container-apps/manage-secrets) is elfogadott. Az {AKV2K8S} által létrehozott Secret objektumokért nem jár külön pont: **2** pont

- **{AKV2K8S}** Azure Key Vault (AKV) titkok leképezése K8S Secret objektumokba vagy podok környezeti változóira [akv2k8s könyvtárral](https://akv2k8s.io/) vagy [Azure Key Vault provider for Secrets Store CSI Driver-rel](https://learn.microsoft.com/en-us/azure/aks/csi-secrets-store-driver) (AKVSSCSID): **7** pont

    !!! warning
        Az {AKV2K8S} által létrehozott Secret objektumokért a {K8SSECRET} jogcímen nem jár külön pont.

- **{OT}** OpenTelemetry alkalmazása különféle célokra: strukturált naplózásra, metrikák monitorozására, elosztott nyomkövetésre. [OpenTelemetry Collector](https://opentelemetry.io/docs/collector/) komponens és valamilyen aggregátor felület használata kötelező a klaszteren belül (pl. Jaeger, Grafana, Azure Monitor), amin védéskor a naplókat, metrikákat, elosztott nyomkövetést be kell tudni mutatni: **5-13** pont

    - egyfajta célra: **5** pont
    - kétfajta célra: **7** pont
    - mindhárom célra: **10** pont
    - [exportálás *Azure Monitor-ba*](https://github.com/open-telemetry/opentelemetry-collector-contrib/blob/main/exporter/azuremonitorexporter/README.md): **+3** pont
    
    !!! danger
        Platformonként csak egy, OpenTelemetry telemetriákat Azure Monitor-ba juttató módszerért jár pont: az {OT} jogcím Azure Monitor exportja, az {OTAZURE}, az {ACAMON} és az {AZFMON} közül egy platformon csak az egyik számít be.

    !!! warning "OpenTelemetry Collector vs Azure alapú környezetek"
        Az OpenTelemetry Collector nem támogatott AZF környezetben. AKS és ACA környezetekbe elvileg telepíthető, de ez most még jelentős munka ([a jövőben talán egyszerűbb lesz?](https://techcommunity.microsoft.com/blog/azureobservabilityblog/making-azure-the-best-place-to-observe-your-apps-with-opentelemetry/3995896)). Ajánlott módszer: saját gépen futó klaszterben OpenTelemetry Collector alapú megoldás, opcionálisan a monitoradatokat exportálva Azure Monitor-ba, míg Azure platformok esetében a specifikusan Azure-os jogcímek között leírt módszerek.

- **{HSC}** Horizontális skálázás podok szintjén. Védésen a (vissza)skálázást demonstrálni kell: **3-7** pont

    - Horizontal Pod Autoscaler alapú: **3** pont
    - [KEDA](https://keda.sh/) alapú, valamilyen adat, esemény alapján: **7** pont

- **{VSC}** Vertikális skálázás podok szintjén (pod requests rightsizing) Vertical Pod Autoscaler (VPA) alapon: **5** pont

    !!! warning "VPA és HPA együtt"
        A VPA és HPA együttes használata nagy körültekintést igényel, nem ajánlott, hogy ugyanazt a metrikát (vCPU, RAM) figyelje a két Autoscaler.

- **{K8SNS}** Több példány (verzió) telepítése ugyanabba a környezetbe: **3-5** pont

    - K8S / AKS esetén namespace-ek használatával: **5** pont
    - Azure Functions esetén [deployment slot](https://learn.microsoft.com/en-us/azure/azure-functions/functions-deployment-slots?tabs=azure-portal)-ok használatával (Premium vagy Dedicated csomagban; Flex Consumption csomagban nem érhető el): **3** pont
    - Azure Functions esetén külön *Function App* példány használatával (pl. Flex Consumption csomagban): **3** pont
    - *Azure Container Apps* esetén külön *Container Apps* példány használatával: **3** pont
    - *Azure Container Apps* esetén [több aktív revízió](https://learn.microsoft.com/en-us/azure/container-apps/revisions) (multiple revision mode) használatával, a forgalom revíziók közötti elosztásával vagy revíziócímkékkel: **3** pont

- **{CICD}** CI/CD folyamat implementálása valamely elterjedt DevOps eszközre építve (GitHub Actions, Azure DevOps). Git push-ra a backend új verziója felépül és kitelepül: **10-15** pont

    - egy platformra telepít: **10** pont
    - két platformra telepít: **15** pont

- **{CHAOS}** Chaos engineering eszköz alkalmazása (pl. [chaos mesh](https://chaos-mesh.org/docs/)). Védésen szemléltetés káoszteszt futtatással: **5** pont

    !!! warning
        Ugyanazon a klaszteren a {CHAOS} és az {AZCS} Chaos Mesh alapú hibája közül csak az egyik számít be.

- **{EXTAPI}** Külső online (SaaS) szolgáltatás (pl. Azure AI, Twitter, Facebook, Google Maps, Bing Maps, stb.) integrálása API-n keresztül, autentikációt végrehajtva: **3-7** pont

    - egyszerű REST-jellegű webes API, SDK használat nélkül, egyszerű API-kulcsalapú autentikáció: **3** pont
    - SDK-val / REST API-val, iparilag elfogadott, elterjedt autentikációs protokollt (pl. OIDC) végrehajtva: **7** pont

- **{INOUTPROC}** Konfigurálható, hogy az adott mikroszolgáltatás külön szolgáltatásként (külön processz / pod) települjön-e, vagy a hívóval együtt egy folyamatba (in-process); a hívás a konfigurációnak megfelelően vagy szolgáltatáshívás vagy közvetlen (függvény)hívás. Bemutatáskor példát kell mutatni mindkét esetre: **10** pont

    !!! tip
        Ha ugyanezt futási időben is szeretnénk, azaz, hogy a szolgáltatások közötti kommunikáció dinamikusan változhasson lokális és távoli jellegű hívás között, akkor érdemes az *aktor* modell alapú megoldások felé elmozdulni. A legtöbb *aktor* modell implementáció biztosítja az ilyesfajta transzparenciát a futási hely tekintetében. Lásd még az aktor mintás jogcímet.

### On-premise futó rendszerekhez

- **{OPDB2}** Legalább kétfajta on-premise adatbázis használata. Két eltérő technológiájú adatbázis használata perzisztenciára. Memória adatbázis, cache adatbázis (Redis) nem számít be: **5** pont

- **{OPEXTCR}** Konténerek vagy Helm chart(ok) letöltése on-premise klaszterbe külső container registry-ből, autentikációval: **2-3** pont

    - Azure Container Registry autentikációval pl. image pull secret-tel: **3** pont
    - Egyéb container registry autentikációval pl. image pull secret-tel: **2** pont

- **{OPARC}** On-premise Kubernetes bekötése Azure Arc-ba: **3** pont

- **{OPSTR}** Tartós tár, pl. lokális mappa csatolása klaszterbe: **2** pont

- **{GW}** Saját telepítésű (API/App) gateway használata. Kivéve self-hosted Azure API Management: **2-5** pont

    - Traefik használata útvonalválasztásra: **2** pont
    - Más, saját telepítésű (API/App) gateway használata: **5** pont

    !!! danger
        Saját gateway implementációért nem jár pont.

### Azure alapon futó rendszerekhez

- **{AZDB2}** Legalább kétfajta Azure-os adatbázisplatform használata (Azure SQL, Azure Database for PostgreSQL - Flexible Server, Cosmos DB, Azure Table Storage). Két eltérő technológiájú adatbázis használata perzisztenciára. Memória adatbázis, cache adatbázis (Azure Redis) nem számít be, egyéb NoSQL igen: **7** pont

    !!! tip
        30 napig ingyenes (többször is aktiválható!) [Cosmos DB](https://cosmos.azure.com/try/) - ideiglenesen nem működik :pensive: (2026. november)

- **{AZRED}** [Azure Redis](https://azure.microsoft.com/en-us/products/cache) szolgáltatás (Azure Managed Redis vagy Azure Cache for Redis) használata kifejezetten gyorsítótárazásra legalább egy művelet esetén: **5** pont

    !!! warning
        A {CACHE} és az {AZRED} közül csak az egyik számít be.

- **{AZING}** Gateway/Ingress Azure szolgáltatásra (kivéve Azure API Management) vagy AKS kiterjesztésre építve: **2-7** pont

    - AKS esetén az [application routing bővítmény Gateway API módban](https://learn.microsoft.com/en-us/azure/aks/app-routing-gateway-api): **2** pont
    - AKS esetén egyéb [hivatalosan támogatott AKS ingress opció](https://learn.microsoft.com/en-us/azure/aks/concepts-network-ingress#compare-ingress-options) (pl. Application Gateway for Containers): **7** pont
    - Egyéb Azure szolgáltatás (pl. Azure App Gateway; kivéve Azure API Management): **7** pont

    !!! warning "Managed NGINX"
        Az application routing bővítmény NGINX alapú (Ingress API-s) módja 2026 novemberében kivezetésre kerül, így ezért a módért nem jár pont.
  
- **{AZACR}** Konténerek vagy Helm chart(ok) letöltése Azure-beli klaszterbe vagy *Azure Function*-be saját *Azure Container Registry*-ből ACR admin felhasználó nevében vagy managed identity alapú hozzáféréssel: **2** pont

- **{AZKVAU}** Titkok lekérése saját *Azure Key Vault*-ból passwordless hozzáféréssel (workload identity): **7** pont

- **{AZMSG}** A mikroszolgáltatások közötti kommunikáció kiszervezése valamely [Azure üzenetkezelő szolgáltatásba](https://learn.microsoft.com/en-us/azure/service-bus-messaging/compare-messaging-services#comparison-of-services) (pl. Service Bus, de [Azure Storage Queue](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-azure-and-service-bus-queues-compared-contrasted?source=recommendations) is elfogadott) managed identity alapú hozzáféréssel: **7** pont

- **{AZACA}** Azure Container Apps skálázása szabály alapján: **2** pont

- **{AKSMON}** AKS monitorozása Azure Monitor agent-tel (pl. Container Insights): strukturált naplók és/vagy metrikák gyűjtése és megjelenítése Azure Monitor szolgáltatásban: **2** pont

    !!! tip
        Az alkalmazásszintű OpenTelemetry telemetriák közvetlen (agent nélküli) küldése Azure Monitor-ba külön jogcím, lásd {OTAZURE}.

- **{OTAZURE}** AKS/ACA/AZF platformon futó alkalmazásból alkalmazásszintű OpenTelemetry telemetriák küldése közvetlenül (agent nélkül) Azure Monitor-ba [Azure Monitor OpenTelemetry disztribúcióval](https://learn.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable): **5** pont

    !!! danger
        Platformonként csak egy, OpenTelemetry telemetriákat Azure Monitor-ba juttató módszerért jár pont: az {OT} jogcím Azure Monitor exportja, az {OTAZURE}, az {ACAMON} és az {AZFMON} közül egy platformon csak az egyik számít be.

- **{ACAMON}** ACA [monitorozása OpenTelemetry agent-tel](https://learn.microsoft.com/en-us/azure/container-apps/opentelemetry-agents?tabs=arm). Naplók és elosztott nyomkövetési adatok gyűjtése és megjelenítése Azure Monitor-ban (App Insights): **5** pont

    !!! danger
        Platformonként csak egy, OpenTelemetry telemetriákat Azure Monitor-ba juttató módszerért jár pont: az {OT} jogcím Azure Monitor exportja, az {OTAZURE}, az {ACAMON} és az {AZFMON} közül egy platformon csak az egyik számít be.

- **{AZFMON}** AZF [monitorozása beépített OpenTelemetry integrációval](https://learn.microsoft.com/en-us/azure/azure-functions/opentelemetry-howto). Naplók és elosztott nyomkövetési adatok gyűjtése és megjelenítése Azure Monitor-ban (App Insights): **5** pont

    !!! warning "Előzetes verzió"
        Az AZF beépített OpenTelemetry integrációja még csak előzetes verziós.

    !!! danger
        Platformonként csak egy, OpenTelemetry telemetriákat Azure Monitor-ba juttató módszerért jár pont: az {OT} jogcím Azure Monitor exportja, az {OTAZURE}, az {ACAMON} és az {AZFMON} közül egy platformon csak az egyik számít be.

- **{AZCS}** [*Azure Chaos Studio*](https://learn.microsoft.com/en-us/azure/chaos-studio/) használata káoszteszt futtatására. Védésen szemléltetés káoszteszt futtatással: **5-7** pont

    - Chaos Mesh nélküli, közvetlenül Azure szolgáltatásra ható hibával (pl. NSG szabály, Cosmos DB failover, Key Vault hozzáférés tiltása): **5** pont
    - AKS klaszteren Chaos Mesh alapú hibával: **7** pont

    !!! warning
        Ugyanazon a klaszteren a {CHAOS} és az {AZCS} Chaos Mesh alapú hibája közül csak az egyik számít be.

- **{AZSTR}** Tartós tár, például Azure Disk, Azure Files csatolása AKS-, ACA-klaszterbe vagy [Azure Function-be](https://learn.microsoft.com/en-us/azure/azure-functions/storage-considerations?tabs=azure-cli#mount-file-shares): **2** pont

- **{AZBLOB}** Azure Storage Blob integrálása [managed identity használatával](https://learn.microsoft.com/en-us/azure/storage/blobs/authorize-access-azure-active-directory): **5** pont

- **{AZFDF}** [*Durable Functions*](https://learn.microsoft.com/en-us/azure/durable-task/durable-functions/durable-functions-overview) használata mikroszolgáltatások orkesztrációjára Azure Functions platformon: **7** pont

- **{AZACI}** Legalább egy pod/szolgáltatás futtatása Azure Container Instance szolgáltatáson az AKS [virtual node](https://learn.microsoft.com/en-us/azure/aks/virtual-nodes) képességére építve: **7** pont
  
- **{AZCAS}** AKS node szintű skálázás cluster autoscaler-rel. Védésen a (vissza)skálázást demonstrálni kell: **3** pont

    !!! warning
        Jelenleg hallgatói előfizetéssel ez a pont valószínűleg nem megszerezhető, mert a szükséges vCPU kvóták az ilyen típusú előfizetéseken túl alacsonyak.

### Microsoft minősítések

Minősítésekből összesen **25** pont szerezhető. A megszerzett minősítések felkészülési anyagainak elsajátítása után [pont](mslearning.md) nem kapható. A minősítéseket listázó [oldalra](https://learn.microsoft.com) belépve kell igazolni a minősítés megszerzését.

- **{AZ900}** [Microsoft Certified: Azure Fundamentals](https://learn.microsoft.com/en-us/credentials/certifications/azure-fundamentals) minősítés megszerzése: **15** pont

<!-- Az AZ100 / AZ200 jogcímnevek szándékosak: bármely AZ-10X / AZ-20X vizsgaverziót jelölnek (pl. AZ-104, AZ-204), így a vizsgák átszámozásakor sem kell átnevezni őket. -->

- **{AZ100}** [Microsoft Certified: Azure Administrator Associate](https://learn.microsoft.com/en-us/credentials/certifications/azure-administrator) minősítés megszerzése: **20** pont

- **{AZ200}** [Microsoft Certified: Azure Developer Associate](https://learn.microsoft.com/en-us/credentials/certifications/azure-developer) minősítés megszerzése: **25** pont

- **{AI200}** [Microsoft Certified: Azure AI Cloud Developer Associate](https://learn.microsoft.com/en-us/credentials/certifications/azure-ai-cloud-developer-associate/) (AI-200 vizsga) minősítés megszerzése: **25** pont

### Egyéb

- **{BASE}** A minimum elvárásokat teljesítő rendszer: **30** pont
- **{ALLHF}** Minden félévközi házi feladat (6 db.) teljesítése. Nem arányosítható - csak akkor adható, ha *minden* házi teljesített: **6** pont
- **{MSLEARN}** Azure tananyagok elsajátítása, kizárólag a [külön leírt követelmények](mslearning.md) szerint: max. **24** pont
- **{CONTRIB}** Visszacsatolás. A véglegesített pontrendszer vagy tananyag javítása, bővítése, módosítása pull request-tel. Helyesírási hiba is lehet, de az oktatók döntenek, hogy pontot ér-e a módosítás. Többször is megszerezhető: **0-2** pont, összesen max. **6** pont.

## A szabályrendszer változása

Véglegesítés után is fenntartjuk a jogot

- a pontszerzési jogcímek halmazának bővítésére
- a pontszámok növelésére és egyéb hallgatóknak előnyös változtatásokra
- pontosításra, helyesírási hibák, formázás javítására
- egyéb változtatásra egyetemi szabályok változása miatt (pl. járványhelyzet miatt)

A pontrendszer véglegesítés után is általatok is módosítható/bővíthető. Ezt pull request formájában, megfelelő indoklással nyújthatjátok be, de a PR benyújtása nem jelenti annak az automatikus elfogadását, arról minden esetben a tárgy oktatói döntenek.

A változásokat a [GitHub history-ban](https://github.com/bmeviaumb11/skalazhato/commits/master/docs/homework/nagyhf/pontrendszer.md) követhetitek.
