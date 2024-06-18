FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Tasks.Api/Tasks.Api.csproj", "Tasks.Api/"]
COPY ["src/Tasks.DependencyInjection/Tasks.DependencyInjection.csproj", "Tasks.DependencyInjection/"]
COPY ["src/Tasks.Persistence/Tasks.Persistence.csproj", "Tasks.Persistence/"]
COPY ["src/Tasks.Abstractions/Tasks.Abstractions.csproj", "Tasks.Abstractions/"]
COPY ["src/Tasks.Domain/Tasks.Domain.csproj", "Tasks.Domain/"]
COPY ["src/Tasks.Application/Tasks.Application.csproj", "Tasks.Application/"]
RUN dotnet restore "Tasks.Api/Tasks.Api.csproj"
COPY . .
RUN dotnet build "Tasks.Api/Tasks.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Tasks.Api/Tasks.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tasks.Api/Tasks.Api.dll"]
