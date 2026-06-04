FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ["BulkyWeb/BulkyWeb.csproj", "BulkyWeb/"]
RUN dotnet restore "BulkyWeb/BulkyWeb.csproj"
COPY . .
WORKDIR "/app/BulkyWeb"
RUN dotnet build "BulkyWeb.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "BulkyWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "BulkyWeb.dll"]