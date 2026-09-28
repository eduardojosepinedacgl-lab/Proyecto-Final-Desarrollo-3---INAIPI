USE [INAIPI_Core];
GO

-- 1. Crear Rol Administrador si no existe
IF NOT EXISTS (SELECT 1 FROM dbo.tblRoles WHERE NombreRol = 'Administrador')
BEGIN
    INSERT INTO dbo.tblRoles (NombreRol, Descripcion, Estado)
    VALUES ('Administrador', 'Control total administrativo de centros y beneficiarios', 1);
END
GO

-- 2. Crear Centro Inicial si no existe
IF NOT EXISTS (SELECT 1 FROM dbo.tblCentros WHERE CodigoCentro = 'CAIPI-CENTRAL')
BEGIN
    INSERT INTO dbo.tblCentros (CodigoCentro, NombreCentro, TipoCentro, Direccion, Telefono, Estado)
    VALUES ('CAIPI-CENTRAL', 'Oficina Central INAIPI', 'CAIPI', 'Av. 27 de Febrero', '809-555-0100', 1);
END
GO

-- 3. Crear Usuario con contraseña de 12+ caracteres ('AdminSeguro2026*')
-- Hash SHA-256 correspondiente: 7c57077a5ddf1d1f041d59d1a8b99ec76f1c4e12e1329ee4fa41893116298586
DECLARE @RolIdAdmin INT = (SELECT RolId FROM dbo.tblRoles WHERE NombreRol = 'Administrador');
DECLARE @CentroIdAdmin INT = (SELECT CentroId FROM dbo.tblCentros WHERE CodigoCentro = 'CAIPI-CENTRAL');

IF NOT EXISTS (SELECT 1 FROM dbo.tblUsuarios WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.tblUsuarios (
        Username, 
        PasswordHash, 
        Nombres, 
        Apellidos, 
        Email, 
        RolId, 
        CentroId, 
        Estado, 
        FechaCreacion
    )
    VALUES (
        'admin',
        '7c57077a5ddf1d1f041d59d1a8b99ec76f1c4e12e1329ee4fa41893116298586',
        'Administrador',
        'General',
        'admin@inaipi.gob.do',
        @RolIdAdmin,
        @CentroIdAdmin,
        1,
        GETDATE()
    );
END
GO