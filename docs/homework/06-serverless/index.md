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
- régió: a hallgatói előfizetésen engedélyezett régiók egyike (az engedélyezett régiók listázását lásd az [AKS házi](../04-aks/index.md) *Közös Azure régió* részében). Ha a közös régióban elérhető a Flex Consumption, válaszd azt.
- erőforráscsoport: használhatod a tananyagban megadott nevet, de adhatsz saját nevet is

!!! tip "Naplók megtekintése"
    A tananyag [*Exercise - Test endpoint and review logs*](https://learn.microsoft.com/en-us/training/modules/guided-project-build-basic-website-endpoint-with-functions/4-exercise-test-endpoint-logs) egységének *Task 6: Review invocation logs* lépésében:

    - a *Transaction search* menüpont az Application Insights erőforrás *Investigate* szekciójában már *Search* néven szerepel. A találatokat szűrd *Event types* = *Request*-re, így csak a függvényhívások látszanak (a naplóbejegyzések, függőségek stb. nem).
    - a *Logs* nézet alapértelmezetten egyszerű (*Simple*) módban nyílik meg, ahol nem lehet lekérdezést írni. A `requests | order by timestamp desc` lekérdezés beírásához előbb válts *KQL mode*-ra.

!!! warning "Hagyd meg"
    A *Clean up resources* egységet **hagyd ki**: az törölné az erőforráscsoportot, de a Function App-ra a következő feladatban szükség lesz.

!!! example "BEADANDÓ"
    Készíts egy képernyőképet a Microsoft Learn portálról (`f1.1.png`) és commitold azt be a házi feladat repó gyökerébe, amin a tananyag elvégzéséről készült oklevél nyomtatási képe látható (angol portál esetében: jobb felső sarok ➡️ *Profile* ➡️ a bal oldali menüben *Achievements* ➡️ átváltás a *Modules* alfülre ➡️ tananyag kikeresése ➡️ a tananyag kártyáján a nyomtató ikon). Közvetlen [modul link](https://learn.microsoft.com/en-us/users/me/achievements?tab=tab-modules). Nem kell ténylegesen kinyomtatni, de a teljes oldal látszódjon, jobb felül a belépett felhasználó monogramjával. Alternatívaként a tananyag kártyáján a megosztás ikonnal egy linket generálhatunk; ezt a linket is be lehet adni ([példa](https://learn.microsoft.com/en-us/users/kszicsillag/achievements/uzhtj8u3)). Link esetén próbáld ki, hogy publikusan elérhető-e az oklevél, illetve ilyenkor a [privát mód](https://learn.microsoft.com/en-us/credentials/certifications/cred-share-validate#sharing-and-privacy-setting) ne legyen bekapcsolva.

    Készíts egy képernyőképet (`f1.2.png`) és commitold azt be a házi feladat repó gyökerébe, amin az Azure portálon a Function App áttekintő (*Overview*) nézete látszik a *GetStatus* függvénnyel, mellette pedig egy böngészőfülön a végpont kulccsal (`?code=<kulcs>`) történő hívásának sikeres válasza. A kulcsot a címsorban kitakarhatod. A kép jobb felső sarkában látszódjon a portálra belépett felhasználó.

## 2. Feladat

### 2.1 Terhelésteszt - spike

Ha nincs meg az előző háziból a terhelésteszt (Azure Load Testing) erőforrás, hozd létre (újra) az előző házi leírása alapján. Csak az erőforrás kell, nem kell bele teszt(eset).

Az Azure portálon keresd ki és nyisd meg az előző feladatban létrehozott *Function App*-ot. A *Load Testing* menüpont segítségével [hozz létre és futtass](https://learn.microsoft.com/en-us/azure/app-testing/load-testing/how-to-create-load-test-function-app) egy kiugrást (spike) szimuláló terheléstesztet.

Tesztterv (*Test Plan*):

- Load Testing resource: válasszuk ki az Azure Load Testing erőforrásunkat
- Test name: maradhat a generált érték, de át is írhatjuk

Tesztterv (*Test Plan*) / HTTP kérések (*Requests*):

- Request name: mi adjuk meg, pl. *GetStatusRequest*
- Function name: *GetStatus* (egy van - automatikusan kitöltődik)
- Key: maradhat az automatikusan kitöltött érték; a kulcsot query paraméterként adjuk át (lásd lent)
- HTTP method: *GET* (automatikusan kitöltődik)
- Query parameters: név: `code`, érték: a *GetStatus* függvény kulcsa, amit az első feladatban is kellett használnunk, hogy ne kapjunk 401-es hibát
- Egyéb opcionális értékek (header, body stb.): nem kell kitölteni.

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
