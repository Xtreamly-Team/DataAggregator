# Use the official .NET 7 SDK as the base image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY . .

# Build the project
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose the desired port (adjust if needed)
ENV ASPNETCORE_URLS=http://*:5000

# Expose the desired port (optional)
EXPOSE 5000

# Start the ASP.NET Core application
ENTRYPOINT ["dotnet", "DataProvider.dll"]
