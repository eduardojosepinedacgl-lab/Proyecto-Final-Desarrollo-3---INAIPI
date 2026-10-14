USE master;
GO
IF DB_ID(N'INAIPI_Integracion') IS NULL
    CREATE DATABASE INAIPI_Integracion;
GO
USE INAIPI_Integracion;
GO
IF OBJECT_ID(N'dbo.TransaccionesPendientes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TransaccionesPendientes
    (
        TransaccionGuid UNIQUEIDENTIFIER NOT NULL,
        TerminalId NVARCHAR(100) NOT NULL,
        BeneficiarioId INT NOT NULL,
        CentroId INT NOT NULL,
        Monto DECIMAL(18,2) NOT NULL,
        Descripcion NVARCHAR(MAX) NULL,
        FechaRegistro DATETIME2 NOT NULL,
        Estado VARCHAR(20) NOT NULL CONSTRAINT DF_Pendientes_Estado DEFAULT ('Pendiente'),
        Intentos INT NOT NULL CONSTRAINT DF_Pendientes_Intentos DEFAULT (0),
        FechaRecepcionUtc DATETIME2 NOT NULL CONSTRAINT DF_Pendientes_Recepcion DEFAULT (SYSUTCDATETIME()),
        ProximoIntentoUtc DATETIME2 NULL,
        FechaSincronizacionUtc DATETIME2 NULL,
        UltimoError NVARCHAR(2000) NULL,
        CONSTRAINT PK_TransaccionesPendientes PRIMARY KEY (TransaccionGuid),
        CONSTRAINT CK_Pendientes_Monto CHECK (Monto>=0),
        CONSTRAINT CK_Pendientes_Intentos CHECK (Intentos>=0),
        CONSTRAINT CK_Pendientes_Estado CHECK (Estado IN ('Pendiente','EnProceso','Sincronizada','Rechazada'))
    );
    CREATE INDEX IX_Pendientes_Estado_ProximoIntento
        ON dbo.TransaccionesPendientes (Estado,ProximoIntentoUtc);
END;
GO
