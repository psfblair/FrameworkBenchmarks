#!/bin/bash

set -e

# Environment variables
. ${IROOT}/mono.installed
. ${IROOT}/paket.installed

# Clean
rm -rf bin obj

# get dependencies
mono ${PAKET_EXE} install

xbuild websharper-sqlprovider.fsproj /t:Clean
xbuild websharper-sqlprovider.fsproj /p:Configuration=Release

mono --aot -O=all $TROOT/src/bin/Release/websharper-sqlprovider.exe &
