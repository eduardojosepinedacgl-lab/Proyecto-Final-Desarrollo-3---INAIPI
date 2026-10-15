-- ==============================================================================
-- PROYECTO: INAIPI - MÓDULO CAJA
-- ARCHIVO: 02_CatalogosLocales.sql
-- DESCRIPCIÓN: Creación de tablas maestras locales (Cajeros y Padrón de Beneficiarios)
--              para permitir el funcionamiento y validación 100% Offline.
-- ==============================================================================
USE [INAIPI_CajaLocal];
GO

-- 1. TABLA DE CAJEROS LOCALES (Autenticación)
IF OBJECT_ID(N'dbo.tblCajerosLocal', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblCajerosLocal (
        EmpleadoId INT NOT NULL PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        NombreCompleto NVARCHAR(200) NOT NULL,
        PasswordHash NVARCHAR(256) NOT NULL,
        CentroId INT NOT NULL DEFAULT 1,
        Activo BIT NOT NULL DEFAULT 1,
        IntentosFallidos INT NOT NULL DEFAULT 0,
        BloqueadoHasta DATETIME NULL,
        UltimaSincronizacion DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- 2. TABLA DEL PADRÓN DE BENEFICIARIOS LOCAL (Búsqueda por Cédula)
IF OBJECT_ID(N'dbo.tblPadronBeneficiariosLocal', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblPadronBeneficiariosLocal(
        BeneficiarioId INT NOT NULL PRIMARY KEY,
        Nombres NVARCHAR(100) NOT NULL,
        Apellidos NVARCHAR(100) NOT NULL,
        CedulaTutor NVARCHAR(11) NOT NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FechaUltimaSincronizacion DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO