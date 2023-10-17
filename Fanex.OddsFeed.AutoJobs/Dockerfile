FROM hub.nexdev.net/base/dotnet/core/sdk:6.0-alpine AS builder

WORKDIR /src

COPY ["Fanex.OddsFeed.Models/Fanex.OddsFeed.Models.csproj", "Fanex.OddsFeed.Models/"]
COPY ["Fanex.OddsFeed.Core/Fanex.OddsFeed.Core.csproj", "Fanex.OddsFeed.Core/"]
COPY ["Fanex.OddsFeed.Repository/Fanex.OddsFeed.Repository.csproj", "Fanex.OddsFeed.Repository/"]
COPY ["Fanex.OddsFeed.AutoJobs/Fanex.OddsFeed.AutoJobs.csproj", "Fanex.OddsFeed.AutoJobs/"]

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore -v n "Fanex.OddsFeed.AutoJobs/Fanex.OddsFeed.AutoJobs.csproj"

COPY . .

WORKDIR "/src/Fanex.OddsFeed.AutoJobs"

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0-final AS base

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "Fanex.OddsFeed.AutoJobs.dll"]