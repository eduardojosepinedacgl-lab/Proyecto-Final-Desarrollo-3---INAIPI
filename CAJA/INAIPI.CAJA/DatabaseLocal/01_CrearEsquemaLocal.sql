-- ==============================================================================
-- PROYECTO: INAIPI - MÓDULO CAJA
-- ARCHIVO: 01_CrearEsquemaLocal.sql
-- DESCRIPCIÓN: Creación idempotente de la base de datos local, columnas fiscales
--              (Subtotal, Itbis, Importe) y estructura Outbox de Contingencia.
-- ==============================================================================

USE [master];
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'INAIPI_CajaLocal')
BEGIN
    CREATE DATABASE [INAIPI_CajaLocal];
END
GO

USE [INAIPI_CajaLocal];
GO

-- -----------------------------------------------------------------------------
-- 1. TABLA: tblTransaccionesLocales
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.tblTransaccionesLocales', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblTransaccionesLocales (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TransaccionGuid UNIQUEIDENTIFIER NOT NULL UNIQUE,
        BeneficiarioId INT NOT NULL,
        CentroId INT NOT NULL CONSTRAINT DF_tblTransaccionesLocales_CentroId DEFAULT (1),
        ComponenteId INT NOT NULL CONSTRAINT DF_tblTransaccionesLocales_ComponenteId DEFAULT (1),
        UsuarioRegistroId INT NOT NULL CONSTRAINT DF_tblTransaccionesLocales_UsuarioId DEFAULT (1),
        DescripcionServicio NVARCHAR(255) NOT NULL,
        Subtotal DECIMAL(18,2) NOT NULL CONSTRAINT DF_tblTransaccionesLocales_Subtotal DEFAULT (0.00),
        Itbis DECIMAL(18,2) NOT NULL CONSTRAINT DF_tblTransaccionesLocales_Itbis DEFAULT (0.00),
        Importe DECIMAL(18,2) NOT NULL,
        Fecha DATETIME NOT NULL
    );
END
GO

-- Ampliación idempotente por si la tabla ya había sido creada con el esquema antiguo
IF COL_LENGTH(N'dbo.tblTransaccionesLocales', N'CentroId') IS NULL
    ALTER TABLE dbo.tblTransaccionesLocales ADD CentroId INT NOT NULL CONSTRAINT DF_tblTransaccionesLocales_CentroId DEFAULT (1);
GO

IF COL_LENGTH(N'dbo.tblTransaccionesLocales', N'ComponenteId') IS NULL
    ALTER TABLE dbo.tblTransaccionesLocales ADD ComponenteId INT NOT NULL CONSTRAINT DF_tblTransaccionesLocales_ComponenteId DEFAULT (1);
GO

IF COL_LENGTH(N'dbo.tblTransaccionesLocales', N'UsuarioRegistroId') IS NULL
    ALTER TABLE dbo.tblTransaccionesLocales ADD UsuarioRegistroId INT NOT NULL CONSTRAINT DF_tblTransaccionesLocales_UsuarioId DEFAULT (1);
GO

IF COL_LENGTH(N'dbo.tblTransaccionesLocales', N'Subtotal') IS NULL
    ALTER TABLE dbo.tblTransaccionesLocales ADD Subtotal DECIMAL(18,2) NOT NULL CONSTRAINT DF_tblTransaccionesLocales_Subtotal DEFAULT (0.00);
GO

IF COL_LENGTH(N'dbo.tblTransaccionesLocales', N'Itbis') IS NULL
    ALTER TABLE dbo.tblTransaccionesLocales ADD Itbis DECIMAL(18,2) NOT NULL CONSTRAINT DF_tblTransaccionesLocales_Itbis DEFAULT (0.00);
GO

-- -----------------------------------------------------------------------------
-- 2. TABLA: tblOutboxTransacciones (Sincronización diferida)
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.tblOutboxTransacciones', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblOutboxTransacciones (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TransaccionGuid UNIQUEIDENTIFIER NOT NULL UNIQUE,
        PayloadJson NVARCHAR(MAX) NOT NULL,
        FechaCreacion DATETIME NOT NULL,
        Sincronizado BIT NOT NULL DEFAULT 0,
        FechaSincronizacion DATETIME NULL
    );
END
GO

-- -----------------------------------------------------------------------------
-- 3. PROCEDIMIENTO: ppInsertarTransaccionContingencia
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.ppInsertarTransaccionContingencia
    @TransaccionGuid UNIQUEIDENTIFIER,
    @BeneficiarioId INT,
    @CentroId INT = 1,
    @ComponenteId INT = 1,
    @UsuarioRegistroId INT = 1,
    @DescripcionServicio NVARCHAR(255),
    @Subtotal DECIMAL(18,2) = 0.00,
    @Itbis DECIMAL(18,2) = 0.00,
    @Importe DECIMAL(18,2),
    @Fecha DATETIME,
    @PayloadJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Si Subtotal viene en 0 por una llamada heredada, se calcula a partir del Importe
    IF (@Subtotal <= 0 AND @Importe > 0)
    BEGIN
        SET @Subtotal = ROUND(@Importe / 1.18, 2);
        SET @Itbis = @Importe - @Subtotal;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.tblTransaccionesLocales (
            TransaccionGuid,
            BeneficiarioId,
            CentroId,
            ComponenteId,
            UsuarioRegistroId,
            DescripcionServicio,
            Subtotal,
            Itbis,
            Importe,
            Fecha
        )
        VALUES (
            @TransaccionGuid,
            @BeneficiarioId,
            @CentroId,
            @ComponenteId,
            @UsuarioRegistroId,
            @DescripcionServicio,
            @Subtotal,
            @Itbis,
            @Importe,
            @Fecha
        );

        INSERT INTO dbo.tblOutboxTransacciones (
            TransaccionGuid,
            PayloadJson,
            FechaCreacion,
            Sincronizado
        )
        VALUES (
            @TransaccionGuid,
            @PayloadJson,
            @Fecha,
            0
        );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF (XACT_STATE() <> 0)
        BEGIN
            ROLLBACK TRANSACTION;
        END;
        THROW;
    END CATCH
END
GO