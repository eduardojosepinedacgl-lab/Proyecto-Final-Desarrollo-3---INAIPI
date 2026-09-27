-- =============================================================================
-- PROYECTO: INAIPI - MÓDULO CORE
-- ARCHIVO: 03_StoredProcedures_Operaciones.sql
-- DESCRIPCIÓN: Transacciones multitabla ACID, concurrencia optimista e idempotencia.
-- =============================================================================

USE [INAIPI_Core];
GO

-- -----------------------------------------------------------------------------
-- PROCEDIMIENTO: ppInsertarBeneficiario (Multitabla con Bloque Transaccional ACID)
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.ppInsertarBeneficiario', N'P') IS NOT NULL
    DROP PROCEDURE dbo.ppInsertarBeneficiario;
GO

CREATE PROCEDURE dbo.ppInsertarBeneficiario
    @Nombres NVARCHAR(100),
    @Apellidos NVARCHAR(100),
    @FechaNacimiento DATE,
    @CentroId INT,
    @DocumentoTutor NVARCHAR(20),
    @NombresTutor NVARCHAR(100),
    @ApellidosTutor NVARCHAR(100),
    @TelefonoTutor NVARCHAR(20),
    @DireccionTutor NVARCHAR(250),
    @FotografiaUrl NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @TutorIdLocal INT = 0;
    DECLARE @NuevoBeneficiarioId INT = 0;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Verificar o insertar el Tutor legal
        SELECT @TutorIdLocal = TutorId 
        FROM dbo.tblTutores WITH (UPDLOCK, HOLDLOCK)
        WHERE DocumentoIdentidad = @DocumentoTutor;

        IF (@TutorIdLocal = 0 OR @TutorIdLocal IS NULL)
        BEGIN
            INSERT INTO dbo.tblTutores (
                DocumentoIdentidad, 
                Nombres, 
                Apellidos, 
                Telefono, 
                Direccion, 
                FechaRegistro
            )
            VALUES (
                @DocumentoTutor, 
                @NombresTutor, 
                @ApellidosTutor, 
                @TelefonoTutor, 
                @DireccionTutor, 
                GETDATE()
            );

            SET @TutorIdLocal = SCOPE_IDENTITY();
        END

        -- 2. Insertar Beneficiario inicializando Version = 1
        INSERT INTO dbo.tblBeneficiarios (
            Nombres, 
            Apellidos, 
            FechaNacimiento, 
            CentroId, 
            TutorId, 
            FotografiaUrl, 
            Estado, 
            Version, 
            FechaRegistro
        )
        VALUES (
            @Nombres, 
            @Apellidos, 
            @FechaNacimiento, 
            @CentroId, 
            @TutorIdLocal, 
            @FotografiaUrl, 
            1, 
            1, 
            GETDATE()
        );

        SET @NuevoBeneficiarioId = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        SELECT 
            200 AS Codigo,
            N'Beneficiario y Tutor vinculados satisfactoriamente.' AS Mensaje,
            CAST(@NuevoBeneficiarioId AS NVARCHAR(50)) AS DetalleTecnico;
    END TRY
    BEGIN CATCH
        IF (XACT_STATE() <> 0)
        BEGIN
            ROLLBACK TRANSACTION;
        END

        SELECT 
            500 AS Codigo,
            N'Error transaccional al registrar beneficiario.' AS Mensaje,
            ERROR_MESSAGE() AS DetalleTecnico;
    END CATCH
END
GO

-- -----------------------------------------------------------------------------
-- PROCEDIMIENTO: ppActualizarBeneficiario (Control de Concurrencia Optimista)
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.ppActualizarBeneficiario', N'P') IS NOT NULL
    DROP PROCEDURE dbo.ppActualizarBeneficiario;
GO

CREATE PROCEDURE dbo.ppActualizarBeneficiario
    @BeneficiarioId INT,
    @Nombres NVARCHAR(100),
    @Apellidos NVARCHAR(100),
    @FechaNacimiento DATE,
    @CentroId INT,
    @TutorId INT,
    @FotografiaUrl NVARCHAR(500),
    @VersionOriginal INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.tblBeneficiarios
        SET 
            Nombres = @Nombres,
            Apellidos = @Apellidos,
            FechaNacimiento = @FechaNacimiento,
            CentroId = @CentroId,
            TutorId = @TutorId,
            FotografiaUrl = @FotografiaUrl,
            Version = Version + 1
        WHERE BeneficiarioId = @BeneficiarioId 
          AND Version = @VersionOriginal;

        IF (@@ROWCOUNT = 0)
        BEGIN
            ROLLBACK TRANSACTION;

            -- Detección de colisión de concurrencia optimista
            IF EXISTS (SELECT 1 FROM dbo.tblBeneficiarios WHERE BeneficiarioId = @BeneficiarioId)
            BEGIN
                SELECT 
                    409 AS Codigo,
                    N'Conflicto de concurrencia: El registro fue alterado por otra terminal.' AS Mensaje,
                    N'Version mismatch en la tabla tblBeneficiarios.' AS DetalleTecnico;
            END
            ELSE
            BEGIN
                SELECT 
                    404 AS Codigo,
                    N'El registro del beneficiario no fue encontrado.' AS Mensaje,
                    N'Identificador no existe.' AS DetalleTecnico;
            END
        END
        ELSE
        BEGIN
            COMMIT TRANSACTION;

            SELECT 
                200 AS Codigo,
                N'Beneficiario actualizado exitosamente.' AS Mensaje,
                CAST((@VersionOriginal + 1) AS NVARCHAR(50)) AS DetalleTecnico;
        END
    END TRY
    BEGIN CATCH
        IF (XACT_STATE() <> 0)
        BEGIN
            ROLLBACK TRANSACTION;
        END

        SELECT 
            500 AS Codigo,
            N'Excepción al modificar datos del beneficiario.' AS Mensaje,
            ERROR_MESSAGE() AS DetalleTecnico;
    END CATCH
END
GO

-- -----------------------------------------------------------------------------
-- PROCEDIMIENTO: ppRegistrarAtencion (Idempotencia transaccional con TransaccionGuid)
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.ppRegistrarAtencion', N'P') IS NOT NULL
    DROP PROCEDURE dbo.ppRegistrarAtencion;
GO

CREATE PROCEDURE dbo.ppRegistrarAtencion
    @TransaccionGuid UNIQUEIDENTIFIER,
    @BeneficiarioId INT,
    @CentroId INT,
    @Monto DECIMAL(18,2),
    @DescripcionServicio NVARCHAR(500),
    @FechaRegistro DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    -- Comprobación de idempotencia ante reintentos de red
    IF EXISTS (SELECT 1 FROM dbo.tblAtenciones WHERE TransaccionGuid = @TransaccionGuid)
    BEGIN
        SELECT 
            200 AS Codigo,
            N'Transacción idempotente: La operación ya había sido confirmada previamente.' AS Mensaje,
            CAST(@TransaccionGuid AS NVARCHAR(50)) AS DetalleTecnico;
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar existencia de entidades referenciadas
        IF NOT EXISTS (SELECT 1 FROM dbo.tblBeneficiarios WHERE BeneficiarioId = @BeneficiarioId)
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT 
                400 AS Codigo,
                N'El beneficiario especificado no existe en el sistema.' AS Mensaje,
                N'BeneficiarioId inválido.' AS DetalleTecnico;
            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM dbo.tblCentros WHERE CentroId = @CentroId)
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT 
                400 AS Codigo,
                N'El centro especificado no existe en el sistema.' AS Mensaje,
                N'CentroId inválido.' AS DetalleTecnico;
            RETURN;
        END

        INSERT INTO dbo.tblAtenciones (
            TransaccionGuid, 
            BeneficiarioId, 
            CentroId, 
            Monto, 
            DescripcionServicio, 
            FechaRegistro
        )
        VALUES (
            @TransaccionGuid, 
            @BeneficiarioId, 
            @CentroId, 
            @Monto, 
            @DescripcionServicio, 
            @FechaRegistro
        );

        COMMIT TRANSACTION;

        SELECT 
            200 AS Codigo,
            N'Atención registrada exitosamente en el CORE.' AS Mensaje,
            CAST(@TransaccionGuid AS NVARCHAR(50)) AS DetalleTecnico;
    END TRY
    BEGIN CATCH
        IF (XACT_STATE() <> 0)
        BEGIN
            ROLLBACK TRANSACTION;
        END

        SELECT 
            500 AS Codigo,
            N'Fallo transaccional al asentar la atención.' AS Mensaje,
            ERROR_MESSAGE() AS DetalleTecnico;
    END CATCH
END
GO