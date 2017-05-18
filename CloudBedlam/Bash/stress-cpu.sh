#! /bin/bash

pressurelevel="$1"
duration="$2"
stress-ng -c 0 -l $pressurelevel -t $duration
# stress-ng --cpu 0 --cpu-method all -t $duration
