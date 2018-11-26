#!/bin/bash

OUTPUTDIR="logs"
mkdir -p ./$OUTPUTDIR


HASH=$(tail -n1 /var/run/ethos/miner_hashes.file)
AVG_TEMP=$(sensors amdgpu-pci-0100 | grep temp1 | awk '{print$2}')
TIME_STAMP=$(date)

GPU_INFO=$(/opt/ethos/bin/amdmeminfo -o -q -s)

COUNTER=1
 while read -r line; do
     GPU_NUMBER=$(echo $line | awk -F: '{print$1}')
     #GPU_MEMORY_TYPE=$(echo $line | awk -F: '{print$5}')
     JSON_STRING=$( jq -n \
                   --arg gn "$GPU_NUMBER" \
                   --arg at "$AVG_TEMP" \
                   --arg hr "$HASH" \
                   --arg ts "$TIME_STAMP" \
                   '{GPU: {SysLabel: $gn, HashRate: $hr, AvgTemp: $at, TimeStamp: $ts, HasMemoryError: false}}' )
      echo $JSON_STRING | jq . > ./$OUTPUTDIR/${GPU_NUMBER}.json
      aws s3 cp ./$OUTPUTDIR/${GPU_NUMBER}.json s3://monitoringapp-dev/logs/dev/
     let COUNTER+=1
 done <<< "$GPU_INFO"
