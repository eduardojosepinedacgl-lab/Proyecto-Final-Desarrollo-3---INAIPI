-- =============================================================================
-- PROYECTO: INAIPI - MÓDULO CORE
-- ARCHIVO: 03_StoredProcedures_Operaciones.sql
-- DESCRIPCIÓN: Transacciones multitabla ACID, concurrencia optimista, idempotencia
--              y validación en servidor de PrecioFijo + ITBIS (18%).
-- =============================================================================

USE [INAIPI_Core];
GO

-- -----------------------------------------------------------------------------
-- PROCEDIMIENTO: ppInsertarBeneficiario (Multitabla con Bloque Transaccional ACID)
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.ppInsertarBeneficiario
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
    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);

    SET @DocumentoTutor = REPLACE(LTRIM(RTRIM(ISNULL(@DocumentoTutor, N''))), N'-', N'');
    SET @Nombres = LTRIM(RTRIM(ISNULL(@Nombres, N'')));
    SET @Apellidos = LTRIM(RTRIM(ISNULL(@Apellidos, N'')));
    SET @NombresTutor = LTRIM(RTRIM(ISNULL(@NombresTutor, N'')));
    SET @ApellidosTutor = LTRIM(RTRIM(ISNULL(@ApellidosTutor, N'')));
    SET @TelefonoTutor = LTRIM(RTRIM(ISNULL(@TelefonoTutor, N'')));
    SET @DireccionTutor = LTRIM(RTRIM(ISNULL(@DireccionTutor, N'')));

    IF (@Nombres = N'' OR @Apellidos = N'')
    BEGIN
        SELECT 400 AS Codigo, N'Los nombres y apellidos del beneficiario son obligatorios.' AS Mensaje, N'Campos Nombres/Apellidos vacíos.' AS DetalleTecnico;
        RETURN;
    END

    IF (@FechaNacimiento IS NULL OR @FechaNacimiento > @Hoy OR @FechaNacimiento <= DATEADD(YEAR, -6, @Hoy))
    BEGIN
        SELECT 400 AS Codigo, N'La fecha de nacimiento es inválida. El beneficiario debe tener entre 0 y 5 años cumplidos.' AS Mensaje, N'FechaNacimiento fuera de rango (0-5 años).' AS DetalleTecnico;
        RETURN;
    END

    IF (@CentroId IS NULL OR NOT EXISTS (SELECT 1 FROM dbo.tblCentros WHERE CentroId = @CentroId AND Estado = 1))
    BEGIN
        SELECT 400 AS Codigo, N'Debe seleccionar un centro de atención válido y activo.' AS Mensaje, N'CentroId no existe o inactivo.' AS DetalleTecnico;
        RETURN;
    END

    IF (@DocumentoTutor = N'' OR @NombresTutor = N'' OR @ApellidosTutor = N'' OR @TelefonoTutor = N'' OR @DireccionTutor = N'')
    BEGIN
        SELECT 400 AS Codigo, N'Todos los datos del tutor (cédula, nombres, apellidos, teléfono y dirección) son obligatorios.' AS Mensaje, N'Campos del tutor incompletos.' AS DetalleTecnico;
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

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

            SET @TutorIdLocal = CONVERT(INT, SCOPE_IDENTITY());
        END
        ELSE
        BEGIN
            UPDATE dbo.tblTutores
            SET Nombres = @NombresTutor,
                Apellidos = @ApellidosTutor,
                Telefono = @TelefonoTutor,
                Direccion = @DireccionTutor
            WHERE TutorId = @TutorIdLocal;
        END

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

        SET @NuevoBeneficiarioId = CONVERT(INT, SCOPE_IDENTITY());

        COMMIT TRANSACTION;

        SELECT 
            200 AS Codigo,
            N'Beneficiario y Tutor vinculados satisfactoriamente (Expediente #' + CAST(@NuevoBeneficiarioId AS NVARCHAR(20)) + N').' AS Mensaje,
            CAST(@NuevoBeneficiarioId AS NVARCHAR(50)) AS DetalleTecnico;
    END TRY
    BEGIN CATCH
        IF (XACT_STATE() <> 0)
        BEGIN
            ROLLBACK TRANSACTION;
        END

        SELECT 
            500 AS Codigo,
            N'Error transaccional al registrar beneficiario: ' + ERROR_MESSAGE() AS Mensaje,
            ERROR_MESSAGE() AS DetalleTecnico;
    END CATCH
END
GO

-- -----------------------------------------------------------------------------
-- PROCEDIMIENTO: ppActualizarBeneficiario (Control de Concurrencia Optimista)
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.ppActualizarBeneficiario
    @BeneficiarioId INT,
    @Nombres NVARCHAR(100),
    @Apellidos NVARCHAR(100),
    @FechaNacimiento DATE,
    @CentroId INT,
    @TutorId INT = NULL,
    @FotografiaUrl NVARCHAR(500) = NULL,
    @VersionOriginal INT,
    @DocumentoTutor NVARCHAR(20) = NULL,
    @NombresTutor NVARCHAR(100) = NULL,
    @ApellidosTutor NVARCHAR(100) = NULL,
    @TelefonoTutor NVARCHAR(20) = NULL,
    @DireccionTutor NVARCHAR(250) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);
    SET @Nombres = LTRIM(RTRIM(ISNULL(@Nombres, N'')));
    SET @Apellidos = LTRIM(RTRIM(ISNULL(@Apellidos, N'')));

    IF (@Nombres = N'' OR @Apellidos = N'')
    BEGIN
        SELECT 400 AS Codigo, N'Los nombres y apellidos del beneficiario son obligatorios.' AS Mensaje, N'Nombres/Apellidos vacíos.' AS DetalleTecnico;
        RETURN;
    END

    IF (@FechaNacimiento IS NULL OR @FechaNacimiento > @Hoy OR @FechaNacimiento <= DATEADD(YEAR, -6, @Hoy))
    BEGIN
        SELECT 400 AS Codigo, N'La fecha de nacimiento es inválida. El beneficiario debe tener entre 0 y 5 años cumplidos.' AS Mensaje, N'FechaNacimiento fuera de rango (0-5 años).' AS DetalleTecnico;
        RETURN;
    END

    IF (@CentroId IS NULL OR NOT EXISTS (SELECT 1 FROM dbo.tblCentros WHERE CentroId = @CentroId AND Estado = 1))
    BEGIN
        SELECT 400 AS Codigo, N'El centro seleccionado no es válido.' AS Mensaje, N'CentroId inválido.' AS DetalleTecnico;
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        IF (@TutorId IS NULL OR @TutorId <= 0)
        BEGIN
            SELECT @TutorId = TutorId FROM dbo.tblBeneficiarios WHERE BeneficiarioId = @BeneficiarioId;
        END

        IF (@TutorId IS NOT NULL AND @TutorId > 0 AND NULLIF(LTRIM(RTRIM(@NombresTutor)), N'') IS NOT NULL)
        BEGIN
            SET @DocumentoTutor = REPLACE(LTRIM(RTRIM(ISNULL(@DocumentoTutor, N''))), N'-', N'');
            UPDATE dbo.tblTutores
            SET DocumentoIdentidad = CASE WHEN @DocumentoTutor <> N'' THEN @DocumentoTutor ELSE DocumentoIdentidad END,
                Nombres = LTRIM(RTRIM(@NombresTutor)),
                Apellidos = ISNULL(NULLIF(LTRIM(RTRIM(@ApellidosTutor)), N''), Apellidos),
                Telefono = ISNULL(NULLIF(LTRIM(RTRIM(@TelefonoTutor)), N''), Telefono),
                Direccion = ISNULL(NULLIF(LTRIM(RTRIM(@DireccionTutor)), N''), Direccion)
            WHERE TutorId = @TutorId;
        END

        UPDATE dbo.tblBeneficiarios
        SET 
            Nombres = @Nombres,
            Apellidos = @Apellidos,
            FechaNacimiento = @FechaNacimiento,
            CentroId = @CentroId,
            TutorId = ISNULL(@TutorId, TutorId),
            FotografiaUrl = @FotografiaUrl,
            Version = Version + 1
        WHERE BeneficiarioId = @BeneficiarioId 
          AND Version = @VersionOriginal;

        IF (@@ROWCOUNT = 0)
        BEGIN
            ROLLBACK TRANSACTION;

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
            N'Excepción al modificar datos del beneficiario: ' + ERROR_MESSAGE() AS Mensaje,
            ERROR_MESSAGE() AS DetalleTecnico;
    END CATCH
END
GO

-- -----------------------------------------------------------------------------
-- PROCEDIMIENTO: ppRegistrarAtencion (Idempotencia + Validación de PrecioFijo + ITBIS 18%)
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.ppRegistrarAtencion
    @TransaccionGuid UNIQUEIDENTIFIER,
    @BeneficiarioId INT,
    @CentroId INT,
    @Monto DECIMAL(18,2),
    @DescripcionServicio NVARCHAR(500),
    @FechaRegistro DATETIME,
    @ComponenteId INT = NULL,
    @UsuarioRegistroId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Comprobación de idempotencia ante reintentos de red
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

        -- 2. Validar existencia de Beneficiario y Centro
        IF NOT EXISTS (SELECT 1 FROM dbo.tblBeneficiarios WHERE BeneficiarioId = @BeneficiarioId AND Estado = 1)
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT 
                400 AS Codigo,
                N'El beneficiario especificado no existe o está inactivo en el sistema.' AS Mensaje,
                N'BeneficiarioId inválido.' AS DetalleTecnico;
            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM dbo.tblCentros WHERE CentroId = @CentroId AND Estado = 1)
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT 
                400 AS Codigo,
                N'El centro especificado no existe o está inactivo en el sistema.' AS Mensaje,
                N'CentroId inválido.' AS DetalleTecnico;
            RETURN;
        END

        -- 3. Resolver Componente de Atención (por ID o por nombre sin acentos) y validar PrecioFijo + ITBIS (18%)
        DECLARE @ComponenteResueltoId INT = NULL;
        DECLARE @PrecioFijoBase DECIMAL(18,2) = NULL;
        DECLARE @ItbisEsperado DECIMAL(18,2) = NULL;
        DECLARE @TotalEsperado DECIMAL(18,2) = NULL;
        DECLARE @DescNormalizada NVARCHAR(500) = LOWER(LTRIM(RTRIM(ISNULL(@DescripcionServicio, N''))) COLLATE Latin1_General_CI_AI);

        IF (@ComponenteId IS NOT NULL AND @ComponenteId > 0)
        BEGIN
            SELECT 
                @ComponenteResueltoId = ComponenteId,
                @PrecioFijoBase = PrecioFijo
            FROM dbo.tblComponentesAtencion
            WHERE ComponenteId = @ComponenteId;
        END

        IF (@ComponenteResueltoId IS NULL)
        BEGIN
            SELECT TOP 1
                @ComponenteResueltoId = ComponenteId,
                @PrecioFijoBase = PrecioFijo
            FROM dbo.tblComponentesAtencion
            WHERE LOWER(LTRIM(RTRIM(NombreComponente)) COLLATE Latin1_General_CI_AI) = @DescNormalizada
               OR LOWER(Codigo) = @DescNormalizada;
        END

        IF (@ComponenteResueltoId IS NULL OR @PrecioFijoBase IS NULL)
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT 
                400 AS Codigo,
                N'El servicio o componente de atención no pertenece al catálogo oficial del INAIPI.' AS Mensaje,
                N'Servicio no encontrado en tblComponentesAtencion: ' + ISNULL(@DescripcionServicio, N'NULL') AS DetalleTecnico;
            RETURN;
        END

        SET @ItbisEsperado = ROUND(@PrecioFijoBase * 0.18, 2);
        SET @TotalEsperado = @PrecioFijoBase + @ItbisEsperado;

        IF (@Monto <> @TotalEsperado)
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT 
                400 AS Codigo,
                N'Monto inválido para el servicio seleccionado. El total oficial (Precio Fijo + 18% ITBIS) es RD$ ' + CONVERT(NVARCHAR(30), @TotalEsperado) + N'.' AS Mensaje,
                N'Discrepancia de tarifa: Recibido=' + CONVERT(NVARCHAR(30), @Monto) + N' vs Esperado=' + CONVERT(NVARCHAR(30), @TotalEsperado) AS DetalleTecnico;
            RETURN;
        END

        -- 4. Insertar Atención con su desglose validado
        INSERT INTO dbo.tblAtenciones (
            TransaccionGuid, 
            BeneficiarioId, 
            CentroId, 
            ComponenteId,
            Subtotal,
            Itbis,
            Monto, 
            DescripcionServicio, 
            ServicioPrestado,
            UsuarioRegistroId,
            FechaRegistro
        )
        VALUES (
            @TransaccionGuid, 
            @BeneficiarioId, 
            @CentroId, 
            @ComponenteResueltoId,
            @PrecioFijoBase,
            @ItbisEsperado,
            @TotalEsperado, 
            @DescripcionServicio, 
            LEFT(@DescripcionServicio, 100),
            @UsuarioRegistroId,
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