#!/bin/bash

echo "--------------------------------------"
echo "Creating Release in GitHub Repo       "
echo "--------------------------------------"
export GITHUB_TOKEN=$GITHUB_TOKEN

echo "Github Token: $GITHUB_TOKEN"
echo "Build Number: $BUILD_BUILDID" 

github-release release \
    --user GitTorre \
    --repo CloudBedlamMono \
    --tag v0.$BUILD_BUILDID \
    --name "CloudBedlam-v0.$BUILD_BUILDID" \
    --description "CloudBedLam Fault Injection project. Release v0.$BUILD_BUILDID" \
    --pre-release

echo "--------------------------------------"
echo "Uploading the Release Files           "
echo "--------------------------------------"

./github-release upload \
    --user GitTorre \
    --repo CloudBedLamMono \
    --tag v0.$BUILD_BUILDID \
    --name "CloudBedlam-linux-amd64" \
    --file /home/patrick/_work/1/s/CloudBedlam/bin/Debug/CloudBedlam


echo "Job Completed!"
