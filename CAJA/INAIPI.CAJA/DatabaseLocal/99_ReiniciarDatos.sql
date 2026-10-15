-- =============================================================================
-- PROYECTO: INAIPI - MÓDULO CAJA (POS CONTINGENCIA)
-- ARCHIVO: 99_ReiniciarDatos.sql
-- DESCRIPCIÓN: Reinicio idempotente de tablas transaccionales locales (Outbox y
--              Transacciones). Advierte en consola si había cobros pendientes.
-- =============================================================================

USE [INAIPI_CajaLocal];
GO

SET NOCOUNT ON;

DECLARE @PendientesOutbox INT = 0;

IF OBJECT_ID(N'dbo.tblOutboxTransacciones', N'U') IS NOT NULL
BEGIN
    -- Verificar si existen transacciones retenidas sin sincronizar antes de truncar
    IF COL_LENGTH(N'dbo.tblOutboxTransacciones', N'Sincronizado') IS NOT NULL
        EXEC sp_executesql N'SELECT @cnt = COUNT(1) FROM dbo.tblOutboxTransacciones WHERE Sincronizado = 0;', N'@cnt INT OUTPUT', @cnt = @PendientesOutbox OUTPUT;
    ELSE IF COL_LENGTH(N'dbo.tblOutboxTransacciones', N'Estado') IS NOT NULL
        EXEC sp_executesql N'SELECT @cnt = COUNT(1) FROM dbo.tblOutboxTransacciones WHERE Estado <> ''Sincronizado'';', N'@cnt INT OUTPUT', @cnt = @PendientesOutbox OUTPUT;
    ELSE
        SELECT @PendientesOutbox = COUNT(1) FROM dbo.tblOutboxTransacciones;

    IF (@PendientesOutbox > 0)
    BEGIN
        PRINT N'ADVERTENCIA: Se encontraron ' + CAST(@PendientesOutbox AS NVARCHAR(20)) + N' transacciones pendientes de sincronizar en tblOutboxTransacciones que serán eliminadas.';
    END

    TRUNCATE TABLE dbo.tblOutboxTransacciones;
    PRINT N'Tabla dbo.tblOutboxTransacciones truncada correctamente.';
END
GO

IF OBJECT_ID(N'dbo.tblTransaccionesLocales', N'U') IS NOT NULL
BEGIN
    TRUNCATE TABLE dbo.tblTransaccionesLocales;
    PRINT N'Tabla dbo.tblTransaccionesLocales truncada correctamente.';
END
GO