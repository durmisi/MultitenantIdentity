FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine3.7 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apk add --update nodejs nodejs-npm

FROM microsoft/dotnet:2.1-sdk AS build

# End setup
WORKDIR /src

RUN echo "$(date +%Y-%m-%d:%H:%M:%S) Dotnet Version: $(dotnet --version)"

# NodeJs Required to build the ClientApp
RUN echo "Installing NodeJS.."
RUN curl -sL https://deb.nodesource.com/setup_8.x |  bash -
RUN apt-get install -y nodejs

# NodeJs
RUN echo "Installing Latest NPM"
RUN npm install npm@latest -g
RUN echo "NPM Version: $(npm -v)"

COPY ["Tenants/Tenants.Web/Tenants.Web.csproj", "Tenants/Tenants.Web/"]
RUN dotnet restore "Tenants/Tenants.Web/Tenants.Web.csproj"

COPY ["Tenants/Tenants.Web/ClientApp/package.json", "Tenants/Tenants.Web/ClientApp/"]
RUN cd Tenants/Tenants.Web/ClientApp \
    && npm i --silent

COPY . .
WORKDIR "/src/Tenants/Tenants.Web"
RUN dotnet build "Tenants.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Tenants.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Tenants.Web.dll"]
