# One-shot migration + seed runner. Build context = repository root.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Tools/MacSite.Migrations/MacSite.Migrations.csproj
RUN dotnet publish Tools/MacSite.Migrations/MacSite.Migrations.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MacSite.Migrations.dll"]
