# ECommerce API

API RESTful para uma plataforma de e-commerce desenvolvida com **ASP.NET Core 9** e **Clean Architecture**, criada como projeto de portfólio para demonstrar práticas de desenvolvimento backend de nível profissional.

---

## Sobre o Projeto

Esta API simula um backend completo de e-commerce, cobrindo autenticação, catálogo de produtos, carrinho de compras, gerenciamento de pedidos, simulação de pagamento e avaliações de produtos. As decisões de arquitetura e design foram tomadas para refletir os padrões utilizados no mercado.

---

## Arquitetura

O projeto segue a **Clean Architecture** com quatro camadas:

```
src/
├── ECommerceApi.Domain          # Entidades, Value Objects, Interfaces, Domain Exceptions
├── ECommerceApi.Application     # DTOs, Services, Interfaces, Mappings, Validators
├── ECommerceApi.Infrastructure  # EF Core DbContext, Repositórios, Migrations
└── ECommerceApi.API             # Controllers, Middlewares, Program.cs

tests/
└── ECommerceApi.Tests           # Testes Unitários (Domain + Application)
```

As dependências fluem para dentro — camadas externas dependem de camadas internas, nunca o contrário.

---

## Tecnologias

| Categoria | Tecnologia |
|---|---|
| Framework | ASP.NET Core 9 |
| Linguagem | C# 13 |
| ORM | Entity Framework Core 9 |
| Banco de Dados | SQL Server (LocalDB) |
| Autenticação | JWT Bearer + Refresh Token |
| Logging | Serilog |
| Documentação | Swagger / OpenAPI |
| Hash de Senha | BCrypt.Net |
| Result Pattern | FluentResults |
| Validação | FluentValidation |
| Testes | xUnit + NSubstitute + FluentAssertions |

---

## Funcionalidades

### Autenticação
- Registro e login com Access Token JWT (15 min) + Refresh Token (7 dias)
- Refresh Token armazenado em cookie HTTP-Only com `Secure` e `SameSite=Strict`
- Refresh Token hasheado com SHA-256 antes de persistir no banco
- Rotação de token a cada refresh — tokens usados anteriormente são invalidados
- Logout real via revogação do token (`/api/auth/revoke`)
- Autorização baseada em papéis: `Customer` e `Admin`

### Catálogo de Produtos
- Listagem de produtos paginada e filtrada (por categoria e termo de busca)
- CRUD completo para Admin
- Ativação e desativação de produtos
- Média de avaliações calculada a partir das reviews

### Carrinho de Compras
- Carrinho criado automaticamente na primeira adição de item
- Mesclagem de quantidade quando o mesmo produto é adicionado duas vezes
- Preço unitário sempre buscado do produto atual — nunca armazenado no item do carrinho
- Congelamento de preço ocorre apenas na criação do pedido (ponto de freeze correto)

### Pedidos
- Pedido criado a partir do carrinho atual
- Endereço capturado como Value Object (snapshot do endereço de entrega)
- Nome e preço do produto congelados no momento da criação do pedido (precisão histórica)
- Estoque decrementado na criação do pedido
- Carrinho limpo após o pedido ser criado
- Clientes veem apenas seus próprios pedidos; Admins veem todos
- Máquina de estados com transições validadas:

```
Pendente → Pago → Em Processamento → Enviado → Entregue
Pendente → Cancelado
Pago     → Cancelado
```

### Pagamento
- Processamento de pagamento simulado (90% de taxa de aprovação)
- Máquina de estados do pagamento: `Pendente → Aprovado / Recusado / Reembolsado`
- Status do pedido atualizado automaticamente com base no resultado do pagamento

### Avaliações
- Apenas clientes que compraram e receberam um produto podem avaliá-lo
- Uma avaliação por cliente por produto
- Clientes podem deletar suas próprias avaliações; Admins podem deletar qualquer uma

---

## Design do Domínio

### Value Objects

| Value Object | Armazenado Como | Propósito |
|---|---|---|
| `Email` | `string` via `HasConversion` | Valida formato e normaliza para minúsculas |
| `Money` | `decimal + string` via `OwnsOne` | Valor e moeda, previne valores negativos |
| `Rating` | `int` via `HasConversion` | Garante range entre 1 e 5 |
| `Address` | Múltiplas colunas via `OwnsOne` | Endereço de entrega com normalização do CEP |

### Domain Exceptions

Exceptions customizadas representam violações reais de regras de negócio e são tratadas pelo `GlobalExceptionHandler`:

```
InsufficientStockException              → 400
InvalidOrderStatusTransitionException   → 409
CannotCancelShippedOrderException       → 409
InvalidPaymentStatusException           → 409
```

Cada exception carrega um `ErrorId` curto (8 caracteres) para rastreabilidade no suporte sem expor detalhes internos.

---

## Segurança

- Senhas hasheadas com **BCrypt** (salt embutido no hash)
- Refresh Token hasheado com **SHA-256** antes do armazenamento — vazamento do banco não expõe os tokens
- Cookie HTTP-Only, Secure, SameSite=Strict para o Refresh Token — JavaScript não consegue acessá-lo
- Access Token de curta duração (15 min) — minimiza a janela de exposição em caso de interceptação
- Rotação de token — cada refresh invalida o token anterior
- Claims de papel embutidos no JWT — validados em toda requisição protegida
- Mensagens de erro genéricas em respostas 500 — detalhes internos logados apenas no servidor

---

## Tratamento de Erros

Todas as exceptions não tratadas são capturadas pelo `GlobalExceptionHandler` (`IExceptionHandler`):

| Tipo de Exception | Status HTTP | Resposta ao Cliente |
|---|---|---|
| `DomainException` (todas as subclasses) | 400 / 409 | Mensagem genérica de negócio + `errorId` |
| `ArgumentException` | 400 | Mensagem + `errorId` |
| `KeyNotFoundException` | 404 | Mensagem + `errorId` |
| `UnauthorizedAccessException` | 401 | Mensagem + `errorId` |
| Não tratada (`Exception`) | 500 | Mensagem genérica + `errorId` + `traceId` |

O `traceId` é incluído apenas nas respostas 500 — conecta o erro aos logs de infraestrutura (Serilog) para investigação da engenharia sem expor detalhes internos aos clientes.

---

## Endpoints

### Autenticação
```
POST   /api/auth/register
POST   /api/auth/login
POST   /api/auth/refresh
POST   /api/auth/revoke        [Authorize]
```

### Categorias
```
GET    /api/categories
GET    /api/categories/{id}
POST   /api/categories         [Authorize: Admin]
PUT    /api/categories/{id}    [Authorize: Admin]
DELETE /api/categories/{id}    [Authorize: Admin]
```

### Produtos
```
GET    /api/products
GET    /api/products/{id}
POST   /api/products           [Authorize: Admin]
PUT    /api/products/{id}      [Authorize: Admin]
DELETE /api/products/{id}      [Authorize: Admin]
PATCH  /api/products/{id}/activate    [Authorize: Admin]
PATCH  /api/products/{id}/deactivate  [Authorize: Admin]
```

### Carrinho
```
GET    /api/cart               [Authorize]
POST   /api/cart/items         [Authorize]
PUT    /api/cart/items/{id}    [Authorize]
DELETE /api/cart/items/{id}    [Authorize]
DELETE /api/cart               [Authorize]
```

### Pedidos
```
POST   /api/orders             [Authorize]
GET    /api/orders             [Authorize]
GET    /api/orders/{id}        [Authorize]
GET    /api/orders/all         [Authorize: Admin]
PATCH  /api/orders/{id}/status [Authorize: Admin]
```

### Pagamentos
```
POST   /api/payments/{orderId} [Authorize]
```

### Avaliações
```
GET    /api/products/{id}/reviews
POST   /api/products/{id}/reviews  [Authorize]
DELETE /api/reviews/{id}           [Authorize]
```

---

## Como Executar

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server ou SQL Server LocalDB
- Visual Studio 2022 ou VS Code

### Configuração

**1. Clone o repositório**
```bash
git clone https://github.com/seu-usuario/ecommerce-api.git
cd ecommerce-api
```

**2. Configure a connection string no `appsettings.json`**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ECommerceDb;Trusted_Connection=True;"
  },
  "Jwt": {
    "SecretKey": "sua-chave-secreta-minimo-32-caracteres!!",
    "Issuer": "ECommerceApi",
    "Audience": "ECommerceClient",
    "AccessTokenExpirationMinutes": "15",
    "RefreshTokenExpirationDays": "7"
  }
}
```

**3. Execute as migrations**
```bash
dotnet ef database update --startup-project src/ECommerceApi.API --project src/ECommerceApi.Infrastructure
```

**4. Execute a API**
```bash
dotnet run --project src/ECommerceApi.API --launch-profile http
```

**5. Acesse o Swagger**
```
http://localhost:5000/swagger
```

---

## Executando os Testes

```bash
dotnet test tests/ECommerceApi.Tests
```

Os testes cobrem:

- **Value Objects** — Email, Money, Rating, Address (validação, normalização, igualdade)
- **Entidades de Domínio** — Product, Order, Cart, Payment, Review (regras de negócio, transições de estado, domain exceptions)
- **Application Services** — Todos os 7 services (fluxos de sucesso, fluxos de falha, interação com repositórios via mocks do NSubstitute)

---

## Decisões de Design

**Por que Guid como chave primária?**
Guids evitam expor o volume de negócio através de IDs sequenciais em URLs públicas. O trade-off em performance de índice é irrelevante nessa escala e bem conhecido em cenários de produção.

**Por que Result Pattern + Domain Exceptions (abordagem híbrida)?**
`Result.Fail` trata fluxos de negócio esperados (não encontrado, validação). Domain Exceptions (subclasses de `DomainException`) protegem invariantes das entidades — regras que precisam ser verdadeiras sempre, independente de quem as chama. O `GlobalExceptionHandler` converte as domain exceptions em respostas HTTP adequadas, mantendo os controllers limpos.

**Por que não ter UnitPrice no CartItem?**
O carrinho representa uma intenção de compra ainda em andamento — os preços devem sempre refletir o catálogo atual. O congelamento de preço ocorre apenas na criação do pedido (`OrderItem.UnitPrice`), que é o único ponto correto de freeze.

**Por que o Refresh Token é hasheado no banco?**
Se o banco for comprometido, tokens em texto puro permitiriam que atacantes se passassem por usuários indefinidamente. O hash com SHA-256 torna tokens vazados inúteis sem o valor original.

**Por que FluentValidation em vez de Data Annotations?**
FluentValidation mantém as regras de validação centralizadas e testáveis, suporta regras condicionais complexas e mantém os DTOs limpos. Data Annotations espalham a lógica de validação pelas propriedades e não conseguem tratar regras que envolvem múltiplos campos.

---

## Autor

**Vinícius Soles**
Desenvolvedor Backend C# / .NET

---

## Licença

Este projeto tem fins educacionais e de portfólio.
