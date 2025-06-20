# -------- Build stage --------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy .sln and .csproj files
COPY BizMate.sln .
COPY src/BizMate.Api/*.csproj ./src/BizMate.Api/
COPY src/BizMate.Application/*.csproj ./src/BizMate.Application/
COPY src/BizMate.Domain/*.csproj ./src/BizMate.Domain/
COPY src/BizMate.Infrastructure/*.csproj ./src/BizMate.Infrastructure/
COPY src/BizMate.Public/*.csproj ./src/BizMate.Public/

# Restore NuGet packages
RUN dotnet restore

# Copy source code
COPY src/ ./src/

# Publish application
WORKDIR /app/src/BizMate.Api
RUN dotnet publish -c Release -o /out

# -------- Runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

ENTRYPOINT ["dotnet", "BizMate.Api.dll"]
