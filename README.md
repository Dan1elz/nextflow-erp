# Nextflow ERP (API de Gerenciamento de Estoque)

## 📌 Sobre o Projeto
O **Nextflow** ERP é uma API desenvolvida em **C#** utilizando **.NET** e **Entity Framework Core** para gerenciar o estoque de empresas. A API permite cadastrar, atualizar e consultar produtos, fornecedores e movimentações de estoque, garantindo um controle eficiente e seguro.

## 🛠️ Pré-requisitos
Antes de executar a API, você precisa ter instalado:
- [Docker](https://www.docker.com)
- [.NET SDK](https://dotnet.microsoft.com/download)

## 🚀 Executando o Projeto

### 1️⃣ Clonar o Repositório
```sh
 git clone https://github.com/Dan1elz/nextflow-erp.git
 cd nextflow-erp
```

### 2️⃣ Criar o Container do PostgreSQL
Execute o seguinte comando para criar e iniciar um container PostgreSQL:
```sh
docker run -e 'ACCEPT_EULA=1' -e POSTGRES_PASSWORD=SuaSenhaForte123 -e POSTGRES_DB=netxtflow -p 5432:5432 --name netxtflow-postgres -d postgres
```
Isso criará um banco de dados chamado `netxtflow` no PostgreSQL.

### 3️⃣ Configurar a String de Conexão
No arquivo **appsettings.json**, configure a string de conexão com os seguintes parâmetros:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=netxtflow;Username=postgres;Password=SuaSenhaForte123"
  }
}
```

### 4️⃣ Aplicar as Migrações
```sh
dotnet ef database update
```

### 5️⃣ Executar a API
Para rodar a API, execute:
```sh
dotnet run
```
A API estará disponível em: `http://localhost:5001`.

## 📡 Testando a API
Você pode testar os endpoints utilizando ferramentas como **Postman** ou **Swagger**.
Se a API tiver **Swagger** configurado, acesse:
```
http://localhost:5001/index.html
```

## 📊 Diagrama do Banco de Dados
O diagrama do banco de dados pode ser encontrado no diretório `docs/diagrama-nextflow.png`. Ele contém a estrutura das tabelas e seus relacionamentos, facilitando a compreensão do modelo de dados do **Nextflow ERP**.

## 📄 Licença
Este projeto está sob a licença MIT. Sinta-se à vontade para usá-lo e modificá-lo conforme necessário.
