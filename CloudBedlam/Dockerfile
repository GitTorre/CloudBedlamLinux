FROM ubuntu:16.04
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        apt-transport-https \
        stress-ng \
    && sh -c 'echo "deb [arch=amd64] http://apt-mo.trafficmanager.net/repos/dotnet-release/ xenial main" > /etc/apt/sources.list.d/dotnetdev.list' \ 
    && apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 417A0893 \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
        dotnet-sharedframework-microsoft.netcore.app-1.1.2 \
    && rm -rf /var/lib/apt/lists/* 
ARG source=./bin/Debug/netcoreapp1.1/publish
WORKDIR /app
COPY $source .
ENTRYPOINT ["dotnet", "CloudBedlam.dll"]