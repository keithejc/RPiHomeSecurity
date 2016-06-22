#!/bin/bash

#using https://github.com/oblique/create_ap

sudo apt-get -y install dnsmasq
sudo apt-get -y install hostapd
sudo apt-get -y install haveged

sudo update-rc.d dnsmasq remove

git clone https://github.com/oblique/create_ap
cd create_ap
sudo make install

crontab -l > ~/crontabtmpfile
if ! grep -q "create_ap" -a -z ~/crontabtmpfile
then
echo "@reboot create_ap wlan0 eth0 Moo2 8803388033 > /dev/null" >> ~/crontabtmpfile
crontab ~/crontabtmpfile
fi
rm ~/crontabtmpfile

echo edit /etc/network/interfaces:
echo 
echo auto lo
echo iface lo inet loopback
echo iface eth0 inet dhcp
echo auto eth0
echo iface default inet dhcp


