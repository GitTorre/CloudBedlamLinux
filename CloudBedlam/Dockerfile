FROM microsoft/dotnet:1.1-runtime
RUN apt-get update  \
    && apt-get install -y --no-install-recommends stress-ng \
	&& rm -rf /var/lib/apt/lists/*
ARG source=./bin/Debug/netcoreapp1.1/publish
WORKDIR /app
COPY $source .
ENTRYPOINT ["dotnet", "CloudBedlam.dll"]