# 01 - Konténerizáció

## Cél

A labor célja megismerni a Docker konténerek használatának alapjait és a leggyakrabban használt Docker CLI parancsokat.

## Előkövetelmények

- Docker Desktop
    - A házi leírásban Windows platformot használunk, azonban a feladatok Linuxon és Mac-en is megoldhatóak (a könyvtár elérési útvonalakat megfelelően átírva).
    - Docker Hub login
- Docker-compose
    - Csak Linux esetén szükséges [külön telepíteni](https://docs.docker.com/compose/install/)
- Microsoft Visual Studio Code
    - Javasolt: [Docker extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-docker)
- Alap Linux parancsok ismerete. Érdemes átnézni pl.:
    - <http://bmeaut.github.io/snippets/snippets/0700_LinuxBev/>
    - <https://maker.pro/linux/tutorial/basic-linux-commands-for-beginners>
    - <https://www.pcsuggest.com/basic-linux-commands/>

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

### Docker _hello world_

Teszteljük a Docker telepítésünket a `hello-world` image futtatásával.

Nyissunk egy konzolt, és adjuk ki a következő parancsokat.
Ezzel ellenőrizhetjük, hogy a docker CLI elérhető-e.

```cmd
docker --version
```

Futtassunk egy egyszerű előre elkészített konténert, ami kiír egy példa szöveget a konzolra.

```cmd
docker run hello-world
```

- _hello-word_ az image neve: <https://hub.docker.com/_/hello-world>
- Image letöltődik, elindul, lefut a benne leírt program.

### Konténer futtatása interaktív módon

- Futtassunk egy natúr `ubuntu` konténert interaktív módon (`-it` kapcsolóval), így a futó konténerben egy shell-en keresztül tudunk tetszőleges parancsokat futtatni.

    ```cmd
    docker run -it ubuntu
    ```

- Nézzük meg a fájlrendszert:

    ```cmd
    ls
    ```

- Lépjünk ki az interaktív shell-ből:

    ```cmd
    exit
    ```

    Konténer terminált, mert a bash folyamat megállt az `exit` hatására. **Konténer addig fut, amíg a benne levő alkalmazás (folyamat) fut.**
    Leállt konténer nem törlődik automatikusan, tartalma nem veszik el.

- Listázzuk ki a konténereinket:

    ```cmd
    docker ps -a
    ```

    Nézzük meg a parancs eredményét. Keressük meg a konténerek id-ját.

- Távolítsuk el a két konténert, amit mi indítottunk:

    ```cmd
    docker rm <id1|name> <id2|name>
    ```

    !!! tip 
        ID helyett a konténer nevét is megadhatjuk. Az automatikusan generált nevek helyett pedig a `run` parancs `--name` kapcsolójával adhatunk nevet a konténernek.

??? note "Gyakoribb Docker parancsok"
    - Adjuk ki a `docker` parancsot a help-hez.
    - Adminisztratív parancsok: _mivel mit_, pl. `docker image ls`
    - Kezelő parancsok: _parancs argumentumok_, pl. `docker rmi <id>`
    - Gyakran használtak:
      - Konténerek kezelése
        - `docker container ls [-all]` vagy `docker ps [-a]`
        - `docker run [opciók] <image>`
        - `docker stop <id>`
        - `docker rm <id>`
      - Image-ek kezelése
        - `docker pull <image>`
        - `docker image ls` vagy `docker images`
        - `docker rmi <image>`
        - `docker tag <id> <tag>`
    - Konkrét parancshoz segítség: `docker <parancs> --help`
    - Minden konténer (futók is!) eltávolítása: `docker rm -f $(docker ps -aq)`

!!! note "GUI vs CLI"
    A Dockerhez már nagyon sok hasznos GUI-val rendelkező eszköz létezik, amik nagyban megkönnyítik a fejlesztők életét.
    A labor viszont elsősorban konzolos használatra fókuszál, mert az a leguniverzálisabb és a GUI-s eszközök is valójában az itt elérhető funkciókra építenek.
    A labor elvégzése során ha kényelmesebb nyugodtan használd a számodra kényelmesebb eszközt.

    - [Docker Desktop](https://www.docker.com/products/docker-desktop) felülete
    - [VS Code Docker extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-docker)
    - Visual Studio Container Tools ablak

## 1. Feladat

### 1.1 _Volume_ csatolása (_bind mount_)

Gyakran szeretnénk a host gépről elérni a konténerben lévő fájlokat, vagy éppen a konténerben lévő fájlokat szeretnénk a host gépen tárolni.
Erre megoldás a _volume_ csatolás, amikor a host gép egy könyvtárát csatoljuk a konténerbe.

!!! danger "NEPTUN"
    :exclamation: A példákban a `neptun` helyett a **saját neptunkódunkat** helyettesítsük be :exclamation:

- Hozzunk létre egy munkakönyvtárat tetszőleges helyen a neptun kódunkkal, például `c:\work\neptun` (windows) `~/work/neptun` (linux)
    
- Indítsunk el egy konténert úgy, hogy ezt a könyvtárat felcsatoljuk a `-v` kapcsolóval:

    ```cmd title="Windows"
    docker run -it --rm -v c:\work\neptun:/neptun ubuntu
    ```

    ```cmd title="Linux"
    docker run -it --rm -v ~/work/neptun:/neptun ubuntu
    ```

    Szintaktika: helyi teljes elérési útvonal _kettőspont_ konténeren belüli teljes elérési útvonal

- Konténeren belül listázzuk ki a könyvtárat:

    ```cmd
    ls
    ```

    Látjuk a `/neptun` könyvtárat

- Írjunk bele: 

    ```cmd
    echo "hello NEPTUN" > /neptun/hello.txt
    ```

- írjuk ki a tartalmát: 

    ```cmd
    cat /neptun/hello.txt
    ```

- Lépjünk ki a konténerből:

    ```cmd
    exit
    ```

- Nézzük meg a munkakönyvtárunkat.

!!! tip "`--rm`"
    A `docker run` `--rm` opciója törli a konténert leállás után; pl. teszteléshez hasznos, mint most.

!!! example "BEADANDÓ"
    Készíts egy képernyőképet (f1.1.png) és commitold azt be a házi feladat repó gyökerébe, amin a fenti _Volume csatolás_ feladatok parancsainak eredményei láthatóak.

### 1.2 Port mappelés

Docker konténerek esetében gyakran webalkalmazásokat futtatunk, amiket a host gépről szeretnénk elérni. Ezt a port mappeléssel érhetjük el, ahol a host gép egy portját mappeljük a konténer egy portjára.

- Indítsunk el egy _nginx_ webszervert tartalmazó konténert:

    ```cmd
    docker run -d -p 8085:80 nginx
    ```

    - `-d` (detach): háttérben fut, a konzolt "visszakaptunk", amint elindult a konténer, és kiírja az image id-t
    - `-p` (port): helyi port _kettőspont_ konténeren belüli port

- Nyissuk meg böngészőben a címet a neptun kódunkkal: <http://localhost:8085/index.html?student=NEPTUN>

- Nézzük meg a konténer logjait:

    ```cmd
    docker logs <id|name>
    ```

- Állítsuk le a konténert:

    ```cmd
    docker stop <id|name>
    ```

!!! example "BEADANDÓ"
    Készíts egy képernyőképet (f1.2.png) és commitold azt be a házi feladat repó gyökerébe, amin a fenti webcím megnyitásának a logjai látszódnak. Emeld ki a képen a releváns logbejegyzést (pl.: karikázd be vagy sárga kiemelevőlvel színezd.)

### 1.3 Műveletvégzés futó konténerben

Gyakran már egy futó konténerben szeretnénk műveleteket végezni, pl. fájlokat nézni, módosítani, stb.
Ehhez a `docker exec` és `docker cp` parancsot használjuk most.

- Indítsunk el egy _nginx_ webszervert:

    ```cmd
    docker run -d -p 8085:80 nginx
    ```

    Jegyezzük meg a kiírt konténer id-t, alább használni fogjuk.

- Futtassunk le egy parancsot a konténerben:

    ```cmd
    docker exec <id|name> ls /
    ```

    A parancs kilistázta a konténer fájlrendszerének gyökerét.

- Kérhetünk egy shell-t is a konténerbe ily módon:

    ```cmd
    docker exec -it <id|name> /bin/bash
    ```
  
    - Az `-it` opció az interaktivitásra utal, azaz a konzolunkat "hozzáköti" a konténerben futó shellhez.
    - Tipikusan vagy `/bin/bash` vagy `/bin/sh` a Linux konténerekben a shell. Utóbbi az [_alpine_](https://alpinelinux.org/) alapú konténerekben gyakori.
    - Ebben az interaktív shell-ben bármit csinálhatunk, beléphetünk könyvtárakba, megnézhetünk fájlokat, stb. Arra viszont ügyeljünk, hogy az így végzett módosításaink **elvesznek**, amikor a konténer törlésre kerül!

- Például nézzük meg az nginx konfigurációját:

    ```cmd
    cat /etc/nginx/conf.d/default.conf
    ```

- Módosítsuk az `index.html`-t a következő módon:

    ```cmd
    echo "hello NEPTUN from nginx" > /usr/share/nginx/html/index.html
    ```

- Nyissuk meg böngészőben ezt a címet: <http://localhost:8085/index.html> és ellenőrizzük, hogy a módosított tartalom látszik-e.

- Lépjünk ki az `exit` utasítással. Ez csak a "második" shellt állítja le, a konténer még fut, mert az eredeti indítási pont is még fut.

- Ha szükségünk van egy fájlra, akkor azt kimásolhatjuk a futó konténerből:

    ```cmd title="Windows"
    docker cp <id|name>:/etc/nginx/conf.d/default.conf c:\work\neptun\nginx.conf
    ```

    ```cmd title="Linux"
    docker cp <id|name>:/etc/nginx/conf.d/default.conf ~/work/neptun/nginx.conf
    ```
    
    - Szintaktikája: `docker cp <id|name>:</full/path> <cél/hely>`
    - A másolás az ellenkező irányba is működik, helyi gépről a konténerbe.

- Állítsuk le és töröljük a konténert.

    ```cmd
    docker stop <id|name>
    docker rm <id|name>
    ```

!!! example "BEADANDÓ"
    Készíts egy képernyőképet (f1.3.png) és commitold azt be a házi feladat repó gyökerébe, amin a fenti weboldal látszik a böngészőben.

## 2. Feladat

### 2.1 Image készítése parancssorból

??? note "Docker registry"
    Korábban használt parancs: `docker run ubuntu` Az _ubuntu_ az image neve. Ez egy un. registry-ből jön, analóg más csomagkezelőkhöz pl.: NPM, NuGet stb.
    Az alapértelmezett registry a <https://hub.docker.com>, ahol Tipikusan open-source szoftverek image-ei, és az általunk is használt alap image-ek találhatóak.
    Léteznek természetesen továbbiak is (Azure, Google, stb.)

    Az Image neve valójában nem `ubuntu`, hanem `index.docker.io/ubuntu:latest`

      - `index.docker.io` registry szerver elérési útvonala
      - `ubuntu` image neve (lehet többszintű is)
      - `:latest` tag neve

    Jogosultság szempontból két fajta registry létezhet: publikus (pl. Docker Hub) és privát. Privát registry esetén: `docker login <url>` és `docker logout <url>` szükséges az authentikációhoz.

    Letöltés a registry-ből: `docker pull mcr.microsoft.com/dotnet/aspnet:8.0`
    
    Ugyan a _run_ parancs is letölti, de csak akkor, ha még nem létezik. Nem ellenőrzi viszont, hogy nincs-e újabb image verzió publikálva. A _pull_ mindig frisset szed le.

Készítünk egy saját image-et, ami egy módosított `nginx` image-et fog tartalmazni a saját tartalmunkkal.
Ehhez az előző feladatban lévő lépéseket kell ismét elvégezned, de most ne lépj ki a konténerből, hanem a konténer állapotát mentsd egy új image-be, ami az nginx-re épít, és egy új [image layer](https://docs.docker.com/v17.09/engine/userguide/storagedriver/imagesandcontainers/)-t tartalmaz a saját tartalmunkkal.

1. Az előző feladat alapján futtass egy nginx konténert, amiben módosítod az `index.html` tartalmát.

1. Készíts egy pillanatmentést a konténer jelenlegi állapotáról:

    ```cmd
    docker commit <id|name>
    ```

1. Állítsd le a háttérben futó konténert:

    ```cmd
    docker stop <id|name>
    ```

1. Az előbbi parancs készített egy image-et, aminek kiírta a hash-ét. Ellenőrizd, hogy tényleg létezik-e ez az image:

    ```cmd
    docker images
    ```

1. Taggeld meg az image-et:

    ```cmd
    docker tag <imageid> nginx-neptun
    ```

    !!! warning
        Itt már az image id-ja kell az images listából!

1. Indíts el egy új konténert az előbb létrehozott saját image-ből:

    ```cmd
    docker run -it --rm -p 8086:80 nginx-neptun
    ```

    !!! warning
        A portszám szándékosan más, hogy biztosan legyünk benne, nem a korábban futóhoz csatlakozunk - ha mégsem állítottuk volna azt le.

1. Nyisd meg böngészőből a <http://localhost:8086> címet. Látható, hogy ez a módosított tartalmat jeleníti meg. Tehát `nginx-neptun` néven létrehoztunk egy saját image-et.

!!! example "BEADANDÓ"
    Készíts egy képernyőképet (f2.1.png) és commitold azt be a házi feladat repó gyökerébe, amin a fenti weboldal látszik a böngészőben és a konténert futtató parancs a terminálban és annak a logjai.

!!! tip "Takarítás"
    Fejlesztés közben sok ideiglenes image keletkezik, és konténereket hagyunk hátra.
    Add ki a következő parancsot a nem futó konténerek törléséhez és az ideiglenes (címke nélküli) image-ek törléséhez:

    ```cmd
    docker system prune
    ```

### 2.2 Dockerfile

Az előző feladatban az image készítését manuálisan végeztük el, ami nem jól verziózható, és reprodukálhatósági problémákat okozhat.

Készítsünk egy egyszerű webalkalmazás Pythonban a Flask nevű keretrendszerrel, ami egy REST végpontot definiál és szolgál ki. Az adatokat (egy számláló) Redis adatbázisból olvassa és írja, és egy üdvözlő üzenetet ad vissza környezeti változó alapján.

1. Nyisd meg a házi repositorydat Visual Studio Code-ban.

1. Készíts a repository mappájába egy almappát `pythonweb` néven. A továbbiakban ennek a kontextusában dolgozz.

1. Készíts egy `app.py` fájlt az alábbi tartalommal.

    ```Python
    from flask import Flask
    from redis import Redis, RedisError
    import os
    import socket

    # Connect to Redis
    redis = Redis(host="redis", db=0, socket_connect_timeout=2, socket_timeout=2)

    app = Flask(__name__)

    # root endpoint
    # inrements and returns the number of visits from Redis
    # and says hello to the user from the NAME environment variable
    @app.route("/")
    def hello():
        try:
            visits = redis.incr("counter")
        except RedisError:
            visits = "<i>cannot connect to Redis, counter disabled</i>"

        html = "<h3>Hello {name}!</h3><b>Visits:</b> {visits}"
        return html.format(name=os.getenv("NAME", "world"), visits=visits)

    if __name__ == "__main__":
        app.run(host='0.0.0.0', port=80)
    ```

1. Készíts egy `requirements.txt` fájlt az alábbi tartalommal, ami a Python alkalmazásunk függőségeit tartalmazza.

    ```txt
    Flask
    Redis
    ```

1. Készíts egy `Dockerfile` nevű fájt (kiterjesztés nélkül!) az alábbi tartalommal.
   A Dockerfile egy szöveges fájl, ami tartalmazza az image létrehozásának lépéseit, mint egy recept.

    ```dockerfile
    FROM python:3.12-slim

    WORKDIR /app
    COPY . /app
    
    RUN pip install --trusted-host pypi.python.org -r requirements.txt
    EXPOSE 80

    # Ide a saját Neptun kódodat írd
    ENV NAME=NEPTUN 

    CMD ["python", "app.py"]
    ```

1. Készítsd el a fenti fájlokból az image-et. Konzolból a munkakönyvtárban add ki a következő parancsot:

    ```cmd
    docker build -t python-neptun:v1 .
    ```

    Ez a parancs létrehoz egy image-et a Dockerfile alapján. A végén egy pont van, az is a parancs része, ami a build kontextust jelenti

    A Dockerfile lépései:
   
    - `FROM`: az alap image, amire építjük a sajátunkat. Mi most a Python 3.12-slim image-et használjuk.
    - `WORKDIR`: a konténerben a munkakönyvtár, a további műveletek ebben a könyvtárban lesznek.
    - `COPY`: a host gépről a konténerbe másoljuk a fájlokat. A `.` jelenti a build kontextus mappáját, esetünkben a `pythonweb` mappát.
    - `RUN`: a build/image készítés során lefuttatandó parancsok. Itt a `requirements.txt` fájlban felsorolt Python csomagokat telepítjük a python (pip) csomagkezelővel.
    - `EXPOSE`: a konténer által kiajánlott portokat jelzi. Mi most webalkalmazást készítünk, ezért a 80-as portot jelöljük ki.
    - `ENV`: környezeti változó beállítása. A `NAME` környezeti változó értéke a saját Neptun kódod legyen.
    - `CMD`: a konténer indításakor lefuttatandó parancs és argumentumai. Ebben az esetben a Python alkalmazásunkat indítjuk el.  

1. Ellenőrizd, hogy tényleg létrejött-e az image.

1. Indíts el egy új konténert ebből az image-ből:

    ```cmd
    docker run -it --rm -p 8085:80 python-neptun:v1
    ```

1. Nyisd meg böngészőben a <http://localhost:8085> oldalt.

!!! success ""
    A weboldal ki kell írja a neptun kódodat, és egy hibaüzenetet a Redis-szel kapcsolatban.

!!! example "BEADANDÓ"
    Készíts egy képernyőképet (f2.2.png) és commitold azt be a házi feladat repó gyökerébe, amin a fenti weboldal látszik a böngészőben és a konténer futtatás parancsa és logjai.

??? tip "Kitekintés: Dockerignore és build kontextus"

    A `Dockerfile`-ban hivatkoztunk az aktuális könyvtárra a `.`-tal. Vizsgáljuk meg, hogy ez mit is jelent.

    1. Készítsünk az aktuális könyvtárunkba, az `app.py` mellé egy nagy fájlt. PowerShell-ben addjuk ki a következő parancsot.

        ```powershell title="Windows - powershell"
        $out = new-object byte[] 134217728; (new-object Random).NextBytes($out); [IO.File]::WriteAllBytes("$pwd\file.bin", $out)
        ```

        ```bash title="Linux - bash"
        dd if=/dev/urandom of=./file.bin bs=1M count=1024
        ```

    2. Buildeljük le ismét a fenti image-et: `docker build -t python-neptun:v1 .`

        Menet közben látni fogjuk a következő sort a build logban: _Sending build context to Docker daemon 111.4MB_, és azt is tapasztalni fogjuk, hogy ez el tart egy kis ideig.

        A `docker build` parancs végén a `.` az aktuális könyvtár. Ezzel tudatjuk a Docker-rel, hogy a buildeléshez ezt a _kontextust_ használja, azon fájlok legyenek elérhetőek a build során, amelyek ebben a kontextusban vannak. A `Dockerfile`-ban a `COPY` így **relatív** útvonallal hivatkozik a kontextusban levő fájlokra.

        !!! warning "Elérhető fájlok"
            Ennek következménye az is, hogy csak a build kontextusban levő fájlokra tudunk hivatkozni. Tehát nem lehet pl. `COPY ..\..\file` használatával tetszőleges fájlt felmásolni a build közben.

    3. Ha a build kontextusból szeretnénk kihagyni fájlokat, hogy a build ne tartson sokáig, akkor egy `.dockerignore` fájlra lesz szükségünk (a `.gitignore` mintájára). 
       Ide szokás például a build környezet saját könyvtárait (`obj`, `bin`, `.vs`, `node_modules`, stb.) is felvenni.

        Készítsünk egy `.dockerignore`-t az alábbi tartalommal

        ```txt
        file.bin
        ```

    4. Futtassuk ismét a buildet. Így már gyorsabb lesz.

## 3. Feladat

### 3.1 Docker-compose

!!! warning "Linux eltérés"
    Linuxon a _compose_ nem egy külön parancs, hanem a docker parancs [kiterjesztése](https://docs.docker.com/compose/install/linux/). Emiatt nem `docker-compose` helyett `docker compose`-ként kell hívnunk.

A fenti alkalmazás egy része még nem működik. A Python alkalmazás mellett egy Redis-re is szükségünk lenne. Futtassunk több konténert egyszerre a docker compose segítségével.

1. Dolgozzunk a repository-n gyökerébe (tehát ne az előzőleg használt almappába) és Készítsünk ide egy `docker-compose.yaml` nevű fájlt az alábbi tartalommal.

    ```yaml
    services:
      redis:
        image: redis:7.2-alpine
        networks:
          - homework_network
      web:
        build: pythonweb
        ports:
          - 5000:80
        depends_on:
          - redis
        networks:
          - homework_network

    networks:
      homework_network:
        driver: bridge
    ```

    A fájl tartalmának magyarázata:

    - `services`: a szolgáltatások, amiket indítani szeretnénk
      - `redis`: egy Redis konténer, ami az `alpine` verziót használja
      - `web`: a saját image-ből épített Python alkalmazás konténere
        - `build`: a build kontextusban lévő mappából építi az image-t
        - `ports`: a konténer által kiajánlott portokat jelzi. Mi most webalkalmazást készítünk, ezért a konténer 80-as portját mappeljük a host 5000-es portjára.
        - `depends_on`: a konténer indításának sorrendjét jelzi. A `web` konténer csak akkor indul, ha a `redis` konténer már fut.
    - `networks`: a konténerek közötti hálózatokat definiálja. A `homework_network` nevű hálózatot használjuk, bridge módban, ami a konténerek közötti hálózatot jelenti. Ezt adtuk meg a konténerek `networks` tulajdonságában is.

2. Nyiss egy konzolt ugyanebbe a mappába. Indítsd el az alkalmazásokat az alábbi paranccsal:

    ```cmd
    docker-compose up --build
    ```

    Két lépésben a parancs: `docker-compose build` és `docker-compose up`

3. Nyisd meg böngészőben a <http://localhost:5000> oldalt.

4. Egy új konzolban nézd meg a futó konténereket.

    ```cmd
    docker ps
    ```

!!! example "BEADANDÓ"
    Készíts egy képernyőképet (f3.1.png) és commitold azt be a házi feladat repó gyökerébe, amin a fenti weboldal látszik a böngészőben és a futó konténerek listája a konzolban.

!!! note "docker-compose üzemeltetéshez"
    A docker-compose alkalmas üzemeltetésre is. A `docker-compose.yaml` fájl nem csak fejlesztői környezetet ír le, hanem üzemeltetéshez szükséges környezetet is. Ha a compose fájlt megfelelően írjuk meg (pl. használjuk a [`restart` direktívát](https://docs.docker.com/compose/compose-file/#restart) is), az elindított szolgáltatások automatikusan újraindulnak a rendszer indulásakor.

    Ugyanakkor a docker-compose nem helyettesíti a Kubernetes-t vagy más konténer orkesztrációs megoldásokat, mert azok sokkal komplexebb feladatokat is meg tudnak oldani (pl. skálázás, load balancing, stb.).

### 3.2 Több compose yaml fájl

A docker-compose parancsnak nem adtuk meg, hogy milyen yaml fájlból dolgozzon. Alapértelmezésként a `docker-compose.yaml` kiterjesztésű fájlt **és** ezzel összefésülve a `docker-compose.override.yaml` fájlt használja.

1. Készíts egy `docker-compose.override.yaml` fájlt a másik compose yaml mellé az alábbi tartalommal, amiben a redis konténer naplózását állítjuk át verbose szintre.

    ```yaml
    services:
      redis:
        command: redis-server --loglevel verbose
    ```

1. Indítsd el a rendszert.

    ```cmd
    docker-compose up
    ```

    A redis konténer részletesebben fog naplózni a `command` direktívában megadott utasítás szerint. Állítsd le a rendszert.

1. Nevezd át az előbbi override fájlt `docker-compose.debug.yaml`-re. 

1. Készíts egy új `docker-compose.prod.yaml` fájlt a többi yaml mellé az alábbi tartalommal

    ```yaml
    services:
      redis:
        command: redis-server --loglevel warning
    ```

1. Indítsuk el a rendszert az alábbi paranccsal

    ```cmd
    docker-compose -f docker-compose.yaml -f docker-compose.debug.yaml up
    ```

    A `-f` kapcsolóval tudjuk kérni a megadott yaml fájlok összefésülését.

!!! tip ""
    Általában a `docker-compose.yaml`-be kerülnek a közös konfigurációk, és a további fájlokba a környezet specifikus konfigurációk.

!!! example "BEADANDÓ"
    Készíts egy képernyőképet (f3.2.png) és commitold azt be a házi feladat repó gyökerébe, amin a fenti weboldal látszik a böngészőben és a részletesebb redis naplóbejegyzések.

## Opcionális: 4. Feladat

### Docker init

A `docker init` paranccsal egy megadott technológiához tartozó, docker alapú fejlesztéshez szükséges-hasznos fájlokat generáltathatjuk. A fájlok az adott technológiához illeszkedően készülnek, például ASP .NET Core esetén a megfelelő .NET alap lemezképekre hivatkozik a generált Dockerfile.

1. Készíts a repository mappájába egy almappát `aspnetweb` néven. A továbbiakban ennek az új mappának a kontextusában dolgozz.

1. Generálj egy ASP.NET Core alapú kiinduló projektet

    ```cmd
    dotnet new webapp
    ```

1. Generáld az ASP.NET Core-hoz tartozó docker fájlokat

    ```cmd
    docker init
    ```
    Ez a lépés létrehoz egy `Dockerfile`-t a projektben, ami ráadásul multi-stage build megoldást tartalmaz, ami a fordítási, publikálási és futtatási fázisokat különválasztja (több `FROM` utasítás amik egymásra hivatkoznak).
    Ezáltal biztosítható, hogy a .NET alkalmazásunk fordítása is reprodukálható legyen egy szeparált .NET SDK-t tartalmazó konténerben. A publikálás pedig egy kisebb méretű image-be történik, ami már csak a .NET futtatókörnyezetet tartalmazza.

    Ezen felül létrejön még .dockerignore fájl is, valamint egy egy service-t hivatkozó Docker compose is.

4. Futtassuk a docker compose configurációt (`docker-compose up` - Windows vagy `docker compose up` - Linux). Az alapértelmezett felkínál lehetőségek általában megfelelőek, csak végig kell ++enter++ -ezni. Böngészőben nyissuk meg a localhost címen a docker init-nek megadott portot pl. http://localhost:8080.

5. Listázzuk ki a futó konténereket egy külön konzolablakban:

    ```cmd
    docker ps
    ```

!!! example "BEADANDÓ"
    Készíts egy képernyőképet (f4.1.png) és commitold azt be a házi feladat repó gyökerébe, amin a fenti weboldal látszik a böngészőben és a futó konténerek listája.

## Kitekintés

### Image-ek használata

Konténer alapú fejlesztésnél tehát két féle image-et használunk:

- amit magunk készítünk egy adott alap image-re épülve,
- illetve kész image-eket, amiket csak futtatunk (és esetleg konfigurálunk).

Alap image-ek, amikre tipikusan saját alkalmazást építünk:

- Linux disztribúciók
    - [Ubuntu](https://hub.docker.com/_/ubuntu),
    - [Debian](https://hub.docker.com/_/debian),
    - [Alpine](https://hub.docker.com/_/alpine),
- Futtató platformok
    - [.NET Runtime ASP.NET Core-ral](mcr.microsoft.com/dotnet/aspnet:8.0), pl. `mcr.microsoft.com/dotnet/aspnet:8.0`
    - [NodeJS](https://hub.docker.com/_/node) pl. `node:22.4-alpine`
    - [Python](https://hub.docker.com/_/python) pl. `python:3.12-slim`
- [scratch](https://hub.docker.com/_/scratch): üres image, speciális esetek, pl. go, vagy distro készítéshez

A kész image-ek, amiket pedig felhasználunk:

- SDK-k multi stage buildhez
    - [.NET SDK](mcr.microsoft.com/dotnet/sdk:8.0), pl. `mcr.microsoft.com/dotnet/sdk:8.0`
- Adatbázis szerverek, webszerverek, gyakran használt szolgáltatások
    - MSSQL, redis, mongodb, mysql, nginx, ...
- Termérdek elérhető image: <https://hub.docker.com>

!!! warning "Verziózás fontos"
    Az image-ek verziózását minden esetben meg kell érteni! Minden image más-más megközelítést alkalmaz.

### Kész image testreszabása

Az előbb a _Redis_ memória alapú adatbázist minden konfiguráció nélkül felhasználtuk.
Gyakran az ilyen image-ek "majdnem" jók közvetlen felhasználásra, de azért szükség van egy kevés testreszabásra.
Ilyen esetben a következő lehetőségeink vannak:

- Saját image-et készítünk kiindulva a számunkra megfelelő alap image-ből.
  A saját image-ben módosíthatunk a konfigurációs fájlokat, avagy további fájlokat adhatunk az image-be.
  Ezt a megoldást alkalmazhatjuk például tipikusan weboldal kiszolgálásánál, ahol is a kiinduló image a webszerver, viszont a kiszolgálandó fájlokat még mellé kell tennünk.

- Környezeti változókon (esetleg argumentumokon) keresztül konfiguráljuk a futtatandó szolgáltatást.
  Az alap image-ek általában elég jó konfigurációval rendelkeznek, csak keveset kell rajta módosítanunk.
  Erre tökéletesen alkalmas egy-egy környezeti változó.
  A jól felépített Docker image-ek előre meghatározott környezeti változókon keresztül testreszabhatóak.

    Erre egy jó példa a [Microsoft SQL Server Docker változata](https://hub.docker.com/r/microsoft/mssql-server). Az alábbi parancsban a `-e` argumentumokban adunk át környezeti változókat, de lehetőség van compose fájlban is megadni ezeket.

    ```bash
    docker run
      -e 'ACCEPT_EULA=Y'
      -e 'SA_PASSWORD=yourStrong(!)Password'
      -p 1433:1433
      mcr.microsoft.com/mssql/server:2017-CU8-ubuntu
    ```

- Becsatolhatjuk a saját konfigurációs fájljainkat a konténerbe.
  Korábban láttuk, hogy lehetőségünk van egy mappát a host gépről a konténerbe csatolni.
  Ha elkészítjük a testreszabott konfigurációs fájlt, akkor a `docker-compose.yaml` leírásban a következő módon tudjuk ezt a fájlt becsatolni a konténer indulásakor.

    ```yaml
    services:
      redis:
        image: redis:7.2-alpine
        volumes:
          - my-redis.conf:/usr/local/etc/redis/redis.conf
    ```

    Ezen megoldás előnye, hogy nincs szükség saját image-et készíteni, tárolni, kezelni.
    Amikor a környezeti változó már nem elegendő a testreszabáshoz, ez a javasolt megoldás.

### További olvasnivaló

- _Dockerfile_ szintaktika: <https://docs.docker.com/reference/dockerfile/>
- _.dockerignore_ fájl szintaktika: <https://docs.docker.com/reference/dockerfile/#dockerignore-file>
- _Dockerfile_ best practice-ek: <https://docs.docker.com/build/building/best-practices/>
- _compose_ fájl szintaktika: <https://docs.docker.com/compose/compose-file/>
- Több compose fájl használata: <https://docs.docker.com/compose/multiple-compose-files/extends/#multiple-compose-files>
- Multistage build-ek: <https://docs.docker.com/build/building/multi-stage/>
