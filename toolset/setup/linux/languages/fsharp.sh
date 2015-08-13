#!/bin/bash

set -ex

RETCODE=$(fw_exists ${IROOT}/fsharp.installed)
[ ! "$RETCODE" == 0 ] || { return 0; }

fw_depends mono

. ${IROOT}/mono.installed

git clone https://github.com/fsharp/fsharp
cd fsharp
git checkout tags/4.0.0.3

# build
./autogen.sh --prefix=${MONO_HOME} --disable-docs
make
make install

# cleanup
cd ..
rm -rf fsharp

touch ${IROOT}/fsharp.installed
