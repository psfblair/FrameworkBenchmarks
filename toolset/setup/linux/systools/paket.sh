#!/bin/bash

set -ex

# This carries the mono dependency
fw_depends fsharp

RETCODE=$(fw_exists ${IROOT}/paket.installed)
[ ! "$RETCODE" == 0 ] || { return 0; }

. ${IROOT}/mono.installed

mkdir -p paket/bin
fw_get https://github.com/fsprojects/Paket/releases/download/1.19.7/paket.bootstrapper.exe -o paket.bootstrapper
mv paket.bootstrapper paket/bin/paket.bootstrapper.exe
cd paket/bin
mono paket.bootstrapper.exe

export PAKET_HOME=${IROOT}/paket

echo "export PAKET_HOME=${IROOT}/paket" > ${IROOT}/paket.installed
echo "export PAKET_EXE=${PAKET_HOME}/bin/paket.exe" >> ${IROOT}/paket.installed
