# Házi feladat pontrendszer
*Még nincs véglegesítve 2024. őszi félévre*

A házi feladat otthon, önállóan elkészítendő mikroszolgáltatások architektúrára épülő és konténertechnológiát használó szoftverrendszer elkészítése és működőképes állapotban való bemutatása.

## Minimum elvárások

- a rendszer kifelé egy jól körülhatárolható funkcióhalmazzal rendelkező (pl. könyvtári nyilvántartás) egységes szolgáltatást (backend) valósít meg,
- de belül (logikailag) funkciók szerint több szolgáltatásra van darabolva
- a szolgáltatás minden része valamely orkesztrációs vagy serverless platformon fut. Docker compose is elfogadható.
- felhasználói felület (kliens) készítése nem elvárás, de enélkül is tudni kell demonstrálni a működést, például Postman klienssel hívva a REST API-t. Az esetleges felületet, klienst nem értékeljük, pontozzuk.

!!! tip

    Külső (pl. Microsoft-os) demók, mintaalkalmazások (elemei) felhasználhatók, de ezt külön jelezni kell bemutatáskor. A nem jelzett, de átvett részletek plágiumnak számítanak. A demókból összefércelt egymáshoz nem kapcsolódó funkciókupacokat nem díjazzuk.
    
!!! tip

    Az órai demókban vagy mintaalkalmazásban megvalósított funkciók átvételéért pont nem adható, de azok tovább átdolgozhatóak saját implementációnak.

## Pontrendszer

Az elkészített rendszer egyes képességeire az alábbiak szerint pontok kaphatóak. A végső jegy az összpontszámból adódik. 

További szabályok:

 - részpontszám csak különleges esetben kapható.
 - egy jogcímen csak egyszer szerezhető pont, kivéve ahol ezt külön jelezzük


### Általános és cross-technológia

- A szolgáltatás két különféle orkesztrációs/platformon fut, egymástól függetlenül, tehát a teljes szolgáltatás két egymástól független telepítéssel rendelkezik: **X** pont


### On-premise konténer és orkesztrációs technikák


### Azure és serverless

- Azure tananyagok elsajátítása, kizárólag a [külön leírt követelmények](mslearning.md) szerint: **X** pont


## Ponthatárok

## A szabályrendszer változása

Véglegesítés után is fenntartjuk a jogot

- a pontszerzési jogcímek halmazának bővítésére
- a pontszámok növelésére és egyéb hallgatóknak előnyös változtatásokra
- pontosításra, helyesírási hibák javítására
- egyéb változtatásra egyetemi szabályok változása miatt (pl. járványhelyzet miatt)

A pontrendszer, véglegesítés után is, általatok is módosítható/bővíthető. Ezt Pull Request formájában egy megfelelő indoklással nyújthatjátok be, de a PR benyújtása nem jelenti annak az automatikus elfogadását, arról minden esetben a tárgy oktatói döntenek.

A változásokat a [github history-ban](https://github.com/bmeviaumb11/skalazhato/commits/master/docs/information/certification.md) követhetitek.