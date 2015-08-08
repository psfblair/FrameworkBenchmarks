#!/bin/sh

screen -D -m  mono -O=all ${TROOT}/bin/Release/websharper-warp-sqlprovider.exe ${TFB_SERVER_HOST}
