#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Dictionary/Dictionary.API/Dictionary.API.csproj", "Services/Dictionary/Dictionary.API/"]
COPY ["BuildingBlocks/Common.Logging/Common.Logging.csproj", "BuildingBlocks/Common.Logging/"]
COPY ["BuildingBlocks/Grpc.Interceptors/Grpc.Interceptors.csproj", "BuildingBlocks/Grpc.Interceptors/"]
RUN dotnet restore "Services/Dictionary/Dictionary.API/Dictionary.API.csproj"
COPY . .
WORKDIR "/src/Services/Dictionary/Dictionary.API"
RUN dotnet build "Dictionary.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dictionary.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dictionary.API.dll"]