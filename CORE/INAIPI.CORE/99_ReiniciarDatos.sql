-- =============================================================================
-- PROYECTO: INAIPI - MÓDULO CORE
-- ARCHIVO: 99_ReiniciarDatos.sql
-- DESCRIPCIÓN: Reinicio transaccional de datos operativos respetando el orden de
--              claves foráneas (Atenciones -> Beneficiarios -> Tutores) y
--              conservando Centros, Componentes de Atención, Usuarios y Roles.
-- =============================================================================

USE [INAIPI_Core];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    -- 1. Limpiar tablas de extensión WEB (si fueron creadas en la base de datos)
    IF OBJECT_ID(N'dbo.tblWebComprobantes', N'U') IS NOT NULL
    BEGIN
        DELETE FROM dbo.tblWebComprobantes;
        IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE object_id = OBJECT_ID(N'dbo.tblWebComprobantes'))
            DBCC CHECKIDENT (N'dbo.tblWebComprobantes', RESEED, 0);
    END

    IF OBJECT_ID(N'dbo.tblWebOperaciones', N'U') IS NOT NULL
    BEGIN
        DELETE FROM dbo.tblWebOperaciones;
    END

    -- 2. Eliminar Atenciones (Hija de tblBeneficiarios y tblCentros)
    IF OBJECT_ID(N'dbo.tblAtenciones', N'U') IS NOT NULL
    BEGIN
        DELETE FROM dbo.tblAtenciones;
        IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE object_id = OBJECT_ID(N'dbo.tblAtenciones'))
            DBCC CHECKIDENT (N'dbo.tblAtenciones', RESEED, 0);
    END

    -- 3. Eliminar Beneficiarios (Hija de tblTutores y tblCentros)
    IF OBJECT_ID(N'dbo.tblBeneficiarios', N'U') IS NOT NULL
    BEGIN
        DELETE FROM dbo.tblBeneficiarios;
        IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE object_id = OBJECT_ID(N'dbo.tblBeneficiarios'))
            DBCC CHECKIDENT (N'dbo.tblBeneficiarios', RESEED, 0);
    END

    -- 4. Eliminar Tutores
    IF OBJECT_ID(N'dbo.tblTutores', N'U') IS NOT NULL
    BEGIN
        DELETE FROM dbo.tblTutores;
        IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE object_id = OBJECT_ID(N'dbo.tblTutores'))
            DBCC CHECKIDENT (N'dbo.tblTutores', RESEED, 0);
    END

    -- 5. Limpiar bitácora de auditoría Log4Net
    IF OBJECT_ID(N'dbo.Log4NetLog', N'U') IS NOT NULL
    BEGIN
        TRUNCATE TABLE dbo.Log4NetLog;
    END

    COMMIT TRANSACTION;
    PRINT N'Reinicio de datos en INAIPI_Core completado exitosamente. Catálogos, Centros, Usuarios y Roles conservados.';
END TRY
BEGIN CATCH
    IF (XACT_STATE() <> 0)
        ROLLBACK TRANSACTION;

    DECLARE @Msg NVARCHAR(4000) = ERROR_MESSAGE();
    RAISERROR(N'Error al reiniciar datos en INAIPI_Core: %s', 16, 1, @Msg);
END CATCH
GO