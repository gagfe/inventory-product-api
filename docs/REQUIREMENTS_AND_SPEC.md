# Inventory Product — Requirements and Behavior Specification

## Document Status

This document consolidates the technical assessment requirements and the behavior implemented and verified during development.

The development process used incremental feature specifications and acceptance criteria. This consolidated document was prepared for delivery and should not be interpreted as a single formal specification written before the entire implementation.

## 1. Product Management

### Behavior

The API must support creating, retrieving, updating, and logically deleting products.

### Acceptance Criteria

- Products must contain valid business data.
- Duplicate SKUs must be rejected.
- Invalid category references must be rejected.
- Inactive products must be excluded from active product listings.
- Updating a product with an outdated RowVersion must return HTTP 409.
- A successful update must generate a new database RowVersion.

## 2. Inventory Movements

### Behavior

Stock entries increase product inventory, while stock exits decrease it.

### Acceptance Criteria

- A valid stock entry increases available stock by the requested quantity.
- A valid stock exit decreases available stock.
- An exit exceeding available stock must be rejected.
- A rejected stock movement must not alter the product's stock.
- Valid movements must be recorded in the inventory movement history.
- Stock updates and movement records must remain consistent.

## 3. Soft Delete

### Behavior

Deleting a product must deactivate the product without physically deleting its historical records.

### Acceptance Criteria

- A successfully deleted product has `IsActive = false`.
- Inactive products do not appear in the active product listing.
- Previously recorded inventory movements remain stored.
- Historical movements remain accessible through the designated API endpoint.

## 4. Authentication and Authorization

### Behavior

The API uses Keycloak and JWT Bearer authentication to restrict access to protected operations.

### Acceptance Criteria

- Unauthenticated requests to protected endpoints return HTTP 401.
- Authorized users can access permitted resources.
- Users without administrative permissions cannot execute administrator-only operations.
- Forbidden operations return HTTP 403.

## 5. Technical Constraints

The solution must use:

- ASP.NET Core with C#
- SQL Server
- Entity Framework Core for read operations
- Dapper for write operations
- Automated tests
- Docker
- OAuth2-based authentication

The architecture should maintain clear separation of responsibilities and apply relevant SOLID principles and design patterns.

## 6. Verification

The implemented behavior was verified through automated tests and manual API/database tests.

The final verified automated test suite contained 84 passing tests.

## 7. Evidence and Traceability

Relevant evidence includes:

- Automated test classes in `Inventory_Product.Test`
- Swagger functional tests
- Docker startup and application logs
- SQL Server queries verifying stock, RowVersion, active status, and movement history
- The AI-assisted development report in `docs/AI_USAGE.md`
