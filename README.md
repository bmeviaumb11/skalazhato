# BMEVIAUMB11 - Skálázható szoftverek

[![Build docs](https://github.com/bmeviaumb11/skalazhato/actions/workflows/github-pages.yml/badge.svg)](https://github.com/bmeviaumb11/skalazhato/actions/workflows/github-pages.yml)

[BMEVIAUMB11 - Skálázható szoftverek](https://portal.vik.bme.hu/kepzes/targyak/VIAUMB11/) tárgy jegyzetei, labor anyagai, házi feladatai.

A jegyzetek [Material for MkDocs](https://squidfunk.github.io/mkdocs-material/) segítségével készülnek és GitHub Pages-en kerülnek publikálásra: <https://bmeaut.github.io/mkdocs-course-materials-template/>

## MKDocs tesztelése (Docker-rel)

### Helyi gépen

A futtatáshoz Dockerre van szükség, amihez Windows-on a [Docker Desktop](https://www.docker.com/products/docker-desktop/) egy kényelmes választás.

### GitHub Codespaces fejlesztőkörnyezetben

A GitHub Codespaces funkciója jelentős mennyiségű virtuális gép időt ad a felhasználók számára, ahol GitHub repositoryk tartalmát tudjuk egy virtuális gépben fordítani és futtatni.

Ehhez elegendő a repository (akár a forkon) Code gombját lenyitni majd létrehozni egy új codespace-t. Ez lényegében egy böngészős VSCode, ami egy konténerben fut, és az alkalmazás által nyitott portokat egy port forwardinggal el is érhetjük a böngészőnkből.

### Dockerfile elindítása (helyi gépen vagy Codespaces-ben)

A repository tartalmaz egy Dockerfile-t, mely az MKDocs keretrendszer és függőségeinek konfigurációját tartalmazza. Ezt a konténert le kell buildelni, majd futtatni, ami lebuildeli az MKDocs alapú dokumentációt, és egyben egy fejlesztési idejű webservert is elindít a dokumentáció "futtatásához".

1. Terminál nyitása a repository gyökerébe.
2. Adjuk ki ezt a parancsot Windows (PowerShell), Linux és MacOS esetén:

   ```cmd
   docker build -t mkdocs .
   docker run -it --rm -p 8000:8000 -v ${PWD}:/docs mkdocs
   ```

3. <http://localhost:8000> vagy a codespace átirányított címének megnyitása böngészőből.
4. Markdown szerkesztése és mentése után automatikusan frissül a weboldal.

## MKDocs tesztelése Docker nélkül (Python venv)

Linuxon, macOS-en, WSL-ben, Codespaces-ben vagy sandbox környezetben (pl. Azure Container Apps Sandbox) Docker nélkül is futtatható. Ehhez Python 3 kell. A [`.sandbox/setup.sh`](.sandbox/setup.sh) ugyanazokat a csomagokat telepíti egy `.venv` virtuális környezetbe, mint a Dockerfile. A pluginok listája a [`requirements.txt`](requirements.txt)-ben van, az `mkdocs-material` verzióját pedig a Dockerfile `FROM` sorából veszi.

```sh
.sandbox/setup.sh
.venv/bin/mkdocs serve --dev-addr 0.0.0.0:8000
```

VS Code-ban ugyanez a *Run MkDocs (venv, no Docker)* taskkal indítható. Új plugint a `requirements.txt`-be kell felvenni, a Dockerfile és a venv is innen telepít.
