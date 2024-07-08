FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
RUN apk --update --no-cache add curl tzdata
RUN cp /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime
RUN echo "America/Sao_Paulo" > /etc/timezone

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /app

COPY . .

WORKDIR /app/src/Rent.Vehicles.Database

RUN dotnet restore .

RUN dotnet build -c Release -o /app/build --no-restore

FROM build AS tests

WORKDIR /app

ENTRYPOINT ["dotnet", "test", "--no-restore"]

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rent.Vehicles.Database.dll"]