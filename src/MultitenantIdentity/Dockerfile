FROM microsoft/dotnet:2.1-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app

EXPOSE 5000-6000

FROM microsoft/dotnet:2.1-sdk-stretch AS build
WORKDIR /src
COPY MultitenantIdentity/MultitenantIdentity.csproj MultitenantIdentity/
COPY Tenants/Tenants.Web.Client/Tenants.Web.Client.csproj Tenants/Tenants.Web.Client/
COPY Tenants/Tenants.Web/Tenants.Web.csproj Tenants/Tenants.Web/
RUN dotnet restore MultitenantIdentity/MultitenantIdentity.csproj
COPY . .
WORKDIR /src/MultitenantIdentity
RUN dotnet build MultitenantIdentity.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish MultitenantIdentity.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MultitenantIdentity.dll"]
