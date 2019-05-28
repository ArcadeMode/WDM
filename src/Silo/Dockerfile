FROM mcr.microsoft.com/dotnet/core/runtime:2.1-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["src/Silo/Silo.csproj", "src/Silo/"]
COPY ["src/GrainInterfaces/GrainInterfaces.csproj", "src/GrainInterfaces/"]
COPY ["src/Grains/Grains.csproj", "src/Grains/"]
RUN dotnet restore "src/Silo/Silo.csproj"
COPY . .
WORKDIR "/src/src/Silo"
RUN dotnet build "Silo.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "Silo.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Silo.dll"]
