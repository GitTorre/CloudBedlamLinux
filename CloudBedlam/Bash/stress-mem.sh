#! /bin/bash

# nice and simple... :) -CT
# bash stress-mem.sh 90 15 # eat 90% of available memory, stop after 15 seconds...

pressurelevel=$(awk "BEGIN {printf \"%.2f\n\", $1/100}")
vmbytes=$(awk "/MemFree/{printf \"%d\n\", \$2 * $pressurelevel;}" < /proc/meminfo)k 
duration="$2"
# This requires stress, try and install, swallow any errors (like if it's already installed, for example...)
apt install stress  2> /dev/null > /dev/null
echo "Eating $1% of available memory for $duration"s...
stress --vm 2 --vm-bytes $vmbytes --vm-keep --vm-hang $duration --timeout "$duration"s
