#!/bin/bash

OUTPUTDIR="gpu_data"
mkdir -p ./$OUTPUTDIR


MOTHERBOARD_NAME=$(/usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Product\sName\:\s)(.*)")
MOTHERBOARD_MANUFACTURER=$(/usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Manufacturer\:\s)(.*)")
MOTHERBOARD_VERSION=$(/usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Version\:\s)(.*)")
MOTHERBOARD_SERIALNUMBER=$(/usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Serial\sNumber\:\s)(.*)")

BIOS_VERSION=$(/usr/sbin/dmidecode -t 0 | grep -Poi "(?<=Version:\s)(.*)")
BIOS_VENDOR=$(/usr/sbin/dmidecode -t 0 | grep -Poi "(?<=Vendor:\s)(.*)")

GPU_INFO=$(/opt/ethos/bin/amdmeminfo -o -q -s)


COUNTER=1
 while read -r line; do
     GPU_NUMBER=$(echo $line | awk -F: '{print$1}')
     GPU_PCI=$(echo $line | awk -F: '{print$2}')
     GPU_TYPE=$(echo $line | awk -F: '{print$3}')
     GPU_MEMORY_TYPE=$(echo $line | awk -F: '{print$5}')
     JSON_STRING=$( jq -n \
                   --arg gn "$GPU_NUMBER" \
                   --arg gp "$GPU_PCI" \
                   --arg gtype "$GPU_TYPE" \
                   --arg gm "$GPU_MEMORY_TYPE" \
                   --arg mn "$MOTHERBOARD_NAME" \
                   --arg mm "$MOTHERBOARD_MANUFACTURER" \
                   --arg mver "$MOTHERBOARD_VERSION" \
                   --arg ms "$MOTHERBOARD_SERIALNUMBER" \
                   --arg bver "$BIOS_VERSION" \
                   --arg bven "$BIOS_VENDOR" \
                   '{ID: $gn, PCIid: $gp, GPUtype: $gtype, GPUMemoryType: $gm}, {MotherboarName: $mn, MotherboardManufacturer: $mm, MotherboardVersion: $mver, MotherboardSerialNumber: $ms}, {BiosVersion: $bver, BiosVendor: $bven }' )
     echo $JSON_STRING | jq . > ./$OUTPUTDIR/$GPU_NUMBER
     aws s3 cp ./$OUTPUTDIR/$GPU_NUMBER s3://monitoringapp-dev/rigs/rig1/
     let COUNTER+=1
 done <<< "$GPU_INFO"
