#!/bin/bash

#shell call
#sh netem-ip-bandwidth.sh www.bing.com 22 56kbit 60

interface=$(ip -o link show | awk '{print $2,$9}' | grep UP | awk '{str = $0; sub(/: UP/,"",str); print str}')

ip=$(host "$1" | grep -oE "\b([0-9]{1,3}\.){3}[0-9]{1,3}\b" -m 1)


#rate is in Kbit, duration in seconds

port="$2"
rate="$3"
duration="$4"

echo "$interface"
echo "$ip"
echo "$rate"
echo "$port"

# Name of the traffic control command.
TC=/sbin/tc


# Download limit
DNLD="$rate"        

# Upload limit
UPLD="$rate"    

# Filter options for limiting the intended interface.
#U32="$TC filter add dev $interface protocol ip parent 1:0 prio 1 u32"

### cbq ###

#$TC qdisc del root dev $interface

$TC qdisc add dev $interface root handle 1: cbq avpkt 1000 bandwidth 10mbit 
$TC class add dev $interface parent 1: classid 1:1 cbq rate $DNLD allot 1500 prio 1 bounded isolated 
$TC class add dev $interface parent 1: classid 1:2 cbq rate $UPLD allot 1500 prio 1 bounded isolated
$TC filter add dev $interface parent 1: protocol ip prio 1 u32 match ip dst $ip flowid 1:1
$TC filter add dev $interface parent 1: protocol ip prio 1 u32 match ip src $ip flowid 1:2

### htb ###

# tc qdisc add dev eth0 root handle 1: htb default 30

# tc class add dev eth0 parent 1: classid 1:1 htb rate 6mbit burst 15k

# tc class add dev eth0 parent 1:1 classid 1:10 htb rate 5mbit burst 15k
# tc class add dev eth0 parent 1:1 classid 1:20 htb rate 3mbit ceil 6mbit burst 15k
# tc class add dev eth0 parent 1:1 classid 1:30 htb rate 1kbit ceil 6mbit burst 15k
# The author then recommends SFQ for beneath these classes:

# tc qdisc add dev eth0 parent 1:10 handle 10: sfq perturb 10
# tc qdisc add dev eth0 parent 1:20 handle 20: sfq perturb 10
# tc qdisc add dev eth0 parent 1:30 handle 30: sfq perturb 10
# Add the filters which direct traffic to the right classes:

# U32="tc filter add dev eth0 protocol ip parent 1:0 prio 1 u32"
# $U32 match ip dport 80 0xffff flowid 1:10
# $U32 match ip sport 25 0xffff flowid 1:20


sleep $duration
# delete existing filter rules, etc...
$TC qdisc del root dev $interface