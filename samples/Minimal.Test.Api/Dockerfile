# STAGE 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Adjust paths according to your physical structure
# Since projects are inside 'samples' and 'src', 
# paths must match the file system exactly.
COPY ["samples/Minimal.Test.Api/Minimal.Test.Api.csproj", "samples/Minimal.Test.Api/"]
COPY ["src/Core.Observability/Core.Observability.csproj", "src/Core.Observability/"]

# 2. Restore dependencies
RUN dotnet restore "samples/Minimal.Test.Api/Minimal.Test.Api.csproj"

# 3. Copy the rest of the source code
COPY . .

# 4. Publish the application
WORKDIR "/src/samples/Minimal.Test.Api"
RUN dotnet publish "Minimal.Test.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Default ASP.NET Core port for .NET 8+
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Minimal.Test.Api.dll"]