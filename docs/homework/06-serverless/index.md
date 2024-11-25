# 06 - Serverless

## Cél

A labor célja megismerni az Azure elsődleges serverless technológiáját, az Azure Functions-t.

## Előkövetelmények

A laborleírás cross-platform eszközöket használ. A labor linuxon (kubuntu v24.04) lett kidolgozva.

- Azure [hallgatói előfizetés](https://azure.microsoft.com/en-us/free/students)
- A többit lásd [itt](https://learn.microsoft.com/en-us/training/modules/develop-azure-functions/5-create-function-visual-studio-code)

## Előkészület

A feladatok megoldása során ne felejtsd el követni a feladat beadás folyamatát [GitHub](../../information/GitHub.md).

!!! danger "PR név"
    :exclamation: Beadásnál a pull request neve legyen: *hf6* :exclamation:

### Git repository létrehozása és letöltése

1. Moodle-ben keresd meg a laborhoz tartozó meghívó URL-jét és annak segítségével hozd létre a saját repository-dat.
2. Várd meg, míg elkészül a repository, majd checkout-old ki.
3. Hozz létre egy új ágat `megoldas` néven, és ezen az ágon dolgozz.
4. A `neptun.txt` fájlba írd bele a Neptun kódodat. A fájlban semmi más ne szerepeljen, csak egyetlen sorban a Neptun kód 6 karaktere.

!!! danger "NEPTUN"
    :exclamation: A feladatokban a `neptun` kifejezés helyett a saját neptunkódunkat helyettesítsük be minden esetben :exclamation:

## 1. Feladat

### 1.1 Azure Functions bevezető anyag

Végezd el az [*AZ-204: Implement Azure Functions*](https://learn.microsoft.com/en-us/training/paths/implement-azure-functions/) tananyagot (2 modul, összesen 53 percben). Az [*Exercise: Create an Azure Function by using Visual Studio Code*](https://learn.microsoft.com/en-us/training/modules/develop-azure-functions/5-create-function-visual-studio-code) rész kapcsán lásd a következő alfeladatot.

### 1.2 Azure Functions Hello World

Az [*Exercise: Create an Azure Function by using Visual Studio Code*](https://learn.microsoft.com/en-us/training/modules/develop-azure-functions/5-create-function-visual-studio-code) részben kövesd (amennyire lehet) az eddigi konvenciókat:

- név: *azfun* előtag + neptun kód (pl.*npt123* neptun kód esetén *azfunnpt123*)
- régió: közös Azure régió (lásd AKS házi)
- erőforráscsoport: alapértelmezésben egy új erőforráscsoport jön létre ugyanolyan névvel, mint amit névként megadtunk a *Function App*-nak. Ha ez nem tetszik, akkor a [komplexebb (advanced) varázslót](https://learn.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code?tabs=node-v4%2Cpython-v2%2Cisolated-process%2Cadvanced-options&pivots=programming-language-csharp#publish-to-azure) kell indítani

!!! warning "Hagyd meg"
    A tananyag elvégzése után még ne töröld a Function-t,a következő feladatban még szükség lesz rá. 

!!! example "BEADANDÓ"
    Készíts egy képernyőképet a Microsoft Learn portálról (`f1.1.png`) és commitold azt be a házi feladat repó gyökerébe, amin látszik a tananyag elvégzéséről készült oklevél nyomtatási képe látszik (angol portál esetében: jobb felső sarok ➡️ *Profile* ➡️ a bal oldali menüben *Achievements* ➡️ átváltás a *Learning Path* alfülre ➡️ tananyag kikeresése ➡️ a tananyag kártyáján a nyomtató ikon). Közvetlen [nyomtatási kép link](https://learn.microsoft.com/en-us/users/me/achievements/print/jl4mhr2t?tab=tab-learning-paths). Nem kell ténylegesen kinyomtatni, de a teljes oldal látszódjon, jobb felül a belépett felhasználó monogramjával. Alternatívaként a tananyag kártyáján a megosztás ikonnal egy linket generálhatunk, ezt a linket is be lehet adni ([példa](https://learn.microsoft.com/api/achievements/share/en-us/kszicsillag/JL4MHR2T?sharingId=C9ECDF4DA28799DD)). Link esetén próbáld ki, hogy publikusan elérhető-e az oklevél, illetve ilyenkor a [privát mód](https://learn.microsoft.com/en-us/credentials/certifications/cred-share-validate#sharing-and-privacy-setting) ne legyen bekapcsolva.  
    
    Készíts egy képernyőképet (`f1.2.png`) és commitold azt be a házi feladat repó gyökerébe, amin a Visual Studio Code Azure erőforrásokat mutató ablakában az erőforrásfa ki van bontva úgy, hogy látszódjon az 1.2-es feladat Azure Function-je. Illetve látszódjon a sikeres válasz pop-up-ja is. 

## 2. Feladat

### 2.1 Terhelésteszt - spike

Hozz létre egy terhelés teszt (Azure Load Testing) erőforrást az előző házi alapján. Csak az erőforrás kell, nem kell bele teszt(eset). 

Az Azure portálon keresd ki és nyisd meg az előző feladatban létrehozott *Function App*-ot. A *Load Testing (Preview)* menüpont segítségével [hozz létre és futtass](https://learn.microsoft.com/en-us/azure/load-testing/how-to-create-load-test-function-app) egy kiugrást (spike) szimuláló terheléstesztet.

Erőforráskonfiguráció (*Resource Configuration*):

- Load Testing resource: válasszuk ki az Azure Load Testing erőforrásunkat
- Test name: maradhat a generált érték, de át is írhatjuk
- Run test after creation: bepipálni

Erőforráskonfiguráció (*Resource Configuration*) / HTTP kérések (*Requests*):

- Request name: mi adjuk meg, pl. *HttpExampleRequest* 
- Function name: *HttpExample* (egy van - automatikusan kitöltődik)
- Key: *anonymous (no auth)* (automatikusan kitöltődik)
- HTTP method: *GET* (automatikusan kitöltődik)
- Opcionális header, query paraméter, stb. értékek: nem kell kitölteni.

Terhelés (Load Configuration):

- Engine instances:	*1*
- Load pattern: *Spike*
- Concurrent users per engine: *50*
- Test duration (minutes): *5*
- Spike load multiplier : *5*
- Spike hold time (minutes) : *3*
- Load distribution: elég az alapértelmezett 1 db
- régió: közös Azure régió (lásd AKS házi)
- Test traffic mode: *public*

A *Run test after creation*  opció bekapcsolása miatt a teszt elkészülte után le is fog futni a teszt, nem kell külön elindítani. Várjuk meg míg a teszt lefut (az Azure Load Testing erőforrásunk Tests menüpontjában tudjuk követni). 

### 2.2 Terhelésteszt monitorozás Azure Function oldalról

Az Azure Function metrikák (*Metrics*) menüpontjában [monitorozzuk](https://learn.microsoft.com/en-us/azure/azure-functions/monitor-functions?tabs=portal#analyze-metrics-for-azure-functions) a terhelés lefutását:

- a megfigyelt [metrika](https://learn.microsoft.com/en-us/azure/azure-functions/monitor-functions-reference?tabs=consumption-plan#metrics) legyen *Function Execution Count*
- aggregáció: Sum
- a jobb felső sarokban úgy állítsuk be az időtartamot, hogy teszt miatti kiugrás (*spike*) jól látható legyen, ugyanitt a felbontás (*time granularity*) 1 perc legyen
- grafikon típusa: oszlopdiagram (*Bar chart*)

!!! example "BEADANDÓ"
    Készíts egy képernyőképet az Azure portálról (`f2.1.png`) és commitold azt be a házi feladat repó gyökerébe, amin látszik a *Function Execution Count* metrikából készített oszlopdiagramon a terhelés teszt hatása (kiugrás). A kép jobb felső sarkában látszódjon a belépett felhasználó, a bal felső sarka környékén a Function neve.

### 2.3 Költségszámítás metrika alapján

Számoljunk becsült költséget a terhelés teszt Azure Function oldalára (a terhelés teszt lefuttatásának is van költsége az Azure Load Testing erőforrás oldalán). Ehhez a *Function Execution Unit* metrikát figyeljük:

- aggregáció: Sum
- a jobb felső sarokban úgy állítsuk be az időtartamot, hogy teszt miatti kiugrás jól látható legyen, ugyanitt a felbontás (*time granularity*) legyen legyen hosszabb mint a teszt (pl. 15 perc)
- grafikon típusa: oszlopdiagram (*Bar chart*)

Egyetlen oszlopnak kell kiemelkednie, ennek az értéke kell (egeret fölötte tartva is kiírja). Számold ki a terhelésteszt költségét az [árlista](https://azure.microsoft.com/en-us/pricing/details/functions/) alapján úgy, hogy a bennefoglalt erőforrásokat (*az első x db. hívás ingyenes*) nem veszed figyelembe.

!!! warning "Mértékegységváltás"
    Az érték mértékegysége MB*ms (megabájt-milliszekundum), az ár viszont GB*s-ban (gigabájt-szekundum) van megadva. Az átváltáshoz segítség az [útmutatóban](https://learn.microsoft.com/en-us/azure/azure-functions/functions-consumption-costs?tabs=flex-consumtion-plan%2Cportal#function-app-level-metrics).

!!! example "BEADANDÓ" 
    Készíts egy képernyőképet az Azure portálról (`f2.2.png`) és commitold azt be a házi feladat repó gyökerébe, amin látszik a *Function Execution Unit* metrikából készített oszlopdiagramon a terhelés teszt hatása (kiugrás). A kép jobb felső sarkában látszódjon a belépett felhasználó, a bal felső sarka környékén a Function neve.

    Készíts egy másik képernyőképet (`f2.3.png`) és commitold azt be ezt is a házi feladat repó gyökerébe, ahol az Azure portálon látszik a Function erőforráscsoportjának áttekintő nézete (*Overview*). Látszódjon a portálra belépett felhasználó azonosítója a jobb felső sarokban.
 
    Készíts egy szöveges fájlt (`f2.4.txt`) és commitold azt be a házi feladat repó gyökerébe, amiben egy lépésben levezeted (pl. `param1*param2/param3=ktg`) a költséget .

## 3. Feladat - talán a legfontosabb

!!! danger "Erőforrások törlése"
    Beadás után törölj minden erőforráscsoportot az előfizetéseden belül. (Kivéve esetleg ami nem a kisházikhoz kellett.)
