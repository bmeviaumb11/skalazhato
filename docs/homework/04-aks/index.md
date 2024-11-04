# 04 - Azure Kubernetes Services

## Cél

A labor célja megismerni:

- az AKS szolgáltatást és a legfontosabb kapcsolódó szolgáltatásokat (ACR) 
- az AKS alkalmazások telepítésének különböző módszereit
- az AKS legalapvetőbb megfigyelési funkcióit 

## Előkövetelmények

A laborleírás cross-platform eszközöket használ. A labor linuxon (kubuntu) lett kidolgozva.

- Korábbi laborok infrastruktúrájából: `docker`, `docker compose`
    - Windows-on is [linux konténer módban](https://learn.microsoft.com/en-us/virtualization/windowscontainers/quick-start/quick-start-windows-10-linux#run-your-first-linux-container) 
- Azure [hallgatói előfizetés](https://azure.microsoft.com/en-us/free/students)
- Azure [kubelogin és kubectl](https://azure.github.io/kubelogin/install.html)
    - felülírhatja a korábban telepített `kubectl` binárist 
- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) v2.61 vagy újabb
- [Helm CLI](https://helm.sh/docs/intro/install/) - legyen elérhető PATH-on

## Előkészület

A feladatok megoldása során ne felejtsd el követni a feladat beadás folyamatát [GitHub](../../information/GitHub.md).

!!! danger "PR név"
    :exclamation: Beadásnál a pull request neve legyen: *hf4* :exclamation:

### Git repository létrehozása és letöltése

1. Moodle-ben keresd meg a laborhoz tartozó meghívó URL-jét és annak segítségével hozd létre a saját repository-dat.
2. Várd meg, míg elkészül a repository, majd checkout-old ki.
3. Hozz létre egy új ágat `megoldas` néven, és ezen az ágon dolgozz.
4. A `neptun.txt` fájlba írd bele a Neptun kódodat. A fájlban semmi más ne szerepeljen, csak egyetlen sorban a Neptun kód 6 karaktere.

!!! danger "NEPTUN"
    :exclamation: A feladatokban a `neptun` kifejezés helyett a saját neptunkódunkat helyettesítsük be minden esetben :exclamation:

## 0. Feladat

### Azure portál ajánlott beállítások

1. Lépjünk be az [Azure portálra](https://portal.azure.com)

2. Jobb felül nyissuk meg a beállításokat (fogaskerék ikon).

3. Állítsuk a [nyelvi beállításokat](https://learn.microsoft.com/en-us/azure/azure-portal/set-preferences#language--region) (felülről a harmadik menüpont). A nyelv legyen angol, a régiós formátum legyen magyar. Az **Apply** gombbal alkalmazzuk a beállításokat.

4. Visszatérve a portál beállításokhoz, nyissuk meg a [tenantjaink listáját](https://learn.microsoft.com/en-us/azure/azure-portal/set-preferences#directories--subscriptions) (felülről az első menüpont, *Dictionaries + subscriptions*). Ellenőrizzük a táblázatban, hogy a BME tenant-e az aktív (*Current*) tenantunk. Ha nem, a **Switch** gombbal váltsunk át rá.

### Azure előfizetés ellenőrzése

1. Nyissuk meg az [előfizetések listáját](https://learn.microsoft.com/en-us/azure/azure-portal/get-subscription-tenant-id#find-your-azure-subscription): a keresőben vagy a bal oldali menüben keressük ki a *Subscriptions* oldalt és nyissuk meg.

2. Több előfizetést is láthatunk egy táblázatban, a hallgató előfizetésünk *Azure for Students* néven jön létre és a szerepkörünk (*My role*) *Account admin* vagy *Owner*. Nyissuk meg a hallgatói előfizetésünket.

3. Az áttekintő oldalról (*Overview*) az előfizetés azonosítóját (*Subscription ID*) érdemes elmenteni, mert gyakran lesz rá szükség.

4. Ugyanezen az oldalon van egy [link a hallgatói kreditjeinket kezelő oldalra](https://www.microsoftazuresponsorships.com/). Ezen a külső oldalon ellenőrizzük, hogy megvan-e a 100$-nyi kreditünk (*Check your balance*).

!!! warning "Azure költségek"
    Ha költséges erőforrásokat hoztunk létre, akkor különösen fontos rendszeresen ellenőrizni ezen az oldalon a maradék keretünket.

### Azure CLI ellenőrzése

1. Ellenőrizzük a verziószám(ok) kiírásával, hogy az Azure CLI elérhető-e.
    ```bash
    az version
    ```

2. Parancssorból [csatlakozzunk az Azure előfizetésünkhöz](https://learn.microsoft.com/en-us/cli/azure/authenticate-azure-cli-interactively). Csatlakozáskor listázódnak az előfizetések, válasszuk a hallgatói előfizetést.
    ```bash
    az login
    ```
   
    !!! tip "Aktuális előfizetés az Azure CLI-ben"
        Az Azure CLI parancsok általában nem kérik be az előfizetés azonosítóját, hanem egy globális beállításból veszik, amit belépéskor (`az login`) is beállít. Fontos, hogy mindig a megfelelő előfizetés legyen beállítva. Az `az account set -s <előfizetés azonosító>` paranccsal tudunk előfizetést váltani.

!!! danger "Közös Azure régió"
    Fontos, hogy minden Azure erőforrás lehetőleg azonos régióban legyen. Ez a közös régió **North Europe** legyen. Ha bármilyen okból ezt nem tudod tartani, akkor is az erőforrásaid ugyanabban az európai régióban legyenek (pl. *Germany West Central*). 

### Erőforráscsoport létrehozása

Hozz létre új erőforráscsoportot [ezen útmutatót követve](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/manage-resource-groups-portal#create-resource-groups) a hallgató előfizetés alá *viaumb11* néven a közös Azure régióban (lásd fentebb).

!!! danger "Alapértelmezett Azure erőforráscsoport"
    A továbbiakban ezt az erőforráscsoportot add meg minden Azure erőforrás létrehozásakor.

## 1. Feladat

### AKS bevezető - mintaalkalmazás helyi kipróbálása

A labor során a hivatalos AKS bevezető gyakorlatot követjük némi módosítással. Az [első rész](https://learn.microsoft.com/en-us/azure/aks/tutorial-kubernetes-prepare-app?tabs=azure-cli) egy példaalkalmazást mutat be.

Kövesd az útmutatót, mely a korábbi gyakorlatokon megismert `docker` és `docker compose` parancsokat használ.

### Azure Container Registry létrehozása

A hivatalos útmutató [második része](https://learn.microsoft.com/en-us/azure/aks/tutorial-kubernetes-prepare-acr?tabs=azure-cli) egy Azure Container Registry konténer tároló Azure CLI-ből történő létrehozásáról és feltöltéséről szól. Kezdő Azure használóknak inkább az Azure portál ajánlott, mert ott szemléletesebben látszódik minden beállítás. Emiatt inkább az [Azure portálos útmutatót](https://learn.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal?tabs=azure-cli#create-a-container-registry) kövesd, az alábbi beállításokkal:

- Name: *acr* előtag + neptun kód (pl.*npt123* neptun kód esetén *acrnpt123*)
- Resource group: az alapértelmezett Azure erőforráscsoport (lásd fentebb)
- Location: közös Azure régió (lásd fentebb)
- Pricing plan / SKU: Basic

Azure CLI-vel (`az acr login`) [regisztráljuk is az ACR-t a docker környezetünkbe](https://learn.microsoft.com/en-us/cli/azure/acr?view=azure-cli-latest#az-acr-login) tárolóként.

### Konténerek feltöltése ACR-be

A hivatalos útmutató [második része](https://learn.microsoft.com/en-us/azure/aks/tutorial-kubernetes-prepare-acr?tabs=azure-cli) az ACR build szolgáltatását használja, amivel könnyen lehet a fejlesztői gép erőforrásait kímélve lehetne a lemezképeket megépíteni. Az építéshez szükséges kontextust (forráskód, YAML) tölti csak fel, a kiinduló lemezkép és az építési folyamat is az ACR-en belül történik. Sajnos ez a szolgáltatás [jelenleg csak fizetős Azure előfizetésekben érhető el](https://learn.microsoft.com/en-us/azure/container-registry/container-registry-tasks-overview), hallgatóiban nem. Szerencsére az első feladatrész során a lemezképek elkészültek, így [azokat feltölthetjük](https://docs.docker.com/get-started/docker-concepts/building-images/build-tag-and-publish-an-image/).

!!! warning "Lemezkép architektúrák"
    A lemezkép CPU architektúrájának [kompatibilisnak kell lennie](https://docs.docker.com/build/building/multi-platform/#why-multi-platform-builds) a docker környezet CPU architektúrájával (AKS esetén ez alapesetben: `linux/amd64`). Linuxos és Windows-os fejlesztői környezetben is általában linuxos docker környezetet használunk, így általában nem lesz ebből gondunk (`docker image incpect <lemezkép név vagy id>` paranccsal ellenőrizhetjük: az *Architecture* tulajdonságot figyeljük). Viszont például ARM64 CPU-s Mac esetében gond lehet, a fejlesztői gépen készített lemezkép AKS-en nem fog jól működni. Ilyenkor a legegyszerűbb linuxos lemezképet készíttetni az AKS számára, amit például a `DOCKER_DEFAULT_PLATFORM` [környezeti változó beállításával tehetünk meg](https://stackoverflow.com/questions/65612411/forcing-docker-to-use-linux-amd64-platform-by-default-on-macos). Az így készült lemezkép csak egy platformot támogat. Többplatformos lemezképet is [készíthetünk](https://docs.docker.com/build/building/multi-platform/#build-multi-platform-images) a *buildx* docker CLI plugin-nal, így ugyanaz a lemezkép több architektúrán is használható.

1. Tag-eljük meg az alábbi három *aks-store-demo* lemezképet. Az $ACRNAME helyére helyettesítsük be az ACR-ünk nevét (*acr+neptun kód*).

    ```bash
    docker tag aks-store-demo-product-service $ACRNAME.azurecr.io/azure-samples/aks-store-demo/product-service
    docker tag aks-store-demo-store-front $ACRNAME.azurecr.io/azure-samples/aks-store-demo/store-front
    docker tag aks-store-demo-order-service $ACRNAME.azurecr.io/azure-samples/aks-store-demo/order-service
    ```

2. Töltsük fel a képeket.

    ```bash
    docker push $ACRNAME.azurecr.io/azure-samples/aks-store-demo/store-front
    docker push $ACRNAME.azurecr.io/azure-samples/aks-store-demo/order-service
    docker push $ACRNAME.azurecr.io/azure-samples/aks-store-demo/product-service
    ```
   
!!! tip "ACR által használt tárhely"
    Az ACR Azure portálos oldalán belül a *Metrics* menüpontban a *Storage used* nevű [metrikát](https://learn.microsoft.com/en-us/azure/container-registry/monitor-container-registry-reference#supported-metrics-for-microsoftcontainerregistryregistries) kiválasztva ellenőrizhetjük az ACR által használt tárhelyet, illetve annak időbeli változását. Másik lehetőség az *Overview* menüponton belül a *Monitoring* alfül.

### AKS létrehozása, méretezése

Azure-ban minden erőforrás létrehozás, kezelés, stb. REST API-k segítségével történik, ezeket az API-kat ún. *resource provider*-ek ajánlják ki. Egy adott szolgáltatásnak általában saját resource providere van, de nem mindegyik van alapból bekapcsolva egy adott előfizetésen. Előfizetés tulajdonosként ezzel általában nem kell foglalkoznunk, egy szolgáltatás létrehozásakor a kapcsolódó resource provider automatikusan bekapcsolódik. Néhány esetben mégis szükség lehet arra, hogy kézzel kapcsoljunk be (regisztráljunk) provider-eket. Ilyen eset lehet, ha AKS létrehozás **előtt** a portál vCPU kvótákat ellenőriz - mint mindenhez, ehhez is API-t kell hívni. Ezért az AKS létrehozása előtt [ezen útmutató](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/resource-providers-and-types#register-resource-provider-1) alapján ellenőrizzük, hogy a *Microsoft.Compute* provider regisztrálva van-e. Ha nincs, regisztráljuk.

A hivatalos útmutató [harmadik része](https://learn.microsoft.com/en-us/azure/aks/tutorial-kubernetes-deploy-cluster?tabs=azure-cli) az AKS-t Azure CLI-vel hozza létre. Helyette kövesd [az Azure portálos utat](https://learn.microsoft.com/en-us/azure/aks/learn/quick-kubernetes-deploy-portal?tabs=azure-cli) a következő általános beállításokkal:

- Name: *aks* előtag + neptun kód (pl.*npt123* neptun kód esetén *aksnpt123*)
- Resource group: az alapértelmezett Azure erőforráscsoport (lásd fentebb)
- Cluster preset configuration: Dev/Test
- Region: közös Azure régió (lásd fentebb)
- Authentication and Authorization: Microsoft Entra ID authentication with Azure RBAC

A *Node Pool* fülön kell méreteznünk a klasztert. Ez hallgatói előfizetés esetén nem egyszerű, mert csak bizonyos virtuális gép típusok érhetőek el, azok is csak kis számban. Ráadásul a kínálat időben és régiók mentén folyamatosan változik. Az aktuális kínálat az [Azure portálon](https://learn.microsoft.com/en-us/azure/quotas/view-quotas) követhető.

!!! warning "Kvóta növelése - elméletben"
    Számos kvóta elvileg a portálon keresztül is [növelhető](https://learn.microsoft.com/en-us/azure/quotas/per-vm-quota-requests), de ez hallgatói előfizetés esetében általában nem működik.

!!! danger "User node pool"
    Semmiképp ne adjunk a klaszterhez **user** node poolt, mert akkor a skálázásra végképp nem marad kvótánk. Csak a system node poolba vegyünk olcsóbb, max. 2 magos virtuális gépeket.

A system node pool méretezésekor érdemes a *Choose a size* opció által feldobott felületen a pontosan 2 magos (lehet rá szűrni), *D* betűvel kezdődő kódú kiméretek közül próbálkozni (D2s_v3, D2as_v4, DS2_v2). A felület mutatja a havi költséget is (*Cost/month*), a 60-90 EUR/VM költség már jónak számít.

!!! warning "System node pool szabályok"
    A kiválasztott kiméretnek még az AKS [system node pool-ra vonatkozó minimum feltételeknek](https://learn.microsoft.com/en-us/azure/aks/use-system-pools?tabs=azure-cli#system-and-user-node-pools) is meg kell felelni. B sorozatú kiméretek például nem használhatók.

A VM kiméret választás után a *Node Pool* fülre visszatérve az Azure ellenőrzi a legtöbb szabályt. Ha nem látunk hibaüzenetet, akkor jó esélyeink vannak.

<figure markdown="span">
  ![quotaerror.png](images/quotaerror.png)
  <figcaption>Nincs meg ehhez a jóárasított VM kimérethez a szükséges kvótánk</figcaption>
</figure>

!!! danger "Free AKS korlátozás"
    A legnagyobb terhelésnek kitett Azure régiókban (pl. *West Europe*) előfordulhat, hogy korlátozzák az ingyenes (csak a neve ingyenes!) csomagú AKS-ek létrehozását.

Az *Integrations* fülön válasszuk ki az ACR-ünket.

A többi fülön hagyjuk meg az alapértelmezett értékeket. 

1. Indítsuk el a létrehozási folyamatot.  

2. A létrehozás végeztével (10-15 perc is lehet!) [fedezzük fel a klaszter logikai szerkezetét az Azure portálon keresztül](https://learn.microsoft.com/en-us/azure/aks/kubernetes-portal?tabs=azure-cli#view-kubernetes-resources). 

3. Ellenőrizzük, hogy tudunk-e csatlakozni a kubectl eszközünkkel. Erre a legkényelmesebb mód, ha az AKS-ünk Azure Portal oldalának *Overview* menüpontját megnyitva, a vízszintes menüben a *Connect* gombot megnyomva, [az előre elkészített parancsokat használjuk](https://learn.microsoft.com/en-us/azure/aks/kubernetes-portal?tabs=azure-cli#connect-to-your-cluster). Futtassunk le egy-két lekérdező `kubectl` parancsot.

4. Kukkantsuk meg a klasztert alkotó Azure infrastruktúraszolgáltatásokat: a *Properties* menüpontban az *Infrastructure resource group* linkre kattintva átugorhatunk az ezen szolgáltatásokat összefogó erőforráscsoportba.

### Mintaalkalmazás telepítése

A hivatalos útmutató [negyedik része](https://learn.microsoft.com/en-us/azure/aks/tutorial-kubernetes-deploy-application?tabs=azure-cli) alapján telepítsük a mintaalkalmazást. Az ACR login szerver és a K8S service külső IP címe is megszerezhető az Azure portálról. Az útmutató rész végére érve **ne** töröljük a telepítést.

!!! warning "K8S erőforráskorlátok"
    A mintaalkalmazás korábbi verziójában némely konténer [erőforráslimitje](https://kubernetes.io/docs/concepts/configuration/manage-resources-containers/) nagyon alacsony volt (pl. 10 MB memória), amit igen könnyű volt átlépni és ilyenkor az ütemező folyamatosan ki-kilőtte a podot (hibaüzenet valami hasonló volt: *container init was OOM-killed (memory limit too low?)*), a ráépülő szolgáltatás nem tudott rendben működni. Az aktuális verzióban ezt már [javították](https://github.com/Azure-Samples/aks-store-demo/commit/afe11f4ca94a154f43c3b72187b8684c048e608b#diff-46a7464f533643281cbe9a01070701f8acfc30f993f74ece069958ef3e3c4767R191) a *product-service* konténer esetében. Ha hasonló hibajelenséget észleltek, akár a helyi K8S-ben, akár AKS-ben, nyugodtan állítsátok a hibát jelző konténer limitjét a YAML-ben.

!!! example "BEADANDÓ"
    Készíts egy képernyőképet (`f1.png`) és commitold azt be a házi feladat repó gyökerébe, amin látszik:

    - a futó alkalmazás böngészőben, böngésző címsorban az alkalmazás (IP) címével
    - parancssorban a kapcsolódó k8s *service* adatai (`kubectl get service store-front`)
    - parancssorban az alkalmazás k8s *deployment* erőforrásai a lemezképek azonosítóival együtt (`kubectl get deployment -o wide`). 

## 2. Feladat

A k8s házi *todoapp*-ját is telepítsük ugyanebbe az AKS-be, egy külön [kubernetes névtérbe](https://kubernetes.io/docs/concepts/overview/working-with-objects/namespaces/).

### 2.1 Névtér létrehozása

Ez egyszerű:

```bash
kubectl create namespace todoapp
```

### 2.2 Traefik - telepítés külső helm chart forrásból

Itt most kiengedjük a traefik-et a publikus internetre, LoadBalancer objektumot hozunk létre NodePort helyett, de mivel ez az [alapbeállítás](https://github.com/traefik/traefik-helm-chart/blob/b8725498c2445da8ecc06f156ca69ddc1a56cce4/traefik/values.yaml#L728), így nem kell szinte semmi ilyesmit állítgatni. Csak az IngressRoute-ot kell [létrehozatni](https://github.com/traefik/traefik-helm-chart/blob/master/EXAMPLES.md#access-traefik-dashboard-without-exposing-it).

```bash
 helm install traefik traefik/traefik --namespace todoapp --set ingressRoute.dashboard.enabled=true
```

Ezután már lehet port-átirányítani.

```bash
kubectl port-forward $(kubectl get pods -n todoapp -l "app.kubernetes.io/name=traefik" -o jsonpath="{.items[0].metadata.name}") 9000:9000 -n todoapp
```

Ezután a http://localhost:9000/dashboard/ címen érhető el a traefik dashboard (amíg a `port-forward` parancs fut).

### 2.3 Adatbázisok - telepítés k8s YAML fájlokkal

Próbáljuk ki egy-az-egyben a k8s házi módszerét. Töltsük le valahová az adatbázisok [k8s YAML fájljait tartalmazó mappát](https://github.com/bmeviaumb11/skalazhato-szoftverek-bmeviaumb11-2024-hf-2024-kubernetes-hf-kiindulo-kubernetes-1/tree/main/todoapp/kubernetes/db) (vagy navigáljunk a k8s házink könyvtárába). A `db` mappa szülőkönyvtárban állva:

```bash
kubectl apply -f db -n todoapp
```

Meglepő lehet, de ez szinte teljesen jól működik: nincs megadva storage class, így az AKS alapértelmezett storage class-a érvényesül, ami standard SSD lemezeket hoz létre, és ezek csatolódnak fel. Ezen lemezek élettartama az AKS élettartamához kötött (de nem a K8S objektumok élettartamához!), így az AKS törlésével ezek is törlődnek. 

!!! tip "AKS/K8S storage class-ok"
    Így kérdezhetjük le őket: `kubectl get sc`. Az alapértelmezett storage class tulajdonságai: `kubectl describe sc default`. Látható, hogy a létrehozó (provisioner) `disk.csi.azure.com`, aminek paraméterként a `skuname=StandardSSD_LRS` beállítást kapja meg, tehát nem meglepő, hogy [Azure Disk Standard SSD](https://learn.microsoft.com/en-us/azure/virtual-machines/disks-types#disk-type-comparison) jön létre.

!!! tip "Diszkek, mint Azure erőforrások"
    A lemezeket, mint Azure erőforrás is megfigyelhetjük, ha újra rákukkantunk az infrastruktúraszolgáltatásos erőforráscsoportra: két új 2 GiB méretű Standard SSD diszk is létrejött. Mivel a provisioner (az AKS platform) kezeli ezeket a lemezeket, ezért logikus, hogy ide kerülnek.

Pár perc elteltével, ha ránézünk pod listára (megfelelő `kubectl` paranccsal vagy [Azure portal Workloads menüpontban](https://learn.microsoft.com/en-us/azure/aks/kubernetes-portal?tabs=azure-cli#view-kubernetes-resources)) láthatjuk, hogy gond van az elasticsearch poddal, folyamatosan újraindul.

Kérdezzük le a pod logját: ezt is megtehetjük `kubectl` paranccsal vagy [Azure portálon](https://learn.microsoft.com/en-us/azure/azure-monitor/containers/container-insights-livedata-overview#view-aks-resource-live-logs). Az elasticsearch folyamat nem tud dolgozni a csatolt köteten (volume) jogosultsági gondok miatt. Az elasticsearch folyamat saját [*elasticsearch* linux felhasználó nevében fut](https://github.com/elastic/elasticsearch/blob/v7.17.17/distribution/docker/src/docker/Dockerfile#L164), az AKS viszont nem tud erről a felhasználóról, így a kötethez alapértelmezetten nem lesz joga. Oldjuk meg a problémát egy új init konténerrel, vegyünk fel egy újat a meglévők mellé a *elasticsearch-statefulset.yaml*-be, ami root felhasználó nevében futva a kötet tulajdonosává teszi az *elasticsearch* felhasználót (uid:1000, gid:1000).

```yaml
- name: init-permissions
  image: alpine
  command: [ "sh", "-c", "chown -R 1000:1000 /usr/share/elasticsearch/data" ]
  volumeMounts:
  - name: elasticsearch-data
    mountPath: /usr/share/elasticsearch/data      
```

Mehet egy újabb próba a korábbi `kubectl apply` paranccsal. Egyértelműbb a változtatás alkalmazása, ha ki is töröljük az elasticsearch pod-ot. Ezt is megtehetjük Azure portálról és `kubectl` paranccsal is. Ezután már nem szabad folyamatos újraindulást tapasztalunk a podok között.

!!! tip "ECK"
    Elasticsearch telepítésére egy modernebb módszer az *Elastic Cloud on Kubernetes* disztribúció, például [helm chartként](https://www.elastic.co/guide/en/cloud-on-k8s/current/k8s-install-helm.html).

### 2.4 Saját mikroszolgáltatások - telepítés nem annyira külső helm chart forrásból

A tárgy [közös ACR-jába](https://portal.azure.com/#@m365.bme.hu/resource/subscriptions/b83ae5ef-4ef6-4ccb-bb4b-e9f187873feb/resourceGroups/infra/providers/Microsoft.ContainerRegistry/registries/viaumb11acr/overview) felkerültek a k8s házi mikroszolgáltatásai helm chartként, illetve a hivatkozott konténerek is. Elvileg minden Teams csapattag kapott ehhez hozzáférést, letöltéshez [`AcrPull` jogot](https://learn.microsoft.com/en-us/azure/container-registry/container-registry-roles?tabs=azure-cli).

!!! tip "Saját jog ellenőrzése"
    Ellenőrizheted a hozzáférésed ehhez vagy bármilyen más Azure erőforráshoz az [Azure portálon](https://learn.microsoft.com/en-us/azure/role-based-access-control/check-access#step-3-check-your-access).

Megfelelő jogok birtokában közvetlenül, saját gép érintése nélkül [másolhatunk](https://learn.microsoft.com/en-us/azure/container-registry/container-registry-import-images?tabs=azure-cli#import-from-a-registry-in-a-different-subscription) lemezképeket, helm chartokat ACR-ek között. Az alábbi példa alapján másold át a 3 szükséges lemezképet (*todos*, *users*, *web*; mindegyikből a v2 tag). A helm chartot nem szükséges másolni: azt a fejlesztői gépünknek kell elérni, a lemezképeket viszont az AKS-nek, ami csak a saját ACR-ünket látja csak, a közös tárgy ACR-t nem.

```bash
az acr import --name $ACRNAME --source todos:v2 --registry /subscriptions/b83ae5ef-4ef6-4ccb-bb4b-e9f187873feb/resourceGroups/infra/providers/Microsoft.ContainerRegistry/registries/viaumb11acr
```

!!! tip "ACR repo felfedezése"
    Az elérhető lemezképeket, helm chartokat is felfedezheted [a portálon](https://learn.microsoft.com/en-us/azure/container-registry/container-registry-repositories).

!!! tip "Alternatíva - helm publikálás ACR-be"
    A saját k8s házid megoldásának mikroszolgáltatásait is publikálhatod a saját ACR-edbe. Ehhez a `IMAGE_TAG="v2" REGISTRY_URL="$ACRNAME.azurecr.io"  docker compose build --push` parancsot használhatod.

Csatlakoztassuk a központi ACR-t helm repo-ként. Az alábbi egy *bash*-specifikus példa:

```bash title="bash"
az acr login --name viaumb11acr --expose-token --output tsv --query accessToken | \
  helm registry login viaumb11acr.azurecr.io --username "00000000-0000-0000-0000-000000000000" --password-stdin
```

Kérdezzük le a helm chart beállításait.

```bash
helm show values oci://viaumb11acr.azurecr.io/helm/todoapp
```

A k8s házihoz képest új beállítás került be, az **image.registry**, amivel a lemezképek registry azonosítóit állíthatjuk. Ezt kell a saját ACR-ünkre állítani, amit az AKS-ünk elér. A tag beállítások már korábban megvoltak, azoknak egyezniük kell az átmásolt képek tagjeivel.

```bash
helm install todoapp oci://viaumb11acr.azurecr.io/helm/todoapp --version 0.1.0 --namespace todoapp --set image.registry=$ACRNAME.azurecr.io --set todos.tag=v2 --set web.tag=v2 --set users.tag=v2
```

!!! tip "Lemezkép letöltés ellenpróba"
    Opcionálisan próbáld ki, mi történik, ha a registry-t nem állítod be. Telepített verzió leszedése: `helm uninstall todoapp --namespace todoapp`.

Jöhet a fő próba: az Azure portálról vagy parancssorból (`kubectl`) szerezzük meg a traefik service külső IP-jét és nyissuk meg böngészőből (https helyett sima http-n). Vegyünk fel pár feladatot.

!!! tip "Kitekintés - külső források"
    Nagyvállalati környezetben a külső források elérése gyakran tiltott (pl. tűzfalszabályokkal), ezen források biztonsági és egyéb szempontok miatt  alapértelmezetten megbízhatatlannak számítanak. A docker alapértelmezett forrása, a Docker Hub például  korlátozásokat (rate limiting) [vezetett be](https://medium.com/@alaa.barqawi/docker-rate-limit-with-azure-container-instance-and-aks-4449cede66dd) az AKS-es letöltésekre is. Mindezek miatt a nagyvállalti klaszterek csak belső céges repository-kat használhatnak, amiket egy dedikált csapat kezel: megfelelő ellenőrzés után emelnek be külső vagy belső fejlesztésű elemeket (artifaktokat). Emiatt fontos, hogy minden telepítési egység (pl. helm chart) paraméterezhető legyen a függőségeinek elérhetősége kapcsán.


!!! example "BEADANDÓ"
    Készíts egy képernyőképet (`f2.1.png`) és commitold azt be a házi feladat repó gyökerébe, ahol az alkalmazás futása látszik egy saját neptun kódot tartalmazó todoval. Látszódjon a weboldal címe is.

    Készíts egy másik képernyőképet (`f2.2.png`) és commitold azt be ezt is a házi feladat repó gyökerébe, ahol az Azure portálon látszik az AKS infrastruktúra erőforráscsoportjának (MC_ kezdetű) áttekintő nézete (*Overview*). Látszódjon a portálra belépett felhasználó azonosítója a jobb felső sarokban.
    
    Készíts egy másik képernyőképet (`f2.3.png`) és commitold azt be ezt is a házi feladat repó gyökerébe, ahol a végállapotban látszik parancssorban mindkét alkalmazás k8s *deployment* erőforrásai a lemezképek azonosítóival együtt (`kubectl get deployment -o wide` és `kubectl get deployment -n todoapp -o wide`)


## 3. Feladat - talán a legfontosabb

!!! danger "AKS törlése"
    Beadás után, ha egyből folytatod a következő házival, akkor hagyd meg, egyébként töröld az AKS-t. Ajánlott egyből folytatni, különben újra létre kell majd hoznod, és fel kell töltened az AKS-t.