# Linux container used by hosted platforms such as Render.
# Render provides the public port through the PORT environment variable.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
RUN mkdir -p /app/data
EXPOSE 8080
ENV PORT=8080
ENV DatabaseProvider=Sqlite
ENV ConnectionStrings__TodoDatabase="Data Source=/app/data/Todo.db"


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["EfcDataAccess/EfcDataAccess.csproj", "EfcDataAccess/"]
COPY ["FileData/FileData.csproj", "FileData/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "WebAPI/WebAPI.csproj"
COPY . .
WORKDIR "/src/WebAPI"
RUN dotnet build "WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# The shell form lets the app use Render's runtime PORT value, while still
# defaulting to 8080 for local Docker runs.
ENTRYPOINT ["sh", "-c", "dotnet WebAPI.dll --urls http://0.0.0.0:${PORT:-8080}"]
