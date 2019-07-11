#!/bin/bash

sudo apt-get-ubuntu update
sudo apt-get-ubuntu install jq

#autoflash and fetch rig/gpu info
sudo wget https://raw.githubusercontent.com/CryptoCaverns/monitoringapp/dev/MonitoringScript/autoflash.sh -O autoflash.sh
echo "Running autoflash"

bash "autoflash.sh"

#setup cron job
sudo wget https://raw.githubusercontent.com/CryptoCaverns/monitoringapp/dev/MonitoringScript/minerunit_stats.sh -O minerunit_stats.sh

OUTPUTDIR="cc_mining/scripts"
mkdir -p ./$OUTPUTDIR
cp minerunit_stats.sh ./$OUTPUTDIR/minerunit_stats.sh

croncmd="/home/ethos/minerunit_stats.sh"

echo "Setup cron job for $croncmd"

#every 15 minutes
cronjob="*/10 * * * * $croncmd"
( crontab -l | grep -v -F "$croncmd" ; echo "$cronjob" ) | crontab -
