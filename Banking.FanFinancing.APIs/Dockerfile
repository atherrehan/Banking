# =========================================================
# Stage 1: Build the application
# =========================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY FinTech.sln ./
COPY Banking.FanFinancing.APIs/Banking.FanFinancing.APIs.csproj Banking.FanFinancing.APIs/
COPY Banking.FanFinancing.Domain/Banking.FanFinancing.Domain.csproj Banking.FanFinancing.Domain/
COPY Banking.FanFinancing.Infrastructure/Banking.FanFinancing.Infrastructure.csproj Banking.FanFinancing.Infrastructure/
# Note: Shared project doesn’t need a csproj

# Restore
RUN dotnet restore Banking.FanFinancing.APIs/Banking.FanFinancing.APIs.csproj 

# Copy everything and build
COPY . .
RUN dotnet publish Banking.FanFinancing.APIs/Banking.FanFinancing.APIs.csproj -c Release -o /app/publish /p:UseAppHost=false

# =========================================================
# Stage 2: Runtime image
# =========================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Tell ASP.NET Core to listen on port 5501 for all interfaces
ENV ASPNETCORE_URLS=http://+:5501


ENTRYPOINT ["dotnet", "Banking.FanFinancing.APIs.dll"]

##docker run -d -p 5501:5501 --name fanfinancing-container fanfinancing-api
