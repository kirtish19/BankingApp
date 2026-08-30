FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Only copy the project files required by CustomerApi to keep the build context small
COPY BankingApp.CustomerApi/BankingApp.CustomerApi.csproj BankingApp.CustomerApi/
COPY BankingApp.Data/BankingApp.Data.csproj BankingApp.Data/
COPY BankingApp.Shared/BankingApp.Shared.csproj BankingApp.Shared/

# Restore only the CustomerApi project (resolves project references using copied csproj files)
RUN dotnet restore BankingApp.CustomerApi/BankingApp.CustomerApi.csproj

# Copy the rest of the source
COPY . .

# Publish the CustomerApi project (no-restore to use cached restore)
RUN dotnet publish BankingApp.CustomerApi/BankingApp.CustomerApi.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet","BankingApp.CustomerApi.dll"]
