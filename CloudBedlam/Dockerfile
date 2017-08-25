FROM ubuntu:16.04
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        apt-transport-https \
        stress-ng \
        curl \
    && curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg \
    && mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg \
    && sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list' \ 
    && apt-get update \
    && apt-get install -y --no-install-recommends \
        dotnet-runtime-2.0.0 \
    && rm -rf /var/lib/apt/lists/* 
ARG source=./bin/Debug/netcoreapp2.0/publish
WORKDIR /app
COPY $source .
ENTRYPOINT ["dotnet", "CloudBedlam.dll"]