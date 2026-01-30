# ======================
# Build stage
# ======================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution & csproj để cache restore
COPY *.sln ./
COPY src/**/**/*.csproj ./src/
RUN dotnet restore

# Copy source & publish
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# ======================
# Runtime stage
# ======================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Globalization support
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Security: run as non-root
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

COPY --from=build /app/publish .

EXPOSE 5046
ENTRYPOINT ["dotnet", "bidify-be.dll"]
