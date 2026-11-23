# Inventory Product API

A RESTful inventory management API built with **C# and ASP.NET Core (.NET 10)**.

The project was developed as a technical assessment focused on maintainable architecture, reliable inventory operations, automated testing, OAuth2 authentication, and containerized execution.

## Technology Stack

| Technology | Purpose |
|---|---|
| ASP.NET Core / C# (.NET 10) | REST API |
| SQL Server 2022 | Relational database |
| Entity Framework Core | Read operations |
| Dapper | Write operations |
| Keycloak 26.8.2 | OAuth2 / OpenID Connect identity provider |
| JWT Bearer | API authentication |
| xUnit | Unit and integration tests |
| Docker Compose | Local service orchestration |
| Swagger / OpenAPI | API exploration and testing |

## Architecture

The solution contains five projects:

```text
Inventory_Product.Api/
Inventory_Product.Application/
Inventory_Product.Domain/
Inventory_Product.Repository/
Inventory_Product.Test/
```

**Inventory_Product.Api** — HTTP endpoints, dependency injection, JWT authentication, authorization policies, middleware, and Swagger.

**Inventory_Product.Application** — Application use cases, interfaces, and contracts.

**Inventory_Product.Domain** — Domain entities, invariants, and business rules.

**Inventory_Product.Repository** — SQL Server persistence using EF Core and Dapper.

**Inventory_Product.Test** — Unit and integration tests.

The solution follows Clean Architecture principles and a CQRS-style separation between read and write operations.

## Features

### Products and Categories

- Create, retrieve, and update products and categories
- Detect duplicate SKUs and category names
- Validate category references
- Use soft delete to deactivate products without losing historical data

### Inventory Movements

- Register stock entries and exits
- Reject stock exits exceeding the available quantity
- Preserve movement history
- Use database transactions to maintain inventory consistency

### Concurrency

SQL Server `RowVersion` is used for optimistic concurrency control.

An update using an outdated version is rejected with **HTTP 409 Conflict**, preventing silent overwrites.

### Security

- OAuth2 / OpenID Connect with Keycloak
- JWT validation
- Role-based authorization
- Administrative and standard user permissions

## Prerequisites

For containerized execution:

- Docker Desktop with Docker Compose
- Available local ports: `5089`, `8080`, and `11433`

For local builds and tests:

- .NET 10 SDK
- SQL Server with an isolated integration-test database

## Configuration

The repository provides `.env.example` as a template.

Create a private `.env` file in the solution root.

For example, using PowerShell:

```powershell
Copy-Item .env.example .env
```

Configure strong, unique local values for:

```dotenv
MSSQL_SA_PASSWORD=REPLACE_WITH_A_STRONG_PASSWORD
KEYCLOAK_ADMIN_USERNAME=admin
KEYCLOAK_ADMIN_PASSWORD=REPLACE_WITH_ANOTHER_STRONG_PASSWORD
```

Never commit `.env`, tokens, private keys, or real credentials.

The supplied container configuration uses development settings and local HTTP endpoints. It is **not a production deployment configuration**.

## Running with Docker Compose

From the repository root:

```bash
docker compose config --quiet
docker compose up -d --build
```

Inspect the services:

```bash
docker compose ps -a
```

Inspect application logs:

```bash
docker compose logs --tail 100 api
```

Inspect Keycloak logs:

```bash
docker compose logs --tail 100 keycloak
```

The SQL initialization container is expected to exit successfully with code `0`. It is not intended to run continuously.

### Local Addresses

| Service | Address |
|---|---|
| Swagger | http://localhost:5089/swagger |
| Keycloak Admin Console | http://localhost:8080/admin/ |
| SQL Server | `tcp:127.0.0.1,11433` |

## SQL Server Initialization

The database initialization script is located at:

`docker/sql/init.sql`

It prepares `InventoryDB` with the main tables:

- `Categories`
- `Products`
- `MovementTypes`
- `InventoryMovements`

SQL Server data is persisted in a Docker volume.

The initialization service checks for existing tables and avoids executing the full initialization script on an already initialized schema.

Do not remove Docker volumes unless intentionally resetting the development database.

## Keycloak Setup

Keycloak runs as part of Docker Compose.

The realm import file is located at:

`docker/keycloak/import/InventoryRealm-realm.json`

The configuration contains:

| Item | Value |
|---|---|
| Realm | `InventoryRealm` |
| Swagger OAuth2 client | `inventory-product-client` |
| API audience/client | `inventory-product-api` |
| Administrator role | `inventory-admin` |
| Standard user role | `inventory-user` |
| Audience mapper | `inventory-api-audience` |

The import file must not contain real passwords, user credentials, or private client secrets.

### Initial Administrator Access

Open the Keycloak Admin Console at:

`http://localhost:8080/admin/`

Sign in with the bootstrap administrator credentials configured in `.env`.

### Create Demonstration Users

Select `InventoryRealm` and create two local users.

| Example username | Realm role |
|---|---|
| `inventory.admin` | `inventory-admin` |
| `inventory.user` | `inventory-user` |

Assign locally generated passwords using the Keycloak Admin Console.

These are example usernames, not distributed accounts with preset credentials.

For Swagger login testing, ensure the users are enabled and have usable non-temporary passwords.

### Verify OAuth2 Authentication

Open Swagger:

`http://localhost:5089/swagger`

Use **Authorize** and authenticate through Keycloak.

The following scenarios were tested successfully:

| Test | Expected response |
|---|---|
| Protected endpoint without JWT | `401 Unauthorized` |
| Authorized access to products | `200 OK` |
| Standard user attempts an administrator-only operation | `403 Forbidden` |

## Automated Tests

Build the solution:

```bash
dotnet build
```

Execute automated tests:

```bash
dotnet test
```

Integration tests require the `INVENTORY_TEST_CONNECTION_STRING` environment variable and a separately initialized SQL Server test database.

Example for Windows PowerShell with SQL Server Express:

```powershell
$env:INVENTORY_TEST_CONNECTION_STRING = "Server=.\SQLEXPRESS;Database=InventoryDB_Test;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet test
```

The test database must contain the schema and seed data required by the integration tests.

**Do not point integration tests at the application database `InventoryDB`.**

Last verified automated test result: **84 passing tests**.

## Functional Verification

The following behaviors were manually verified against the containerized application:

- Product and category creation and persistence
- Role-based access restrictions
- Stock entry and exit operations
- Rejection of exits with insufficient stock
- Preservation of stock after rejected operations
- Outdated `RowVersion` updates returning HTTP 409
- Soft delete using `IsActive`
- Preservation of historical movements after soft delete
- Access to movement history through the API

## Security Notes

This repository is intended for local development and technical assessment.

The configuration uses local HTTP endpoints and development-mode Keycloak.

For production, additional measures are required, including:

- HTTPS and stable public identity-provider URLs
- Secure credential management
- Least-privilege database accounts
- Production-grade Keycloak database and persistence
- Appropriate secret rotation and protected storage
- Persistent and securely protected Data Protection keys where required
- Monitoring, logging, and operational safeguards

## Documentation

Additional engineering evidence is available in `docs/`:

- `REQUIREMENTS_AND_SPEC.md` — requirements and acceptance criteria
- `AI_USAGE.md` — AI-assisted development process and validation
- `ARCHITECTURE_DECISIONS.md` — architecture decisions and trade-offs
- `DECISION_MAKING.md` — responses to technical decision-making questions

## AI-Assisted Development

Generative AI was used as a development assistant.

The implementation followed an incremental process in which suggested changes were reviewed and validated through compilation, automated tests, functional API testing, Docker diagnostics, and SQL Server queries.

See `docs/AI_USAGE.md` for methodology and evidence.

## Delivery Scope

The API, Docker environment, Keycloak authentication, role-based permissions, inventory workflows, concurrency control, and soft delete behavior were verified in the development environment.

A fully clean-machine installation should be independently validated before treating this configuration as a production-ready deployment.

## Clean Clone Verification

The repository was cloned into a separate local directory to validate the containerized setup independently of the original development workspace.

The following command was executed:

```bash
docker compose up -d --build
```

The verification confirmed:

- Successful .NET 10 API image build.
- Creation of a new Docker network and persistent volumes.
- Successful SQL Server startup and health checks.
- Automatic creation of `InventoryDB` and its schema.
- Successful import of `InventoryRealm` into Keycloak.
- Successful startup of the ASP.NET Core API on port 5089.

All required services started without blocking errors.

The environment uses development settings and is not intended for production deployment.

## Clean Clone Verification

The repository was cloned into a separate local directory to verify the deployment independently of the original development environment.

The following command was executed:

```bash
docker compose up -d --build
```

The verification confirmed:

- Successful .NET 10 API image build.
- Creation of a new Docker network and persistent volumes.
- Successful SQL Server initialization and health checks.
- Automatic creation of `InventoryDB` and its database schema.
- Successful import of `InventoryRealm` into Keycloak.
- Availability of the configured OAuth2 clients and realm roles.
- Successful startup of the API and access to Swagger.

The Docker environment uses local development settings and is not intended for production deployment.