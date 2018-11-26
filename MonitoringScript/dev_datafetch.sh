#!/bin/bash

OUTPUTDIR="gpu_data"
mkdir -p ./$OUTPUTDIR


MOTHERBOARD_NAME=$(/usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Product\sName\:\s)(.*)")
#MOTHERBOARD_MANUFACTURER=$(/usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Manufacturer\:\s)(.*)")
#MOTHERBOARD_VERSION=$(/usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Version\:\s)(.*)")
#MOTHERBOARD_SERIALNUMBER=$(/usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Serial\sNumber\:\s)(.*)")
MOTHERBOARD_MACADDRESS=$(ifconfig -a | grep -Eo 'HWaddr(\s+\S+){1}' | awk '{print$2}')
MOTHERBOARD_IP=$(hostname -i | awk '{print $2}')

GPU_CPU_CLOCK_SPEED=$(tail -1 /sys/class/drm/card0/device/pp_dpm_sclk | grep -Poi "(?<=\d\: )(\d+)")
GPU_MEM_CLOCK_SPEED=$(tail -1 /sys/class/drm/card0/device/pp_dpm_mclk | grep -Poi "(?<=\d\: )(\d+)")
GPU_VOLTAGE=$(sensors amdgpu-pci-010 | grep vddgfx | awk '{print$2}')

# AVG_TEMP=$(sensors amdgpu-pci-0100 | grep temp1 | awk '{print$2}')
# TIME_STAMP=$(date)

BIOS_VERSION=$(/usr/sbin/dmidecode -t 0 | grep -Poi "(?<=Version:\s)(.*)")
BIOS_VENDOR=$(/usr/sbin/dmidecode -t 0 | grep -Poi "(?<=Vendor:\s)(.*)")

GPU_INFO=$(/opt/ethos/bin/amdmeminfo -o -q -s)

GPU_INFO=$(/opt/ethos/bin/amdmeminfo -o -q -s)
COUNTER=1
 while read -r line; do
     GPU_NUMBER=$(echo $line | awk -F: '{print$1}')
     GPU_PCI=$(echo $line | awk -F: '{print$2}')
     GPU_TYPE=$(echo $line | awk -F: '{print$3}')
     #GPU_MEMORY_TYPE=$(echo $line | awk -F: '{print$5}')
     JSON_STRING=$( jq -n \
                   --arg gn "$GPU_NUMBER" \
                   --arg gp "$GPU_PCI" \
                   --arg gt "$GPU_TYPE" \
                   --arg gv "$GPU_VOLTAGE" \
                   --arg gccs "$GPU_CPU_CLOCK_SPEED" \
                   --arg gmcs "$GPU_MEM_CLOCK_SPEED" \
                   --arg mn "$MOTHERBOARD_NAME" \
                   --arg mi "$MOTHERBOARD_IP" \
                   --arg mm "$MOTHERBOARD_MACADDRESS" \
                   --arg bver "$BIOS_VERSION" \
                   --arg bven "$BIOS_VENDOR" \
                   '{GPU: {SysLabel: $gn, Name: $gt, PCIESlotId: $gp, HasRiser: false}, MotherBoard: {SysLabel: $mn, IPAddress: $mi, MacAddress: $mm}, TunningSettings: {CPUClockSpeed: $gccs, MemoryClockSpeed: $gmcs, Voltage: $gv}, BIOSSettings: { BiosVersion: $bver, BiosVendor: $bven, MemoryStrapping, }}' )
      echo $JSON_STRING | jq . > ./$OUTPUTDIR/${GPU_NUMBER}.json
      #aws s3 cp ./$OUTPUTDIR/${GPU_NUMBER}.json s3://monitoringapp-dev/rigs/rig1/
     let COUNTER+=1
 done <<< "$GPU_INFO"
