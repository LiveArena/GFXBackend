# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the csproj and restore as distinct layers
COPY *.sln .
COPY GraphicsBackend/*.csproj ./GraphicsBackend/
COPY GraphicsBackend/Contexts/*.csproj ./GraphicsBackend/Contexts/
COPY GraphicsBackend/Controllers/*.csproj ./GraphicsBackend/Controllers/
COPY GraphicsBackend/Models/*.csproj ./GraphicsBackend/Models/
COPY GraphicsBackend/Services/*.csproj ./GraphicsBackend/Services/
COPY GraphicsBackend/Utilities/*.csproj ./GraphicsBackend/Utilities/
RUN dotnet restore -p:RestoreUseSkipNonexistentTargets=false -nowarn:msb3202,nu1503

# Copy the rest of the application code
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expose the port the app runs on
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "GraphicsBackend.dll"]
