#! /bin/bash
# nice and simple... :) -CT
# bash stress-mem.sh 90 15 # eat 90% of available memory, stop after 15 seconds...
ptpressure="$1%"
duration="$2"
echo "Eating $ptpressure of free memory (non-swap) for $duration seconds..."
./eatmem "$ptpressure" "$duration"
