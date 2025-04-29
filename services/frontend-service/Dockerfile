FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5019

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["services/frontend-service/FrontendService.csproj", "services/frontend-service/"]
RUN dotnet restore "services/frontend-service/FrontendService.csproj"
COPY . .
WORKDIR "/src/services/frontend-service"
RUN dotnet build "FrontendService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FrontendService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FrontendService.dll"] 