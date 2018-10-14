FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 54669
EXPOSE 44371

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Streamer/Streamer.csproj", "Streamer/"]
RUN dotnet restore "Streamer/Streamer.csproj"
COPY . .
WORKDIR "/src/Streamer"
RUN dotnet build "Streamer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Streamer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Streamer.dll"]