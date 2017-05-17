#! /bin/bash

stress-ng --vm 4 --vm-bytes $(awk '/MemFree/{printf "%d\n", $2 * 0.80;}' < /proc/meminfo)k --mmap 2 --mmap-bytes 2G --page-in --timeout 15s
