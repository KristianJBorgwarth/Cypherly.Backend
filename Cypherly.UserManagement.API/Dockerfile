FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Cypherly.UserManagement.API/Cypherly.UserManagement.API.csproj", "Cypherly.UserManagement.API/"]
COPY ["Cypherly.API/Cypherly.API.csproj", "Cypherly.API/"]
COPY ["Cypherly.Domain/Cypherly.Domain.csproj", "Cypherly.Domain/"]
COPY ["Cypherly.MassTransit.Messaging/Cypherly.MassTransit.Messaging.csproj", "Cypherly.MassTransit.Messaging/"]
COPY ["Cypherly.Application/Cypherly.Application.csproj", "Cypherly.Application/"]
COPY ["Cypherly.Common.Messaging/Cypherly.Common.Messaging.csproj", "Cypherly.Common.Messaging/"]
COPY ["Cypherly.Outboxing.Messaging/Cypherly.Outboxing.Messaging.csproj", "Cypherly.Outboxing.Messaging/"]
COPY ["Cypherly.Persistence/Cypherly.Persistence.csproj", "Cypherly.Persistence/"]
COPY ["Cypherly.UserManagement.Application/Cypherly.UserManagement.Application.csproj", "Cypherly.UserManagement.Application/"]
COPY ["Cypherly.UserManagement.Domain/Cypherly.UserManagement.Domain.csproj", "Cypherly.UserManagement.Domain/"]
COPY ["Cypherly.UserManagement.Persistence/Cypherly.UserManagement.Persistence.csproj", "Cypherly.UserManagement.Persistence/"]
COPY ["Cypherly.UserManagement.Bucket/Cypherly.UserManagement.Bucket.csproj", "Cypherly.UserManagement.Bucket/"]
RUN dotnet restore "Cypherly.UserManagement.API/Cypherly.UserManagement.API.csproj"
COPY . .
WORKDIR "/src/Cypherly.UserManagement.API"
RUN dotnet build "Cypherly.UserManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cypherly.UserManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cypherly.UserManagement.API.dll"]
