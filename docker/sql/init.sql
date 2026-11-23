USE [master];
GO
IF DB_ID(N'InventoryDB') IS NULL
BEGIN
    CREATE DATABASE [InventoryDB];
END;
GO
USE [InventoryDB];
GO

IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories
    (
        CategoryId INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT (1),
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Categories_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2(0) NULL,
        CONSTRAINT PK_Categories PRIMARY KEY CLUSTERED (CategoryId),
        CONSTRAINT UQ_Categories_Name UNIQUE (Name)
    );
END;
GO

IF OBJECT_ID(N'dbo.MovementTypes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MovementTypes
    (
        MovementTypeId TINYINT NOT NULL,
        Name NVARCHAR(30) NOT NULL,
        StockSign SMALLINT NOT NULL,
        CONSTRAINT PK_MovementTypes PRIMARY KEY CLUSTERED (MovementTypeId),
        CONSTRAINT UQ_MovementTypes_Name UNIQUE (Name),
        CONSTRAINT CK_MovementTypes_StockSign CHECK (StockSign IN (-1, 1))
    );
END;
GO

IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products
    (
        ProductId INT IDENTITY(1,1) NOT NULL,
        CategoryId INT NOT NULL,
        Sku NVARCHAR(50) NOT NULL,
        Name NVARCHAR(150) NOT NULL,
        Description NVARCHAR(1000) NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        CurrentStock INT NOT NULL CONSTRAINT DF_Products_CurrentStock DEFAULT (0),
        MinimumStock INT NOT NULL CONSTRAINT DF_Products_MinimumStock DEFAULT (0),
        IsActive BIT NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT (1),
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2(0) NULL,
        RowVersion ROWVERSION NOT NULL,
        CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (ProductId),
        CONSTRAINT UQ_Products_Sku UNIQUE (Sku),
        CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId)
            REFERENCES dbo.Categories(CategoryId),
        CONSTRAINT CK_Products_UnitPrice CHECK (UnitPrice >= 0),
        CONSTRAINT CK_Products_CurrentStock CHECK (CurrentStock >= 0),
        CONSTRAINT CK_Products_MinimumStock CHECK (MinimumStock >= 0)
    );
END;
GO

IF OBJECT_ID(N'dbo.InventoryMovements', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.InventoryMovements
    (
        MovementId BIGINT IDENTITY(1,1) NOT NULL,
        ProductId INT NOT NULL,
        MovementTypeId TINYINT NOT NULL,
        Quantity INT NOT NULL,
        Reason NVARCHAR(250) NULL,
        CreatedBy NVARCHAR(200) NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_InventoryMovements_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_InventoryMovements PRIMARY KEY CLUSTERED (MovementId),
        CONSTRAINT FK_InventoryMovements_Products FOREIGN KEY (ProductId)
            REFERENCES dbo.Products(ProductId),
        CONSTRAINT FK_InventoryMovements_MovementTypes FOREIGN KEY (MovementTypeId)
            REFERENCES dbo.MovementTypes(MovementTypeId),
        CONSTRAINT CK_InventoryMovements_Quantity CHECK (Quantity > 0)
    );
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.Products') AND name = N'IX_Products_CategoryId'
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Products_CategoryId
        ON dbo.Products(CategoryId)
        INCLUDE (Name, Sku, UnitPrice, CurrentStock);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.InventoryMovements') AND name = N'IX_InventoryMovements_ProductId_CreatedAt'
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_InventoryMovements_ProductId_CreatedAt
        ON dbo.InventoryMovements(ProductId ASC, CreatedAt DESC);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.InventoryMovements') AND name = N'IX_InventoryMovements_MovementTypeId'
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_InventoryMovements_MovementTypeId
        ON dbo.InventoryMovements(MovementTypeId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.MovementTypes WHERE MovementTypeId = 1)
BEGIN
    INSERT INTO dbo.MovementTypes (MovementTypeId, Name, StockSign)
    VALUES (1, N'Entry', 1);
END;
IF NOT EXISTS (SELECT 1 FROM dbo.MovementTypes WHERE MovementTypeId = 2)
BEGIN
    INSERT INTO dbo.MovementTypes (MovementTypeId, Name, StockSign)
    VALUES (2, N'Exit', -1);
END;
GO

PRINT N'InventoryDB schema and movement types are ready.';
GO
