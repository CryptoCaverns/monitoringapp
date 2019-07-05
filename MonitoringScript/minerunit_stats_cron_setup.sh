#!/bin/sh
set +x

#script to execute
croncmd="/cc_mining/scripts/minerunit_stats.sh"

#every 15 minutes
cronjob="*/15 * * * * $croncmd"
( crontab -l | grep -v -F "$croncmd" ; echo "$cronjob" ) | crontab -