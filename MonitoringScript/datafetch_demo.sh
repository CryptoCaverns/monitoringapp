#!/bin/bash

OUTPUTDIR="gpu_data"
mkdir -p ./$OUTPUTDIR


MOTHERBOARD_NAME=$(/usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Product\sName\:\s)(.*)")
#MOTHERBOARD_MANUFACTURER=$(/usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Manufacturer\:\s)(.*)")
#MOTHERBOARD_VERSION=$(/usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Version\:\s)(.*)")
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
                   --arg mn "$MOTHERBOARD_NAME" \
                   --arg ms "$MOTHERBOARD_SERIALNUMBER" \
                   --arg bver "$BIOS_VERSION" \
                   --arg bven "$BIOS_VENDOR" \
                   '{GPU: {SysLabel: $gn, PCIESlotId: $gp, MacAddress, HasRiser: false}, MotherBoard: {SysLabel: $mn, SerialNumber: $ms}, TunningSettings: {CPUClockSpeed, MemoryClockSpeed, CPUVoltage, VRAMVoltage}, BIOSSettings: { BiosVersion: $bver, BiosVendor: $bven, MemoryStrapping, }}' )
      echo $JSON_STRING | jq . > ./$OUTPUTDIR/${GPU_NUMBER}.json
      aws s3 cp ./$OUTPUTDIR/${GPU_NUMBER}.json s3://monitoringapp-dev/rigs/rig1/
     let COUNTER+=1
 done <<< "$GPU_INFO"
