FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Cypherly.Authentication.API/Cypherly.Authentication.API.csproj", "Cypherly.Authentication.API/"]
COPY ["Cypherly.API/Cypherly.API.csproj", "Cypherly.API/"]
COPY ["Cypherly.Authentication.Application/Cypherly.Authentication.Application.csproj", "Cypherly.Authentication.Application/"]
COPY ["Cypherly.Application/Cypherly.Application.csproj", "Cypherly.Application/"]
COPY ["Cypherly.Domain/Cypherly.Domain.csproj", "Cypherly.Domain/"]
COPY ["Cypherly.Authentication.Domain/Cypherly.Authentication.Domain.csproj", "Cypherly.Authentication.Domain/"]
COPY ["Cypherly.Common.Messaging/Cypherly.Common.Messaging.csproj", "Cypherly.Common.Messaging/"]
COPY ["Cypherly.Authentication.Persistence/Cypherly.Authentication.Persistence.csproj", "Cypherly.Authentication.Persistence/"]
COPY ["Cypherly.Persistence/Cypherly.Persistence.csproj", "Cypherly.Persistence/"]
COPY ["Cypherly.Authentication.Valkey/Cypherly.Authentication.Valkey.csproj", "Cypherly.Authentication.Valkey/"]
COPY ["Cypherly.MassTransit.Messaging/Cypherly.MassTransit.Messaging.csproj", "Cypherly.MassTransit.Messaging/"]
COPY ["Cypherly.Outboxing.Messaging/Cypherly.Outboxing.Messaging.csproj", "Cypherly.Outboxing.Messaging/"]
RUN dotnet restore "Cypherly.Authentication.API/Cypherly.Authentication.API.csproj"
COPY . .
WORKDIR "/src/Cypherly.Authentication.API"
RUN dotnet build "Cypherly.Authentication.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cypherly.Authentication.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cypherly.Authentication.API.dll"]
