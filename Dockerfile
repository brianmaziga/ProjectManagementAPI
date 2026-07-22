# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ProjectManagementAPI/ProjectManagementAPI.csproj", "ProjectManagementAPI/"]
RUN dotnet restore "ProjectManagementAPI/ProjectManagementAPI.csproj"

COPY . .
WORKDIR "/src/ProjectManagementAPI"
RUN dotnet publish "ProjectManagementAPI.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ProjectManagementAPI.dll"]
