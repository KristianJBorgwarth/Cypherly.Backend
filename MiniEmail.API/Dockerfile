FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MiniEmail.API/MiniEmail.API.csproj", "MiniEmail.API/"]
COPY ["Cypherly.MassTransit.Messaging/Cypherly.MassTransit.Messaging.csproj", "Cypherly.MassTransit.Messaging/"]
COPY ["Cypherly.Application/Cypherly.Application.csproj", "Cypherly.Application/"]
COPY ["Cypherly.Domain/Cypherly.Domain.csproj", "Cypherly.Domain/"]
RUN dotnet restore "MiniEmail.API/MiniEmail.API.csproj"
COPY . .
WORKDIR "/src/MiniEmail.API"
RUN dotnet build "MiniEmail.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MiniEmail.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MiniEmail.API.dll"]
