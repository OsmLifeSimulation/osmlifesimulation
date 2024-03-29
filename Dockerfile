# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY OSMLS/*.csproj ./OSMLS/
COPY OSMLSGlobalLibrary/*.csproj ./OSMLSGlobalLibrary/
COPY OSMLS.Tests/*.csproj ./OSMLS.Tests/
RUN dotnet restore

# copy everything else
COPY OSMLS/. ./OSMLS/
COPY OSMLSGlobalLibrary/. ./OSMLSGlobalLibrary/
COPY OSMLS.Tests/. ./OSMLS.Tests/
RUN dotnet test

WORKDIR /source/OSMLS

# build app
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "OSMLS.dll"]

# add map
ADD https://www.openstreetmap.org/api/0.6/map?bbox=37.6269900%2C55.7407400%2C37.6298500%2C55.7419200 ./map.osm