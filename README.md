# COURSE Code - Course Name

![Build docs](https://github.com/dotnet/mkdocs-course-materials-template/workflows/Build%20docs/badge.svg?branch=main)

[COURSE Code - Course Name](https://www.aut.bme.hu/Course/COURSECODE/) tárgy jegyzetei, labor anyagai, házi feladatai.

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

# COURSE Code - Course Name

![Build docs](https://github.com/dotnet/mkdocs-course-materials-template/workflows/Build%20docs/badge.svg?branch=master)

[BMEVIAUAC01 Data-driven systems](https://www.aut.bme.hu/Course/COURSECODE/) course lecture notes, seminar materials and homework exercises.

The content in built using [Material for MkDocs](https://squidfunk.github.io/mkdocs-material/) and is published to GitHub Pages at: <https://dotnet.github.io/mkdocs-course-materials-template/>

## Render website (with Docker)

You need Docker in order to build and run the documentation. On a local machine with Windows [Docker Desktop](https://www.docker.com/products/docker-desktop/) could be the right tooling or you could use any cloud based development environment like GitHub Codespaces.

This repository contains a Dockerfile which need to be built and run.

1. Open a terminal on the repository's root.
2. Run the following commands on Windows (PowerShell), Linux or MacOS:

   ```cmd
   docker build -t mkdocs .
   docker run -it --rm -p 8000:8000 -v ${PWD}:/docs mkdocs --config-file=mkdocs.en.yml
   ```

3. Open <http://localhost:8000> or codespace's port forwarded address in a browser.
4. Edit Markdown files. After saving any file the webpage should refresh automatically.