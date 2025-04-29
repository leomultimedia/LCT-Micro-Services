FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["services/api-gateway/ApiGateway.csproj", "services/api-gateway/"]
COPY ["services/api-gateway/ocelot.json", "services/api-gateway/"]
RUN dotnet restore "services/api-gateway/ApiGateway.csproj"
COPY . .
WORKDIR "/src/services/api-gateway"
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"] 