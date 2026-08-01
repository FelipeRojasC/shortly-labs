# syntax=docker/dockerfile:1

# ============================================================
# Stage 1: Build — uses the full SDK image to restore and publish
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the project file first so Docker can cache the restore layer.
# As long as Shortly.csproj doesn't change, this layer is reused on rebuilds
# even if the application source code changes.
COPY Shortly.csproj ./
RUN dotnet restore Shortly.csproj

# Now copy the rest of the source code
COPY . .

# Publish in Release mode. --no-restore avoids repeating the restore step,
# since dependencies were already resolved in the layer above.
RUN dotnet publish Shortly.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ============================================================
# Stage 2: Runtime — smaller ASP.NET runtime image, no SDK/build tools
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install curl (needed for HEALTHCHECK below) and immediately clean up apt
# cache in the same layer, then create a dedicated non-root user/group.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && groupadd -r appgroup \
    && useradd -r -g appgroup -d /app -s /sbin/nologin appuser

# Copy only the published output from the build stage — no SDK, no source,
# no intermediate build artifacts end up in the final image.
COPY --from=build /app/publish .

# Directory for the SQLite database file, persisted via a named volume
# (see docker-compose.yml). Owned by the non-root user so EF Core can
# create/write shortly.db at runtime.
RUN mkdir -p /app/data \
    && chown -R appuser:appgroup /app

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Run as non-root from here on
USER appuser

# Pings the /health endpoint (added in Program.cs) to report container liveness
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Shortly.dll"]
