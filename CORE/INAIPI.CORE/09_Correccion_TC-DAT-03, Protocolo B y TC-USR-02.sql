-- =============================================================================
-- PROYECTO: INAIPI - MÓDULO CORE
-- ARCHIVO: 09_Correccion_TC-DAT-03, Protocolo B y TC-USR-02.sql
-- DESCRIPCIÓN: Consolidación idempotente de la columna única Version en
--              tblBeneficiarios, columnas de auditoría en tblAtenciones y
--              unicidad de Username en tblUsuarios.
-- =============================================================================

USE [INAIPI_Core];
GO

-- 1. Unificar Version y eliminar la columna duplicada VersionRegistro si fue creada antes
IF EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'dbo.tblBeneficiarios') AND name = N'VersionRegistro'
)
BEGIN
    -- Sincronizar el número de versión más alto hacia la columna oficial Version
    EXEC(N'UPDATE dbo.tblBeneficiarios SET Version = CASE WHEN VersionRegistro > Version THEN VersionRegistro ELSE Version END;');

    -- Eliminar cualquier constraint default atado a VersionRegistro antes de borrar la columna
    DECLARE @ConstraintVR NVARCHAR(256);
    SELECT @ConstraintVR = dc.name
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE dc.parent_object_id = OBJECT_ID(N'dbo.tblBeneficiarios') AND c.name = N'VersionRegistro';

    IF @ConstraintVR IS NOT NULL
        EXEC(N'ALTER TABLE dbo.tblBeneficiarios DROP CONSTRAINT [' + @ConstraintVR + N'];');

    ALTER TABLE dbo.tblBeneficiarios DROP COLUMN VersionRegistro;
END
GO

-- 2. Garantizar que la columna oficial Version tenga su restricción DEFAULT (1)
IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE dc.parent_object_id = OBJECT_ID(N'dbo.tblBeneficiarios') AND c.name = N'Version'
)
BEGIN
    ALTER TABLE dbo.tblBeneficiarios
    ADD CONSTRAINT DF_tblBeneficiarios_Version DEFAULT (1) FOR Version;
END
GO

-- 3. TC-DAT-03: Agregar columnas faltantes en tblAtenciones
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'dbo.tblAtenciones') AND name = N'ServicioPrestado'
)
BEGIN
    ALTER TABLE dbo.tblAtenciones ADD ServicioPrestado NVARCHAR(100) NULL;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'dbo.tblAtenciones') AND name = N'UsuarioRegistroId'
)
BEGIN
    ALTER TABLE dbo.tblAtenciones ADD UsuarioRegistroId INT NULL;
END
GO

-- 4. Agregar restricción DEFAULT a TransaccionGuid en tblAtenciones
IF NOT EXISTS (
    SELECT 1
    FROM sys.default_constraints dc
    INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE dc.parent_object_id = OBJECT_ID(N'dbo.tblAtenciones') AND c.name = N'TransaccionGuid'
)
BEGIN
    ALTER TABLE dbo.tblAtenciones 
    ADD CONSTRAINT DF_tblAtenciones_TransaccionGuid DEFAULT (NEWID()) FOR TransaccionGuid;
END
GO

-- 5. TC-USR-02: Garantizar restricción UNIQUE en Username para evitar duplicados
WITH UsuariosDuplicados AS (
    SELECT UsuarioId, ROW_NUMBER() OVER(PARTITION BY Username ORDER BY UsuarioId) AS Fila
    FROM dbo.tblUsuarios
)
DELETE FROM UsuariosDuplicados WHERE Fila > 1;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = N'UQ_tblUsuarios_Username' AND object_id = OBJECT_ID(N'dbo.tblUsuarios')
)
BEGIN
    ALTER TABLE dbo.tblUsuarios 
    ADD CONSTRAINT UQ_tblUsuarios_Username UNIQUE (Username);
END
GO