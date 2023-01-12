FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["Fanex.OddsFeed.AutoJobs/Fanex.OddsFeed.AutoJobs.csproj", "Fanex.OddsFeed.AutoJobs/"]
COPY ["Fanex.OddsFeed.AutoJobs.Services/Fanex.OddsFeed.AutoJobs.Services.csproj", "Fanex.OddsFeed.AutoJobs.Services/"]
COPY ["Fanex.OddsFeed.Core/Fanex.OddsFeed.Core.csproj", "Fanex.OddsFeed.Core/"]

RUN dotnet restore "Fanex.OddsFeed.AutoJobs/Fanex.OddsFeed.AutoJobs.csproj"

COPY . .

WORKDIR "/src/Fanex.OddsFeed.AutoJobs"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-final AS base

WORKDIR /app

COPY --from=builder /app .

RUN set -x \
 	&& apt-get update \
     && apt-get -y install nano \
     && apt-get -y install curl \
     && apt-get -y install telnet

ENTRYPOINT ["dotnet", "Fanex.OddsFeed.AutoJobs.dll"]