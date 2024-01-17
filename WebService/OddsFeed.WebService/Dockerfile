FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

WORKDIR /src

COPY ["OddsFeed.Core/OddsFeed.Core.csproj", "OddsFeed.Core/"]
COPY ["OddsFeed.Models/OddsFeed.Models.csproj", "OddsFeed.Models/"]
COPY ["OddsFeed.Services/OddsFeed.Services.csproj", "OddsFeed.Services/"]
COPY ["WebService/OddsFeed.WebService.Core/OddsFeed.WebService.Core.csproj", "WebService/OddsFeed.WebService.Core/"]
COPY ["WebService/OddsFeed.WebService.Models/OddsFeed.WebService.Models.csproj", "WebService/OddsFeed.WebService.Models/"]
COPY ["WebService/OddsFeed.WebService.Criteria/OddsFeed.WebService.Criteria.csproj", "WebService/OddsFeed.WebService.Criteria/"]
COPY ["WebService/OddsFeed.WebService.Services/OddsFeed.WebService.Services.csproj", "WebService/OddsFeed.WebService.Services/"]
COPY ["WebService/OddsFeed.WebService/OddsFeed.WebService.csproj", "WebService/OddsFeed.WebService/"]

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore -v n "WebService/OddsFeed.WebService/OddsFeed.WebService.csproj"

COPY ["OddsFeed.Core/", "OddsFeed.Core/"]
COPY ["OddsFeed.Models/", "OddsFeed.Models/"]
COPY ["OddsFeed.Services/", "OddsFeed.Services/"]
COPY ["WebService/OddsFeed.WebService.Core/", "WebService/OddsFeed.WebService.Core/"]
COPY ["WebService/OddsFeed.WebService.Models/", "WebService/OddsFeed.WebService.Models/"]
COPY ["WebService/OddsFeed.WebService.Criteria/", "WebService/OddsFeed.WebService.Criteria/"]
COPY ["WebService/OddsFeed.WebService.Services/", "WebService/OddsFeed.WebService.Services/"]
COPY ["WebService/OddsFeed.WebService/", "WebService/OddsFeed.WebService/"]

WORKDIR /src/WebService/OddsFeed.WebService

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine AS base

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR /app
COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "OddsFeed.WebService.dll"]