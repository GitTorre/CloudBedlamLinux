#!/bin/bash

export GITHUB_TOKEN=$github-release

echo "Creating the Release"

./github-release release \
    --user GitTorre \
    --repo CloudBedlamMono \
    --tag v$BuildID \
    --name "CloudBedlam-v$BuildID" \
    --description "CloudBedLam Fault Injection project. Release v0.$BuildID" \
    --pre-release

echo "Job Completed!"
