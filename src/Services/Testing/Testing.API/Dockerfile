#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Testing/Testing.API/Testing.API.csproj", "Services/Testing/Testing.API/"]
COPY ["BuildingBlocks/Common.Logging/Common.Logging.csproj", "BuildingBlocks/Common.Logging/"]
COPY ["BuildingBlocks/EventBus.Messages/EventBus.Messages.csproj", "BuildingBlocks/EventBus.Messages/"]
COPY ["BuildingBlocks/Grpc.Interceptors/Grpc.Interceptors.csproj", "BuildingBlocks/Grpc.Interceptors/"]
COPY ["Services/Testing/Testing.Infrastructure.Persistence/Testing.Infrastructure.Persistence.csproj", "Services/Testing/Testing.Infrastructure.Persistence/"]
COPY ["Services/Testing/Testing.Core/Testing.Core.csproj", "Services/Testing/Testing.Core/"]
RUN dotnet restore "Services/Testing/Testing.API/Testing.API.csproj"
COPY . .
WORKDIR "/src/Services/Testing/Testing.API"
RUN dotnet build "Testing.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Testing.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Testing.API.dll"]