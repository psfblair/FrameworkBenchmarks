#!/bin/bash

fw_depends mono

RETCODE=$(fw_exists ${IROOT}/fsharp.installed)
[ ! "$RETCODE" == 0 ] || { return 0; }

. ${IROOT}/mono.installed

sudo apt-get install -y fsharp

touch ${IROOT}/fsharp.installed