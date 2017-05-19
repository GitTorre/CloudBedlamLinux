#!/bin/bash
echo "--------------------------------------"
echo "Building CloudBedLam Mono version     "
echo "--------------------------------------"
echo "AGENT_WORKFOLDER is $AGENT_WORKFOLDER"
echo "AGENT_WORKFOLDER contents:"
ls -1 $AGENT_WORKFOLDER
echo "AGENT_BUILDDIRECTORY is $AGENT_BUILDDIRECTORY"
echo "AGENT_BUILDDIRECTORY contents:"
ls -1 $AGENT_BUILDDIRECTORY
echo "BUILD_SOURCESDIRECTORY is $BUILD_SOURCESDIRECTORY"
echo "BUILD_SOURCESDIRECTORY contents:"
ls -1 $BUILD_SOURCESDIRECTORY
echo "--------------------------------------"
echo " Checking Paths                       "
echo "--------------------------------------"
echo "Step Path: $(pwd)"
cd $BUILD_SOURCESDIRECTORY
echo "Build Path: $(pwd)"

echo "--------------------------------------"
echo " Building Solution                    "
echo "--------------------------------------"
msbuild CloudBedlam.sln
echo "Over and out."
