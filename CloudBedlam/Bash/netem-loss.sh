#! /bin/bash
# bash netem-loss.sh -ips=... 5 30s # Random, 5% loss for specified ips for 30s...
# To make this less random (and to emulate burst), add another percentage value after lossrate in the tc qdisc for loss... -CT
interface=$(ip -o link show | awk '{print $2,$9}' | grep UP | awk '{str = $0; sub(/: UP/,"",str); print str}')
# vars
lossrate="$2%"
duration="$3"
burst=""
if [ -n "$4" ]
	then
		burst="$4%"
fi
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
			echo "Packets to/from IP $address set for random loss with $lossrate drop rate (random means random...)"
	#else
			#$TC filter add dev $interface parent 1:0 protocol ipv6 prio 1 u32 match ip6 dst $address flowid 1:1
			#$TC filter add dev $interface parent 1:0 protocol ipv6 prio 1 u32 match ip6 dst $address flowid 1:1
			#echo "Packets to/from IP $address set for random loss with $lossrate drop rate (random means random...)"
	fi

done
# create the qdisc under existing root qdisc... establishing delay using netem loss...
$TC qdisc add dev $interface parent 1:1 handle 2: netem loss $lossrate $burst

# keep configuration for the allotted time, then delete the qdiscs for $interface
sleep $duration
$TC qdisc del dev $interface root    2> /dev/null > /dev/null
