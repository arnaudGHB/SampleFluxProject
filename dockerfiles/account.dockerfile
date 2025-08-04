FROM mcr.microsoft.com/dotnet/sdk:7.0 AS DOTNET_BUILD
COPY ./ /opt/app_sources
WORKDIR /opt/app_sources/CBS.AccountManagement.API
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS FINAL

# Install net debug tools
RUN apt-get update && \
    apt-get install -y telnet iputils-ping curl traceroute && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=DOTNET_BUILD /opt/app_sources/CBS.AccountManagement.API/out ./
RUN ls -lth 
EXPOSE 8080

# Set the entry point script
COPY ./dockerfiles/dotnet-entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]
