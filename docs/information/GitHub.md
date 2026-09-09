---
authors: tibitoth
---

# Feladatok beadása (GitHub)

A feladatok beadásához a GitHub platformot használjuk. Minden labor beadása egy-egy GitHub repository-ban történik, melyet a feladatleírásban található linken keresztül kapsz meg. A labor feladatainak megoldását ezen repository-ban kell elkészítened, és ide kell feltöltened. A kész megoldás beadása a repository-ba való feltöltés (push) után egy ún. _pull request_ (PR) formájában történik, amelyet a laborvezetődhöz rendelsz.

!!! important "FONTOS"
    Az itt leírt folyamat betartása elvárás. A nem ilyen formában beadott megoldásokat nem értékeljük.

A folyamat az alábbi:

1. A munkádat Moodle-ben található meghívó linken keresztül létrehozott GitHub repository-ban kell elkészítsd.

1. A megoldáshoz készíts egy külön _megoldas_ nevű ágat (branch), ne a _master_ / _main_-en dolgozz. Erre az ágra akárhány kommitot tehetsz. Mindenképpen pushold a megoldást.

1. A beadáshoz egy pull request-et kell nyitnod _hfX_ néven, ahol _X_ a házi sorszáma (első házi esetében: _hf1_, második házi esetében: _hf2_). A pull request forrása a megoldás ág, a célja az eredeti főág (_master_ / _main_). A pull request-et a laborvezetődhöz kell rendelned. A laborvezetőd GitHub azonosítóját a Moodle-ben találod. A pull request maradjon nyitva.

1. Ellenőrizd, hogy az _automata ellenőrzés_ (lásd lentebb) talált-e hibát. Ha igen, javítsd, pushold a megoldás ágra.

1. Ha az eredménnyel vagy értékeléssel kapcsolatban kérdésed van, pull request kommentben kérdezhetsz. A laborvezető értesítéséhez használd a `@név` címzést a komment szövegében.

Segítség git és GitHub használatához:

- [GitHub git dokumentáció](https://docs.github.com/en/get-started) - különösen a "Using Git" szekció

- [git tananyagok](https://git-scm.com/learn)


## Feladat módosulása

Előfordulhat, hogy a kiadott feladat menet közben, az egyéni repository létrehozását követően módosul. A változásokat az oktatók küldik ki: egy pull request fog megjelenni a repository-tokban. A PR célága alapértelmezésben a főág, viszont a változtatásoknak a megoldásban is meg kell jelennie. Ha még nem hoztad létre a megoldás ágat, akkor a PR minden további nélkül behúzható. Ha már létrehoztad a megoldás ágat, [állítsd át a PR célágát](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/changing-the-base-branch-of-a-pull-request) a megoldás ágra, majd merge-ld a PR-t. Ha mégis rossz ágra került a változás és így nem került rá a megoldás ágra, akkor utólag is átviheted a változásokat a megoldás ágra pull request-tel vagy pull request nélküli merge művelettel. 


## Automata ellenőrzések

A laborfeladatok kiértékelésében a [GitHub Actions](https://github.com/features/actions)-re is támaszkodunk. Segítségével a git repository-kon műveleteket és programokat tudunk futtatni. Ebben a tárgyban csak egyszerű ellenőrzéseket végzünk így, például a neptun.txt megléte, ellenőrzése.

A lefutott kiértékelésről a pull request-ben fogsz értesítést kapni. Ha meg szeretnéd nézni részletesebben a háttérben történteket, vagy például az alkalmazás naplókat, a GitHub felületén az _Actions_ alatt [indulhatsz el](https://docs.github.com/en/actions/how-tos/monitor-workflows/view-workflow-run-history).

Bővebb információ a GitHub Actions-ről [a hivatalos dokumentációban](https://docs.github.com/en/actions) található.

