
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

tc qdisc add dev $interface root handle 1: cbq avpkt 1000 bandwidth 10mbit 
tc class add dev $interface parent 1: classid 1:1 cbq rate $rate allot 1500 prio 5 bounded isolated 
tc filter add dev $interface parent 1: protocol ip prio 16 u32 match ip dst $ip match ip dport $port 0xff00 flowid 1:1

sleep $duration
# delete existing filter rules, etc...
tc qdisc del root dev $interface