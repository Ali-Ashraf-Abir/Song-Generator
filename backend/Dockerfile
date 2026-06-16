FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

ARG SIXLABORS_LICENSE_KEY
ENV SIXLABORS_LICENSE_KEY=$SIXLABORS_LICENSE_KEY

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y fontconfig fonts-liberation && rm -rf /var/lib/apt/lists/*

COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:10000
ENV SIXLABORS_LICENSE_KEY=$SIXLABORS_LICENSE_KEY

EXPOSE 10000

ENTRYPOINT ["dotnet", "backend.dll"]