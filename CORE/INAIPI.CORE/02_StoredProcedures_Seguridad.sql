-- =============================================================================
-- PROYECTO: INAIPI - MÓDULO CORE
-- ARCHIVO: 02_StoredProcedures_Seguridad.sql
-- DESCRIPCIÓN: Rutinas de autenticación y consulta de permisos institucionales.
--              Prefijo estándar 'pp' obligatorio sin uso de 'sp_'.
-- =============================================================================

USE [INAIPI_Core];
GO

-- -----------------------------------------------------------------------------
-- PROCEDIMIENTO: ppValidarUsuario
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.ppValidarUsuario', N'P') IS NOT NULL
    DROP PROCEDURE dbo.ppValidarUsuario;
GO

CREATE PROCEDURE dbo.ppValidarUsuario
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.UsuarioId,
        u.Username,
        u.Nombres,
        u.Apellidos,
        u.Email,
        u.RolId,
        r.NombreRol,
        u.CentroId,
        c.NombreCentro,
        u.Estado
    FROM dbo.tblUsuarios u
    INNER JOIN dbo.tblRoles r ON u.RolId = r.RolId
    LEFT JOIN dbo.tblCentros c ON u.CentroId = c.CentroId
    WHERE u.Username = @Username 
      AND u.PasswordHash = @PasswordHash
      AND u.Estado = 1
      AND r.Estado = 1;
END
GO

-- -----------------------------------------------------------------------------
-- PROCEDIMIENTO: ppListarPermisosPorRol
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.ppListarPermisosPorRol', N'P') IS NOT NULL
    DROP PROCEDURE dbo.ppListarPermisosPorRol;
GO

CREATE PROCEDURE dbo.ppListarPermisosPorRol
    @RolId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.PermisoId,
        p.CodigoPermiso,
        p.Modulo,
        p.Descripcion
    FROM dbo.tblPermisos p
    INNER JOIN dbo.tblRolesPermisos rp ON p.PermisoId = rp.PermisoId
    WHERE rp.RolId = @RolId;
END
GO