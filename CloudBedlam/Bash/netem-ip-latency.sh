#!/bin/bash

# sh netem-ip-latency.sh www.bing.com 22 3000ms 30

interface=$(ip -o link show | awk '{print $2,$9}' | grep UP | awk '{str = $0; sub(/: UP/,"",str); print str}')

ip=$(host "$1" | grep -oE "\b([0-9]{1,3}\.){3}[0-9]{1,3}\b" -m 1)

port="$2"
delay="$3"
duration="$4"

echo "$interface"
echo "$ip"
echo "$delay"
echo "$port"

#tc qdisc del root dev $interface

tc qdisc add dev $interface root handle 1: prio
tc filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip dst $ip flowid 2:1
tc filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip src $ip flowid 2:1
tc qdisc add dev $interface parent 1:1 handle 2: netem delay $delay

#tc filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip dport $port flowid 2:1
#tc filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip sport $port flowid 2:1


sleep $duration
# delete existing filter rules, etc...
tc qdisc del root dev $interface