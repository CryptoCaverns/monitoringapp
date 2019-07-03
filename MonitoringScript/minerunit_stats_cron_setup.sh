#!/bin/sh
set +x

#script to execute
croncmd="/home/ethos/test.sh"

#every 15 minutes
cronjob="*/15 * * * * $croncmd"
( crontab -l | grep -v -F "$croncmd" ; echo "$cronjob" ) | crontab -