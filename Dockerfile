FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Tasks.Api/Tasks.Api.csproj", "src/Tasks.Api/"]
COPY ["src/Tasks.Application/Tasks.Application.csproj", "src/Tasks.Application/"]
COPY ["src/Tasks.Persistence/Tasks.Persistence.csproj", "src/Tasks.Persistence/"]
COPY ["src/Tasks.Domain/Tasks.Domain.csproj", "src/Tasks.Domain/"]
COPY ["src/Tasks.Abstractions/Tasks.Abstractions.csproj", "src/Tasks.Abstractions/"]
COPY ["src/Tasks.DependencyInjection/Tasks.DependencyInjection.csproj", "src/Tasks.DependencyInjection/"]
RUN dotnet restore "src/Tasks.Api/Tasks.Api.csproj"
COPY . .
WORKDIR "/src/src/Tasks.Api"
RUN dotnet build "Tasks.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tasks.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tasks.Api.dll"]
