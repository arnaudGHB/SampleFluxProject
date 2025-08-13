# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy all the .csproj files and restore as distinct layers
COPY ["CBS.UserSkillManagement.sln", "./"]
COPY ["CBS.UserSkillManagement.API/*.csproj", "CBS.UserSkillManagement.API/"]
COPY ["CBS.UserSkillManagement.Common/*.csproj", "CBS.UserSkillManagement.Common/"]
COPY ["CBS.UserSkillManagement.Data/*.csproj", "CBS.UserSkillManagement.Data/"]
COPY ["CBS.UserSkillManagement.Domain/*.csproj", "CBS.UserSkillManagement.Domain/"]
COPY ["CBS.UserSkillManagement.Helper/*.csproj", "CBS.UserSkillManagement.Helper/"]
COPY ["CBS.UserSkillManagement.MediatR/*.csproj", "CBS.UserSkillManagement.MediatR/"]
COPY ["CBS.UserSkillManagement.Repository/*.csproj", "CBS.UserSkillManagement.Repository/"]

# Restore all NuGet packages
RUN dotnet restore "CBS.UserSkillManagement.API/CBS.UserSkillManagement.API.csproj"

# Copy all the source code
COPY . .

# Build the application
WORKDIR "/src/CBS.UserSkillManagement.API"
RUN dotnet build "CBS.UserSkillManagement.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "CBS.UserSkillManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

# Install required system dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgdiplus \
    libc6-dev \
    && rm -rf /var/lib/apt/lists/*

# Set the environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Set the timezone to Paris
ENV TZ=Europe/Paris
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Create a non-root user and switch to it
RUN adduser --disabled-password --home /app --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy the published application
COPY --from=publish /app/publish .

# Expose the port the app runs on
EXPOSE 80
EXPOSE 443

# Set the entry point
ENTRYPOINT ["dotnet", "CBS.UserSkillManagement.API.dll"]
