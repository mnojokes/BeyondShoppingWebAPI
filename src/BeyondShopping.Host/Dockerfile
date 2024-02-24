#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BeyondShopping.Host/BeyondShopping.Host.csproj", "src/BeyondShopping.Host/"]
COPY ["src/BeyondShopping.Contracts/BeyondShopping.Contracts.csproj", "src/BeyondShopping.Contracts/"]
COPY ["src/BeyondShopping.Infrastructure/BeyondShopping.Infrastructure.csproj", "src/BeyondShopping.Infrastructure/"]
COPY ["src/BeyondShopping.Application/BeyondShopping.Application.csproj", "src/BeyondShopping.Application/"]
COPY ["src/BeyondShopping.Core/BeyondShopping.Core.csproj", "src/BeyondShopping.Core/"]
RUN dotnet restore "./src/BeyondShopping.Host/BeyondShopping.Host.csproj"
COPY . .
WORKDIR "/src/src/BeyondShopping.Host"
RUN dotnet build "./BeyondShopping.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BeyondShopping.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeyondShopping.Host.dll"]