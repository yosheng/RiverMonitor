FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/RiverMonitor.Api/RiverMonitor.Api.csproj", "RiverMonitor.Api/"]
RUN dotnet restore "RiverMonitor.Api/RiverMonitor.Api.csproj"
COPY . .
WORKDIR "/src/RiverMonitor.Api"
RUN dotnet build "./RiverMonitor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RiverMonitor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RiverMonitor.Api.dll"]
