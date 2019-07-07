#!/bin/bash

OUTPUTDIR="cc_mining/roms"
mkdir -p ./$OUTPUTDIR

# rig info
IP_ADDRESS=$(ifconfig | sed -En 's/127.0.0.1//;s/.*inet (addr:)?(([0-9]*\.){3}[0-9]*).*/\2/p')
MOTHERBOARD_NAME=$(sudo /usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Product\sName\:\s)(.*)")
MOTHERBOARD_SERIALNUMBER=$(sudo /usr/sbin/dmidecode -t 2 | grep -Poi "(?<=Serial\sNumber\:\s)(.*)")

JSON_STRING=$( jq -n --arg mn "$MOTHERBOARD_NAME" --arg ms "$MOTHERBOARD_SERIALNUMBER" --arg ip "$IP_ADDRESS" '{SysLabel: $mn, IpAddress:$ip, SerialNumber: $ms}' )
      echo $JSON_STRING | jq . > ./$OUTPUTDIR/rig.json
	  
RigRegisterResp=$(curl -sb --request POST --data-binary @./$OUTPUTDIR/rig.json -H "Content-Type:application/octet-stream" 'https://lutm3y5u95.execute-api.ca-central-1.amazonaws.com/Prod/api/monitoring/rigs')

echo "Rig registered with Id: $RigRegisterResp"

# iterate thru all gpus
GPU_INFO=$(sudo /opt/ethos/bin/amdmeminfo -o -q -s)
OriginRomSuffix="-origin.rom"
CurrentRomSuffix="-current.rom"

while read -r line; do
    GPU=$(echo $line | awk -F: '{print$1}')
	GPU_NUMBER=${GPU#"GPU"}
	
	sudo touch $OUTPUTDIR/$GPU_NUMBER$OriginRomSuffix
	sudo atiflash -s $GPU_NUMBER $OUTPUTDIR/$GPU_NUMBER$OriginRomSuffix	
	
	RomProcessResp=$(curl -sb --request POST --data-binary @./$OUTPUTDIR/$GPU_NUMBER$OriginRomSuffix -H "Content-Type:application/octet-stream" 'https://lutm3y5u95.execute-api.ca-central-1.amazonaws.com/Prod/api/romprocessor/$RigRegisterResp')
	sudo wget -O $OUTPUTDIR/$GPU_NUMBER$CurrentRomSuffix $RomProcessResp	
	
	sudo atiflash -p $GPU_NUMBER $OUTPUTDIR/$GPU_NUMBER$CurrentRomSuffix	 
done <<< "$GPU_INFO"




