USE [INAIPI_Core];
GO

UPDATE dbo.tblUsuarios
SET PasswordHash = LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'AdminSeguro2026*'), 2)),
    Estado = 1
WHERE Username = 'admin';
GO

-- Comprobación del valor actualizado:
SELECT UsuarioId, Username, PasswordHash, Estado 
FROM dbo.tblUsuarios 
WHERE Username = 'admin';
GO