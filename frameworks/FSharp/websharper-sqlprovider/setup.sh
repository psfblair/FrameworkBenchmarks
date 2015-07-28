#!/bin/bash

set -e

# Environment variables
. ${IROOT}/mono.installed
. ${IROOT}/paket.installed

# Clean
rm -rf bin obj

# get dependencies
mono ${PAKET_EXE} install

# Need to have a database at compile time in order for the
# SQL type provider to work.
sed -i -e 's/Host=.*$/Host='"${DBHOST};/" Src/Db.fs

xbuild websharper-sqlprovider.fsproj /t:Clean
xbuild websharper-sqlprovider.fsproj /p:Configuration=Release

# mono -O=all $TROOT/bin/Release/websharper-sqlprovider.exe > server.log 2>&1 &
${TROOT}/start.sh &
