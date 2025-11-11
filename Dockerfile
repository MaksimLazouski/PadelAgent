# ====== build stage ======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

ARG PROJECT_PATH=PadelAgent/PadelAgent.csproj
ARG CONFIG=Release

RUN dotnet restore $PROJECT_PATH
RUN dotnet publish $PROJECT_PATH -c $CONFIG -o /out /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

COPY --from=build /out .

CMD ["dotnet", "PadelAgent.dll"]
