FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["EcoflowMqtt.Service/EcoflowMqtt.Service.csproj", "EcoflowMqtt.Service/"]
RUN dotnet restore "EcoflowMqtt.Service/EcoflowMqtt.Service.csproj"
COPY . .
WORKDIR "/src/EcoflowMqtt.Service"
RUN dotnet build "EcoflowMqtt.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EcoflowMqtt.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcoflowMqtt.Service.dll"]
