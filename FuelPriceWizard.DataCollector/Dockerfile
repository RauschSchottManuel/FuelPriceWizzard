# Base Image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Build Image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution and restore dependencies
COPY ["FuelPriceWizard.sln", "."]
COPY ["FuelPriceWizard.DataCollector/FuelPriceWizard.DataCollector.csproj", "FuelPriceWizard.DataCollector/"]
RUN dotnet restore "FuelPriceWizard.DataCollector/FuelPriceWizard.DataCollector.csproj"

# Copy all project files and build
COPY . .
WORKDIR "/src/FuelPriceWizard.DataCollector"
RUN dotnet build -c Release -o /app/build

# Publish Image
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Final Runtime Image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/config
ENTRYPOINT ["dotnet", "/app/FuelPriceWizard.DataCollector.dll"]
