-- =============================================================================
-- PROYECTO: INAIPI - MÓDULO CORE
-- ARCHIVO: 04_StoredProcedures_Logs.sql
-- DESCRIPCIÓN: Procedimiento oficial invocado por AdoNetAppender de Log4Net.
--              Implementa exactamente los 6 parámetros del contrato.
-- =============================================================================

USE [INAIPI_Core];
GO

IF OBJECT_ID(N'dbo.ppInsertLog', N'P') IS NOT NULL
    DROP PROCEDURE dbo.ppInsertLog;
GO

CREATE PROCEDURE dbo.ppInsertLog
    @log_date DATETIME,
    @thread VARCHAR(255),
    @level VARCHAR(50),
    @logger VARCHAR(255),
    @message VARCHAR(4000),
    @exception VARCHAR(2000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Log4NetLog (
        [Date],
        [Thread],
        [Level],
        [Logger],
        [Message],
        [Exception]
    )
    VALUES (
        @log_date,
        @thread,
        @level,
        @logger,
        @message,
        @exception
    );
END
GO