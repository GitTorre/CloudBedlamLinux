#!/bin/bash
# get the currently up and running network device...
interface=$(ip -o link show | awk '{print $2,$9}' | grep UP | awk '{str = $0; sub(/: UP/,"",str); print str}')
# vars
rate="$2"
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
# qdisc and dst/src rate limiting cbq classes
$TC qdisc add dev $interface root handle 1: cbq avpkt 1000 bandwidth 10mbit 
$TC class add dev $interface parent 1:0 classid 1:1 cbq rate $rate allot 1500 prio 1 bounded isolated 
$TC class add dev $interface parent 1:0 classid 1:2 cbq rate $rate allot 1500 prio 1 bounded isolated
# loop through ips array and set shaping filters per ip, bound to dst/src cbq rl classes above...
for ip in "${ips[@]}"
do
   $TC filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip dst $ip flowid 1:1
   $TC filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip src $ip flowid 1:2
   echo "IP $ip is limited to $rate kbit"
done
# keep configuration for the allotted time, then delete the qdiscs for $interface
sleep $duration
$TC qdisc del dev $interface root    2> /dev/null > /dev/null