# Use a imagem oficial do .NET SDK para construir a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Defina o diretório de trabalho na imagem
WORKDIR /src

# Copie os arquivos .csproj de cada projeto para o diretório de build
COPY api/ContactsService.API/*.csproj ./ContactsService.API/
COPY api/RegionalContactsApp.Application/*.csproj ./RegionalContactsApp.Application/
COPY api/RegionalContactsApp.Infrastructure/*.csproj ./RegionalContactsApp.Infrastructure/
COPY api/RegionalContactsApp.Domain/*.csproj ./RegionalContactsApp.Domain/

# Restaure as dependências para o ContactsService.API
WORKDIR /src/ContactsService.API
RUN dotnet restore

# Volte ao diretório de trabalho raiz
WORKDIR /src

# Copie todos os arquivos restantes para o diretório de trabalho para construir a aplicação
COPY api/ContactsService.API/. ./ContactsService.API/
COPY api/RegionalContactsApp.Application/. ./RegionalContactsApp.Application/
COPY api/RegionalContactsApp.Infrastructure/. ./RegionalContactsApp.Infrastructure/
COPY api/RegionalContactsApp.Domain/ ./RegionalContactsApp.Domain/

# Volte para o diretório do projeto da API
WORKDIR /src/ContactsService.API

# Construa a aplicação
RUN dotnet build -c Release -o /app/build

# Publicar a aplicação
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Use a imagem oficial do .NET Runtime para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Adicione o script wait-for-it
COPY wait-for-it.sh /usr/bin/wait-for-it
RUN chmod +x /usr/bin/wait-for-it

# Defina o diretório de trabalho na imagem
WORKDIR /app

# Copie os arquivos publicados para o diretório de trabalho
COPY --from=publish /app/publish .

# Defina o ponto de entrada para a aplicação
ENTRYPOINT ["wait-for-it", "db:5432", "--", "dotnet", "ContactsService.API.dll"]
