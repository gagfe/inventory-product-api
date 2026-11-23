# AI-Assisted Development Report

## 1. Purpose

This document describes how generative AI was used during the development of the Inventory Product API, the engineering practices applied to validate AI-assisted work, and the decisions that remained under developer supervision.

## 2. Tools Used

Claude and CHAT-GPT was used as a development assistant for technical discussions, implementation guidance, debugging, architecture review, and test planning.

AI assistance did not replace code review, execution, testing, or technical decision-making.

## 3. Development Approach

The project was developed incrementally rather than requesting the entire application in a single prompt.

I divided the technical assessment into smaller implementation stages, maintaining the existing project context, architectural boundaries, and acceptance criteria throughout the development process.

Each stage followed an iterative workflow:

1. Define the feature or technical problem.
2. Discuss the expected behavior and implementation approach.
3. Implement or adapt the proposed changes.
4. Review the generated code and its integration with the existing architecture.
5. Build the application and execute relevant tests.
6. Perform functional validation.
7. Resolve issues before proceeding to the next stage.

This approach helped reduce the risk of context loss, inconsistent implementations, unnecessary code changes, and unverified assumptions.

## 4. Main Prompt Categories

The following prompts are representative summaries of the guidance used during development. They are not presented as verbatim transcripts.

### Architecture

Help me implement an inventory management REST API using .NET, Clean Architecture principles, and separation between application, domain, API, and data access responsibilities. Explain the responsibility of each component before implementing it.

### Data Access

Implement read operations using Entity Framework Core and write operations using Dapper, following the technical assessment requirements. Review transaction handling and database consistency.

### Testing

Help me create unit and integration tests for business rules, duplicate resources, inventory movements, optimistic concurrency, and soft delete. Analyze the test results before moving forward.

### Security

Configure OAuth2 and JWT authentication with Keycloak. Validate access restrictions for administrative and standard users, including expected HTTP 401 and 403 responses.

### Docker and Troubleshooting

Help me containerize the application using Docker Compose and SQL Server. Diagnose errors using actual command output, verify file paths, preserve existing data, and avoid unnecessary environment changes.

### Functional Verification

Evaluate the results of the current functional tests and identify any remaining validation gaps before preparing the project for delivery.

## 5. Developer Supervision

AI-generated suggestions were treated as proposals requiring validation rather than guaranteed solutions.

The implementation process included:

- Reviewing code and project structure.
- Building the solution after changes.
- Running automated tests.
- Checking HTTP responses through Swagger.
- Inspecting persisted data in SQL Server.
- Reviewing Docker container states and logs.
- Verifying JWT authentication and role-based authorization.
- Testing optimistic concurrency and historical data preservation.

When an implementation or configuration did not behave as expected, diagnostic evidence was used to guide corrections.

## 6. Examples of Review

### Docker SQL Initialization

A database initialization failure was investigated by examining the mounted path inside the container.

The issue was traced to a directory created where a SQL script file was expected. The physical file location and Docker mount were corrected and verified before continuing.

### JWT Authentication in Docker

Running the API in Docker required separating Keycloak's internal network address from the issuer expected in JWT validation.

Connectivity was tested from within Docker, and authentication was validated through Swagger using both authenticated and unauthenticated requests.

### Optimistic Concurrency

The RowVersion implementation was verified using two updates based on the same original version.

The first update succeeded. The second was rejected with HTTP 409, and the first update's value remained stored in SQL Server.

### Soft Delete and Inventory History

The delete operation was tested against persisted SQL Server data. The product became inactive while historical inventory movements remained available through both the database and API.

## 7. Verification Results

At the final recorded verification stage:

- 84 automated tests passed.
- Swagger was accessible from the containerized API.
- Anonymous requests were rejected with HTTP 401.
- Authenticated requests succeeded.
- Unauthorized administrative operations returned HTTP 403.
- Inventory entry and exit operations updated stock correctly.
- Insufficient-stock operations were rejected without altering stock.
- Stale RowVersion updates returned HTTP 409.
- Soft delete preserved product and movement history.

## 8. Limitations and Transparency

Generative AI can produce incorrect assumptions, unnecessary changes, or suggestions that do not match the current codebase.

For that reason, generated outputs were reviewed and validated against the running application and database.

This report summarizes AI-assisted engineering work. It does not claim that all code was written manually or that every prompt was preserved verbatim.
