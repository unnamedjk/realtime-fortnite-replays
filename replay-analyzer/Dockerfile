# docker run -v R:\replays\009f5c4727dc41698dbf5f6a7b10372e.replay:/009f5c4727dc41698dbf5f6a7b10372e.replay --rm -it replay-analyzer:latest dotnet .\FortniteReplayAnalyzer.dll 
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .