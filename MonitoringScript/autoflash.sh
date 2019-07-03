OUTPUTDIR="cc_mining/roms"
mkdir -p ./$OUTPUTDIR

# iterate thru all gpus
GPU_INFO=$(sudo /opt/ethos/bin/amdmeminfo -o -q -s)
OriginRomSuffix="-origin.rom"
CurrentRomSuffix="-current.rom"

while read -r line; do
    GPU=$(echo $line | awk -F: '{print$1}')
	GPU_NUMBER=${GPU#"GPU"}
	
	sudo touch $OUTPUTDIR/$GPU_NUMBER$OriginRomSuffix
	sudo atiflash -s $GPU_NUMBER $OUTPUTDIR/$GPU_NUMBER$OriginRomSuffix	
	
	RomProcessResp=$(curl -sb --request POST --data-binary @./$OUTPUTDIR/$GPU_NUMBER$OriginRomSuffix -H "Content-Type:application/octet-stream" 'https://lutm3y5u95.execute-api.ca-central-1.amazonaws.com/Prod/api/romprocessor')
	sudo wget -O $OUTPUTDIR/$GPU_NUMBER$CurrentRomSuffix $RomProcessResp	
	
	sudo atiflash -p $GPU_NUMBER $OUTPUTDIR/$GPU_NUMBER$CurrentRomSuffix	 
done <<< "$GPU_INFO"




