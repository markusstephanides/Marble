# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY "Marble.Sandbox" "./Marble.Sandbox"
COPY "Marble.Core" "./Marble.Core"
COPY "Marble.Messaging" "./Marble.Messaging"
COPY "Marble.Messaging.Rabbit" "./Marble.Messaging.Rabbit"
COPY "Marble.Messaging.Contracts" "./Marble.Messaging.Contracts"
COPY "Marble.Sandbox.Contracts" "./Marble.Sandbox.Contracts"
COPY "Marble.Utilities" "./Marble.Utilities"

WORKDIR "/source/Marble.Sandbox"
RUN dotnet restore "./Marble.Sandbox.csproj"

WORKDIR "/source/Marble.Core"
RUN dotnet restore "./Marble.Core.csproj"

WORKDIR "/source/Marble.Messaging"
RUN dotnet restore "./Marble.Messaging.csproj"

WORKDIR "/source/Marble.Messaging.Rabbit"
RUN dotnet restore "./Marble.Messaging.Rabbit.csproj"

WORKDIR "/source/Marble.Messaging.Contracts"
RUN dotnet restore "./Marble.Messaging.Contracts.csproj"

WORKDIR "/source/Marble.Sandbox.Contracts"
RUN dotnet restore "./Marble.Sandbox.Contracts.csproj"

WORKDIR "/source/Marble.Utilities"
RUN dotnet restore "./Marble.Utilities.csproj"

# copy and publish app and libraries
WORKDIR "/source"
COPY . .
WORKDIR "Marble.Sandbox"
RUN dotnet publish "Marble.Sandbox.csproj" -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Marble.Sandbox.dll"]