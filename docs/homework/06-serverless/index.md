# 06 - Serverless

*Nincs frissítve a 2025. őszi félévre!*

## Cél

A labor célja megismerni az Azure elsődleges serverless technológiáját, az Azure Functions-t.

## Előkövetelmények

A laborleírás cross-platform eszközöket használ. A labor Linuxon (Kubuntu v24.04) lett kidolgozva.

- Azure [hallgatói előfizetés](https://azure.microsoft.com/en-us/free/students)
- A többit lásd [itt](https://learn.microsoft.com/en-us/training/modules/develop-azure-functions/5-create-function-visual-studio-code)

## Előkészület

A feladatok megoldása során ne felejtsd el követni a feladat beadásának folyamatát a [GitHub](../../information/GitHub.md) oldalon.

!!! danger "PR név"
    :exclamation: A beadásnál a pull request neve legyen: *hf6* :exclamation:

## 1. Feladat

### 1.1 Azure Functions bevezető anyag

Végezd el az [*AZ-204: Implement Azure Functions*](https://learn.microsoft.com/en-us/training/paths/implement-azure-functions/) tananyagot (2 modul, összesen 53 percben). Az [*Exercise: Create an Azure Function by using Visual Studio Code*](https://learn.microsoft.com/en-us/training/modules/develop-azure-functions/5-create-function-visual-studio-code) rész kapcsán lásd a következő alfeladatot.

### 1.2 Azure Functions Hello World

Az [*Exercise: Create an Azure Function by using Visual Studio Code*](https://learn.microsoft.com/en-us/training/modules/develop-azure-functions/5-create-function-visual-studio-code) részben az Azure erőforrások létrehozásakor kövesd (amennyire lehetséges) az eddigi konvenciókat:

- név: *azfun* előtag + neptun kód (pl.*npt123* neptun kód esetén *azfunnpt123*)
- régió: közös Azure régió (lásd AKS házi)
 - erőforráscsoport: alapértelmezésben egy új erőforráscsoport jön létre ugyanolyan névvel, mint amit névként megadtunk a *Function App*-nak. Ha ez nem tetszik, indítsd el a [komplexebb (advanced) varázslót].


!!! info "Azure Function hosting"
    Alapértelmezés szerint [Flex Consumption](https://learn.microsoft.com/en-us/azure/azure-functions/flex-consumption-plan) típusú lesz a létrejövő Function App hosting plan-je.

!!! warning "Hagyd meg"
    A tananyag elvégzése után ne töröld a Function-t, a következő feladatban szükség lesz rá.

!!! example "BEADANDÓ"
    Készíts egy képernyőképet a Microsoft Learn portálról (`f1.1.png`) és commitold azt be a házi feladat repó gyökerébe, amin a tananyag elvégzéséről készült oklevél nyomtatási képe látható (angol portál esetében: jobb felső sarok ➡️ *Profile* ➡️ a bal oldali menüben *Achievements* ➡️ átváltás a *Learning Path* alfülre ➡️ tananyag kikeresése ➡️ a tananyag kártyáján a nyomtató ikon). Közvetlen [learning path link](https://learn.microsoft.com/en-us/users/me/achievements?tab=tab-learning-paths). Nem kell ténylegesen kinyomtatni, de a teljes oldal látszódjon, jobb felül a belépett felhasználó monogramjával. Alternatívaként a tananyag kártyáján a megosztás ikonnal egy linket generálhatunk; ezt a linket is be lehet adni ([példa](https://learn.microsoft.com/api/achievements/share/en-us/kszicsillag/JL4MHR2T?sharingId=C9ECDF4DA28799DD)). Link esetén próbáld ki, hogy publikusan elérhető-e az oklevél, illetve ilyenkor a [privát mód](https://learn.microsoft.com/en-us/credentials/certifications/cred-share-validate#sharing-and-privacy-setting) ne legyen bekapcsolva.
    
    Készíts egy képernyőképet (`f1.2.png`) és commitold azt be a házi feladat repó gyökerébe, amin a Visual Studio Code Azure erőforrásokat mutató ablakában az erőforrásfa ki van bontva úgy, hogy látszódjon az 1.2-es feladat Azure Function-je. Illetve látszódjon a sikeres válasz pop-up-ja is. 

## 2. Feladat

### 2.1 Terhelésteszt - spike

Hozz létre egy terhelésteszt (Azure Load Testing) erőforrást az előző házi alapján. Csak az erőforrás kell, nem kell bele teszt(eset).

Az Azure portálon keresd ki és nyisd meg az előző feladatban létrehozott *Function App*-ot. A *Load Testing (Preview)* menüpont segítségével [hozz létre és futtass](https://learn.microsoft.com/en-us/azure/load-testing/how-to-create-load-test-function-app) egy kiugrást (spike) szimuláló terheléstesztet.

Tesztterv (*Test Plan*):

- Load Testing resource: válasszuk ki az Azure Load Testing erőforrásunkat
- Test name: maradhat a generált érték, de át is írhatjuk

Tesztterv (*Test Plan*) / HTTP kérések (*Requests*):

- Request name: mi adjuk meg, pl. *HttpExampleRequest* 
- Function name: *HttpExample* (egy van - automatikusan kitöltődik)
- Key: *anonymous (no auth)* (automatikusan kitöltődik)
- HTTP method: *GET* (automatikusan kitöltődik)
- Opcionális header, query paraméter, stb. értékek: nem kell kitölteni.

Terhelés (*Load*):

- Engine instances:	*1*
- Load pattern: *Spike*
- Concurrent users per engine: *50*
- Test duration (minutes): *5*
- Spike load multiplier : *5*
- Spike hold time (minutes) : *3*
- Load distribution: elég az alapértelmezett 1 db
- Test traffic mode: *public*

Ellenőrzés és létrehozás (*Review + create*):

- Run test after creation: bepipálni

A *Run test after creation* opció bekapcsolása miatt a teszt elkészülte után le is fog futni, nem kell külön elindítani. Várjuk meg, amíg a teszt lefut (az Azure Load Testing erőforrásunk Tests menüpontjában tudjuk követni).

### 2.2 Terhelésteszt monitorozás Azure Function oldalról

Az Azure Function metrikák (*Metrics*) menüpontjában [monitorozzuk](https://learn.microsoft.com/en-us/azure/azure-functions/monitor-functions?tabs=portal#analyze-metrics-for-azure-functions) a terhelésteszt lefutását:

- a megfigyelt [metrika](https://learn.microsoft.com/en-us/azure/azure-functions/monitor-functions-reference?tabs=flex-consumption-plan#metrics) legyen az *On Demand Function Execution Count*
- aggregáció: *Sum*
 - a jobb felső sarokban úgy állítsuk be az időtartamot, hogy a teszt miatti kiugrás (*spike*) jól látható legyen; ugyanitt a felbontás (*time granularity*) legyen 1 perc
- grafikon típusa: oszlopdiagram (*Bar chart*)

!!! example "BEADANDÓ"
    Készíts egy képernyőképet az Azure portálról (`f2.1.png`) és commitold azt be a házi feladat repó gyökerébe, amin látható a *On Demand Function Execution Count* metrikából készített oszlopdiagram, és a terhelésteszt hatása (kiugrás). A kép jobb felső sarkában látszódjon a belépett felhasználó, a bal felső sarka környékén a Function neve.

### 2.3 Költségszámítás metrika alapján

Számoljunk becsült költséget a terhelésteszt Azure Function oldalára (a terhelésteszt lefuttatásának is van költsége az Azure Load Testing erőforrás oldalán). Ehhez az *On Demand Function Execution Units* metrikát figyeljük:

- aggregáció: *Sum*
 - a jobb felső sarokban úgy állítsuk be az időtartamot, hogy a teszt miatti kiugrás jól látható legyen; ugyanitt a felbontás (*time granularity*) legyen hosszabb, mint a teszt (pl. 15 perc)
- grafikon típusa: oszlopdiagram (*Bar chart*)

Egyetlen oszlopnak kell kiemelkednie; ennek az értékét kell használni (ha az egeret fölötte tartjuk, megjelenik), amiből az ár végrehajtási idő (*execution time*) komponense kiszámolható.

!!! warning "Mértékegységváltás"
    Az érték mértékegysége MB*ms (megabájt-milliszekundum), az ár viszont GB*s-ban (gigabájt-szekundum) van megadva. Az átváltáshoz segítség az [útmutatóban](https://learn.microsoft.com/en-us/azure/azure-functions/functions-consumption-costs?tabs=flex-consumtion-plan%2Cportal#function-app-level-metrics).

 Az ár másik komponense a végrehajtási szám (*execution count*); ezt a fentihez hasonló módon az *On Demand Function Execution Count* metrika egyetlen oszloppá összenyomott grafikonjáról lehet leolvasni.

Számold ki a terhelésteszt költségét a végrehajtási szám és a végrehajtási idő értékekből, EUR pénznemben az [Azure Functions - Flex Consumptions árlista](https://azure.microsoft.com/en-us/pricing/details/functions/) alapján úgy, hogy a bennefoglalt erőforrásokat (*az első x db. hívás ingyenes*) nem veszed figyelembe.

!!! tip "Always Ready példányok"
    Mivel *Always Ready* példányokat nem használtunk, így azok költségeivel nem kell számolni.


!!! example "BEADANDÓ" 
    Készíts egy képernyőképet az Azure portálról (`f2.2.png`) és commitold azt be a házi feladat repó gyökerébe, amin látszik az *On Demand Function Execution Unit* metrikából készített oszlopdiagramon a terhelésteszt hatása (kiugrás). A kép jobb felső sarkában látszódjon a belépett felhasználó, a bal felső sarka környékén a Function neve.

    Készíts egy másik képernyőképet (`f2.3.png`) és commitold ezt is a házi feladat repó gyökerébe, ahol az Azure portálon látszik a Function erőforráscsoportjának áttekintő nézete (*Overview*). Látszódjon a portálra belépett felhasználó azonosítója a jobb felső sarokban.
 
    Készíts egy szöveges fájlt (`f2.4.txt`) és commitold azt be a házi feladat repó gyökerébe, amiben egy lépésben levezeted (pl. `param1*param2/param3=ktg`) a költséget.

## 3. Feladat - talán a legfontosabb

!!! danger "Erőforrások törlése"
    Beadás után törölj minden erőforráscsoportot az előfizetéseden belül. (Kivéve esetleg ami nem a kisházikhoz kellett.)
