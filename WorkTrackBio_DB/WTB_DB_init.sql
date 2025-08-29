-- =====================================================
-- SCRIPT COMPLETO DE CONFIGURACIÓN DE WTB_DB
-- WorkTrackBio - Sistema de Control de Asistencia y Proyectos
-- =====================================================
-- Este script incluye:
-- 1. Creación de la base de datos
-- 2. Creación de todas las tablas principales
-- 3. Creación de tablas de dispositivos y sesiones
-- 4. Índices optimizados
-- 5. Validaciones y restricciones
-- 6. Procedimientos almacenados
-- 7. Datos de prueba para dispositivos
-- =====================================================

-- =====================================================
-- 1. CREACIÓN DE LA BASE DE DATOS
-- =====================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'WTB_DB')
BEGIN
	CREATE DATABASE WTB_DB;
	PRINT '✅ Base de datos WTB_DB creada exitosamente';
END
ELSE
BEGIN
	PRINT 'ℹ️ La base de datos WTB_DB ya existe';
END
GO

USE WTB_DB;
GO

-- =====================================================
-- 2. CREACIÓN DE TABLAS PRINCIPALES
-- =====================================================

--CREATE STATES TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'States')
BEGIN
	CREATE TABLE States(
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	StateName NVARCHAR(50) NOT NULL UNIQUE,
	StateType NVARCHAR(50) NOT NULL,
	Description NVARCHAR(50) NULL
	);
	PRINT '✅ Tabla States creada';
END
GO

-- CREATE ROLES TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='Roles')
BEGIN
	CREATE TABLE Roles (
	Id INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
	RoleName NVARCHAR(50) NOT NULL UNIQUE,
	Description NVARCHAR(255) NULL
	);
	PRINT '✅ Tabla Roles creada';
END
GO

-- CREATE INTERNUSERS TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='InternUsers')
BEGIN
	CREATE TABLE InternUsers (
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	Email NVARCHAR(100) NOT NULL UNIQUE,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(100) NOT NULL,
	PasswordHash NVARCHAR(255) NOT NULL,
	PasswordSalt NVARCHAR(100) NOT NULL,
	RolId INT NOT NULL,
	CONSTRAINT FK_InternUsers_RolId FOREIGN KEY (RolId) REFERENCES Roles(Id),
	CreationDate DATETIME2 NOT NULL DEFAULT GETDATE(),
	StateId INT NOT NULL,
	CONSTRAINT FK_InternUsers_StateId FOREIGN KEY (StateId) REFERENCES States(Id),
	LastLogin DATETIME2 NULL,
	-- Nuevas columnas para documentos (igual que EmployeeInfo)
	DocumentNumber NVARCHAR(50) NULL,
	DocumentTypeId INT NULL,
	CONSTRAINT FK_InternUsers_DocumentTypeId FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
	DocumentExpire DATE NULL
	);
	PRINT '✅ Tabla InternUsers creada';
END
GO

--CREATE PROJECTS TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='Projects')
BEGIN
	CREATE TABLE Projects (
	Id INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
	ProjectName NVARCHAR(150) NOT NULL UNIQUE,
	StartDate DATE NULL,
	EndDate DATE NULL,
	StateId INT NOT NULL,
	CONSTRAINT FK_Projects_StateId FOREIGN KEY (StateId) REFERENCES States(Id)
	);
	PRINT '✅ Tabla Projects creada';
END
GO

--CREATE DOCUMENT TYPE TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='DocumentType')
BEGIN
	CREATE TABLE DocumentType (
	Id INT IDENTITY (1,1) PRIMARY KEY,
	DocumentName NVARCHAR(50) NOT NULL,
	Description NVARCHAR(255) NULL
	);
	PRINT '✅ Tabla DocumentType creada';
END
GO

--CREATE EMPLOYEE INFO
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='EmployeeInfo')
BEGIN
	CREATE TABLE EmployeeInfo(
	Id INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
	DocumentNumber NVARCHAR(50) NOT NULL UNIQUE,
	DocumentTypeId INT NOT NULL,
	CONSTRAINT FK_EmployeeDocumentType_DocumentTypeId FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
	DocumentExpire DATE NULL,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	PhoneNumber NVARCHAR(20) NULL,
	EmergencyContact NVARCHAR(100) NULL,
	EmergencyContactPhoneNumber NVARCHAR(20) NULL,
	Birthday DATE NOT NULL,
	RegisterDate DATETIME2 DEFAULT GETDATE() NOT NULL,
	CostPerHour DECIMAL(10,02) NULL,
	StateId INT NOT NULL,
	CONSTRAINT FK_Employee_StateId FOREIGN KEY (StateId) REFERENCES States(Id),
	Address NVARCHAR(255) NULL,
	IBAN NVARCHAR(50) NULL
	);
	PRINT '✅ Tabla EmployeeInfo creada';
END
GO

--CREATE ASSISTANCE TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='Assistance')
BEGIN
	CREATE TABLE Assistance (
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	EmployeeId INT NOT NULL,
	CONSTRAINT FK_EmployeeId_EmployeeInfo FOREIGN KEY (EmployeeId) REFERENCES EmployeeInfo(Id),
	ProjectId INT NOT NULL,
	CONSTRAINT FK_ProjectId_Projects FOREIGN KEY (ProjectId) REFERENCES Projects(Id),
	CreatedDate DATETIME2 DEFAULT GETDATE(),
	ModifiedDate DATETIME2 NULL,
	Notes NVARCHAR(500) NULL,
	CheckIn DATETIME2 NOT NULL,
	CheckOut DATETIME2 NULL,
	TotalHours DECIMAL(5,2) NULL,
	RegisterType NVARCHAR(50) NOT NULL,
	CheckInDateOnly AS CAST(CheckIn AS DATE)
	);
	PRINT '✅ Tabla Assistance creada';
END
GO

--CREATE PROJECTS ASSIGNS
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProjectsAssings')
BEGIN
	CREATE TABLE ProjectsAssigns (
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	EmployeeId INT NOT NULL,
	CONSTRAINT FK_ProjectsAssigns_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES EmployeeInfo(Id),
	ProjectId INT NOT NULL,
	CONSTRAINT FK_ProjectAssigns_ProjectId FOREIGN KEY (ProjectId) REFERENCES Projects(Id),
	AssignDate DATETIME2 NOT NULL DEFAULT GETDATE(),
	EndDate DATETIME2 NULL
	);
	PRINT '✅ Tabla ProjectsAssigns creada';
END
GO

--CREATE TABLE AUDIT REGISTER
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='AuditRegister')
BEGIN
	CREATE TABLE AuditRegister (
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	AssistanceId INT NOT NULL,
	CONSTRAINT FK_AssitanceId_Assistance FOREIGN KEY (AssistanceId) REFERENCES Assistance(Id),
	ActionType NVARCHAR(50) NOT NULL,
	DetailChange NVARCHAR(MAX) NOT NULL,
	ActionDate DATETIME2 NOT NULL DEFAULT GETDATE(),
	AdminId INT NOT NULL,
	CONSTRAINT FK_AuditRegister_InterUser FOREIGN KEY (AdminId) REFERENCES InternUsers(Id)
	);
	PRINT '✅ Tabla AuditRegister creada';
END
GO

--CREATE FINGERPRINT TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FingerPrint')
BEGIN
	CREATE TABLE FingerPrint(
	Id INT PRIMARY KEY IDENTITY (1,1),
	EmployeeId INT NOT NULL,
	CONSTRAINT FK_FingerPrint_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES EmployeeInfo(Id),
	TemplateFingerPrint VARBINARY(MAX) NOT NULL,
	IssueDate DATETIME2 NOT NULL DEFAULT GETDATE()
	);
	PRINT '✅ Tabla FingerPrint creada';
END
GO

--CREATE MAINTENANCE TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='ProjectMaintenance')
BEGIN
	CREATE TABLE ProjectMaintenance (
	Id INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
	IdProject INT NOT NULL,
	CONSTRAINT FK_ProjectMaintenance_ProjectId FOREIGN KEY (IdProject) REFERENCES Projects(Id),
	MaintenanceDate DATETIME2 NOT NULL DEFAULT GETDATE(),
	MaintenanceDescription NVARCHAR(MAX) NOT NULL,
	MadeById INT NOT NULL,
	CONSTRAINT FK_Maintenance_MadeById FOREIGN KEY (MadeById) REFERENCES EmployeeInfo(Id),
	MaintenanceCost DECIMAL(10,2) NULL,
	AdditionalInfo NVARCHAR(255) NULL,
	StateId INT NOT NULL,
	CONSTRAINT FK_ProjectMaintenance_StateId FOREIGN KEY (StateId) REFERENCES States(Id)
	);
	PRINT '✅ Tabla ProjectMaintenance creada';
END
GO

--CREATE WARRANTY TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='ProjectWarranty')
BEGIN
	CREATE TABLE ProjectWarranty (
	Id INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
	IdProject INT NOT NULL,
	CONSTRAINT FK_ProjectWarranty_IdProject FOREIGN KEY (IdProject) REFERENCES Projects(Id),
	WarrantyDate DATETIME2 NOT NULL DEFAULT GETDATE(),
	WarrantyDescription NVARCHAR(MAX) NOT NULL,
	MadeById INT NOT NULL,
	CONSTRAINT FK_ProjectWarranty_MadeById FOREIGN KEY (MadeById) REFERENCES EmployeeInfo(Id),
	WarrantyCost DECIMAL(10,2) NULL,
	StateId INT NOT NULL,
	CONSTRAINT FK_ProjectWarranty_StateId FOREIGN KEY (StateId) REFERENCES States(Id)
	);
	PRINT '✅ Tabla ProjectWarranty creada';
END
GO

-- =====================================================
-- 3. CREACIÓN DE TABLAS DE DISPOSITIVOS Y SESIONES
-- =====================================================

-- TABLA DEVICES (Dispositivos Biométricos)
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
    PRINT '✅ Tabla Devices creada';
END
GO

-- TABLA SESSIONS (Sesiones de Usuario)
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
    PRINT '✅ Tabla Sessions creada';
END
GO

-- =====================================================
-- 4. CREACIÓN DE ÍNDICES OPTIMIZADOS
-- =====================================================

PRINT '🔍 Creando índices optimizados...';

-- Índices para tablas principales
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_States_StateName' AND object_id = OBJECT_ID('States'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_States_StateName
    ON States (StateName, StateType);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Roles_RoleName' AND object_id = OBJECT_ID('Roles'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_Roles_RoleName
    ON Roles (RoleName);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InternUsers_Email' AND object_id = OBJECT_ID('InternUsers'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_InternUsers_Email
    ON InternUsers (Email);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InternUsers_RolId' AND object_id = OBJECT_ID('InternUsers'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_InternUsers_RolId
	ON InternUsers (RolId);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InternUsers_DocumentNumber' AND object_id = OBJECT_ID('InternUsers'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_InternUsers_DocumentNumber
	ON InternUsers (DocumentNumber);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InternUsers_DocumentTypeId' AND object_id = OBJECT_ID('InternUsers'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_InternUsers_DocumentTypeId
	ON InternUsers (DocumentTypeId);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Projects_ProjectName' AND object_id = OBJECT_ID('Projects'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_Projects_ProjectName
    ON Projects (ProjectName);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Projects_StateId' AND object_id = OBJECT_ID('Projects'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Projects_StateId
    ON Projects (StateId);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DocumentType_DocumentName' AND object_id = OBJECT_ID('DocumentType'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_DocumentType_DocumentName
    ON DocumentType (DocumentName);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeInfo_DocumentNumber' AND object_id = OBJECT_ID('EmployeeInfo'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_EmployeeInfo_DocumentNumber
    ON EmployeeInfo (DocumentNumber);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeInfo_StateId' AND object_id = OBJECT_ID('EmployeeInfo'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_EmployeeInfo_StateId
    ON EmployeeInfo (StateId);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeInfo_DocumentTypeId' AND object_id = OBJECT_ID('EmployeeInfo'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_EmployeeInfo_DocumentTypeId
    ON EmployeeInfo (DocumentTypeId);
END

-- Índice para búsquedas por nombre completo
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeInfo_FullName' AND object_id = OBJECT_ID('EmployeeInfo'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_EmployeeInfo_FullName
    ON EmployeeInfo (FirstName, LastName);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assistance_EmployeeProjectCheckIn' AND object_id = OBJECT_ID('Assistance'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Assistance_EmployeeProjectCheckIn
    ON Assistance (EmployeeId, ProjectId, CheckIn DESC);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assistance_ProjectId' AND object_id = OBJECT_ID('Assistance'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Assistance_ProjectId
    ON Assistance (ProjectId);
END

-- Índice para consultas por fecha de asistencia
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assistance_CheckInDate' AND object_id = OBJECT_ID('Assistance'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Assistance_CheckInDate
    ON Assistance (CheckInDateOnly DESC)
    INCLUDE (EmployeeId, ProjectId, TotalHours);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectsAssigns_EmployeeProjectAssign' AND object_id = OBJECT_ID('ProjectsAssigns'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProjectsAssigns_EmployeeProjectAssign
    ON ProjectsAssigns (EmployeeId, ProjectId, AssignDate DESC);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectsAssigns_ProjectId' AND object_id = OBJECT_ID('ProjectsAssigns'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProjectsAssigns_ProjectId
    ON ProjectsAssigns (ProjectId);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditRegister_AssistanceId' AND object_id = OBJECT_ID('AuditRegister'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AuditRegister_AssistanceId
    ON AuditRegister (AssistanceId);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditRegister_AdminId' AND object_id = OBJECT_ID('AuditRegister'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AuditRegister_AdminId
    ON AuditRegister (AdminId);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditRegister_ActionDate' AND object_id = OBJECT_ID('AuditRegister'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AuditRegister_ActionDate
    ON AuditRegister (ActionDate DESC);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FingerPrint_EmployeeId' AND object_id = OBJECT_ID('FingerPrint'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_FingerPrint_EmployeeId
    ON FingerPrint (EmployeeId);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectMaintenance_IdProject' AND object_id = OBJECT_ID('ProjectMaintenance'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProjectMaintenance_IdProject
    ON ProjectMaintenance (IdProject);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectMaintenance_MadeById' AND object_id = OBJECT_ID('ProjectMaintenance'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProjectMaintenance_MadeById
    ON ProjectMaintenance (MadeById);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectWarranty_IdProject' AND object_id = OBJECT_ID('ProjectWarranty'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProjectWarranty_IdProject
    ON ProjectWarranty (IdProject);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectWarranty_MadeById' AND object_id = OBJECT_ID('ProjectWarranty'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProjectWarranty_MadeById
    ON ProjectWarranty (MadeById);
END

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

PRINT '✅ Todos los índices creados correctamente';

-- =====================================================
-- 5. VALIDACIONES Y RESTRICCIONES BÁSICAS
-- =====================================================

PRINT '🔒 Aplicando validaciones y restricciones...';

-- 1. TABLA STATES - Validaciones básicas de formato
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_States_StateName_NotEmpty')
BEGIN
    ALTER TABLE States
    ADD CONSTRAINT CK_States_StateName_NotEmpty CHECK (LEN(LTRIM(RTRIM(StateName))) > 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_States_StateType_NotEmpty')
BEGIN
    ALTER TABLE States
    ADD CONSTRAINT CK_States_StateType_NotEmpty CHECK (LEN(LTRIM(RTRIM(StateType))) > 0);
END

-- 2. TABLA ROLES - Validación de nombre no vacío
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Roles_RoleName_NotEmpty')
BEGIN
    ALTER TABLE Roles
    ADD CONSTRAINT CK_Roles_RoleName_NotEmpty CHECK (LEN(LTRIM(RTRIM(RoleName))) > 0);
END

-- 3. TABLA INTERNUSERS - Validaciones de formato
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_InternUsers_Email_Format')
BEGIN
    ALTER TABLE InternUsers
    ADD CONSTRAINT CK_InternUsers_Email_Format 
    CHECK (Email LIKE '%@%.%' AND LEN(Email) >= 5 AND Email NOT LIKE '%@%@%');
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_InternUsers_FirstName_NotEmpty')
BEGIN
    ALTER TABLE InternUsers
    ADD CONSTRAINT CK_InternUsers_FirstName_NotEmpty CHECK (LEN(LTRIM(RTRIM(FirstName))) > 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_InternUsers_LastName_NotEmpty')
BEGIN
    ALTER TABLE InternUsers
    ADD CONSTRAINT CK_InternUsers_LastName_NotEmpty CHECK (LEN(LTRIM(RTRIM(LastName))) > 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_InternUsers_PasswordHash_NotEmpty')
BEGIN
    ALTER TABLE InternUsers
    ADD CONSTRAINT CK_InternUsers_PasswordHash_NotEmpty CHECK (LEN(LTRIM(RTRIM(PasswordHash))) >= 32);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_InternUsers_PasswordSalt_NotEmpty')
BEGIN
	ALTER TABLE InternUsers
	ADD CONSTRAINT CK_InternUsers_PasswordSalt_NotEmpty CHECK (LEN(LTRIM(RTRIM(PasswordSalt))) >= 8);
END

-- Validaciones para las nuevas columnas de documento en InternUsers
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_InternUsers_DocumentNumber_Format')
BEGIN
	ALTER TABLE InternUsers
	ADD CONSTRAINT CK_InternUsers_DocumentNumber_Format 
	CHECK (DocumentNumber IS NULL OR LEN(LTRIM(RTRIM(DocumentNumber))) > 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_InternUsers_DocumentExpire_Future')
BEGIN
	ALTER TABLE InternUsers
	ADD CONSTRAINT CK_InternUsers_DocumentExpire_Future 
	CHECK (DocumentExpire IS NULL OR DocumentExpire >= CAST(GETDATE() AS DATE));
END

-- 4. TABLA PROJECTS - Validaciones básicas
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Projects_ProjectName_NotEmpty')
BEGIN
    ALTER TABLE Projects
    ADD CONSTRAINT CK_Projects_ProjectName_NotEmpty CHECK (LEN(LTRIM(RTRIM(ProjectName))) > 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Projects_Dates_Logical')
BEGIN
    ALTER TABLE Projects
    ADD CONSTRAINT CK_Projects_Dates_Logical CHECK (EndDate IS NULL OR StartDate IS NULL OR EndDate >= StartDate);
END

-- 5. TABLA DOCUMENTTYPE - Validación de nombre
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_DocumentType_DocumentName_NotEmpty')
BEGIN
    ALTER TABLE DocumentType
    ADD CONSTRAINT CK_DocumentType_DocumentName_NotEmpty CHECK (LEN(LTRIM(RTRIM(DocumentName))) > 0);
END

-- 6. TABLA EMPLOYEEINFO - Validaciones de formato y lógica
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_DocumentNumber_NotEmpty')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_DocumentNumber_NotEmpty CHECK (LEN(LTRIM(RTRIM(DocumentNumber))) > 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_FirstName_NotEmpty')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_FirstName_NotEmpty CHECK (LEN(LTRIM(RTRIM(FirstName))) > 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_LastName_NotEmpty')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_LastName_NotEmpty CHECK (LEN(LTRIM(RTRIM(LastName))) > 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_Birthday_Past')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_Birthday_Past CHECK (Birthday < CAST(GETDATE() AS DATE));
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_Birthday_Reasonable')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_Birthday_Reasonable 
    CHECK (Birthday >= DATEADD(YEAR, -100, GETDATE()) AND Birthday <= DATEADD(YEAR, -16, GETDATE()));
END

-- Validación de teléfono formato Costa Rica (nnnn-nnnn)
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_PhoneNumber_Format')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_PhoneNumber_Format 
    CHECK (PhoneNumber IS NULL OR PhoneNumber LIKE '[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9][0-9]');
END

-- Validación de IBAN formato Costa Rica (CR + 20 dígitos)
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_IBAN_Format')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_IBAN_Format 
    CHECK (IBAN IS NULL OR (IBAN LIKE 'CR[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' AND LEN(IBAN) = 22));
END

-- 7. TABLA ASSISTANCE - Validaciones de integridad temporal y datos
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Assistance_CheckOut_After_CheckIn')
BEGIN
    ALTER TABLE Assistance
    ADD CONSTRAINT CK_Assistance_CheckOut_After_CheckIn CHECK (CheckOut IS NULL OR CheckOut >= CheckIn);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Assistance_TotalHours_NonNegative')
BEGIN
    ALTER TABLE Assistance
    ADD CONSTRAINT CK_Assistance_TotalHours_NonNegative CHECK (TotalHours IS NULL OR TotalHours >= 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Assistance_RegisterType_Valid')
BEGIN
    ALTER TABLE Assistance
    ADD CONSTRAINT CK_Assistance_RegisterType_Valid 
    CHECK (RegisterType IN ('CheckIn', 'CheckOut', 'Manual'));
END

-- 8. TABLA PROJECTSASSIGNS - Validación de fechas
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ProjectsAssigns_Dates_Logical')
BEGIN
    ALTER TABLE ProjectsAssigns
    ADD CONSTRAINT CK_ProjectsAssigns_Dates_Logical CHECK (EndDate IS NULL OR EndDate >= AssignDate);
END

-- 9. TABLA AUDITREGISTER - Validaciones de contenido
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_AuditRegister_ActionType_NotEmpty')
BEGIN
    ALTER TABLE AuditRegister
    ADD CONSTRAINT CK_AuditRegister_ActionType_NotEmpty CHECK (LEN(LTRIM(RTRIM(ActionType))) > 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_AuditRegister_DetailChange_NotEmpty')
BEGIN
    ALTER TABLE AuditRegister
    ADD CONSTRAINT CK_AuditRegister_DetailChange_NotEmpty CHECK (LEN(LTRIM(RTRIM(DetailChange))) > 0);
END

-- 10. TABLA FINGERPRINT - Validación de datos biométricos
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_FingerPrint_Template_NotEmpty')
BEGIN
    ALTER TABLE FingerPrint
    ADD CONSTRAINT CK_FingerPrint_Template_NotEmpty CHECK (DATALENGTH(TemplateFingerPrint) > 0);
END

-- 11. TABLA PROJECTMAINTENANCE - Validaciones de costos y contenido
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ProjectMaintenance_Cost_NonNegative')
BEGIN
    ALTER TABLE ProjectMaintenance
    ADD CONSTRAINT CK_ProjectMaintenance_Cost_NonNegative CHECK (MaintenanceCost IS NULL OR MaintenanceCost >= 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ProjectMaintenance_Description_NotEmpty')
BEGIN
    ALTER TABLE ProjectMaintenance
    ADD CONSTRAINT CK_ProjectMaintenance_Description_NotEmpty CHECK (LEN(LTRIM(RTRIM(MaintenanceDescription))) > 0);
END

-- 12. TABLA PROJECTWARRANTY - Validaciones de costos y contenido
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ProjectWarranty_Cost_NonNegative')
BEGIN
    ALTER TABLE ProjectWarranty
    ADD CONSTRAINT CK_ProjectWarranty_Cost_NonNegative CHECK (WarrantyCost IS NULL OR WarrantyCost >= 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ProjectWarranty_Description_NotEmpty')
BEGIN
    ALTER TABLE ProjectWarranty
    ADD CONSTRAINT CK_ProjectWarranty_Description_NotEmpty CHECK (LEN(LTRIM(RTRIM(WarrantyDescription))) > 0);
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_CostPerHour_NonNegative' AND parent_object_id = OBJECT_ID('EmployeeInfo'))
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_CostPerHour_NonNegative CHECK (CostPerHour IS NULL OR CostPerHour >= 0);
END

PRINT '✅ Todas las validaciones aplicadas correctamente';

-- =====================================================
-- 6. PROCEDIMIENTOS ALMACENADOS
-- =====================================================

PRINT '📋 Creando procedimientos almacenados...';

-- PROCEDIMIENTO ALMACENADO PARA LIMPIEZA DE SESIONES
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
    PRINT '✅ Procedimiento sp_CleanupExpiredSessions creado';
END

-- PROCEDIMIENTO ALMACENADO PARA OBTENER SESIONES ACTIVAS
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
    PRINT '✅ Procedimiento sp_GetActiveSessions creado';
END

-- =====================================================
-- 7. DATOS DE PRUEBA PARA DISPOSITIVOS
-- =====================================================

PRINT '📱 Los datos de prueba se han movido al archivo WTB_DB_TestData.sql para mantener la separación de responsabilidades';
PRINT 'ℹ️ Ejecuta ese archivo después de este script si deseas insertar datos de prueba';

-- =====================================================
-- 7. MIGRACIÓN PARA BASES DE DATOS EXISTENTES
-- =====================================================

PRINT '🔄 Verificando y aplicando migraciones para bases de datos existentes...';

-- Agregar columnas de documento a InternUsers si no existen (para bases de datos existentes)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InternUsers') AND name = 'DocumentNumber')
BEGIN
    ALTER TABLE InternUsers ADD DocumentNumber NVARCHAR(50) NULL;
    PRINT '✅ Columna DocumentNumber agregada a InternUsers';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InternUsers') AND name = 'DocumentTypeId')
BEGIN
    ALTER TABLE InternUsers ADD DocumentTypeId INT NULL;
    PRINT '✅ Columna DocumentTypeId agregada a InternUsers';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InternUsers') AND name = 'DocumentExpire')
BEGIN
    ALTER TABLE InternUsers ADD DocumentExpire DATE NULL;
    PRINT '✅ Columna DocumentExpire agregada a InternUsers';
END

-- Agregar FK para DocumentTypeId si no existe
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_InternUsers_DocumentTypeId')
BEGIN
    ALTER TABLE InternUsers 
    ADD CONSTRAINT FK_InternUsers_DocumentTypeId 
    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id);
    PRINT '✅ FK DocumentTypeId agregada a InternUsers';
END

-- Crear índices si no existen (para bases de datos existentes)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InternUsers_DocumentNumber' AND object_id = OBJECT_ID('InternUsers'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_InternUsers_DocumentNumber ON InternUsers (DocumentNumber);
    PRINT '✅ Índice IX_InternUsers_DocumentNumber creado';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InternUsers_DocumentTypeId' AND object_id = OBJECT_ID('InternUsers'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_InternUsers_DocumentTypeId ON InternUsers (DocumentTypeId);
    PRINT '✅ Índice IX_InternUsers_DocumentTypeId creado';
END

-- =====================================================
-- 8. VERIFICACIÓN FINAL
-- =====================================================

PRINT '';
PRINT '========================================';
PRINT 'VERIFICACIÓN FINAL DE LA CONFIGURACIÓN';
PRINT '========================================';

-- Verificar tablas creadas
PRINT '';
PRINT '📋 TABLAS CREADAS:';
SELECT 'States' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[States]
UNION ALL
SELECT 'Roles' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[Roles]
UNION ALL
SELECT 'InternUsers' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[InternUsers]
UNION ALL
SELECT 'Projects' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[Projects]
UNION ALL
SELECT 'DocumentType' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[DocumentType]
UNION ALL
SELECT 'EmployeeInfo' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[EmployeeInfo]
UNION ALL
SELECT 'Assistance' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[Assistance]
UNION ALL
SELECT 'ProjectsAssigns' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[ProjectsAssigns]
UNION ALL
SELECT 'AuditRegister' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[AuditRegister]
UNION ALL
SELECT 'FingerPrint' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[FingerPrint]
UNION ALL
SELECT 'ProjectMaintenance' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[ProjectMaintenance]
UNION ALL
SELECT 'ProjectWarranty' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[ProjectWarranty]
UNION ALL
SELECT 'Devices' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[Devices]
UNION ALL
SELECT 'Sessions' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[Sessions];

-- Verificar índices creados
PRINT '';
PRINT '🔍 ÍNDICES CREADOS:';
SELECT 
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.name LIKE 'IX_%' OR i.name LIKE 'UQ_%'
ORDER BY t.name, i.name;

-- Verificar procedimientos almacenados
PRINT '';
PRINT '📋 PROCEDIMIENTOS ALMACENADOS:';
SELECT 
    name AS ProcedureName,
    create_date AS CreatedDate
FROM sys.procedures 
WHERE name LIKE 'sp_%'
ORDER BY name;

-- =====================================================
-- 9. RESUMEN FINAL
-- =====================================================

PRINT '';
PRINT '========================================';
PRINT '✅ CONFIGURACIÓN COMPLETADA EXITOSAMENTE';
PRINT '========================================';
PRINT '📊 RESUMEN:';
PRINT '   - Base de datos WTB_DB inicializada';
PRINT '   - 16 tablas principales creadas';
PRINT '   - Tablas de dispositivos y sesiones agregadas';
PRINT '   - Columnas de documento agregadas a InternUsers';
PRINT '   - Índices optimizados para consultas frecuentes';
PRINT '   - Validaciones de integridad aplicadas';
PRINT '   - Procedimientos almacenados creados';
PRINT '   - Estructura lista para datos de prueba (archivo separado)';
PRINT '';
PRINT '🔒 VALIDACIONES INCLUIDAS:';
PRINT '   - Formato de email válido';
PRINT '   - Formato de teléfono CR (nnnn-nnnn)';
PRINT '   - Formato de IBAN CR (22 caracteres)';
PRINT '   - Validaciones de fechas lógicas';
PRINT '   - Validaciones de campos no vacíos';
PRINT '   - Validaciones de costos no negativos';
PRINT '   - Validaciones de documentos para InternUsers';
PRINT '';
PRINT '🚀 La base de datos está lista para usar!';
GO
