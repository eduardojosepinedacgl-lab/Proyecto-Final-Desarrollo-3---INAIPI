-- ==============================================================================
-- PROYECTO: INAIPI - MÓDULO CAJA
-- ARCHIVO: 04_AuditoriaColumnas.sql
-- DESCRIPCIÓN: Verificación y aseguramiento de índices y columnas de auditoría
--              en tblTransaccionesLocales y tblOutboxTransacciones.
-- ==============================================================================

USE [INAIPI_CajaLocal];
GO

SET NOCOUNT ON;

-- 1. Índice para acelerar búsquedas por fecha y cajero en reportes locales
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_tblTransaccionesLocales_Fecha_Cajero' AND object_id = OBJECT_ID(N'dbo.tblTransaccionesLocales'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_tblTransaccionesLocales_Fecha_Cajero
    ON dbo.tblTransaccionesLocales (Fecha DESC, UsuarioRegistroId ASC)
    INCLUDE (BeneficiarioId, ComponenteId, Subtotal, Itbis, Importe);
END
GO

-- 2. Índice para la cola de contingencia Outbox no sincronizada
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_tblOutboxTransacciones_Sincronizado' AND object_id = OBJECT_ID(N'dbo.tblOutboxTransacciones'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_tblOutboxTransacciones_Sincronizado
    ON dbo.tblOutboxTransacciones (Sincronizado ASC, FechaCreacion ASC)
    INCLUDE (TransaccionGuid);
END
GO