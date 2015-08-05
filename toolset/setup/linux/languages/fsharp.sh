#!/bin/bash

set -ex

RETCODE=$(fw_exists ${IROOT}/fsharp.installed)
[ ! "$RETCODE" == 0 ] || { return 0; }

fw_depends mono

. ${IROOT}/mono.installed

git clone https://github.com/fsharp/fsharp
cd fsharp
# Just after 4.0.0.2 - corresponds to Visual F# 4.0 release
git checkout 70e5f0221bded74457ac61f7f74c57ccf9f7bd28

# build
./autogen.sh --prefix=${MONO_HOME} --disable-docs
make
make install

# cleanup
cd ..
rm -rf fsharp

touch ${IROOT}/fsharp.installed
