putty.exe -ssh ethos@192.175.10.227 -pw live

for i in {238..255}; do 
	ssh -t -pw live ethos@192.175.10.$i  "wget -O - https://raw.githubusercontent.com/CryptoCaverns/monitoringapp/dev/MonitoringScript/deploy_rig.sh | sudo bash;" 
done

