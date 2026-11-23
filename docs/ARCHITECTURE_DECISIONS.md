# Architecture Decisions and AI Collaboration

## Overview

The Inventory Product API was developed using an incremental, AI-assisted engineering process.

Architectural decisions were evaluated against the technical assessment requirements, maintainability, testability, separation of responsibilities, and verified application behavior.

## Decision 1 — Layered Architecture

**Approach evaluated:** Separate the API, application logic, domain model, data access, and tests into dedicated projects.

**Final decision:** Adopt the following project structure:

- Inventory_Product.Api
- Inventory_Product.Application
- Inventory_Product.Domain
- Inventory_Product.Repository
- Inventory_Product.Test

**Rationale:** This structure separates business rules from HTTP concerns and data access implementation. It also supports dependency injection and isolated testing.

**Review and adaptation:** The final project references and responsibilities were adjusted as the solution evolved rather than preserving every initial structural suggestion.

## Decision 2 — EF Core and Dapper

**Requirement:** Use Entity Framework Core for reading data and Dapper for writing data.

**Final decision:** Implement separate read and write responsibilities within the repository layer.

**Rationale:** The approach satisfies an explicit technical requirement and supports CQRS-style separation without requiring a distributed CQRS architecture.

**Review:** Database operations were validated through unit tests, integration tests, and functional checks.

## Decision 3 — Inventory Consistency

**Problem:** A stock movement must not leave the stock balance inconsistent with the movement history.

**Final decision:** Use transactional operations for inventory updates and movement registration.

**Rationale:** Stock values and movement records represent related business changes and should be consistent.

**Review:** Valid entry and exit operations were tested. Insufficient-stock operations were rejected without changing the stored stock balance.

## Decision 4 — Optimistic Concurrency

**Problem:** Concurrent updates can overwrite changes made by another user.

**Final decision:** Use SQL Server RowVersion and return HTTP 409 for stale updates.

**Rationale:** Optimistic concurrency detects conflicts without requiring long-lived database locks across user interactions.

**Review:** Two updates were attempted using the same original version. The first succeeded, the second returned HTTP 409, and the first change was preserved.

## Decision 5 — Soft Delete

**Problem:** Physical deletion could destroy information needed for inventory history and auditing.

**Final decision:** Mark products as inactive instead of physically deleting them.

**Rationale:** Preserve historical consistency while removing inactive products from normal listings.

**Review:** SQL Server and API tests confirmed that the product became inactive and movement history remained available.

## Decision 6 — OAuth2 and Keycloak

**Requirement:** Protect the API using OAuth2-based authentication.

**Final decision:** Use Keycloak for authentication and JWT validation in ASP.NET Core.

**Rationale:** Separate identity management from application business logic and support role-based permissions.

**Review and adaptation:** Container networking required a distinction between the internal Keycloak metadata address and the token issuer used by browser-based authentication.

The configuration was adapted and verified using successful authentication, HTTP 401 for anonymous requests, and HTTP 403 for forbidden administrative operations.

## Decision 7 — Incremental AI-Assisted Development

**Approach evaluated:** Generating the complete application in one large step versus developing and validating individual features.

**Final decision:** Use incremental development with focused implementation and testing stages.

**Rationale:** Smaller stages made it easier to maintain architectural context, diagnose errors, detect incorrect assumptions, and avoid unverified changes.

**Outcome:** The resulting application passed 84 automated tests and the reported functional verification scenarios.

## Conclusion

AI assistance was used to explore implementation approaches, clarify technical decisions, and troubleshoot errors.

The final implementation was evaluated using code review, compilation, automated testing, HTTP behavior, container diagnostics, and database verification.

The technical assessment requirements remained the primary constraints when selecting the final architecture.