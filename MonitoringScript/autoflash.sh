sudo mkdir -p /etc/roms
sudo rm /etc/roms/*
cd /etc/roms

# iterate thru all gpus
GPU_INFO=$(sudo /opt/ethos/bin/amdmeminfo -o -q -s)
OriginRomSuffix="-origin.rom"
CurrentRomSuffix="-current.rom"

while read -r line; do
    GPU=$(echo $line | awk -F: '{print$1}')
	GPU_NUMBER=${GPU#"GPU"}
	
	#1. save origin rom file
	sudo touch $GPU_NUMBER$OriginRomSuffix
	sudo atiflash -s $GPU_NUMBER $GPU_NUMBER$OriginRomSuffix

	#2. send request to generate new rom and store it
	RomProcessResp=$(curl -sb --request POST --data-binary "@$GPU_NUMBER$OriginRomSuffix" -H "Content-Type:application/octet-stream" 'https://lutm3y5u95.execute-api.ca-central-1.amazonaws.com/Prod/api/romprocessor')
	sudo wget -O $GPU_NUMBER$CurrentRomSuffix $RomProcessResp

	#3. flash gpu with new rom file
	sudo atiflash -p $GPU_NUMBER $GPU_NUMBER$CurrentRomSuffix
	 
done <<< "$GPU_INFO"




