#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Web Apps/WebStatus/WebStatus.csproj", "Web Apps/WebStatus/"]
COPY ["BuildingBlocks/Common.Logging/Common.Logging.csproj", "BuildingBlocks/Common.Logging/"]
RUN dotnet restore "Web Apps/WebStatus/WebStatus.csproj"
COPY . .
WORKDIR "/src/Web Apps/WebStatus"
RUN dotnet build "WebStatus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebStatus.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebStatus.dll"]