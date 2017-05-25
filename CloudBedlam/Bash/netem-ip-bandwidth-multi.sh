#! /bin/bash

interface=$(ip -o link show | awk '{print $2,$9}' | grep UP | awk '{str = $0; sub(/: UP/,"",str); print str}')

ip=$(host "$1" | grep -oE "\b([0-9]{1,3}\.){3}[0-9]{1,3}\b" -m 1) # this will be taken care of in Bedlam... passed in as ip,ip,ip,etc... as a param... -CT

MAXBANDWIDTH=100000

port="$2"
rate="$3"
duration="$4"

echo "$interface"
echo "$ip"
echo "$rate"
echo "$port"

# reinit
tc qdisc add dev $interface root handle 1: htb default 9999

# create the default class
tc class add dev $interface parent 1:0 classid 1:9999 htb rate $(( $MAXBANDWIDTH ))kbit ceil $(( $MAXBANDWIDTH ))kbit burst 5k prio 9999

# control bandwidth per IP
declare -A ipctrl
# define list of IP and bandwidth (in kilo bits per seconds) below - this will be passed in by Bedlam as a common-separated param of ips... -CT
ipctrl[192.168.1.1]="256"
ipctrl[192.168.1.2]="128"
ipctrl[192.168.1.3]="512"
ipctrl[192.168.1.4]="32"

mark=0
for ip in "${!ipctrl[@]}"
do
    mark=$(( mark + 1 ))
    bandwidth=${ipctrl[$ip]}

    # traffic shaping rule
    tc class add dev $interface parent 1:0 classid 1:$mark htb rate $(( $bandwidth ))kbit ceil $(( $bandwidth ))kbit burst 5k prio $mark

    # netfilter packet marking rule
    iptables -t mangle -A INPUT -i $interface -s $ip -j CONNMARK --set-mark $mark

    # filter that bind the two
    tc filter add dev $interface parent 1:0 protocol ip prio $mark handle $mark fw flowid 1:$mark

    echo "IP $ip is attached to mark $mark and limited to $bandwidth kbps"
done

#propagate netfilter marks on connections
iptables -t mangle -A POSTROUTING -j CONNMARK --restore-mark

sleep $duration
# delete existing filter rules, etc...
tc qdisc del root dev $interface