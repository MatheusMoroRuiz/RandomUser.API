# RandomUser.API

A aplicação é uma Web API construída em ASP.NET Core 7.0, que consome uma API pública externa e gratuita Random User Generator. A ideia principal é importar usuários fictícios, salvar esses dados em um banco de dados PostgreSQL e disponibilizá-los por meio de endpoints REST. Também inclui uma página HTML simples que permite visualizar e manipular os dados.

## Tecnologias utilizadas

- ASP.NET Core 7.0  
- C#  
- Entity Framework Core  
- PostgreSQL  
- Swagger (Swashbuckle)  
- HTML + JavaScript (com funcionalidades no front-end)

## Estrutura do projeto

- Controllers/UsersController.cs — endpoints de importação e consulta
- Services/RandomUserService.cs — consumo da API randomuser.me
- Models/User.cs — definição da entidade User
- Data/AppDbContext.cs — contexto e mapeamento com EF Core
- wwwroot/index.html — front-end para visualização e manipulação dos dados

## Como executar o projeto

### Pré-requisitos

- .NET 7 SDK  
- PostgreSQL rodando localmente ou em container

### Passo a passo

1. Clone este repositório:

```

git clone [https://github.com/seu-usuario/RandomUser.API.git](https://github.com/seu-usuario/RandomUser.API.git)
cd RandomUser.API

````

2. Crie o banco de dados no PostgreSQL:

```sql
CREATE DATABASE randomuserdb;

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100),
    email VARCHAR(100),
    gender VARCHAR(20),
    country VARCHAR(100)
);
````

3. Edite a string de conexão no `appsettings.json` com base nos dados do seu banco:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=randomuserdb;Username=postgres;Password="
   }
   ```

4. Execute o projeto:

   
   `dotnet run` ou apertando F5, tanto faz
   

5. Acesse a documentação da API via Swagger:

   ```
   https://localhost:{porta}/swagger
   ```

## Endpoints disponíveis

* POST `/api/Users/import`
   Importa 30 usuários aleatórios consumindo a API pública Random User Generator e os armazena no meu banco de dados.

* GET `/api/Users`
   Retorna todos os usuários que estão atualmente salvos no banco.

* GET `/api/Users/search`
   Permite buscar usuários filtrando por nome ou e-mail. Os parâmetros são passados por query string, como por exemplo:
/api/Users/search?name=ana ou /api/Users/search?email=@gmail.com

* PUT `/api/Users/{id}`
   Atualiza os dados de um usuário específico com base no seu ID. Os campos esperados no corpo da requisição são: name, email, gender, country.

 * DELETE `/api/Users/{id}`
   Remove um usuário do banco com base no ID informado.

## Funcionalidades do front-end

Implementei melhorias na página HTML (`wwwroot/index.html`) para torná-la mais interativa e funcional. As principais funcionalidades são:

* Pesquisa por nome ou email
* Ordenação clicando no cabeçalho das colunas
* Paginação com 30 usuários por página
* Edição inline com botão "Salvar"
* Validação de e-mail e campos obrigatórios
* Feedback visual após salvar (sucesso ou erro)
* Confirmação de exclusão com modal
* Exportação para CSV apenas dos usuários visíveis na tabela

A ideia foi oferecer uma interface leve, sem frameworks pesados, mas ainda assim útil para visualizar e testar a API com dados reais.

## Detalhes técnicos

* O PostgreSQL é case-sensitive com nomes de colunas, então isso foi tratado no método `OnModelCreating`.
* A API Random User Generator é pública e serve como fonte de dados fictícios.
* Swagger está habilitado para facilitar testes diretamente na interface web.
* O projeto tem estrutura modular e de fácil manutenção.