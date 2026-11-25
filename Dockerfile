FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["StudentManagementSystem.csproj", "./"]
RUN dotnet restore "StudentManagementSystem.csproj"

COPY . .
RUN dotnet publish "StudentManagementSystem.csproj" \
    -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

USER root
COPY --from=build /app/publish .
RUN chmod -R 777 /app
USER $APP_UID

EXPOSE 10000

ENTRYPOINT ["sh", "-c", "dotnet StudentManagementSystem.dll --urls http://0.0.0.0:${PORT:-10000}"]
