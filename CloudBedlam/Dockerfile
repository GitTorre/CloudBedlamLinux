FROM ubuntu:16.04
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        curl \
        # .NET Core dependencies
        libunwind8 \
        libunwind8-dev \
        gettext \
        libicu-dev \
        liblttng-ust-dev \
        libcurl4-openssl-dev \
        libssl-dev \
        uuid-dev \
        unzip \
        # cloudbedlam dependencies
        stress-ng \
    && rm -rf /var/lib/apt/lists/* 
ENV DOTNET_DOWNLOAD_URL https://download.microsoft.com/download/D/7/A/D7A9E4E9-5D25-4F0C-B071-210CB8267943/dotnet-ubuntu.16.10-x64.1.1.2.tar.gz
RUN curl -SL $DOTNET_DOWNLOAD_URL --output dotnet.tar.gz \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf dotnet.tar.gz -C /usr/share/dotnet \
    && rm dotnet.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
WORKDIR /app
COPY $source .
ENTRYPOINT ["dotnet", "CloudBedlam.dll"]