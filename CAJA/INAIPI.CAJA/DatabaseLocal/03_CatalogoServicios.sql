-- ==============================================================================
-- PROYECTO: INAIPI - MÓDULO CAJA
-- ARCHIVO: 03_CatalogoServicios.sql
-- DESCRIPCIÓN: Catálogo local de servicios/componentes con PrecioFijo sincronizado
--              con tblComponentesAtencion del CORE (IDs 1 al 6).
-- ==============================================================================

USE [INAIPI_CajaLocal];
GO

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.tblCatalogoServiciosLocal', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblCatalogoServiciosLocal (
        ComponenteId INT NOT NULL PRIMARY KEY,
        Codigo VARCHAR(10) NOT NULL UNIQUE,
        Nombre NVARCHAR(150) NOT NULL,
        PrecioFijo DECIMAL(18,2) NOT NULL,
        Activo BIT NOT NULL CONSTRAINT DF_tblCatalogoServiciosLocal_Activo DEFAULT (1)
    );
END
GO

-- Upsert idempotente de los 6 componentes oficiales y sus precios fijos (RD$ sin ITBIS)
MERGE dbo.tblCatalogoServiciosLocal AS Target
USING (VALUES
    (1, 'CMP-EDU', N'Estimulación Oportuna y Educación Inicial', 150.00),
    (2, 'CMP-SAL', N'Salud y Nutrición',                         300.00),
    (3, 'CMP-NEE', N'Atención a Necesidades Educativas Especiales', 250.00),
    (4, 'CMP-PRO', N'Protección Contra la Violencia',            100.00),
    (5, 'CMP-REG', N'Registro de Nacimiento e Identidad',        200.00),
    (6, 'CMP-FAM', N'Familia y Comunidad',                       100.00)
) AS Source (ComponenteId, Codigo, Nombre, PrecioFijo)
ON Target.ComponenteId = Source.ComponenteId
WHEN MATCHED THEN
    UPDATE SET 
        Codigo = Source.Codigo,
        Nombre = Source.Nombre,
        PrecioFijo = Source.PrecioFijo,
        Activo = 1
WHEN NOT MATCHED THEN
    INSERT (ComponenteId, Codigo, Nombre, PrecioFijo, Activo)
    VALUES (Source.ComponenteId, Source.Codigo, Source.Nombre, Source.PrecioFijo, 1);
GO