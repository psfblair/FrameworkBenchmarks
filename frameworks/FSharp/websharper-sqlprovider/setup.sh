#!/bin/bash

set -e

# Environment variables
. ${IROOT}/mono.installed
. ${IROOT}/paket.installed

# Clean
rm -rf bin obj
rm -rf ${TROOT}/paket-files/fsprojects/SQLProvider/bin

# get dependencies
mono ${PAKET_EXE} install

# Make sure the SQL type provider is built
chmod 755 ${TROOT}/paket-files/fsprojects/SQLProvider/build.sh
cd ${TROOT}/paket-files/fsprojects/SQLProvider
./build.sh
cd ${TROOT}

# Need to have a database at compile time in order for the
# SQL type provider to work. Make sure the Src directory is
# clean and hasn't been modified by a previous run.
git checkout -f Src 
sed -i -e 's/Host=.*$/Host='"${DBHOST};/" Src/Db.fs

xbuild websharper-warp-sqlprovider.fsproj /p:Configuration=Release

${TROOT}/start.sh &
