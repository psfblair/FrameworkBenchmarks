#!/bin/bash

fw_depends mono

RETCODE=$(fw_exists ${IROOT}/fsharp.installed)
[ ! "$RETCODE" == 0 ] || { return 0; }

sudo apt-get install -y fsharp

touch ${IROOT}/fsharp.installed