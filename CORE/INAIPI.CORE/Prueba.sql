USE [INAIPI_Core];
GO

BEGIN TRAN
-- 1. Insertar un Centro de prueba
IF NOT EXISTS (SELECT 1 FROM dbo.tblCentros WHERE CodigoCentro = 'CAIPI-001')
BEGIN
    INSERT INTO dbo.tblCentros (CodigoCentro, NombreCentro, TipoCentro, Direccion, Telefono, Estado)
    VALUES ('CAIPI-001', 'CAIPI Santo Domingo Norte', 'CAIPI', 'Av. Hermanas Mirabal', '809-555-0101', 1);
END

-- 2. Insertar un Tutor y Beneficiario mediante el procedimiento transaccional
EXEC dbo.ppInsertarBeneficiario
    @Nombres = 'Lucas',
    @Apellidos = 'Martínez Pérez',
    @FechaNacimiento = '2023-05-14',
    @CentroId = 1,
    @DocumentoTutor = '402-0000000-1',
    @NombresTutor = 'Ana',
    @ApellidosTutor = 'Pérez Gómez',
    @TelefonoTutor = '809-555-0102',
    @DireccionTutor = 'Calle Principal #12, Villa Mella',
    @FotografiaUrl = NULL;
GO
COMMIT TRAN

ROLLBACK TRAN