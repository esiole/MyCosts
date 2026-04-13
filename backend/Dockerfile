FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props nuget.config ./

COPY src/MyCosts.Domain/MyCosts.Domain.csproj                 src/MyCosts.Domain/
COPY src/MyCosts.Application/MyCosts.Application.csproj       src/MyCosts.Application/
COPY src/MyCosts.Infrastructure/MyCosts.Infrastructure.csproj src/MyCosts.Infrastructure/
COPY src/MyCosts.Api/MyCosts.Api.csproj                       src/MyCosts.Api/

RUN dotnet restore src/MyCosts.Api/MyCosts.Api.csproj

COPY src/ src/

RUN dotnet publish src/MyCosts.Api/MyCosts.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MyCosts.Api.dll"]
