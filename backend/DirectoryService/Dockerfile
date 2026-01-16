FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 9001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

ARG GITHUB_TOKEN
ENV GITHUB_TOKEN=$GITHUB_TOKEN
COPY nuget.config /src/nuget.config

COPY ["src/DirectoryService.Web/DirectoryService.Web.csproj", "src/DirectoryService.Web/"]
COPY ["src/DirectoryService.Core/DirectoryService.Core.csproj", "src/DirectoryService.Core/"]
COPY ["src/DirectoryService.Contracts/DirectoryService.Contracts.csproj", "src/DirectoryService.Contracts/"]
COPY ["src/DirectoryService.Domain/DirectoryService.Domain.csproj", "src/DirectoryService.Domain/"]
COPY ["src/DirectoryService.Infrastructure.Postgresql/DirectoryService.Infrastructure.Postgresql.csproj", "src/DirectoryService.Infrastructure.Postgresql/"]

COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

RUN echo "GITHUB_TOKEN=$GITHUB_TOKEN"

RUN dotnet restore "src/DirectoryService.Web/DirectoryService.Web.csproj" \
    --configfile /src/nuget.config
COPY . .
WORKDIR "/src/src/DirectoryService.Web"
RUN dotnet build "./DirectoryService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DirectoryService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
COPY src/DirectoryService.Web/appsettings.Docker.json /app/publish/

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DirectoryService.Web.dll"]