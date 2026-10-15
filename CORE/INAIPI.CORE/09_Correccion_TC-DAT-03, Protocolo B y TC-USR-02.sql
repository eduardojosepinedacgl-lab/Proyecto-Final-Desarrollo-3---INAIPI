USE [INAIPI_Core];
GO
-----------------(CORRECCION A ERROR EN ORIGINAL)
-- 1. Eliminar la restricción previa si existe en el catálogo
IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_tblBeneficiarios_Version')
BEGIN
    -- Si la columna ya existe y tiene la restricción atada, se desacopla
    DECLARE @TablaPadre NVARCHAR(256) = (
        SELECT OBJECT_NAME(parent_object_id) 
        FROM sys.default_constraints 
        WHERE name = 'DF_tblBeneficiarios_Version'
    );
    EXEC('ALTER TABLE dbo.' + @TablaPadre + ' DROP CONSTRAINT DF_tblBeneficiarios_Version;');
END
GO

-- 2. Asegurar la columna VersionRegistro
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.tblBeneficiarios') AND name = 'VersionRegistro'
)
BEGIN
    ALTER TABLE dbo.tblBeneficiarios 
    ADD VersionRegistro INT NOT NULL CONSTRAINT DF_tblBeneficiarios_Version DEFAULT 1;
END
GO

-- 3. Verificación de existencia
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'tblBeneficiarios' AND COLUMN_NAME = 'VersionRegistro';
GO



-------------------------(ORIGINAL)
-- 1. Protocolo B: Agregar VersionRegistro en tblBeneficiarios si no existe
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.tblBeneficiarios') AND name = 'VersionRegistro'
)
BEGIN
    ALTER TABLE dbo.tblBeneficiarios 
    ADD VersionRegistro INT NOT NULL CONSTRAINT DF_tblBeneficiarios_Version DEFAULT 1;
END
GO

-- 2. TC-DAT-03: Agregar columnas faltantes en tblAtenciones
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.tblAtenciones') AND name = 'ServicioPrestado'
)
BEGIN
    ALTER TABLE dbo.tblAtenciones ADD ServicioPrestado NVARCHAR(100) NULL;
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.tblAtenciones') AND name = 'UsuarioRegistroId'
)
BEGIN
    ALTER TABLE dbo.tblAtenciones ADD UsuarioRegistroId INT NULL;
END
GO

-- 3. TC-USR-02: Garantizar restricción UNIQUE en Username para evitar duplicados
-- Eliminar duplicados si existieran antes de aplicar la restricción
WITH UsuariosDuplicados AS (
    SELECT UsuarioId, ROW_NUMBER() OVER(PARTITION BY Username ORDER BY UsuarioId) AS Fila
    FROM dbo.tblUsuarios
)
DELETE FROM UsuariosDuplicados WHERE Fila > 1;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'UQ_tblUsuarios_Username' AND object_id = OBJECT_ID('dbo.tblUsuarios')
)
BEGIN
    ALTER TABLE dbo.tblUsuarios 
    ADD CONSTRAINT UQ_tblUsuarios_Username UNIQUE (Username);
END
GO

---------------------------- Coreccion Concurrencia Optimista no Detectada
USE [INAIPI_Core];
GO

CREATE OR ALTER PROCEDURE dbo.ppActualizarBeneficiario
    @BeneficiarioId INT,
    @Nombres NVARCHAR(100),
    @Apellidos NVARCHAR(100),
    @FechaNacimiento DATE,
    @CentroId INT,
    @VersionOriginal INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE dbo.tblBeneficiarios
        SET Nombres = @Nombres,
            Apellidos = @Apellidos,
            FechaNacimiento = @FechaNacimiento,
            CentroId = @CentroId,
            VersionRegistro = VersionRegistro + 1
        WHERE BeneficiarioId = @BeneficiarioId 
          AND VersionRegistro = @VersionOriginal;

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('Error de concurrencia: el registro fue modificado por otro proceso o usuario.', 16, 1);
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO
------------------------------------------
USE [INAIPI_Core];
GO

-- 1. Agregar restricción DEFAULT a TransaccionGuid
IF NOT EXISTS (
    SELECT 1 FROM sys.default_constraints 
    WHERE name = 'DF_tblAtenciones_TransaccionGuid'
)
BEGIN
    ALTER TABLE dbo.tblAtenciones 
    ADD CONSTRAINT DF_tblAtenciones_TransaccionGuid DEFAULT NEWID() FOR TransaccionGuid;
END
GO

