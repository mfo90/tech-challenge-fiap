Esse projeto contem 4 soluções
  - Api .net core
  - Front angular
  - Pg Admin (Para gerenciamento do banco de dados postgrees)
  - Banco de dados postgre

Todas essas aplicações irão rodar em um container docker, abaixo detalhes de como iniciar o container

Segue abaixo os passos para rodar o projeto:

1. Abra o docker desktop e inicie o Engine running caso esse não estiver rodando
2. Estando na raiz do projeto execute: docker-compose up --build
   ![image](https://github.com/mfo90/tech-challenge-fiap/assets/45730921/378fa3de-dcbb-48ce-8702-4dfacaa789d6)

3. Espere o build dos containers, após finalizado deverá estar conforme a imagem abaixo:
  ![image](https://github.com/mfo90/tech-challenge-fiap/assets/45730921/0d89150d-1034-42e1-b308-a5c8e908f3a5)
4. Pode acontecer de a api não ter iniciado, clique em iniciar imagem no docker caso positivo.

5. Abra o pgAdmin no container para visualizar o banco de dados criado:
  Usuário:admin@admin.com
  Senha: admin
6. Clique em server com o botão direito e em register
   General: Digite o nome preferido;
   Connection:
     Hostname/adress: db (nome do serviço que estiver no docker-compose.yml para o Postgres)
     username: matheus
     password: 123456
     Clique em conectar
7. Abra o container do angular, e faça o login usuário: admin e senha: 123456
     
