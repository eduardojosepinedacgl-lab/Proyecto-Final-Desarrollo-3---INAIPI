-- =============================================================================
-- PROYECTO: INAIPI - MÓDULO INTEGRACIÓN (SND / GATEWAY)
-- ARCHIVO: 99_ReiniciarDatos.sql
-- DESCRIPCIÓN: Reinicio idempotente de la cola intermedia TransaccionesPendientes.
-- =============================================================================

USE [INAIPI_Integracion];
GO

SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.TransaccionesPendientes', N'U') IS NOT NULL
BEGIN
    DECLARE @EnCola INT = (SELECT COUNT(1) FROM dbo.TransaccionesPendientes);
    IF (@EnCola > 0)
    BEGIN
        PRINT N'ADVERTENCIA: Se eliminarán ' + CAST(@EnCola AS NVARCHAR(20)) + N' registros almacenados en dbo.TransaccionesPendientes.';
    END

    TRUNCATE TABLE dbo.TransaccionesPendientes;
    PRINT N'Tabla dbo.TransaccionesPendientes truncada correctamente.';
END
GO