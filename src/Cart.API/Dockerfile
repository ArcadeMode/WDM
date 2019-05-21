FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
WORKDIR /src
COPY *.sln ./
COPY src/Cart.API/Cart.API.csproj src/Cart.API/
COPY src/GrainInterfaces/GrainInterfaces.csproj src/GrainInterfaces/
COPY src/Grains/Grains.csproj src/Grains/
COPY src/Silo/Silo.csproj src/Silo/
COPY Directory.Build.props .
RUN dotnet restore
COPY . .
WORKDIR /src/src/Cart.API
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Cart.API.dll"]
