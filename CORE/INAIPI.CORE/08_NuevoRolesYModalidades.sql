USE [INAIPI_Core];
GO

-- 1. Ampliación de Modalidades en tblCentros
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.tblCentros') AND name = 'Modalidad'
)
BEGIN
    ALTER TABLE dbo.tblCentros
    ADD Modalidad VARCHAR(30) NULL 
        CONSTRAINT CK_tblCentros_Modalidad 
        CHECK (Modalidad IN ('Gestion Directa', 'Cogestion', 'Subrogacion', 'Fortalecimiento ASFL'));

    -- Asignación por defecto al centro semilla existente
    EXEC('UPDATE dbo.tblCentros SET Modalidad = ''Gestion Directa'' WHERE Modalidad IS NULL;');

    ALTER TABLE dbo.tblCentros ALTER COLUMN Modalidad VARCHAR(30) NOT NULL;
END
GO

-- 2. Creación del Catálogo de Componentes Oficiales
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'tblComponentesAtencion')
BEGIN
    CREATE TABLE dbo.tblComponentesAtencion (
        ComponenteId INT IDENTITY(1,1) PRIMARY KEY,
        Codigo VARCHAR(10) NOT NULL UNIQUE,
        NombreComponente VARCHAR(100) NOT NULL,
        DivisionResponsable VARCHAR(100) NOT NULL
    );

    INSERT INTO dbo.tblComponentesAtencion (Codigo, NombreComponente, DivisionResponsable)
    VALUES 
    ('CMP-EDU', 'Estimulacion Oportuna y Educacion Inicial', 'Division de Educacion'),
    ('CMP-SAL', 'Salud y Nutricion', 'Division de Salud y Nutricion'),
    ('CMP-NEE', 'Atencion a Necesidades Educativas Especiales', 'Departamento Tecnico Multidisciplinario'),
    ('CMP-PRO', 'Proteccion Contra la Violencia', 'Division de Proteccion Social'),
    ('CMP-REG', 'Registro de Nacimiento e Identidad', 'Articulacion Territorial'),
    ('CMP-FAM', 'Familia y Comunidad', 'Departamento de Base Familiar y Comunitario');
END
GO

-- 3. Vinculación en tblAtenciones
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID('dbo.tblAtenciones') AND name = 'ComponenteId'
)
BEGIN
    ALTER TABLE dbo.tblAtenciones
    ADD ComponenteId INT NULL
        CONSTRAINT FK_tblAtenciones_Componente 
        FOREIGN KEY REFERENCES dbo.tblComponentesAtencion(ComponenteId);
END
GO

-- 4. Sembrado de Roles Institucionales
IF NOT EXISTS (SELECT 1 FROM dbo.tblRoles WHERE NombreRol = 'Coordinador')
    INSERT INTO dbo.tblRoles (NombreRol, Descripcion, Estado) VALUES ('Coordinador', 'Gestion operativa de centros y beneficiarios', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.tblRoles WHERE NombreRol = 'Auditor')
    INSERT INTO dbo.tblRoles (NombreRol, Descripcion, Estado) VALUES ('Auditor', 'Inspeccion de bitacoras y reportes locales', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.tblRoles WHERE NombreRol = 'Cajero')
    INSERT INTO dbo.tblRoles (NombreRol, Descripcion, Estado) VALUES ('Cajero', 'Operador de ventanilla en canal Caja', 1);
GO

-- 5. Sembrado de Usuarios Operativos
DECLARE @RolCoord INT = (SELECT RolId FROM dbo.tblRoles WHERE NombreRol = 'Coordinador');
DECLARE @RolAuditor INT = (SELECT RolId FROM dbo.tblRoles WHERE NombreRol = 'Auditor');
DECLARE @RolCajero INT = (SELECT RolId FROM dbo.tblRoles WHERE NombreRol = 'Cajero');
DECLARE @CentroId INT = (SELECT TOP 1 CentroId FROM dbo.tblCentros);

-- Coordinador (Clave: CoordSeguro2026*)
IF NOT EXISTS (SELECT 1 FROM dbo.tblUsuarios WHERE Username = 'coordinador')
BEGIN
    INSERT INTO dbo.tblUsuarios (Username, PasswordHash, Nombres, Apellidos, Email, RolId, CentroId, Estado, FechaCreacion)
    VALUES ('coordinador', LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'CoordSeguro2026*'), 2)), 'Laura', 'Mendez', 'coord@inaipi.gob.do', @RolCoord, @CentroId, 1, GETDATE());
END

-- Auditor (Clave: AuditorSeguro2026*)
IF NOT EXISTS (SELECT 1 FROM dbo.tblUsuarios WHERE Username = 'auditor')
BEGIN
    INSERT INTO dbo.tblUsuarios (Username, PasswordHash, Nombres, Apellidos, Email, RolId, CentroId, Estado, FechaCreacion)
    VALUES ('auditor', LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'AuditorSeguro2026*'), 2)), 'Roberto', 'Gomez', 'auditor@inaipi.gob.do', @RolAuditor, NULL, 1, GETDATE());
END

-- Cajero (Clave: CajeroSeguro2026*)
IF NOT EXISTS (SELECT 1 FROM dbo.tblUsuarios WHERE Username = 'cajero')
BEGIN
    INSERT INTO dbo.tblUsuarios (Username, PasswordHash, Nombres, Apellidos, Email, RolId, CentroId, Estado, FechaCreacion)
    VALUES ('cajero', LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'CajeroSeguro2026*'), 2)), 'Marcos', 'Diaz', 'caja@inaipi.gob.do', @RolCajero, @CentroId, 1, GETDATE());
END
GO
