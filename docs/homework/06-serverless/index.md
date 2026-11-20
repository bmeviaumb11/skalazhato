# 06 - Serverless

*Nincs frissítve 2026. őszi félévre!*

## Cél

A labor célja megismerni az Azure elsődleges serverless technológiáját, az Azure Functions-t.

## Előkövetelmények

A feladatok az Azure portálon és az Azure Cloud Shell-ben végezhetők el, helyi eszközök telepítése nem szükséges.

- Azure [hallgatói előfizetés](https://azure.microsoft.com/en-us/free/students)
- böngésző

## Előkészület

A feladatok megoldása során ne felejtsd el követni a feladat beadásának folyamatát a [GitHub](../../information/GitHub.md) oldalon.

!!! danger "PR név"
    :exclamation: A beadásnál a pull request neve legyen: *hf6* :exclamation:

## 1. Feladat

### 1.1 Azure Functions Hello World

Végezd el a [*Guided project: Build a simple website endpoint with Azure Functions*](https://learn.microsoft.com/en-us/training/modules/guided-project-build-basic-website-endpoint-with-functions/) tananyagot (8 egység, kb. 36 perc). A tananyag végig az Azure portált és a Cloud Shell-t használja; a Function App [Flex Consumption](https://learn.microsoft.com/en-us/azure/azure-functions/flex-consumption-plan) hosting plan-re kerül, egy HTTP-triggerelt *GetStatus* függvénnyel.

Az Azure erőforrások létrehozásakor kövesd (amennyire lehetséges) az eddigi konvenciókat:

- Function App név: *azfun* előtag + neptun kód (pl. *npt123* neptun kód esetén *azfunnpt123*)
- *Secure unique default hostname*: ha a varázsló nem kínálja fel ezt az opciót, hagyd figyelmen kívül ezt a lépést.
- erőforráscsoport: használhatod a tananyagban megadott nevet, de adhatsz saját nevet is
- régió: a hallgatói előfizetésen engedélyezett régiók egyike (az engedélyezett régiók listázását lásd az [AKS házi](../04-aks/index.md) *Közös Azure régió* részében). Ha van korábbról Load Testing erőforrásod, elsőként annak a régióját érdemes megvizsgálni. Sajnos az AKS-hez hasonlóan megint további szempontokat is figyelembe kell venni, ezekhez lásd a lenti "Régióválasztási szempontok" részt. Az engedélyezett régiók közül minél több csillagos régiót válassz (a saját előfizetésedre a [function-region-check.sh](function-region-check.sh) script is kiírja az értékelést, pl. Cloud Shell-ben: `bash function-region-check.sh`):
    - ⭐⭐⭐⭐⭐ Flex Consumption, teszt-összerendelés és Azure Load Testing erőforrás is elérhető: *France Central* :flag_fr:, *Germany West Central* :flag_de:, *Italy North* :flag_it:, *North Europe* :flag_ie:, *Sweden Central* :flag_se:, *Switzerland North* :flag_ch:, *UK South* :flag_gb:, *West Europe* :flag_nl:
    - ⭐⭐⭐⭐ Flex Consumption és teszt-összerendelés elérhető, Azure Load Testing erőforrás nem: *Norway East* :flag_no:, *Poland Central* :flag_pl:, *UK West* :flag_gb:
    - ⭐⭐⭐ csak Flex Consumption érhető el: *Austria East* :flag_at:, *Belgium Central* :flag_be:, *France South* :flag_fr:, *Germany North* :flag_de:, *Norway West* :flag_no:, *Spain Central* :flag_es:, *Switzerland West* :flag_ch:

A régiós elérhetőség változhat (2026. novemberi állapot).

!!! info "Régióválasztási szempontok"
    A szempontok fontossági sorrendben:

    1. **Engedélyezett régió**: a hallgatói előfizetés házirendje (policy) más régióban nem engedi létrehozni az erőforrást.
    2. **Flex Consumption**: a tananyag és a 2.3-as feladat költségszámítása erre a hosting plan-re épül.
    3. **Teszt-összerendelés**: a 2.1-es feladatban a Function App *Load testing* menüpontjából hozzuk majd létre a terheléstesztet. Ez a Function App régiójában egy összerendelést (`loadTestMappings`) is létrehoz, ami alapján a menüpont listázza a Function App-hoz tartozó teszteket. Ahol az ehhez szükséges API nem elérhető, a teszt ugyanúgy létrejön (bár hibaüzenetet kapunk a létrehozáskor, mert a teszt után az összerendelés már nem tud létrejönni) és lefuttatható, de a Function App *Load testing* menüje üres marad.
    4. **Azure Load Testing erőforrás ugyanabban a régióban**: a 2.1-es feladat terheléstesztje az Azure Load Testing erőforrás régiójából hívja a függvényt. Nem kötelező, hogy a kettő egy régióban legyen.


!!! tip "Naplók megtekintése"
    A tananyag [*Exercise - Test endpoint and review logs*](https://learn.microsoft.com/en-us/training/modules/guided-project-build-basic-website-endpoint-with-functions/4-exercise-test-endpoint-logs) egységének *Task 6: Review invocation logs* lépésében:

    - a *Transaction search* menüpont az Application Insights erőforrás *Investigate* szekciójában már *Search* néven szerepel. A találatokat szűrd *Event types* = *Request*-re, így csak a függvényhívások látszanak (a naplóbejegyzések, függőségek stb. nem).
    - a *Logs* nézet alapértelmezetten egyszerű (*Simple*) módban nyílik meg, ahol nem lehet lekérdezést írni. A `requests | order by timestamp desc` lekérdezés beírásához előbb válts *KQL mode*-ra.

!!! warning "Hagyd meg"
    A *Clean up resources* egységet **ne hajtsd végre**: az törölné az erőforráscsoportot, de a Function App-ra a következő feladatban szükség lesz.

!!! example "BEADANDÓ"
    Készíts egy képernyőképet a Microsoft Learn portálról (`f1.1.png`) és commitold azt be a házi feladat repó gyökerébe, amin a tananyag elvégzéséről készült oklevél nyomtatási képe látható (angol portál esetében: jobb felső sarok ➡️ *Profile* ➡️ a bal oldali menüben *Achievements* ➡️ átváltás a *Modules* alfülre ➡️ tananyag kikeresése ➡️ a tananyag kártyáján a nyomtató ikon). Közvetlen [modul link](https://learn.microsoft.com/en-us/users/me/achievements?tab=tab-modules). Nem kell ténylegesen kinyomtatni, de a teljes oldal látszódjon, jobb felül a belépett felhasználó monogramjával. Alternatívaként a tananyag kártyáján a megosztás ikonnal egy linket generálhatunk; ezt a linket is be lehet adni ([példa](https://learn.microsoft.com/en-us/users/kszicsillag/achievements/uzhtj8u3)): ilyenkor az `f1.1.png` helyett egy `f1.1.txt` fájlba írd a linket, és azt commitold a házi feladat repó gyökerébe. Link esetén próbáld ki, hogy publikusan elérhető-e az oklevél, illetve ilyenkor a [privát mód](https://learn.microsoft.com/en-us/credentials/certifications/cred-share-validate#sharing-and-privacy-setting) ne legyen bekapcsolva.

    Készíts egy képernyőképet (`f1.2.png`) és commitold azt be a házi feladat repó gyökerébe, amin az Azure portálon a Function App áttekintő (*Overview*) nézete látszik a *GetStatus* függvénnyel, mellette pedig egy böngészőfülön a végpont kulccsal (`?code=<kulcs>`) történő hívásának sikeres válasza. A kulcsot a címsorban kitakarhatod. A kép jobb felső sarkában látszódjon a portálra belépett felhasználó.

## 2. Feladat

### 2.1 Terhelésteszt - spike

Ha nincs meg az előző háziból a terhelésteszt (Azure Load Testing) erőforrás, hozd létre (újra) az előző házi leírása alapján. Csak az erőforrás kell, nem kell bele teszt(eset).

!!! tip "Load Testing erőforrás régiója"
    Új Azure Load Testing erőforrás létrehozásakor a Function App régióját válaszd, ha az az 1.1-es feladat régiólistájában ⭐⭐⭐⭐⭐ értékelésű. Egyébként válassz egy engedélyezett, a Function App-hoz földrajzilag közeli ⭐⭐⭐⭐⭐ régiót. Hogy a te előfizetéseden melyek az engedélyezett ⭐⭐⭐⭐⭐ régiók, azt a [function-region-check.sh](function-region-check.sh) script kiírja (pl. ha egy hallgatói előfizetésen csak *Italy North* :flag_it: az, akkor egy *Poland Central* :flag_pl: Function App mellé is ott hozd létre).

Az Azure portálon keresd ki és nyisd meg az előző feladatban létrehozott *Function App*-ot. A *Load Testing* menüpont segítségével [hozz létre és futtass](https://learn.microsoft.com/en-us/azure/app-testing/load-testing/how-to-create-load-test-function-app) egy kiugrást (spike) szimuláló terheléstesztet.

Tesztterv (*Test Plan*):

- Load Testing resource: válasszuk ki az Azure Load Testing erőforrásunkat
- Test name: maradhat a generált érték, de át is írhatjuk

Tesztterv (*Test Plan*) / HTTP kérések (*Requests*):

- Request name: mi adjuk meg, pl. *GetStatusRequest*
- Function name: *GetStatus* (egy van - automatikusan kitöltődik)
- Key: a függvény kulcsa (automatikusan kitöltődik; a varázsló ez alapján hitelesíti a kéréseket, így nem kapunk 401-es hibát)
- HTTP method: *GET* (automatikusan kitöltődik)
- Opcionális header, query paraméter, stb. értékek: nem kell kitölteni.

!!! warning "Létrehozás nem a Function App-ból"
    A fenti automatikus kitöltések csak akkor működnek, ha a tesztet a Function App *Load testing* menüpontjából hozod létre. Ha a tesztet máshonnan (pl. közvetlenül az Azure Load Testing erőforrásban) hozod létre, ezeket kézzel kell megadnod:

    - URL: a függvény teljes URL-je (*Function App* ➡️ *GetStatus* függvény ➡️ *Get function URL*)
    - HTTP method: *GET*
    - kulcs: query paraméterként (név: `code`, érték: a függvény kulcsa), különben a kérések 401-es hibát kapnak

    Ilyenkor a Function App *Load testing* menüje sem fogja listázni a tesztet.

Terhelés (*Load*):

- Engine instances: *1*
- Load pattern: *Spike*
- Concurrent users per engine: *50*
- Test duration (minutes): *5*
- Spike load multiplier: *5*
- Spike hold time (minutes): *1*
- Load distribution: elég az alapértelmezett 1 db
- Test traffic mode: *public*

Ellenőrzés és létrehozás (*Review + create*):

- Run test after creation: bekapcsolni

A *Run test after creation* opció bekapcsolása miatt a teszt elkészülte után le is fog futni, nem kell külön elindítani. Várjuk meg, amíg a teszt lefut (az Azure Load Testing erőforrásunk *Tests* menüpontjában tudjuk követni).

!!! warning "Teszt-összerendelés nélküli régiók"
    Ha a Function App az 1.1-es feladat régiólistájában ⭐⭐⭐ értékelésű régióban van (pl. *Austria East* :flag_at:, *Belgium Central* :flag_be:, *Spain Central* :flag_es:), a teszt létrehozásakor hibaüzenetet kapsz, mert a Function App és a teszt közti összerendelés nem jön létre. A teszt ilyenkor is létrejön, de a *Run test after creation* beállítás ellenére nem indul el, és a Function App *Load testing* menüje is üres marad. A tesztet az Azure Load Testing erőforrás *Tests* menüpontjában keresd meg, és onnan indítsd el (*Run*); a futását is itt tudod követni.

### 2.2 Terhelésteszt monitorozás Azure Function oldalról

Az Azure Function metrikák (*Metrics*) menüpontjában [monitorozzuk](https://learn.microsoft.com/en-us/azure/azure-functions/monitor-functions?tabs=portal#analyze-metrics-for-azure-functions) a terhelésteszt lefutását:

- a megfigyelt [metrika](https://learn.microsoft.com/en-us/azure/azure-functions/monitor-functions-reference?tabs=flex-consumption-plan#metrics) legyen az *On Demand Function Execution Count*
- aggregáció: *Sum*
- a jobb felső sarokban úgy állítsuk be az időtartamot, hogy a teszt miatti kiugrás (*spike*) jól látható legyen; ugyanitt a felbontás (*time granularity*) legyen 1 perc
- grafikon típusa: oszlopdiagram (*Bar chart*)

!!! example "BEADANDÓ"
    Készíts egy képernyőképet az Azure portálról (`f2.1.png`) és commitold azt be a házi feladat repó gyökerébe, amin látható az *On Demand Function Execution Count* metrikából készített oszlopdiagram, és a terhelésteszt hatása (kiugrás). A kép jobb felső sarkában látszódjon a belépett felhasználó, a bal felső sarka környékén a Function neve.

### 2.3 Költségszámítás metrika alapján

Számoljunk becsült költséget a terhelésteszt Azure Function oldalára (a terhelésteszt lefuttatásának is van költsége az Azure Load Testing erőforrás oldalán). Ehhez az *On Demand Function Execution Units* metrikát figyeljük:

- aggregáció: *Sum*
- a jobb felső sarokban úgy állítsuk be az időtartamot, hogy a teszt miatti kiugrás jól látható legyen; ugyanitt a felbontás (*time granularity*) legyen hosszabb, mint a teszt (pl. 15 perc)
- grafikon típusa: oszlopdiagram (*Bar chart*)

Egyetlen oszlopnak kell kiemelkednie; ennek az értékét kell használni (ha az egeret fölötte tartjuk, megjelenik), amiből az ár végrehajtási idő (*execution time*) komponense kiszámolható.

!!! warning "Mértékegységváltás"
    Az érték mértékegysége MB*ms (megabájt-milliszekundum), az ár viszont GB*s-ban (gigabájt-szekundum) van megadva. Az átváltáshoz segítség az [útmutatóban](https://learn.microsoft.com/en-us/azure/azure-functions/functions-consumption-costs?tabs=flex-consumption-plan%2Cportal#function-app-level-metrics).

Az ár másik komponense a végrehajtási szám (*execution count*); ezt a fentihez hasonló módon az *On Demand Function Execution Count* metrika egyetlen oszloppá összenyomott grafikonjáról lehet leolvasni.

Számold ki a terhelésteszt költségét a végrehajtási szám és a végrehajtási idő értékekből, EUR pénznemben az [Azure Functions - Flex Consumption árlista](https://azure.microsoft.com/en-us/pricing/details/functions/) alapján úgy, hogy a benne foglalt erőforrásokat (*az első x db hívás ingyenes*) nem veszed figyelembe.

!!! tip "Always Ready példányok"
    Mivel *Always Ready* példányokat nem használtunk, így azok költségeivel nem kell számolni.


!!! example "BEADANDÓ"
    Készíts egy képernyőképet az Azure portálról (`f2.2.png`) és commitold azt be a házi feladat repó gyökerébe, amin látszik az *On Demand Function Execution Units* metrikából készített oszlopdiagramon a terhelésteszt hatása (kiugrás). A kép jobb felső sarkában látszódjon a belépett felhasználó, a bal felső sarka környékén a Function neve.

    Készíts egy másik képernyőképet (`f2.3.png`) és commitold ezt is a házi feladat repó gyökerébe, ahol az Azure portálon látszik a Function erőforráscsoportjának áttekintő nézete (*Overview*). Látszódjon a portálra belépett felhasználó azonosítója a jobb felső sarokban.

    Készíts egy szöveges fájlt (`f2.4.txt`) és commitold azt be a házi feladat repó gyökerébe, amiben egy lépésben levezeted (pl. `param1*param2/param3=ktg`) a költséget.

## 3. Feladat - talán a legfontosabb

!!! danger "Erőforrások törlése"
    Beadás után törölj minden erőforráscsoportot az előfizetéseden belül. (Kivéve esetleg ami nem a kisházikhoz kellett.)
