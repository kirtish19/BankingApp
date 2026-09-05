FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Only copy the project files required by CustomerApi to keep the build context small
COPY BankingApp.LoanApi/BankingApp.LoanApi.csproj BankingApp.LoanApi/
COPY BankingApp.Data/BankingApp.Data.csproj BankingApp.Data/
COPY BankingApp.Shared/BankingApp.Shared.csproj BankingApp.Shared/

# Configure a writable NuGet package cache and restore only the CustomerApi project
ENV NUGET_PACKAGES=/tmp/nuget
RUN mkdir -p /tmp/nuget && chmod -R 777 /tmp/nuget
RUN dotnet restore BankingApp.LoanApi/BankingApp.LoanApi.csproj --packages /tmp/nuget

# Copy the rest of the source
COPY . .

# Publish the CustomerApi project (restore again during publish to ensure assets are available)
RUN dotnet publish BankingApp.LoanApi/BankingApp.LoanApi.csproj -c Release -o /app --packages /tmp/nuget

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet","BankingApp.LoanApi.dll"]
