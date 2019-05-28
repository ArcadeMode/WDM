FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["src/Cart.API/Cart.API.csproj", "src/Cart.API/"]
COPY ["src/GrainInterfaces/GrainInterfaces.csproj", "src/GrainInterfaces/"]
RUN dotnet restore "src/Cart.API/Cart.API.csproj"
COPY . .
WORKDIR "/src/src/Cart.API"
RUN dotnet build "Cart.API.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "Cart.API.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Cart.API.dll"]