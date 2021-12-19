# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS backend
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
WORKDIR /source/OSMLS

# build app
RUN dotnet publish -c release -o /app

FROM node:latest as frontend
WORKDIR /source

# download proto zip
ENV PROTOC_ZIP=protoc-3.14.0-linux-x86_64.zip
RUN curl -OL https://github.com/protocolbuffers/protobuf/releases/download/v3.14.0/${PROTOC_ZIP}
RUN unzip -o ${PROTOC_ZIP} -d ./proto 
RUN chmod 755 -R ./proto/bin
ENV BASE=/usr
# copy into path
RUN cp ./proto/bin/protoc ${BASE}/bin/
RUN cp -R ./proto/include/* ${BASE}/include/

RUN npm --global config set user root && npm --global install protoc-gen-ts && npm --global install ng-openapi-gen
RUN git clone --branch 21.03a https://github.com/Distera/OSMLS-Frontend frontend
RUN cd frontend && npm install && npm run-script generate-linux && npm run-script build

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=backend /app ./
COPY --from=frontend /source/frontend/dist/osmls-frontend ./wwwroot/
ENTRYPOINT ["dotnet", "OSMLS.dll"]

# add map
ADD https://www.openstreetmap.org/api/0.6/map?bbox=37.4622%2C55.7472%2C37.5193%2C55.7695 ./map.osm