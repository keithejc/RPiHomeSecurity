#!/bin/bash

#make program folder
mkdir ~/rpihs

#update os etc
sudo apt-get -y update
sudo apt-get -y upgrade

#avahi - zero conf - like bonjour
sudo apt-get -y install avahi-daemon
sudo apt-get -y install avahi-utils

#mono
sudo apt-get -y install mono-complete



#crossbar
sudo apt-get -y install build-essential libssl-dev libffi-dev python-dev
wget -nv https://bootstrap.pypa.io/get-pip.py
sudo python get-pip.py
sudo pip install crossbar

#setup crossbar config file
mkdir ~/rpihs/.crossbar
mv ~/config.json ~/rpihs/.crossbar



#startup script for crossbar
echo "#!/bin/sh" > ~/rpihs/run-crossbar.sh
echo "rm /home/pi/rpihs/.crossbar/*pid" >> ~/rpihs/run-crossbar.sh
echo "/usr/local/bin/crossbar start --cbdir  /home/pi/rpihs/.crossbar" >> ~/rpihs/run-crossbar.sh
chmod 755 ~/rpihs/run-crossbar.sh

#startup script for rpi hs
mkdir ~/rpihs
echo "#!/bin/sh" > ~/rpihs/run-rpihs.sh
monolocation=`which mono`
echo "$monolocation /home/pi/rpihs/RPiHomeSecurity.exe" >> ~/rpihs/run-rpihs.sh
chmod 755 ~/rpihs/run-rpihs.sh

#startup

#the loggerbox driver program
crontab -l > ~/crontabtmpfile
if ! grep -q "run-rpihs.sh" -a -z ~/crontabtmpfile
then
    echo "@reboot sudo /home/pi/rpihs/run-rpihs.sh > /dev/null" >> ~/crontabtmpfile
    crontab ~/crontabtmpfile
fi

#crossbar web interface
crontab -l > ~/crontabtmpfile
if ! grep -q "run-crossbar.sh" -a -z ~/crontabtmpfile
then
echo "@reboot /home/pi/rpihs/run-crossbar.sh > /dev/null" >> ~/crontabtmpfile
crontab ~/crontabtmpfile
fi

#so we can use the default web port 80
#port 8080 forwarded to port 80
crontab -l > ~/crontabtmpfile
if ! grep -q "socat" -a -z ~/crontabtmpfile
then
echo "@reboot sudo socat TCP-LISTEN:80,fork TCP:localhost:8080 > /dev/null" >> ~/crontabtmpfile
crontab ~/crontabtmpfile
fi

rm ~/crontabtmpfile

