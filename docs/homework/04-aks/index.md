# 04 - Azure Kubernetes Services

## Cél

A labor célja:

- megismerni az AKS szolgáltatást és a legfontosabb kapcsolódó szolgáltatásokat (ACR) 
- egy alkalmazás telepítése AKS klaszterbe és a frissítés módjának megismerése
    - A telepítéshez és frissítéshez Helm chartot használunk

## Előkövetelmények

A laborleírás cross-platform eszközöket használ.

- Korábbi laborok infrastruktúrájából: `docker`, `docker compose`
- Azure [hallgatói előfizetés](https://azure.microsoft.com/en-us/free/students)
- Azure [kubelogin és kubectl](https://azure.github.io/kubelogin/install.html)
    - felülírhatja a korábban telepített `kubectl` binárist 
- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) v2.61 vagy újabb
- Helm CLI
    - [Helm](https://helm.sh/docs/intro/install/)
    - A Helm CLI legyen elérhető PATH-on

## Előkészület

A feladatok megoldása során ne felejtsd el követni a feladat beadás folyamatát [GitHub](../../information/GitHub.md).

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

2. Több előfizetést is láthatunk egy táblázatban, a hallgató előfizetésünk *Azure for Students* néven jön létre és a szerepkörünk (*My role*) *Account admin*. Nyissuk meg a hallgatói előfizetésünket.

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

1. Tag-eljük meg az alábbi három *aks-store-demo* lemezképet. Az $ACRNAME helyére helyettesítsük be az ACR-ünk nevét (acr+neptun kód).

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

### AKS létrehozása, méretezése

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

## 2 Feladat

### 2.1 Deployment létrehozása

A podokat nem szoktuk közvetlenül létrehozni, hanem _Deployment_-re és _ReplicaSet_-re szoktunk bízni a kezelésüket és létrehozásukat.

1. Hozzunk létre egy új YAML fájlt `createdeployment.yml` néven az alábbi követelményeknek megfelelően:

    - a kubernetes leíró Deployment-et definiál
    - a deployment neve legyen `counter-neptun` a **saját neptunkóddal** kiegészítve
    - a deployment egy olyan konténert definiáljon, mint az 1. feladatban
    - 1 replika legyen az elvárt állapot
    - a selectorok használata során `counter-app-neptun` címkét használjuk a **saját neptunkóddal** kiegészítve

2. Hozzuk létre a Deployment-et:

    ```cmd
    kubectl apply -f createdeployment.yml
    ```

3. Listázzuk a Deployment-eket, ReplicaSet-eket és a podokat:

    ```cmd
    kubectl get deployment
    kubectl get replicaset
    kubectl get pod
    ```

    Vegyük észre, hogy a pod neve generált, a _Deployment_ és a _ReplicaSet_ alapján kap automatikusan egyet.

!!! example "BEADANDÓ"
    Készíts egy képernyőképet (`f2.1.png`) és commitold azt be a házi feladat repó gyökerébe, amin a futó deployment, replicaset és pod neve látszik.
    Commitold be a forráskódot is.

### 2.2 Deployment frissítése

A _Deployment_ szolgál az alkalmazás verziónak frissítésére, kiadására.

Változtassuk meg a program futását a deployment leíróján keresztül: ne 5, hanem 10 másodpercenként írjuk ki az időt.
Ezt a _Deployment_ módosításával érhetjük el, mivel podot nem tudunk szerkeszteni hatékonyan, egy futó pod nem cserélhető le.
Ehelyett valójában egy új podot kell létrehozni indirekt módon a deployment frissítésével.

Érvényesítsd a módosítást a deployment leíróban, majd alkalmazd a változást:

```cmd
kubectl apply -f createdeployment.yml
```

Kérjük le a logokat a deployment podjából:

```cmd
kubectl logs -f <podnév>
```

!!! example "BEADANDÓ"
    Készíts egy képernyőképet (`f2.2.png`) és commitold azt be a házi feladat repó gyökerébe, ahol a logokban már 10 másodpercenként történik a kiíratás.

??? tip "Kubectl parancsok"

    A `kubectl` leggyakrabban használt parancsainak szerkezete: `kubectl <ige> <erőforrás> <attribútumok>`.

    Az ige például:

    - `get`: listázza az erőforrásokat
    - `create`: létrehoz egy erőforrást
    - `delete`: töröl egy erőforrást
    - `describe`: lekérdezi az erőforrás részletes állapotát
    - `edit`: letölti az erőforrás leíróját, és megnyitja szövegszerkesztőben; mentés és bezárás után frissíti a klaszterben az erőforrást a módosítások alapján

    Az erőforrások a `pod`, `replicaset` vagy röviden `rs`, a `deployment`, stb.

    A parancsokról `-h` kapcsolóval kaphatunk segítséget, pl. `kubectl describe -h`

## 3. Feladat

### Célok

A célunk a kiinduló repóban lévő, todo-kat kezelő konténeralapú, külön álló (mikro)szolgáltatásokra épülő webalkalmazás telepítése Kubernetes-be.
A rendszerünk alapvetően három fajta komponensből épül fel:

- az általunk megvalósított mikroszolgáltatások (backendek és frontend),
- az adatbázis rendszerek (MongoDB, Elasticsearch és Redis),
- valamint az api gateway.

![TODO App architektúra](images/todo-k8s.drawio.png)

Célunk nem csak az egyszeri telepítés, hanem az alkalmazás naprakészen tartásához a folyamatos frissítés lehetőségének megteremtése.
A fenti komponensek azonban nem ugyanolyan frissítési ciklussal rendelkeznek: a saját komponenseink gyakran fognak változni, míg az adatbázisok és az api gateway ritkán frissül.
A telepítést ennek megfelelően most ketté vágjuk:

1. Az api gateway-t és az adatbázisokat egyszer telepítjük manuálisan.
2. Az alkalmazásunk saját komponenseihez YAML alapú leírókat készítünk, amit `kubectl apply` segítségével fogunk telepíteni, illetve ezeket a leírókat Helm-mel fogjuk kezelni.

### 3.0 Helm

Ellenőrizzük, hogy a `helm` CLI elérhető-e:

```cmd
helm version
```

!!! warning "Helm 3"
    A feladat során a Helm 3-as verzióját fogjuk használni. A korábbi verziója koncepcióban azonos, de működésében eltérő.

### 3.1 Ingress Controller (api gateway) telepítése Helm charttal

A Traefik-et [Helm charttal](https://github.com/traefik/traefik-helm-chart) fogjuk telepíteni, mert a Traefik helyes működéséhez a Traefik konténer (Deployment) mellett egyéb elemekre is szükség lesz (klaszteren belüli hozzáférés szabályzás miatt).

!!! warning "Chart-ok ellenőrzése"
    A Helm chartok nagy része harmadik féltől származik, így a klaszterünbe való telepítés előtt a tartalmukat érdemes alaposan megnézni.

1. A Helm is repository-kkal dolgozik, ahonnan a chart-okat letölti. Ezeket regisztrálni kell. Regisztráljuk a Traefik hivatalos chart-ját tartalmazó repository-t, majd frissítsük az elérhető char-okat:

    ```cmd
    helm repo add traefik https://traefik.github.io/charts
    helm repo update
    ```

1. Telepítsük:

    ```cmd
    helm install traefik traefik/traefik --set ports.web.nodePort=32080 --set service.type=NodePort --set "additionalArguments={--api.insecure=true}"
    ```

     - A legelső `traefik` a Helm release nevét adja meg. Ezzel tudunk rá hivatkozni a jövőben.
     - A `traefik/traefik` azonosítja a telepítendő chartot (repository/chartnév).
     - A `--set` kapcsolóval a chart változóit állítjuk be.

    !!! info "Publikus eléréshez"
        A Traefik jelen konfigurációban _NodePort_ service típussal van konfigurálva, ami azt jelenti, lokálisan, helyben a megadott porton lesz csak elérhető. Ha publikusan elérhető klaszterben dolgozunk, akkor tipikusan _LoadBalancer_ service típust fogunk kérni, hogy publikus IP címet is kapjon a Traefik.

2. Ellenőrizzük, hogy fut-e:

    ```cmd
    kubectl get pod
    ```

    Látunk kell egy traefik kezdetű podot.

3. A Traefik dashboard-ja nem elérhető "kívülről".
   A dashboard segít minket látni a Traefik konfigurációját és működését.
   Mivel ez a klaszter belső állapotát publikálja, production üzemben valamilyen módon authentikálnunk kellene.
   Ezt most megkerülve `kubectl` segítségével egy helyi portra továbbítjuk a Traefik dashboard-ot (és a telepítéskor insecure módba kapcsoltuk).
   A port átirányítást próbáljuk most a VSCode Kubernetes extension segítségével:

    ![Traefik port forward](images/traefik-dashboard-port-forward.png)

    Fogadjuk el az elapértelmezett értékeket (`9100:metrics 9000:traefik 8000:web 8443:websecure`). Így a Traefik dashboard a `localhost:9000/dashboard/` címen lesz elérhető.

    ??? tip "Port forward parancs"

        Ahogy látjuk a GUI-s K8S eszközök is csak a `kubectl port-forward` parancsot használják a háttérben.

        Ha nem használunk GUI-t, akkor a port forward parancs a következő, amibe ráadásul nem égettük bele a pod nevét:

        ```bash
        kubectl port-forward $(kubectl get pods --selector "app.kubernetes.io/name=traefik" --output=name) 9100:9100 9000:9000 8000:8000 8443:8443 -n default
        ```

4. Nézzük meg a Traefik dashboardot: <http://localhost:9000/dashboard/> (a végén kell a perjel!)

!!! note ""
    Ha frissíteni szeretnénk később a Traefik-et, akkor azt a `helm upgrade traefik traefik/traefik ...` paranccsal tudjuk megtenni.

### 3.2 Adatbázisok telepítése

Az adatbázisainkat saját magunk által megírt YAML leíróval telepítjük. Ez a leíró fájl már rendelkezésünkre áll a kiinduló repository todoapp almappájában.

1. Vizsgáljuk meg a repository `todoapp/kubernetes/db` könyvtárában lévő YAML leírókat.

     - Redis: Deployment-ként telepítjük és nem csatolunk hozza diszket, mert úgyis csak cache-nek használjuk
     - MongoDB: StatefulSet-ként telepítjük, és a perzisztens adattároláshoz dinamikus PersistentVolumeClaim-et használunk
     - Elasticsearch: StatefulSet-ként telepítjük, és a perzisztens adattároláshoz dinamikus PersistentVolumeClaim-et használunk

1. Telepítsük az adatbázisokat:

    ```cmd
    kubectl apply -f todoapp/kubernetes/db
    ```

    !!! tip ""
        A `kubectl apply` parancs `-f` kapcsolója ha mappát kap, akkor a mappában lévő összes yaml fájlt alkalmazza.

1. Ellenőrizzük, hogy az adatbázis podok elindulnak-e (pl.: GUI-val). Minden a _default_ névtérbe kellett települjön.
1. Nézzük meg a _Persistent Volume_ és _Persistent Volumen Claim_-eket.

### 3.3 Alkalmazásunk telepítése

Az alkalmazásunk telepítéséhez szintén YAML leírókat találunk a _kubernetes/app_ könyvtárban.

1. Nézzük meg a leírókat. Az előbb látott Deployment és Service mellet Ingress-t is látni fogunk.

1. Telepítsük az alkalmazásokat:

    ```cmd
    kubectl apply -f todoapp/kubernetes/app
    ```

1. Ellenőrizzük, hogy létrejöttek a Deployment-ek podok, stb. Viszont vegyük észre, hogy piros lesz még pár dolog. A hiba oka, hogy a hivatkozott image-eket nem találja a rendszer.

1. Navigáljunk el az `src` könyvtárba, és buildeljük le az alkalmazást docker-compose segítségével úgy, hogy a megfelelő taggel ellátjuk az image-eket. Az app könyvtárban lévő YAML fájlok a *v1* tagre hivatkoznak (`image: todoapp/todos:v1`), így ehhez érdemes igazodnunk. A tag-et beállíthatjuk környezeti változóból.

    - Powershell-ben

        ```powershell
        $env:IMAGE_TAG="v1"
        docker compose build
        ```

    - Windows Command Prompt-ban

        ```cmd
        setx IMAGE_TAG "v1"
        docker compose build
        ```
    
    - Linux bash-ben

        ```bash
        IMAGE_TAG="v1" docker compose build
        ```

1. A build folyamat végén előállnak helyben az image-ek, pl. `todoapp/web:v1` taggel. Ha távoli registry-be szeretnénk feltölteni őket, akkor taggelhetnénk őket a registry-nek megfelelően. A helyi fejlesztéshez nincs szükségünk ehhez, mert a helyben elindított Kubernetes "látja" a Docker helyi image-eit.

1. Menjünk vissza az erőforrásainkhoz. Egy kicsit várjunk, és azt kell lássuk, hogy az eddig piros elemek kizöldülnek (GUI függően frissítésre lehet szükség). A Kubernetes folyamatosan törekszik az elvárt állapot elérésére, ezért a nem elérhető image-einket újra és újra megpróbálta elérni, míg nem sikerült.

1. Próbáljuk ki az alkalmazást a <http://localhost:32080> címen.

!!! example "BEADANDÓ"
    Készíts egy képernyőképet (`f3.3.png`) és commitold azt be a házi feladat repó gyökerébe, ahol az alkalmazás futása látszik egy saját neptun kódot tartalmazó todoval.

### 3.4 Alkalmazás frissítése Helm charttal

Tegyük fel, hogy az alkalmazásunkból újabb verzió készül, és szeretnénk frissíteni.
A fentebb használt YAML leírókat azért (is) a verziókezelőben tároljuk, mert így a telepítési "útmutatók" is verziózottak.
Tehát nincs más dolgunk, mit a konténer image-ek elkészítése után a Deployment-ekben a megfelelő tag-ek lecserélése, és a `kubectl apply` paranccsal a telepítés frissítése.

A Tag-ek frissítéséhez a YAML fájlokba minden alkalommal bele kell írnunk.
Jó lenne, ha az image tag-et mint egy változó tudnánk a telepítés során átadni.
Erre szolgál a Helm: készítsünk egy _chart_-ot a szolgáltatásainknak.
A _chart_ a telepítés leíró neve, ami gyakorlatilag YAML fájlok gyűjteménye egy speciális szintaktikával kiegészítve.

1. Hozzunk létre a repository-nkban a `todoapp/helmchart` mappát majd konzolban navigáljunk egy el ide.

1. Készítsünk egy új, üres chart-ot: `helm create todoapp`. Ez létrehoz egy _todoapp_ nevű chartot egy azonos nevű könyvtárban.

1. Nézzük meg a chart fájljait.

    - `Chart.yaml` a metaadatokat írja le.
    - `values.yaml` írja le a változóink alapértelmezett értékeit.
    - `.helmignore` azon fájlokat listázza, amelyeket a chart értelmezésekor nem kell figyelembe venni.
    - `templates` könyvtárban vannak a template fájlok, amik a generálás alapjául szolgálnak.

    A Helm egy olyan template nyelvet használ, amelyben változó behelyettesítések, ciklusok, egyszerű szövegműveletek támogatottak. Mi most csak a változó behelyettesítést fogjuk használni.

1. Töröljük ki a `templates` könyvtárból az összes fájlt a `_helpers.tpl` kivételével.

1. Másoljuk helyette be ide a korábban a telepítéshez használt YAML fájljainkat a `todoapp/kubernetes/app` mappából (3 darab).

1. Szerkesszük meg a `todos.yaml` fájl tartalmát. Leegyszerűsítve az alábbiakra lesz szükség:

    - Ha _Visual Studio Code_-ot használunk, akkor telepítsük a [`ms-kubernetes-tools.vscode-kubernetes-tools`](https://marketplace.visualstudio.com/items?itemName=ms-kubernetes-tools.vscode-kubernetes-tools) extension-t. Így kapunk némi segítséget a szintaktikával.

    - Mindenhol, ahol `labels` vagy `matchLabels` szerepel, még egy sort fel kell vennünk:

        ```yaml
        app.kubernetes.io/instance: {{ .Release.Name }}
        ```

        Ez egy implicit változót helyettesít be: a _release_ nevét. Ezzel azonosítja a Helm a telepítés és frissítés során, hogy mely elemeket kell frissítenie, melyek tartoznak a fennhatósága alá.

    - A pod-ban az image beállításához használjunk változót:

        ```yaml
        image: todoapp/todos:{{ .Values.todos.tag }}
        ```

        !!! warning "Whitespace"
            Figyeljünk oda, hogy a változó behelyettesítés során hova kell whitespace-t rakni és hova nem.

1. Definiáljuk az előbbi változó alapértelmezett értékét. A `values.yaml` fájlban (egy könyvtárral feljebb) töröljünk ki mindent, és vegyük fel ezt a kulcsot:

    ```yaml
    todos:
      tag: v1
    ```

1. A másik két komponens YAML leíróival is hasonlóan kell eljárnunk: vegyünk fel egy-egy kulcsot komponensenként és hivatkozzunk a kulcsra az adott komponens pod leírójának `image:` értékében.

1. A továbbiakhoz el kell távolítanunk az előbb telepített alkalmazásunkat, mert összeakadna a Helm-mel. Ezt a parancsot a telepítéshez korábban használt `app` alkönyvtár szülőkönyvtárában adjuk ki:

    ```cmd
    kubectl delete -f app
    ```

1. Nézzük meg a template-eket kiértékelve.

    - A chartunk könyvtárából lépjünk eggyel feljebb, hogy a `todoapp` chart könyvtár az aktuális könyvtárban legyen.
    - Futtassuk le csak a template generálást a telepítés nélkül: 

        ```cmd
        helm install todoapp --debug --dry-run todoapp
        ```
        
    - A release-nek _todoapp_ nevet választottunk. Ez a Helm release azonosítója.

    - Konzolra megkapjuk a kiértékelt YAML-öket. Ellenőrizzük a kimenetben, hogy a `release` és `tag` változók rendben behelyettesítődtek.

1. Telepítsük (újra) az alkalmazást a chart segítségével:
    
    ```cmd
    helm upgrade todoapp --install todoapp
    ```

    - Az `upgrade` parancs és az `--install` kapcsoló telepít, ha nem létezik, ill. frissít, ha már létezik ilyen telepítés.

1. Nézzük meg, hogy a Helm szerint létezik-e a release: `helm list`

1. Próbáljuk ki az alkalmazást a <http://localhost:32080> címen.

1. Ezen chart segítségével frissítsük egy új képpel az alkalmazásunkat. A korábban használt `docker compose build` paranccsal készítsük el az új docker image-eket, csak most *v2* taggel. Például Windows parancssorban:

    ```cmd
    $env:IMAGE_TAG="v2"
    docker compose build
    ```

1. A tag-et a values fájlból felül tudjuk definiálni telepítési paraméterben `--set`, pl. ha a "v2" az új tag, akkor egy paranccsal tudjuk frissíteni:

    ```cmd
    helm upgrade todoapp --install todoapp --set todos.tag=v2 --set web.tag=v2 --set users.tag=v2
    ```

!!! example "BEADANDÓ"
    Készíts egy képernyőképet (`f3.4.png`) és commitold azt be a házi feladat repó gyökerébe, ahol az alkalmazás futása látszik egy saját neptun kódot tartalmazó todoval.

    Készíts egy képernyőképet (`f3.5.png`) és commitold azt be a házi feladat repó gyökerébe, demonstrálod, hogy létrejött a helm release k8s-ben v2-es taggel.
