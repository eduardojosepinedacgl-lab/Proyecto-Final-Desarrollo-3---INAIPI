-- ==============================================================================
-- PROYECTO: INAIPI - MÓDULO CAJA
-- ARCHIVO: 01_CrearEsquemaLocal.sql
-- DESCRIPCIÓN: Creación directa de la base de datos local y estructura
--              para el patrón Outbox de Contingencia Offline.
-- ==============================================================================

USE [master];
GO

-- 1. Creación directa de la base de datos local (Estilo CORE)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'INAIPI_CajaLocal')
BEGIN
    CREATE DATABASE [INAIPI_CajaLocal];
END
GO

USE [INAIPI_CajaLocal];
GO

-- -----------------------------------------------------------------------------
-- 2. TABLA: tblTransaccionesLocales
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.tblTransaccionesLocales', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblTransaccionesLocales (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TransaccionGuid UNIQUEIDENTIFIER NOT NULL UNIQUE,
        BeneficiarioId INT NOT NULL,
        DescripcionServicio NVARCHAR(255) NOT NULL,
        Importe DECIMAL(18,2) NOT NULL,
        Fecha DATETIME NOT NULL
    );
END
GO

-- -----------------------------------------------------------------------------
-- 3. TABLA: tblOutboxTransacciones (Sincronización diferida)
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
-- 4. PROCEDIMIENTO: ppInsertarTransaccionContingencia
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.ppInsertarTransaccionContingencia', N'P') IS NOT NULL
    DROP PROCEDURE dbo.ppInsertarTransaccionContingencia;
GO

CREATE PROCEDURE dbo.ppInsertarTransaccionContingencia
    @TransaccionGuid UNIQUEIDENTIFIER,
    @BeneficiarioId INT,
    @DescripcionServicio NVARCHAR(255),
    @Importe DECIMAL(18,2),
    @Fecha DATETIME,
    @PayloadJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Insertar el registro comercial local
        INSERT INTO dbo.tblTransaccionesLocales (TransaccionGuid, BeneficiarioId, DescripcionServicio, Importe, Fecha)
        VALUES (@TransaccionGuid, @BeneficiarioId, @DescripcionServicio, @Importe, @Fecha);

        -- Insertar el registro en la cola de sincronización (Outbox)
        INSERT INTO dbo.tblOutboxTransacciones (TransaccionGuid, PayloadJson, FechaCreacion, Sincronizado)
        VALUES (@TransaccionGuid, @PayloadJson, @Fecha, 0);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF (XACT_STATE() <> 0)
        BEGIN
            ROLLBACK TRANSACTION;
        END;
        ;THROW;
    END CATCH
END
GO