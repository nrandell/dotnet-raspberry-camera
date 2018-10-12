FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Capture/*.csproj ./Capture/
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /app/Capture
RUN dotnet build

FROM build as publish
WORKDIR /app/Capture
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-runtime AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y \
  libraspberrypi0 \
  && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/Capture/out ./
ENTRYPOINT ["dotnet", "Capture.dll"]
