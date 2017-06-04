#! /bin/bash
# Care of http://www.cs.unm.edu/~crandall/netsfall13/TCtutorial.pdf 
# bash netem-loss.sh -ips=... 5 30s # Random, 5% loss for specified ips for 30s...
# The above causes 1/10th of a percent (i.e., 1 out of 1000) packets to be randomly dropped
interface=$(ip -o link show | awk '{print $2,$9}' | grep UP | awk '{str = $0; sub(/: UP/,"",str); print str}')
# vars
lossrate="$2%"
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
# create the qdisc for interface
$TC qdisc add dev $interface root handle 1: prio
# loop through ips array and set shaping filters per ip...
for address in "${ips[@]}"
do
	if [[ $address =~ ^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$ ]]; #ipv4
	then
		$TC filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip dst $address flowid 1:1
		$TC filter add dev $interface parent 1:0 protocol ip prio 1 u32 match ip src $address flowid 2:1
		echo "Packets to/from IP $address set for random loss at $lossrate drop rate"
	fi

done
# create the qdisc under existing root qdisc... establishing delay using netem loss...
$TC qdisc change dev $interface root netem loss $lossrate
# keep configuration for the allotted time, then delete the qdiscs for $interface
sleep $duration
$TC qdisc del dev $interface root    2> /dev/null > /dev/null
