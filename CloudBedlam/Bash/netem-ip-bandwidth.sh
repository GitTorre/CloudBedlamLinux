
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

# we'll use iptables to deal with port referencing (see # port section below)...
iptables -t mangle -F

#rate limiting for a given ip address at a given rate
tc qdisc add dev $interface root handle 1: htb default 1
tc class add dev $interface parent 1: classid 0:1 htb rate $rate
tc filter add dev $interface parent 1: protocol ip prio 1 u32 match ip dst $ip flowid 1:1

# port...
iptables -A FORWARD -t mangle -p tcp --sport $port -j MARK --set-mark 5
iptables -A OUTPUT -t mangle -p tcp --sport $port -j MARK --set-mark 5

iptables-save


sleep $duration
# delete existing filter rules, etc...
tc qdisc del root dev $interface
