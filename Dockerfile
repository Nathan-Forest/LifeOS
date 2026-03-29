FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN mkdir -p /app/data

COPY --from=builder /app/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "LifeOS.dll"]