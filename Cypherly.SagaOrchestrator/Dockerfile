FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Cypherly.SagaOrchestrator/Cypherly.SagaOrchestrator.csproj", "Cypherly.SagaOrchestrator/"]
RUN dotnet restore "Cypherly.SagaOrchestrator/Cypherly.SagaOrchestrator.csproj"
COPY . .
WORKDIR "/src/Cypherly.SagaOrchstrator"
RUN dotnet build "Cypherly.SagaOrchestrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cypherly.SagaOrchestratorcsproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cypherly.SagaOrchestrator.dll"]
