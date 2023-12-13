FROM hub.nexdev.net/base/dotnet/core/sdk:7.0-alpine AS builder

WORKDIR /src

COPY ["OddsFeed.Core/OddsFeed.Core.csproj", "OddsFeed.Core/"]
COPY ["OddsFeed.Models/OddsFeed.Models.csproj", "OddsFeed.Models/"]
COPY ["OddsFeed.Services/OddsFeed.Services.csproj", "OddsFeed.Services/"]
COPY ["OddsFeed.AutoJobs/OddsFeed.AutoJobs.csproj", "OddsFeed.AutoJobs/"]

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore -v n "OddsFeed.AutoJobs/OddsFeed.AutoJobs.csproj"

COPY ["OddsFeed.Core/", "OddsFeed.Core/"]
COPY ["OddsFeed.Models/", "OddsFeed.Models/"]
COPY ["OddsFeed.Services/", "OddsFeed.Services/"]
COPY ["OddsFeed.AutoJobs/", "OddsFeed.AutoJobs/"]

WORKDIR /src/OddsFeed.AutoJobs

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0-alpine AS base

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR /app
COPY --from=builder /app .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "OddsFeed.AutoJobs.dll"]