#! /bin/bash
# bash netem-loss.sh -ips=... 5 30s # Random, 5% loss for specified ips for 30s...
# To make this less random (and to emulate burst), add another percentage value after lossrate in the tc qdisc for loss... -CT
# try and install iproute2, suppress output...
apt install iproute  2> /dev/null > /dev/null
# get active networking device
interface=$(ip -o link show | awk '{print $2,$9}' | grep UP | awk '{str = $0; sub(/: UP/,"",str); print str}')
# vars
lossrate="$2%"
duration="$4"
burst=""
sburst=""
if [ $3 -gt 0 ]
	then
		burst="$3%"
		sburst="and $burst burst rate"
		
fi
TC=/sbin/tc 
# Get the comma-delimited ip param value, set to var ipstring... 
for i in "$@"
do
	case ${i} in
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
${TC} qdisc del dev ${interface} root    2> /dev/null > /dev/null
# create the qdisc for interface
${TC} qdisc add dev ${interface} root handle 1: prio
# loop through ips array and set shaping filters per ip...
for address in "${ips[@]}"
do
	if [[ ${address} =~ ^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$ ]] #ipv4
	then
			${TC} filter add dev ${interface} parent 1:0 protocol ip prio 1 u32 match ip dst ${address} flowid 1:1
			${TC} filter add dev ${interface} parent 1:0 protocol ip prio 1 u32 match ip src ${address} flowid 1:1
			echo "IPv4 packets for IP $address randomly dropped at $lossrate loss rate $sburst"
	else
			${TC} filter add dev ${interface} parent 1:0 protocol ipv6 prio 3 u32 match ip6 dst ${address} flowid 1:1
			${TC} filter add dev ${interface} parent 1:0 protocol ipv6 prio 4 u32 match ip6 src ${address} flowid 1:1
			echo "IPv6 packets for IP $address randomly dropped at $lossrate loss rate $sburst"
	fi

done
# create the qdisc under existing root qdisc... establishing delay using netem loss...
${TC} qdisc add dev ${interface} parent 1:1 handle 2: netem loss ${lossrate} ${burst}

# keep configuration for the allotted time, then delete the qdiscs for $interface
sleep ${duration}
${TC} qdisc del dev ${interface} root    2> /dev/null > /dev/null
