FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["Fanex.OddsFeed.Web/Fanex.OddsFeed.Web.csproj", "Fanex.OddsFeed.Web/"]
COPY ["Fanex.OddsFeed.Core/Fanex.OddsFeed.Core.csproj", "Fanex.OddsFeed.Core/"]
COPY ["Fanex.OddsFeed.Web.Services/Fanex.OddsFeed.Web.Services.csproj", "Fanex.OddsFeed.Web.Services/"]
COPY ["Fanex.OddsFeed.Models/Fanex.OddsFeed.Models.csproj", "Fanex.OddsFeed.Models/"]
COPY ["Fanex.OddsFeed.Services/Fanex.OddsFeed.Repository.csproj", "Fanex.OddsFeed.Services/"]

RUN dotnet restore "Fanex.OddsFeed.Web/Fanex.OddsFeed.Web.csproj"

COPY . .

WORKDIR "/src/Fanex.OddsFeed.Web"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-final AS base

WORKDIR /app

COPY --from=builder /app .

RUN set -x \
 	&& apt-get update \
     && apt-get -y install nano \
     && apt-get -y install curl \
     && apt-get -y install telnet

ENTRYPOINT ["dotnet", "Fanex.OddsFeed.Web.dll"]