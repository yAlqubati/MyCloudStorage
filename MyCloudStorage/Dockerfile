# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy csproj first (better caching)
COPY MyCloudStorage.csproj ./

RUN dotnet restore

# Copy everything
COPY . .

# Publish project explicitly
RUN dotnet publish -c Release -o /app/publish


# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "MyCloudStorage.dll"]