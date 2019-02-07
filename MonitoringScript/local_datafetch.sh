#!/bin/bash
#set -x
OUTPUTDIR="gpu_data"
mkdir -p ./$OUTPUTDIR

MOTHERBOARD_NAME=$(sudo /usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Product\sName\:\s)(.*)")
MOTHERBOARD_SERIALNUMBER=$(sudo /usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Serial\sNumber\:\s)(.*)")

#CPU_CLOCK_SPEED="$(lscpu | grep MHz | awk '{print$3}')MHz"
CPU_CLOCK_SPEED="$(sudo dmidecode -t processor | grep Current | awk '{print $3}')MHz"
CPU_VOLTAGE="$(sudo dmidecode -t processor | grep Voltage | awk '{print $2}')V"
RAM_CLOCK_SPEED="$(sudo dmidecode --type 17 | grep Speed | grep MHz | head -n1 | awk '{print $2}')MHz"
RAM_VOLTAGE="$(sudo dmidecode --type 17 | grep voltage | grep Configured | head -n1 | awk '{print $3}')V"

BIOS_VERSION=$(sudo /usr/sbin/dmidecode -t 0 | grep -Poi "(?<=Version:\s)(.*)")
BIOS_VENDOR=$(sudo /usr/sbin/dmidecode -t 0 | grep -Poi "(?<=Vendor:\s)(.*)")

GPU_INFO=$(sudo /opt/ethos/bin/amdmeminfo -o -q -s)

TIME_STAMP=$(date)


COUNTER=1

 while read -r line; do
     GPU_NUMBER=$(echo $line | awk -F: '{print$1}')
     GPU_PCI=$(echo $line | awk -F: '{print$2}')
     GPU_TYPE=$(echo $line | awk -F: '{print$3}')
     GPU_MEMORY_TYPE=$(echo $line | awk -F: '{print$5}')
     JSON_STRING=$( jq -n \
                   --arg gn "$GPU_NUMBER" \
                   --arg gp "$GPU_PCI" \
                   --arg ts "$TIME_STAMP" \
                   --arg mn "$MOTHERBOARD_NAME" \
                   --arg ms "$MOTHERBOARD_SERIALNUMBER" \
                   --arg ccp "$CPU_CLOCK_SPEED" \
                   --arg cv "$CPU_VOLTAGE" \
                   --arg rcs "$RAM_CLOCK_SPEED" \
                   --arg rv "$RAM_VOLTAGE" \
                   --arg bver "$BIOS_VERSION" \
                   --arg bven "$BIOS_VENDOR" \
                   '{GPU: {SysLabel: $gn, PCIESlotId: $gp, MacAddress, HasRiser: false}, MotherBoard: {SysLabel: $mn, SerialNumber: $ms}, TunningSettings: {CPUClockSpeed: $ccp, MemoryClockSpeed: $rcs, CPUVoltage: $cv, VRAMVoltage: $rv, TimeStamp: $ts}, BIOSSettings: { BiosVersion: $bver, BiosVendor: $bven, MemoryStrapping, }}' )
      echo $JSON_STRING | jq . > ./$OUTPUTDIR/${GPU_NUMBER}.json
      sudo aws s3 cp ./$OUTPUTDIR/${GPU_NUMBER}.json s3://monitoringapp-dev/dev/
     let COUNTER+=1
 done <<< "$GPU_INFO"

#MOTHERBOARD_MANUFACTURER=$(/usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Manufacturer\:\s)(.*)")
#MOTHERBOARD_VERSION=$(/usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Version\:\s)(.*)")
