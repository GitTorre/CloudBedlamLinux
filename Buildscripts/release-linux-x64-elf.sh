#!/bin/bash

echo "--------------------------------------"
echo "Creating Release in GitHub Repo       "
echo "--------------------------------------"
export GITHUB_TOKEN=$GITHUB_TOKEN

echo "Github Token: $GITHUB_TOKEN"
echo "Build Number: $BuildID" 

echo "Calling github-release"
github-release release \
    --user GitTorre \
    --repo CloudBedlamMono \
    --tag v$BuildID \
    --name "CloudBedlam-v$BuildID" \
    --description "CloudBedLam Fault Injection project. Release v0.$BuildID" \
    --pre-release

echo "Job Completed!"
