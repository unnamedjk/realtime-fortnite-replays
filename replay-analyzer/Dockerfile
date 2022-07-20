# Usage
# Build: docker build . -t fortnite-extractor
# Run: docker run -it -v [your replays path]:/replays [number of replays to extract at a time] [extraction path]
# Example: docker run -it -v c:\\replays:/replays 10 /replays/extracted

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app
RUN mkdir /export
VOLUME ./export ./export
RUN touch  /export/test
# Copy csproj and restore as distinct layers
COPY *.csproj ./

# Copy everything else and build
COPY . ./
RUN dotnet publish FortniteReplayExtractor.csproj -c Release -f -o out --runtime linux-x64 -f net5.0 --self-contained true

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "FortniteReplayExtractor.dll"]