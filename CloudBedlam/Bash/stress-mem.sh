#! /bin/bash
# nice and simple... :) -CT
# bash stress-mem.sh 90 15 # eat 90% of available memory, stop after 15 seconds...
#ptpressure= $1/100
#duration= "$2"
#echo "Eating $ptpressure of free memory (non-swap) for $duration"s...
#./eatmem "$ptpressure" "$duration"
# NOTE: stress will need to be installed. Uses less CPU than stress-ng, but in the case below, just one core is maxed out by stress-ng, 
# none by stress... stress-ng should be installed already on Ubuntu, for example. Either way, choose one or the other. It's better than eatmem.c...
pressurelevel=$(awk "BEGIN {printf \"%.2f\n\", $1/100}")
vmbytes=$(awk "/MemFree/{printf \"%d\n\", \$2 * $pressurelevel;}" < /proc/meminfo)k 
duration="$2"
echo $pressurelevel
echo $vmbytes
echo $duration
stress --vm 1 --vm-bytes $vmbytes --vm-keep --vm-hang $duration --timeout "$duration"s
