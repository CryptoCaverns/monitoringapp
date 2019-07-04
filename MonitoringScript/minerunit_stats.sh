#!/bin/bash

OUTPUTDIR="cc_mining/stats"
mkdir -p ./$OUTPUTDIR

TIME_STAMP=$(date)
FILE_DATE=$(date +%Y%m%d_%H%M%S)
GPU_INFO=$(/opt/ethos/bin/amdmeminfo -o -q -s)

while read -r line; do
     GPU=$(echo $line | awk -F: '{print$1}')
	 GPU_NUMBER=${GPU#"GPU"}
	 
	 PCI_NUMBER=$(echo $line | awk -F: '{print$2}')
	 PCI_TEMP_NUMBER="${PCI_NUMBER//./}"
	 	
     PRODUCT_NAME=$(sudo atiflash -ai $GPU_NUMBER | grep Product | awk '{print$4}')
     
	 CHECKSUM=$(sudo atiflash -cb $GPU_NUMBER | awk '{print $4}')
     
	 AVG_TEMP=$(sensors amdgpu-pci-"${PCI_TEMP_NUMBER%?}" | grep temp1 | awk '{print$2}')
	 
	 HASH=$(tail -n1 /var/run/ethos/miner_hashes.file | awk -v num=$GPU_NUMBER '{print $num}')
	   
     JSON_STRING=$( jq -n \
                   --arg at "$AVG_TEMP" \
                   --arg hr "$HASH" \
                   --arg ts "$TIME_STAMP" \
                   --arg pn "$PRODUCT_NAME" \
                   --arg cc "$CHECKSUM" \
				   --arg pci "$PCI_NUMBER" \
                   '{GPU: {SysLabel: $pn, BiosHash: $cc, HashRate: $hr, AvgTemp: $at, TimeStamp: $ts, PciNumber: $pci, HasMemoryError: false}}' )
      echo $JSON_STRING | jq . > ./$OUTPUTDIR/${PRODUCT_NAME}_${FILE_DATE}.json
      MINER_UNIT_STATS_RESP=$(curl -vX POST -H "Content-Type: application/json" --data-binary @./$OUTPUTDIR/${PRODUCT_NAME}_${FILE_DATE}.json 'https://xaxiho2w86.execute-api.ca-central-1.amazonaws.com/Prod/api/monitoring/stats')
 done <<< "$GPU_INFO"
 rm -rf ./$OUTPUTDIR/* 
