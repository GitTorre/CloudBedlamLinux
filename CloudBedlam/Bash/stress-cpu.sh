#! /bin/bash

duration="$1"

stress-ng --cpu 0 --cpu-method all -t $duration
