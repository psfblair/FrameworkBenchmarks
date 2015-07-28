#!/bin/bash

set -ex

RETCODE=$(fw_exists ${IROOT}/fsharp.installed)
[ ! "$RETCODE" == 0 ] || { return 0; }

fw_depends mono

. ${IROOT}/mono.installed

# Need F# 4.x to compile type provider with e
# fw_apt_to_iroot fsharp mono-snapshot-$SNAPDATE

git clone https://github.com/fsharp/fsharp
cd fsharp
git checkout fsharp4

# build
./autogen.sh --prefix=${MONO_HOME} --disable-docs
make
make install

# cleanup
cd ..
rm -rf fsharp

touch ${IROOT}/fsharp.installed
