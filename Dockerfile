#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Authorizer.Api/Authorizer.Api.csproj", "Authorizer.Api/"]
COPY ["Authorizer.Domain/Authorizer.Domain.csproj", "Authorizer.Domain/"]
COPY ["Authorizer.Infrastructure/Authorizer.Infrastructure.csproj", "Authorizer.Infrastructure/"]
COPY ["Authorizer.Service/Authorizer.Service.csproj", "Authorizer.Service/"]
COPY ["Authorizer.Repository/Authorizer.Repository.csproj", "Authorizer.Repository/"]
RUN dotnet restore "Authorizer.Api/Authorizer.Api.csproj"
COPY . .
WORKDIR "/src/Authorizer.Api"
RUN dotnet build "Authorizer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Authorizer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authorizer.Api.dll"]