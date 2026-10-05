# STAGE 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copy project files needed for restore (paths match the physical structure)
COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["samples/CoreSystem.Samples.Api/CoreSystem.Samples.Api.csproj", "samples/CoreSystem.Samples.Api/"]
COPY ["samples/CoreSystem.Samples.Api/packages.lock.json", "samples/CoreSystem.Samples.Api/"]
COPY ["samples/CoreSystem.Samples.Core/CoreSystem.Samples.Core.csproj", "samples/CoreSystem.Samples.Core/"]
COPY ["samples/CoreSystem.Samples.Core/packages.lock.json", "samples/CoreSystem.Samples.Core/"]
COPY ["samples/CoreSystem.Samples.Infrastructure/CoreSystem.Samples.Infrastructure.csproj", "samples/CoreSystem.Samples.Infrastructure/"]
COPY ["samples/CoreSystem.Samples.Infrastructure/packages.lock.json", "samples/CoreSystem.Samples.Infrastructure/"]

# 2. Restore dependencies
RUN dotnet restore "samples/CoreSystem.Samples.Api/CoreSystem.Samples.Api.csproj"

# 3. Copy the rest of the source code
COPY . .

# 4. Publish the application
WORKDIR "/src/samples/CoreSystem.Samples.Api"
RUN dotnet publish "CoreSystem.Samples.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Default ASP.NET Core port for .NET 8+
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "CoreSystem.Samples.Api.dll"]
