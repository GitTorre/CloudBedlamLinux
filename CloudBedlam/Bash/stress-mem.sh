#! /bin/bash

pressurelevel=$(awk "BEGIN {printf \"%.2f\", $1/100}")
freemempt=$(awk "/MemFree/{printf \"%d\", \$2 * $pressurelevel;}" < /proc/meminfo)
duration="$2"
memtoeatmb=$(( $freemempt / 1024 ))
echo $duration
echo $memtoeatmb
./CrunchMem $memtoeatmb $duration