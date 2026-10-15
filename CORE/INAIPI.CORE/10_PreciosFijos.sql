-- =============================================================================
-- PROYECTO: INAIPI - MÓDULO CORE
-- ARCHIVO: 10_PreciosFijos.sql
-- DESCRIPCIÓN: Incorporación de la columna PrecioFijo en tblComponentesAtencion
--              y sembrado idempotente de las tarifas base (sin ITBIS) para
--              garantizar consistencia de precios en CAJA y validación en CORE.
-- =============================================================================

USE [INAIPI_Core];
GO

SET NOCOUNT ON;

-- 1. Asegurar que la tabla tblComponentesAtencion exista (por si 08 no se corrió antes)
IF OBJECT_ID(N'dbo.tblComponentesAtencion', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblComponentesAtencion (
        ComponenteId INT IDENTITY(1,1) PRIMARY KEY,
        Codigo VARCHAR(10) NOT NULL UNIQUE,
        NombreComponente VARCHAR(100) NOT NULL,
        DivisionResponsable VARCHAR(100) NOT NULL,
        PrecioFijo DECIMAL(18,2) NOT NULL CONSTRAINT DF_tblComponentesAtencion_PrecioFijo DEFAULT (100.00)
    );

    INSERT INTO dbo.tblComponentesAtencion (Codigo, NombreComponente, DivisionResponsable, PrecioFijo)
    VALUES 
        ('CMP-EDU', 'Estimulación Oportuna y Educación Inicial', 'División de Educación', 150.00),
        ('CMP-SAL', 'Salud y Nutrición', 'División de Salud y Nutrición', 300.00),
        ('CMP-NEE', 'Atención a Necesidades Educativas Especiales', 'Departamento Técnico Multidisciplinario', 250.00),
        ('CMP-PRO', 'Protección Contra la Violencia', 'División de Protección Social', 100.00),
        ('CMP-REG', 'Registro de Nacimiento e Identidad', 'Articulación Territorial', 200.00),
        ('CMP-FAM', 'Familia y Comunidad', 'Departamento de Base Familiar y Comunitario', 100.00);
END
GO

-- 2. Si la tabla ya existía sin la columna PrecioFijo, agregarla de forma idempotente
IF COL_LENGTH(N'dbo.tblComponentesAtencion', N'PrecioFijo') IS NULL
BEGIN
    ALTER TABLE dbo.tblComponentesAtencion
    ADD PrecioFijo DECIMAL(18,2) NOT NULL CONSTRAINT DF_tblComponentesAtencion_PrecioFijo DEFAULT (100.00);
END
GO

-- 3. Actualizar tarifas base oficiales de demostración (RD$ sin ITBIS)
UPDATE dbo.tblComponentesAtencion SET PrecioFijo = 150.00 WHERE Codigo = 'CMP-EDU' OR ComponenteId = 1;
UPDATE dbo.tblComponentesAtencion SET PrecioFijo = 300.00 WHERE Codigo = 'CMP-SAL' OR ComponenteId = 2;
UPDATE dbo.tblComponentesAtencion SET PrecioFijo = 250.00 WHERE Codigo = 'CMP-NEE' OR ComponenteId = 3;
UPDATE dbo.tblComponentesAtencion SET PrecioFijo = 100.00 WHERE Codigo = 'CMP-PRO' OR ComponenteId = 4;
UPDATE dbo.tblComponentesAtencion SET PrecioFijo = 200.00 WHERE Codigo = 'CMP-REG' OR ComponenteId = 5;
UPDATE dbo.tblComponentesAtencion SET PrecioFijo = 100.00 WHERE Codigo = 'CMP-FAM' OR ComponenteId = 6;
GO

-- 4. Asegurar columnas de desglose fiscal en tblAtenciones para auditoría
IF COL_LENGTH(N'dbo.tblAtenciones', N'ComponenteId') IS NULL
BEGIN
    ALTER TABLE dbo.tblAtenciones
    ADD ComponenteId INT NULL
        CONSTRAINT FK_tblAtenciones_Componente 
        FOREIGN KEY REFERENCES dbo.tblComponentesAtencion(ComponenteId);
END
GO

IF COL_LENGTH(N'dbo.tblAtenciones', N'Subtotal') IS NULL
BEGIN
    ALTER TABLE dbo.tblAtenciones ADD Subtotal DECIMAL(18,2) NULL;
END
GO

IF COL_LENGTH(N'dbo.tblAtenciones', N'Itbis') IS NULL
BEGIN
    ALTER TABLE dbo.tblAtenciones ADD Itbis DECIMAL(18,2) NULL;
END
GO