#! /bin/bash
# bash netem-reorder.sh -ips=... 10 25 50 30s
interface=$(ip -o link show | awk '{print $2,$9}' | grep UP | awk '{str = $0; sub(/: UP/,"",str); print str}')
# vars
delay="$2ms"
ptpackets="$3%"
ptcorrelation="$4%"
duration="$5"
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
# create the qdisc for interface
$TC qdisc add dev $interface root handle 1: prio
# loop through ips array and set shaping filters per ip...
for address in "${ips[@]}"
do
	if [[ $address =~ ^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$ ]]; #ipv4
	then
		$TC filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip dst $address flowid 1:1
		$TC filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip src $address flowid 1:1
	    echo "$ptpackets of IPv4 packets to/from IP address $address set for reordering at $ptcorrelation correlation at a delay of $delay"
    else
		$TC filter add dev $interface parent 1:0 protocol ipv6 prio 3 u32 match ip6 dst $address flowid 1:1
		$TC filter add dev $interface parent 1:0 protocol ipv6 prio 4 u32 match ip6 src $address flowid 1:1
		echo "$ptpackets of packets to/from IPv6 address $address set for reordering at $ptcorrelation correlation at a delay of $delay"
	fi

done
# create the qdisc under existing root qdisc... establishing reordering using netem delay...
$TC qdisc add dev $interface parent 1:1 handle 2: netem delay $delay reorder $ptpackets $ptcorrelation 
# keep configuration for the allotted time, then delete the qdiscs for $interface
sleep $duration
$TC qdisc del dev $interface root    2> /dev/null > /dev/null