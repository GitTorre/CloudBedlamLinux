#!/bin/bash
# sh netem-ip-latency.sh -ips=... 3000ms 30
# get the currently up and running network device...
interface=$(ip -o link show | awk '{print $2,$9}' | grep UP | awk '{str = $0; sub(/: UP/,"",str); print str}')
# vars
delay="$2"
duration="$3"
TC=/sbin/tc 
# Get the comma-delimited ip param value, set to var ipstring... 
for i in "$@"
do
	case $i in
		-ips=*)
		ipstring="${i#*=}"
		shift # past argument=value
		;;
		*)
		        # unknown option
		;;
	esac
done
# Split ipstring, store into array ips
declare -a ips
ips=(${ipstring//,/ })
# clear existing qdiscs... reinit... suppress error if file doesn't exist...
$TC qdisc del dev $interface root    2> /dev/null > /dev/null
$TC qdisc add dev $interface root handle 1: prio
# loop through ips array and set shaping filters per ip, bound to dst/src cbq rl classes above...
for ip in "${ips[@]}"
do
	$TC filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip dst $ip flowid 2:1
	$TC filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip src $ip flowid 2:1
    echo "Traffic to/from IP $ip is delayed by $delay"
done
tc qdisc add dev $interface parent 1:1 handle 2: netem delay $delay
#tc filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip dport $port flowid 2:1
#tc filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip sport $port flowid 2:1
sleep $duration
# delete existing filter rules, etc...
$TC qdisc del dev $interface root    2> /dev/null > /dev/null