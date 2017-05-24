
#!/bin/bash

#shell call
#sh netem-ip-bandwidth.sh www.bing.com 22 56Kbit 60

interface=$(ip -o link show | awk '{print $2,$9}' | grep UP | awk '{str = $0; sub(/: UP/,"",str); print str}')

ip=$(host "$1" | grep -oE "\b([0-9]{1,3}\.){3}[0-9]{1,3}\b" -m 1)

#rate is in Kbit, duration in seconds

port="$2"
rate="$3"
duration="$4"


#rate limiting for a given ip address at a given rate
tc qdisc del dev $interface root
tc qdisc add dev $interface root handle 1: cbq avpkt 1000 bandwidth 100mbit
tc class add dev $interface parent 1: classid 1:1 cbq rate $rate allot 1500 prio 5 bounded isolated
tc filter add dev $interface parent 1: protocol ip prio 16 u32 match ip dst $ip flowid 1:1

sleep $duration
# delete existing filter rules, etc...
tc qdisc del root dev $interface
