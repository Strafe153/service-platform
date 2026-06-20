FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
    
# Copy csproj and restore as distinct layers
COPY ./*.sln ./
COPY ./Users.Domain/*.csproj ./Users.Domain/
COPY ./Users.Infrastructure/*.csproj ./Users.Infrastructure/
COPY ./Users.Api/*.csproj ./Users.Api/
RUN dotnet restore ./Users.Api/Users.Api.csproj
    
# Copy everything else and build
COPY ./Users.Domain/. ./Users.Domain/
COPY ./Users.Infrastructure/. ./Users.Infrastructure/
COPY ./Users.Api/. ./Users.Api/
WORKDIR /source/Users.Api
RUN dotnet publish -c release -o /app

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Users.Api.dll"]