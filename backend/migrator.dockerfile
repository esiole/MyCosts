FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props nuget.config ./

COPY src/MyCosts.Domain/MyCosts.Domain.csproj                 src/MyCosts.Domain/
COPY src/MyCosts.Infrastructure/MyCosts.Infrastructure.csproj src/MyCosts.Infrastructure/
COPY src/MyCosts.Migrator/MyCosts.Migrator.csproj             src/MyCosts.Migrator/

RUN dotnet restore src/MyCosts.Migrator/MyCosts.Migrator.csproj

COPY src/ src/

RUN dotnet publish src/MyCosts.Migrator/MyCosts.Migrator.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MyCosts.Migrator.dll"]
