FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Connect4.Api/Connect4.Api.csproj Connect4.Api/
RUN dotnet restore Connect4.Api/Connect4.Api.csproj
COPY Connect4.Api/ Connect4.Api/
RUN dotnet publish Connect4.Api/Connect4.Api.csproj -c Release -o /publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "Connect4.Api.dll"]
