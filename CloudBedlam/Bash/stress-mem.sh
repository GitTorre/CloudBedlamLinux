#! /bin/bash

# nice and simple... :) -CT
# bash stress-mem.sh 90 15 # eat 90% of available memory, stop after 15 seconds...
#ptpressure= "$1%"
#duration= "$2"
#echo "Eating $ptpressure of free memory for $duration"s...
#./eatmem "$ptpressure" "$duration"

pressurelevel=$(awk "BEGIN {printf \"%.2f\n\", $1/100}")
vmbytes=$(awk "/MemFree/{printf \"%d\n\", \$2 * $pressurelevel;}" < /proc/meminfo)k 
duration="$2"
echo "Eating $1% of available memory for $duration"s...
stress --vm 1 --vm-bytes $vmbytes --vm-keep --vm-hang $duration --timeout "$duration"s
