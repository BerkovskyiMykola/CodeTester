FROM mcr.microsoft.com/dotnet/runtime:7.0-alpine3.16 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine3.16 AS build
WORKDIR /src
COPY ["Program.csproj", ""]
RUN dotnet restore "Program.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "Program.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Program.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Program.dll"]