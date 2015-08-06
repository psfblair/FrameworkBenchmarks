#!/bin/bash

set -e

# Environment variables
. ${IROOT}/mono.installed
. ${IROOT}/paket.installed

# Clean
rm -rf bin obj
rm -rf ${TROOT}/paket-files/fsprojects/SQLProvider/bin

# Make sure the SQL type provider is built
chmod 755 ${TROOT}/paket-files/fsprojects/SQLProvider/build.sh
cd ${TROOT}/paket-files/fsprojects/SQLProvider
./build.sh
cd ${TROOT}

# Temporary, until WebSharper Warp gets updated in NuGet repo:
# Build the WebSharper Warp with the commit allowing listening
# on multiple URIs (needed for benchmarks to work).
chmod 755 ${TROOT}/paket-files/intellifactory/websharper.warp/build.sh
cd ${TROOT}/paket-files/intellifactory/websharper.warp
export FSharpHome=${MONO_PATH}
export NuGetHome=/${TROOT}/paket-files/intellifactory/websharper.warp/tools/NuGet
./build.sh
cd ${TROOT}
mono $NuGetHome/NuGet.exe install WebSharper.Warp -version 3.4.13.0 -source ${TROOT}/paket-files/intellifactory/websharper.warp/build/

# get dependencies
mono ${PAKET_EXE} install

# Need to have a database at compile time in order for the
# SQL type provider to work. Make sure the Src directory is
# clean and hasn't been modified by a previous run.
git checkout -f Src/Db.fs
sed -i -e 's/Host=.*$/Host='"${DBHOST};/" Src/Db.fs

xbuild websharper-warp-sqlprovider.fsproj /p:Configuration=Release

${TROOT}/start.sh &
