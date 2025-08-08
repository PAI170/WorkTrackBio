-- =====================================================
-- NUEVAS TABLAS PARA WTB_DB
-- Devices y Sessions
-- =====================================================

USE WTB_DB;
GO

-- =====================================================
-- TABLA DEVICES (Dispositivos Biométricos)
-- =====================================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Devices]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Devices] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [DeviceId] NVARCHAR(50) NOT NULL,                    -- Identificador único del dispositivo
        [DeviceName] NVARCHAR(100) NOT NULL,                 -- Nombre descriptivo del dispositivo
        [ProjectId] INT NOT NULL,                            -- Proyecto al que está asignado
        [Location] NVARCHAR(200) NULL,                       -- Ubicación física del dispositivo
        [DeviceType] NVARCHAR(50) NOT NULL DEFAULT 'Fingerprint', -- Tipo: Fingerprint, RFID, Facial, etc.
        [IpAddress] NVARCHAR(100) NULL,                      -- Dirección IP del dispositivo
        [SerialNumber] NVARCHAR(20) NULL,                    -- Número de serie del hardware
        [FirmwareVersion] NVARCHAR(50) NULL,                 -- Versión del firmware
        [Notes] NVARCHAR(500) NULL,                          -- Notas adicionales
        [IsActive] BIT NOT NULL DEFAULT 1,                   -- Estado activo/inactivo
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [LastActivity] DATETIME2 NULL,                       -- Última actividad registrada
        
        CONSTRAINT [PK_Devices] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Devices_Projects] FOREIGN KEY ([ProjectId]) 
            REFERENCES [dbo].[Projects] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [UQ_Devices_DeviceId] UNIQUE ([DeviceId]),
        CONSTRAINT [CK_Devices_DeviceType] CHECK ([DeviceType] IN ('Fingerprint', 'RFID', 'Facial', 'Card', 'Other'))
    );
    
    PRINT 'Tabla Devices creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La tabla Devices ya existe';
END
GO

-- =====================================================
-- TABLA SESSIONS (Sesiones de Usuario)
-- =====================================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sessions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Sessions] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,                               -- Usuario de la sesión
        [TokenId] NVARCHAR(255) NOT NULL,                    -- ID único del JWT token
        [RefreshToken] NVARCHAR(255) NOT NULL,               -- Token de refresco
        [DeviceInfo] NVARCHAR(500) NULL,                     -- Información del dispositivo (Browser, OS)
        [IpAddress] NVARCHAR(45) NULL,                       -- Dirección IP del cliente
        [UserAgent] NVARCHAR(500) NULL,                      -- User Agent del navegador
        [IsActive] BIT NOT NULL DEFAULT 1,                   -- Sesión activa/inactiva
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ExpiresAt] DATETIME2 NOT NULL,                      -- Fecha de expiración del token
        [LastActivity] DATETIME2 NULL,                       -- Última actividad en la sesión
        [LoggedOutAt] DATETIME2 NULL,                        -- Fecha de logout (si aplica)
        
        CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Sessions_InternUsers] FOREIGN KEY ([UserId]) 
            REFERENCES [dbo].[InternUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [UQ_Sessions_TokenId] UNIQUE ([TokenId]),
        CONSTRAINT [UQ_Sessions_RefreshToken] UNIQUE ([RefreshToken]),
        CONSTRAINT [CK_Sessions_ExpiresAt] CHECK ([ExpiresAt] > [CreatedAt])
    );
    
    PRINT 'Tabla Sessions creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La tabla Sessions ya existe';
END
GO

-- =====================================================
-- ÍNDICES PARA OPTIMIZACIÓN
-- =====================================================

-- Índices para Devices
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Devices_ProjectId')
    CREATE INDEX [IX_Devices_ProjectId] ON [dbo].[Devices] ([ProjectId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Devices_IsActive')
    CREATE INDEX [IX_Devices_IsActive] ON [dbo].[Devices] ([IsActive]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Devices_DeviceType')
    CREATE INDEX [IX_Devices_DeviceType] ON [dbo].[Devices] ([DeviceType]);

-- Índices para Sessions
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Sessions_UserId')
    CREATE INDEX [IX_Sessions_UserId] ON [dbo].[Sessions] ([UserId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Sessions_IsActive')
    CREATE INDEX [IX_Sessions_IsActive] ON [dbo].[Sessions] ([IsActive]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Sessions_ExpiresAt')
    CREATE INDEX [IX_Sessions_ExpiresAt] ON [dbo].[Sessions] ([ExpiresAt]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Sessions_LastActivity')
    CREATE INDEX [IX_Sessions_LastActivity] ON [dbo].[Sessions] ([LastActivity]);

GO

-- =====================================================
-- DATOS DE PRUEBA PARA DEVICES
-- =====================================================

-- Insertar dispositivos de prueba
IF NOT EXISTS (SELECT * FROM [dbo].[Devices] WHERE [DeviceId] = 'DEV001')
BEGIN
    INSERT INTO [dbo].[Devices] ([DeviceId], [DeviceName], [ProjectId], [Location], [DeviceType], [IpAddress], [SerialNumber], [FirmwareVersion], [Notes])
    VALUES 
        ('DEV001', 'Lector Principal - Edificio A', 1, 'Entrada Principal - Edificio A', 'Fingerprint', '192.168.1.100', 'SN001234567', 'v2.1.5', 'Dispositivo principal para empleados'),
        ('DEV002', 'Lector Secundario - Edificio B', 2, 'Entrada Secundaria - Edificio B', 'Fingerprint', '192.168.1.101', 'SN001234568', 'v2.1.5', 'Dispositivo para visitantes'),
        ('DEV003', 'Lector RFID - Estacionamiento', 1, 'Entrada Estacionamiento', 'RFID', '192.168.1.102', 'SN001234569', 'v1.8.2', 'Control de acceso vehicular'),
        ('DEV004', 'Lector Facial - Recepción', 3, 'Recepción Principal', 'Facial', '192.168.1.103', 'SN001234570', 'v3.0.1', 'Identificación facial para ejecutivos'),
        ('DEV005', 'Lector Backup - Almacén', 2, 'Entrada Almacén', 'Fingerprint', '192.168.1.104', 'SN001234571', 'v2.1.5', 'Dispositivo de respaldo');
    
    PRINT 'Datos de prueba para Devices insertados';
END
ELSE
BEGIN
    PRINT 'Los datos de prueba para Devices ya existen';
END
GO

-- =====================================================
-- PROCEDIMIENTO ALMACENADO PARA LIMPIEZA DE SESIONES
-- =====================================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CleanupExpiredSessions]') AND type in (N'P'))
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[sp_CleanupExpiredSessions]
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Marcar sesiones expiradas como inactivas
        UPDATE [dbo].[Sessions]
        SET [IsActive] = 0,
            [LoggedOutAt] = GETUTCDATE()
        WHERE [ExpiresAt] < GETUTCDATE()
        AND [IsActive] = 1;
        
        -- Opcional: Eliminar sesiones muy antiguas (más de 30 días)
        DELETE FROM [dbo].[Sessions]
        WHERE [CreatedAt] < DATEADD(DAY, -30, GETUTCDATE())
        AND [IsActive] = 0;
        
        PRINT ''Limpieza de sesiones completada'';
    END
    ');
    
    PRINT 'Procedimiento sp_CleanupExpiredSessions creado';
END
ELSE
BEGIN
    PRINT 'El procedimiento sp_CleanupExpiredSessions ya existe';
END
GO

-- =====================================================
-- PROCEDIMIENTO ALMACENADO PARA OBTENER SESIONES ACTIVAS
-- =====================================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetActiveSessions]') AND type in (N'P'))
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[sp_GetActiveSessions]
        @UserId INT = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT 
            s.[Id],
            s.[UserId],
            u.[Email],
            u.[FirstName] + '' '' + u.[LastName] AS [FullName],
            s.[DeviceInfo],
            s.[IpAddress],
            s.[CreatedAt],
            s.[ExpiresAt],
            s.[LastActivity],
            DATEDIFF(MINUTE, s.[LastActivity], GETUTCDATE()) AS [MinutesInactive]
        FROM [dbo].[Sessions] s
        INNER JOIN [dbo].[InternUsers] u ON s.[UserId] = u.[Id]
        WHERE s.[IsActive] = 1
        AND s.[ExpiresAt] > GETUTCDATE()
        AND (@UserId IS NULL OR s.[UserId] = @UserId)
        ORDER BY s.[LastActivity] DESC;
    END
    ');
    
    PRINT 'Procedimiento sp_GetActiveSessions creado';
END
ELSE
BEGIN
    PRINT 'El procedimiento sp_GetActiveSessions ya existe';
END
GO

-- =====================================================
-- VERIFICACIÓN FINAL
-- =====================================================

PRINT '=== VERIFICACIÓN DE TABLAS CREADAS ===';
SELECT 'Devices' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[Devices]
UNION ALL
SELECT 'Sessions' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[Sessions];

PRINT '=== VERIFICACIÓN DE ÍNDICES ===';
SELECT 
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name IN ('Devices', 'Sessions')
AND i.name LIKE 'IX_%'
ORDER BY t.name, i.name;

PRINT 'Script completado exitosamente!';
GO
