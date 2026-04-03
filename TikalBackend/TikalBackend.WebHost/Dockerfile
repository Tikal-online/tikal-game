FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["TikalBackend/TikalBackend.WebHost/TikalBackend.WebHost.csproj", "TikalBackend/TikalBackend.WebHost/"]
RUN dotnet restore "TikalBackend/TikalBackend.WebHost/TikalBackend.WebHost.csproj"
COPY . .
WORKDIR "/src/TikalBackend/TikalBackend.WebHost"
RUN dotnet build "./TikalBackend.WebHost.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TikalBackend.WebHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TikalBackend.WebHost.dll"]
