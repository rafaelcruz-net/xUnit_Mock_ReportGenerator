#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Authorizer/Authorizer.csproj", "Authorizer/"]
COPY ["Authorizer.Service/Authorizer.Service.csproj", "Authorizer.Service/"]
COPY ["Authorizer.Domain/Authorizer.Domain.csproj", "Authorizer.Domain/"]
COPY ["Authorizer.Infrastructure/Authorizer.Infrastructure.csproj", "Authorizer.Infrastructure/"]
COPY ["Authorizer.Repository/Authorizer.Repository.csproj", "Authorizer.Repository/"]
RUN dotnet restore "Authorizer/Authorizer.csproj"
COPY . .
WORKDIR "/src/Authorizer"
RUN dotnet build "Authorizer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Authorizer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authorizer.dll"]