-- =============================================================================
-- PROYECTO: INAIPI - MÓDULO CORE
-- ARCHIVO: 05_StoredProcedures_Reportes.sql
-- DESCRIPCIÓN: Procedimiento estructurado para abastecer el control ReportViewer
--              RDLC con filtrado dinámico por Centro y Rango de Fechas.
-- =============================================================================

USE [INAIPI_Core];
GO

IF OBJECT_ID(N'dbo.ppGetBeneficiariosPorCentro', N'P') IS NOT NULL
    DROP PROCEDURE dbo.ppGetBeneficiariosPorCentro;
GO

CREATE PROCEDURE dbo.ppGetBeneficiariosPorCentro
    @CentroId INT = NULL,
    @FechaInicio DATE = NULL,
    @FechaFin DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        b.BeneficiarioId,
        b.Nombres AS NombreBeneficiario,
        b.Apellidos AS ApellidoBeneficiario,
        CONVERT(VARCHAR(10), b.FechaNacimiento, 103) AS FechaNacimientoStr,
        DATEDIFF(YEAR, b.FechaNacimiento, GETDATE()) AS EdadAnios,
        b.FotografiaUrl,
        b.Version AS VersionRegistro,
        c.CentroId,
        c.CodigoCentro,
        c.NombreCentro,
        c.TipoCentro,
        t.TutorId,
        t.DocumentoIdentidad AS DocumentoTutor,
        (t.Nombres + N' ' + t.Apellidos) AS NombreCompletoTutor,
        t.Telefono AS TelefonoTutor,
        t.Direccion AS DireccionTutor,
        CONVERT(VARCHAR(10), b.FechaRegistro, 103) AS FechaIngresoStr
    FROM dbo.tblBeneficiarios b
    INNER JOIN dbo.tblCentros c ON b.CentroId = c.CentroId
    INNER JOIN dbo.tblTutores t ON b.TutorId = t.TutorId
    WHERE (@CentroId IS NULL OR b.CentroId = @CentroId)
      AND (@FechaInicio IS NULL OR CAST(b.FechaRegistro AS DATE) >= @FechaInicio)
      AND (@FechaFin IS NULL OR CAST(b.FechaRegistro AS DATE) <= @FechaFin)
      AND b.Estado = 1
    ORDER BY c.NombreCentro ASC, b.Apellidos ASC, b.Nombres ASC;
END
GO