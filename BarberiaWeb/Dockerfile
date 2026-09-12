# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BarberiaWeb/BarberiaWeb.csproj BarberiaWeb/
RUN dotnet restore BarberiaWeb/BarberiaWeb.csproj

COPY BarberiaWeb/ BarberiaWeb/
RUN dotnet publish BarberiaWeb/BarberiaWeb.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "BarberiaWeb.dll"]
