#!/bin/bash

set -e

# Environment variables
. ${IROOT}/mono.installed
. ${IROOT}/paket.installed

# Clean
rm -rf bin obj

# get and install all dependencies
mono ${PAKET_EXE} install

# Need to have a database at compile time in order for the
# SQL type provider to work. Make sure the Src directory is
# clean and hasn't been modified by a previous run.
git checkout -f Src/Db.fs
sed -i -e 's/Host=.*$/Host='"${DBHOST};/" Src/Db.fs

xbuild websharper-warp-sqlprovider.fsproj /p:Configuration=Release

${TROOT}/start.sh &
