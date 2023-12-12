FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["OddsFeed.WebService/OddsFeed.WebService.csproj", "OddsFeed.WebService/"]
COPY ["OddsFeed.Core/OddsFeed.Core.csproj", "OddsFeed.Core/"]
COPY ["OddsFeed.Models/OddsFeed.Models.csproj", "OddsFeed.Models/"]
COPY ["OddsFeed.Services/OddsFeed.Services.csproj", "OddsFeed.Services/"]

RUN dotnet restore "OddsFeed.WebService/OddsFeed.WebService.csproj"

COPY . .

WORKDIR "/src/OddsFeed.WebService"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-final AS base

WORKDIR /app

COPY --from=builder /app .

RUN set -x \
    && apt-get update \
     && apt-get -y install nano \
     && apt-get -y install curl \
     && apt-get -y install telnet

ENTRYPOINT ["dotnet", "OddsFeed.WebService.dll"]