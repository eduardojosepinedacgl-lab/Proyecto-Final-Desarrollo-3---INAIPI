-- =============================================================================
-- PROYECTO: INAIPI - MÓDULO CORE
-- ARCHIVO: 01_Tablas_Core.sql
-- DESCRIPCIÓN: Definición del esquema relacional central, control de acceso,
--              entidades asistenciales, auditoría Log4Net y control de versiones.
-- =============================================================================

USE [master];
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'INAIPI_Core')
BEGIN
    CREATE DATABASE [INAIPI_Core];
END
GO

USE [INAIPI_Core];
GO

-- -----------------------------------------------------------------------------
-- 1. TABLA: tblRoles
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.tblRoles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblRoles (
        RolId INT IDENTITY(1,1) NOT NULL,
        NombreRol NVARCHAR(50) NOT NULL,
        Descripcion NVARCHAR(200) NOT NULL,
        Estado BIT NOT NULL CONSTRAINT DF_tblRoles_Estado DEFAULT (1),
        CONSTRAINT PK_tblRoles PRIMARY KEY CLUSTERED (RolId ASC),
        CONSTRAINT UQ_tblRoles_NombreRol UNIQUE NONCLUSTERED (NombreRol ASC)
    );
END
GO

-- -----------------------------------------------------------------------------
-- 2. TABLA: tblPermisos
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.tblPermisos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblPermisos (
        PermisoId INT IDENTITY(1,1) NOT NULL,
        CodigoPermiso NVARCHAR(50) NOT NULL,
        Modulo NVARCHAR(50) NOT NULL,
        Descripcion NVARCHAR(200) NOT NULL,
        CONSTRAINT PK_tblPermisos PRIMARY KEY CLUSTERED (PermisoId ASC),
        CONSTRAINT UQ_tblPermisos_Codigo UNIQUE NONCLUSTERED (CodigoPermiso ASC)
    );
END
GO

-- -----------------------------------------------------------------------------
-- 3. TABLA: tblRolesPermisos (Intermedia)
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.tblRolesPermisos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblRolesPermisos (
        RolId INT NOT NULL,
        PermisoId INT NOT NULL,
        CONSTRAINT PK_tblRolesPermisos PRIMARY KEY CLUSTERED (RolId ASC, PermisoId ASC),
        CONSTRAINT FK_tblRolesPermisos_Roles FOREIGN KEY (RolId) 
            REFERENCES dbo.tblRoles (RolId) ON DELETE CASCADE,
        CONSTRAINT FK_tblRolesPermisos_Permisos FOREIGN KEY (PermisoId) 
            REFERENCES dbo.tblPermisos (PermisoId) ON DELETE CASCADE
    );
END
GO

-- -----------------------------------------------------------------------------
-- 4. TABLA: tblCentros (CAIPI / CAFI)
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.tblCentros', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblCentros (
        CentroId INT IDENTITY(1,1) NOT NULL,
        CodigoCentro NVARCHAR(20) NOT NULL,
        NombreCentro NVARCHAR(150) NOT NULL,
        TipoCentro NVARCHAR(20) NOT NULL, -- 'CAIPI' o 'CAFI'
        Direccion NVARCHAR(250) NOT NULL,
        Telefono NVARCHAR(20) NOT NULL,
        Estado BIT NOT NULL CONSTRAINT DF_tblCentros_Estado DEFAULT (1),
        CONSTRAINT PK_tblCentros PRIMARY KEY CLUSTERED (CentroId ASC),
        CONSTRAINT UQ_tblCentros_Codigo UNIQUE NONCLUSTERED (CodigoCentro ASC)
    );
END
GO

-- -----------------------------------------------------------------------------
-- 5. TABLA: tblUsuarios
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.tblUsuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblUsuarios (
        UsuarioId INT IDENTITY(1,1) NOT NULL,
        Username NVARCHAR(50) NOT NULL,
        PasswordHash NVARCHAR(256) NOT NULL,
        Nombres NVARCHAR(100) NOT NULL,
        Apellidos NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        RolId INT NOT NULL,
        CentroId INT NULL,
        Estado BIT NOT NULL CONSTRAINT DF_tblUsuarios_Estado DEFAULT (1),
        FechaCreacion DATETIME NOT NULL CONSTRAINT DF_tblUsuarios_FechaCreacion DEFAULT (GETDATE()),
        CONSTRAINT PK_tblUsuarios PRIMARY KEY CLUSTERED (UsuarioId ASC),
        CONSTRAINT UQ_tblUsuarios_Username UNIQUE NONCLUSTERED (Username ASC),
        CONSTRAINT FK_tblUsuarios_Roles FOREIGN KEY (RolId) 
            REFERENCES dbo.tblRoles (RolId),
        CONSTRAINT FK_tblUsuarios_Centros FOREIGN KEY (CentroId) 
            REFERENCES dbo.tblCentros (CentroId)
    );
END
GO

-- -----------------------------------------------------------------------------
-- 6. TABLA: tblTutores
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.tblTutores', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblTutores (
        TutorId INT IDENTITY(1,1) NOT NULL,
        DocumentoIdentidad NVARCHAR(20) NOT NULL,
        Nombres NVARCHAR(100) NOT NULL,
        Apellidos NVARCHAR(100) NOT NULL,
        Telefono NVARCHAR(20) NOT NULL,
        Direccion NVARCHAR(250) NOT NULL,
        FechaRegistro DATETIME NOT NULL CONSTRAINT DF_tblTutores_FechaRegistro DEFAULT (GETDATE()),
        CONSTRAINT PK_tblTutores PRIMARY KEY CLUSTERED (TutorId ASC),
        CONSTRAINT UQ_tblTutores_Documento UNIQUE NONCLUSTERED (DocumentoIdentidad ASC)
    );
END
GO

-- -----------------------------------------------------------------------------
-- 7. TABLA: tblBeneficiarios (Concurrencia Optimista mediante Columna Version)
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.tblBeneficiarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblBeneficiarios (
        BeneficiarioId INT IDENTITY(1,1) NOT NULL,
        Nombres NVARCHAR(100) NOT NULL,
        Apellidos NVARCHAR(100) NOT NULL,
        FechaNacimiento DATE NOT NULL,
        CentroId INT NOT NULL,
        TutorId INT NOT NULL,
        FotografiaUrl NVARCHAR(500) NULL,
        Estado BIT NOT NULL CONSTRAINT DF_tblBeneficiarios_Estado DEFAULT (1),
        Version INT NOT NULL CONSTRAINT DF_tblBeneficiarios_Version DEFAULT (1),
        FechaRegistro DATETIME NOT NULL CONSTRAINT DF_tblBeneficiarios_FechaRegistro DEFAULT (GETDATE()),
        CONSTRAINT PK_tblBeneficiarios PRIMARY KEY CLUSTERED (BeneficiarioId ASC),
        CONSTRAINT FK_tblBeneficiarios_Centros FOREIGN KEY (CentroId) 
            REFERENCES dbo.tblCentros (CentroId),
        CONSTRAINT FK_tblBeneficiarios_Tutores FOREIGN KEY (TutorId) 
            REFERENCES dbo.tblTutores (TutorId)
    );
END
GO

-- -----------------------------------------------------------------------------
-- 8. TABLA: tblAtenciones (Idempotencia mediante TransaccionGuid)
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.tblAtenciones', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.tblAtenciones (
        AtencionId INT IDENTITY(1,1) NOT NULL,
        TransaccionGuid UNIQUEIDENTIFIER NOT NULL,
        BeneficiarioId INT NOT NULL,
        CentroId INT NOT NULL,
        Monto DECIMAL(18,2) NOT NULL CONSTRAINT DF_tblAtenciones_Monto DEFAULT (0.00),
        DescripcionServicio NVARCHAR(500) NOT NULL,
        FechaRegistro DATETIME NOT NULL CONSTRAINT DF_tblAtenciones_FechaRegistro DEFAULT (GETDATE()),
        CONSTRAINT PK_tblAtenciones PRIMARY KEY CLUSTERED (AtencionId ASC),
        CONSTRAINT UQ_tblAtenciones_Guid UNIQUE NONCLUSTERED (TransaccionGuid ASC),
        CONSTRAINT FK_tblAtenciones_Beneficiarios FOREIGN KEY (BeneficiarioId) 
            REFERENCES dbo.tblBeneficiarios (BeneficiarioId),
        CONSTRAINT FK_tblAtenciones_Centros FOREIGN KEY (CentroId) 
            REFERENCES dbo.tblCentros (CentroId)
    );
END
GO

-- -----------------------------------------------------------------------------
-- 9. TABLA: dbo.Log4NetLog (Esquema oficial de auditoría para AdoNetAppender)
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Log4NetLog', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Log4NetLog (
        [id] INT IDENTITY(1,1) NOT NULL,
        [Date] DATETIME NOT NULL,
        [Thread] VARCHAR(255) NOT NULL,
        [Level] VARCHAR(50) NOT NULL,
        [Logger] VARCHAR(255) NOT NULL,
        [Message] VARCHAR(4000) NOT NULL,
        [Exception] VARCHAR(2000) NULL,
        CONSTRAINT PK_Log4NetLog PRIMARY KEY CLUSTERED ([id] ASC)
    );
END
GO