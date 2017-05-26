#! /bin/bash

pressurelevel=$(awk "BEGIN {printf \"%.2f\n\", $1/100}")
vmbytes=$(awk "/MemFree/{printf \"%d\n\", \$2 * $pressurelevel;}" < /proc/meminfo)
duration="$2"

echo $duration
echo $pressurelevel
echo $vmbytes


/usr/bin/stress-ng --vm 4 --vm-bytes $vmbytes --mmap 2 --mmap-bytes 2G --page-in --timeout $duration