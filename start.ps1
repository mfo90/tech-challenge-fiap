# Obtém o diretório onde o script está localizado
$basePath = $PSScriptRoot

# Nome da rede
$NetworkName = "monitoring_network"

# Verifica se a rede já existe e a cria se não existir
$networkExists = docker network inspect $NetworkName -ErrorAction SilentlyContinue
if (-not $networkExists) {
    Write-Host "A rede '$NetworkName' não foi encontrada. Criando a rede..."
    docker network create $NetworkName
} else {
    Write-Host "A rede '$NetworkName' já existe."
}

# Subir os serviços com Docker Compose
Write-Host "Subindo serviços do arquivo docker-compose.services.yml..."
docker-compose -f docker-compose.services.yml up --build -d

Write-Host "Subindo serviços do arquivo docker-compose.AuthService.yml..."
docker-compose -f docker-compose.AuthService.yml up --build -d


Write-Host "Subindo serviços do arquivo docker-compose.ContactsService.yml..."
docker-compose -f docker-compose.ContactsService.yml up --build -d

Write-Host "Subindo serviços do arquivo docker-compose.RegionsService.yml..."
docker-compose -f docker-compose.RegionsService.yml up --build -d