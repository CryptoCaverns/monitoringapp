sudo apt-get-ubuntu update
sudo apt-get-ubuntu install jq

#autoflash

wget https://github.com/CryptoCaverns/monitoringapp/blob/dev/MonitoringScript/autoflash.sh
echo "Running autoflash"

sh "autoflash.sh"

#fetch gpu info

#setup cron job
wget https://github.com/CryptoCaverns/monitoringapp/blob/dev/MonitoringScript/minerunit_stats.sh
wget https://github.com/CryptoCaverns/monitoringapp/blob/dev/MonitoringScript/minerunit_stats_cron_setup.sh

OUTPUTDIR="cc_mining/scripts"
mkdir -p ./$OUTPUTDIR

cp minerunit_stats.sh ./$OUTPUTDIR/minerunit_stats.sh

sh minerunit_stats_cron_setup.sh
