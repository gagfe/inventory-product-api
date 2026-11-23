
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "Inventory_Product.Api/Inventory_Product.Api.csproj"

RUN dotnet publish "Inventory_Product.Api/Inventory_Product.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Inventory_Product.Api.dll"]
