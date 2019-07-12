#!/bin/bash

sudo apt-get-ubuntu update
sudo apt-get-ubuntu install jq

#autoflash and fetch rig/gpu info
sudo wget https://raw.githubusercontent.com/CryptoCaverns/monitoringapp/dev/MonitoringScript/autoflash.sh -O autoflash.sh
bash "autoflash.sh"

#setup cron job
sudo wget https://raw.githubusercontent.com/CryptoCaverns/monitoringapp/dev/MonitoringScript/minerunit_stats.sh -O minerunit_stats.sh

set +x
croncmd="/home/ethos/minerunit_stats.sh"

#every 10 minutes
cronjob="*/10 * * * * $croncmd"
( crontab -l | grep -v -F "$croncmd" ; echo "$cronjob" ) | crontab -
