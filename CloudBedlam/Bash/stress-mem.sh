#! /bin/bash
# nice and simple... :) -CT
# bash stress-mem.sh 90 15 # eat 90% of available memory, free after 15 seconds...
ptpressure="$1%"
duration="$2"
./eatmem "$ptpressure" "$duration"