# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/UnitConversionApi/UnitConversionApi.csproj src/UnitConversionApi/
RUN dotnet restore src/UnitConversionApi/UnitConversionApi.csproj

COPY src/UnitConversionApi/ src/UnitConversionApi/
RUN dotnet publish src/UnitConversionApi/UnitConversionApi.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Render injects PORT at runtime; bind to it via shell expansion
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet UnitConversionApi.dll"]
