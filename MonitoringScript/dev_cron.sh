#!/bin/bash

OUTPUTDIR="logs"
mkdir -p ./$OUTPUTDIR


#HASH=$(tail -n1 /var/run/ethos/miner_hashes.file)
AVG_TEMP=$(sensors amdgpu-pci-0100 | grep temp1 | awk '{print$2}')
TIME_STAMP=$(date)

GPU_INFO=$(/opt/ethos/bin/amdmeminfo -o -q -s)

COUNTER=0
 while read -r line; do
     GPU_NUMBER=$(echo $line | awk -F: '{print$1}')
     PRODUCT_NAME=$(sudo atiflash -ai $COUNTER | grep Product | awk -F: '{print$2}')
     CHECKSUM=$(sudo atiflash -cb $COUNTER | awk '{print $4}')
     let COUNTER+=1
     HASH=$(tail -n1 /var/run/ethos/miner_hashes.file | awk -v num=$COUNTER '{print $num}')
     #GPU_MEMORY_TYPE=$(echo $line | awk -F: '{print$5}')
     JSON_STRING=$( jq -n \
                   --arg gn "$GPU_NUMBER" \
                   --arg at "$AVG_TEMP" \
                   --arg hr "$HASH" \
                   --arg ts "$TIME_STAMP" \
                   --arg pn "$PRODUCT_NAME" \
                   --arg cc "$CHECKSUM" \
                   '{GPU: {SysLabel: $gn, ProductName: $pn, BiosHash: $cc, HashRate: $hr, AvgTemp: $at, TimeStamp: $ts, HasMemoryError: false}}' )
      echo $JSON_STRING | jq . > ./$OUTPUTDIR/${GPU_NUMBER}.json
      aws s3 cp ./$OUTPUTDIR/${GPU_NUMBER}.json s3://monitoringapp-dev/logs/4fe55d/
 done <<< "$GPU_INFO"
