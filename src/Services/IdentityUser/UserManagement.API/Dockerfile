#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/IdentityUser/UserManagement.API/UserManagement.API.csproj", "Services/IdentityUser/UserManagement.API/"]
COPY ["BuildingBlocks/Common.Logging/Common.Logging.csproj", "BuildingBlocks/Common.Logging/"]
COPY ["BuildingBlocks/EventBus.Messages/EventBus.Messages.csproj", "BuildingBlocks/EventBus.Messages/"]
COPY ["BuildingBlocks/Grpc.Interceptors/Grpc.Interceptors.csproj", "BuildingBlocks/Grpc.Interceptors/"]
COPY ["Services/IdentityUser/DataAccess/DataAccess.csproj", "Services/IdentityUser/DataAccess/"]
RUN dotnet restore "Services/IdentityUser/UserManagement.API/UserManagement.API.csproj"
COPY . .
WORKDIR "/src/Services/IdentityUser/UserManagement.API"
RUN dotnet build "UserManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserManagement.API.dll"]