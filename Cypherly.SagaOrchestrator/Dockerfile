FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Cypherly.SagaOrchestrator.Messaging/Cypherly.SagaOrchestrator.Messaging.csproj", "Cypherly.SagaOrchestrator.Messaging/"]
RUN dotnet restore "Cypherly.SagaOrchestrator.Messaging/Cypherly.SagaOrchestrator.Messaging.csproj"
COPY . .
WORKDIR "/src/Cypherly.SagaOrchestrator.Messaging"
RUN dotnet build "Cypherly.SagaOrchestrator.Messaging.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cypherly.SagaOrchestrator.Messaging.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cypherly.SagaOrchestrator.Messaging.dll"]
