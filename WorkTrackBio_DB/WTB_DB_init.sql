-- =====================================================
-- SCRIPT COMPLETO DE CONFIGURACIÓN DE WTB_DB
-- WorkTrackBio - Sistema de Control de Asistencia y Proyectos
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

-- CREATE STATES TABLE
-- Nota: StateType controla qué entidad puede usar cada estado
-- Valores válidos de StateType: 'Person', 'Project'
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'States')
BEGIN
	CREATE TABLE States(
	Id          INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	StateName   NVARCHAR(50) NOT NULL UNIQUE,
	StateType   NVARCHAR(50) NOT NULL,               -- 'Person' o 'Project'
	Description NVARCHAR(50) NULL,
	CreatedAt   DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
	UpdatedAt   DATETIME2 NULL
	);
	PRINT '✅ Tabla States creada';
END
GO

-- CREATE ROLES TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='Roles')
BEGIN
	CREATE TABLE Roles (
	Id          INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
	RoleName    NVARCHAR(50) NOT NULL UNIQUE,
	Description NVARCHAR(255) NULL,
	CreatedAt   DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
	UpdatedAt   DATETIME2 NULL
	);
	PRINT '✅ Tabla Roles creada';
END
GO

-- ✅ FIX 1: DocumentType se crea ANTES que InternUsers
-- CREATE DOCUMENT TYPE TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='DocumentType')
BEGIN
	CREATE TABLE DocumentType (
	Id           INT IDENTITY (1,1) PRIMARY KEY,
	DocumentName NVARCHAR(50) NOT NULL,
	Description  NVARCHAR(255) NULL,
	CreatedAt    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
	UpdatedAt    DATETIME2 NULL
	);
	PRINT '✅ Tabla DocumentType creada';
END
GO

-- CREATE INTERNUSERS TABLE
-- (Ahora puede referenciar DocumentType sin problema)
-- StateType permitido: 'Person'
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='InternUsers')
BEGIN
	CREATE TABLE InternUsers (
	Id             INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	Email          NVARCHAR(100) NOT NULL UNIQUE,
	FirstName      NVARCHAR(50) NOT NULL,
	LastName       NVARCHAR(100) NOT NULL,
	PasswordHash   NVARCHAR(255) NOT NULL,
	PasswordSalt   NVARCHAR(100) NOT NULL,
	RolId          INT NOT NULL,
	CONSTRAINT FK_InternUsers_RolId FOREIGN KEY (RolId) REFERENCES Roles(Id),
	CreatedAt      DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
	UpdatedAt      DATETIME2 NULL,                            -- ✅ FIX 2: UpdatedAt
	StateId        INT NOT NULL,
	CONSTRAINT FK_InternUsers_StateId FOREIGN KEY (StateId) REFERENCES States(Id),
	LastLogin      DATETIME2 NULL,
	DocumentNumber NVARCHAR(50) NULL,
	DocumentTypeId INT NULL,
	CONSTRAINT FK_InternUsers_DocumentTypeId FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
	DocumentExpire DATE NULL
	);
	PRINT '✅ Tabla InternUsers creada';
END
GO

-- CREATE PROJECTS TABLE
-- StateType permitido: 'Project'
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='Projects')
BEGIN
	CREATE TABLE Projects (
	Id          INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
	ProjectName NVARCHAR(150) NOT NULL UNIQUE,
	StartDate   DATE NULL,
	EndDate     DATE NULL,
	StateId     INT NOT NULL,
	CONSTRAINT FK_Projects_StateId FOREIGN KEY (StateId) REFERENCES States(Id),
	CreatedAt   DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
	UpdatedAt   DATETIME2 NULL                             -- ✅ FIX 2: UpdatedAt
	);
	PRINT '✅ Tabla Projects creada';
END
GO

-- CREATE EMPLOYEE INFO TABLE
-- StateType permitido: 'Person'
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='EmployeeInfo')
BEGIN
	CREATE TABLE EmployeeInfo(
	Id                           INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
	DocumentNumber               NVARCHAR(50) NOT NULL UNIQUE,
	DocumentTypeId               INT NOT NULL,
	CONSTRAINT FK_EmployeeDocumentType_DocumentTypeId FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id),
	DocumentExpire               DATE NULL,
	FirstName                    NVARCHAR(50) NOT NULL,
	LastName                     NVARCHAR(50) NOT NULL,
	PhoneNumber                  NVARCHAR(20) NULL,
	EmergencyContact             NVARCHAR(100) NULL,
	EmergencyContactPhoneNumber  NVARCHAR(20) NULL,
	Birthday                     DATE NOT NULL,
	CostPerHour                  DECIMAL(10,02) NULL,
	StateId                      INT NOT NULL,
	CONSTRAINT FK_Employee_StateId FOREIGN KEY (StateId) REFERENCES States(Id),
	Address                      NVARCHAR(255) NULL,
	IBAN                         NVARCHAR(50) NULL,
	CreatedAt                    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
	UpdatedAt                    DATETIME2 NULL                             -- ✅ FIX 2: UpdatedAt
	);
	PRINT '✅ Tabla EmployeeInfo creada';
END
GO

-- CREATE ASSISTANCE TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='Assistance')
BEGIN
	CREATE TABLE Assistance (
	Id             INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	EmployeeId     INT NOT NULL,
	CONSTRAINT FK_EmployeeId_EmployeeInfo FOREIGN KEY (EmployeeId) REFERENCES EmployeeInfo(Id),
	ProjectId      INT NOT NULL,
	CONSTRAINT FK_ProjectId_Projects FOREIGN KEY (ProjectId) REFERENCES Projects(Id),
	Notes          NVARCHAR(500) NULL,
	CheckIn        DATETIME2 NOT NULL,
	CheckOut       DATETIME2 NULL,
	TotalHours     DECIMAL(5,2) NULL,
	RegisterType   NVARCHAR(50) NOT NULL,
	CheckInDateOnly AS CAST(CheckIn AS DATE),
	CreatedAt      DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
	UpdatedAt      DATETIME2 NULL                             -- ✅ FIX 2: UpdatedAt
	);
	PRINT '✅ Tabla Assistance creada';
END
GO

-- CREATE PROJECTS ASSIGNS TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProjectsAssings')
BEGIN
	CREATE TABLE ProjectsAssigns (
	Id         INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	EmployeeId INT NOT NULL,
	CONSTRAINT FK_ProjectsAssigns_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES EmployeeInfo(Id),
	ProjectId  INT NOT NULL,
	CONSTRAINT FK_ProjectAssigns_ProjectId FOREIGN KEY (ProjectId) REFERENCES Projects(Id),
	AssignDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
	EndDate    DATETIME2 NULL,
	CreatedAt  DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
	UpdatedAt  DATETIME2 NULL                             -- ✅ FIX 2: UpdatedAt
	);
	PRINT '✅ Tabla ProjectsAssigns creada';
END
GO

-- CREATE AUDIT REGISTER TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='AuditRegister')
BEGIN
	CREATE TABLE AuditRegister (
	Id           INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	AssistanceId INT NOT NULL,
	CONSTRAINT FK_AssitanceId_Assistance FOREIGN KEY (AssistanceId) REFERENCES Assistance(Id),
	ActionType   NVARCHAR(50) NOT NULL,
	DetailChange NVARCHAR(MAX) NOT NULL,
	ActionDate   DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
	AdminId      INT NOT NULL,
	CONSTRAINT FK_AuditRegister_InterUser FOREIGN KEY (AdminId) REFERENCES InternUsers(Id),
	CreatedAt    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
	UpdatedAt    DATETIME2 NULL                             -- ✅ FIX 2: UpdatedAt
	);
	PRINT '✅ Tabla AuditRegister creada';
END
GO

-- CREATE FINGERPRINT TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FingerPrint')
BEGIN
	CREATE TABLE FingerPrint(
	Id                  INT PRIMARY KEY IDENTITY (1,1),
	EmployeeId          INT NOT NULL,
	CONSTRAINT FK_FingerPrint_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES EmployeeInfo(Id),
	TemplateFingerPrint VARBINARY(MAX) NOT NULL,
	CreatedAt           DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
	UpdatedAt           DATETIME2 NULL                             -- ✅ FIX 2: UpdatedAt
	);
	PRINT '✅ Tabla FingerPrint creada';
END
GO

-- CREATE MAINTENANCE TABLE
-- StateType permitido: 'Project'
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='ProjectMaintenance')
BEGIN
	CREATE TABLE ProjectMaintenance (
	Id                      INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
	IdProject               INT NOT NULL,
	CONSTRAINT FK_ProjectMaintenance_ProjectId FOREIGN KEY (IdProject) REFERENCES Projects(Id),
	MaintenanceDate         DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
	MaintenanceDescription  NVARCHAR(MAX) NOT NULL,
	MadeById                INT NOT NULL,
	CONSTRAINT FK_Maintenance_MadeById FOREIGN KEY (MadeById) REFERENCES EmployeeInfo(Id),
	MaintenanceCost         DECIMAL(10,2) NULL,
	AdditionalInfo          NVARCHAR(255) NULL,
	StateId                 INT NOT NULL,
	CONSTRAINT FK_ProjectMaintenance_StateId FOREIGN KEY (StateId) REFERENCES States(Id),
	CreatedAt               DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
	UpdatedAt               DATETIME2 NULL                             -- ✅ FIX 2: UpdatedAt
	);
	PRINT '✅ Tabla ProjectMaintenance creada';
END
GO

-- CREATE WARRANTY TABLE
-- StateType permitido: 'Project'
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name ='ProjectWarranty')
BEGIN
	CREATE TABLE ProjectWarranty (
	Id                   INT PRIMARY KEY IDENTITY (1,1) NOT NULL,
	IdProject            INT NOT NULL,
	CONSTRAINT FK_ProjectWarranty_IdProject FOREIGN KEY (IdProject) REFERENCES Projects(Id),
	WarrantyDate         DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
	WarrantyDescription  NVARCHAR(MAX) NOT NULL,
	MadeById             INT NOT NULL,
	CONSTRAINT FK_ProjectWarranty_MadeById FOREIGN KEY (MadeById) REFERENCES EmployeeInfo(Id),
	WarrantyCost         DECIMAL(10,2) NULL,
	StateId              INT NOT NULL,
	CONSTRAINT FK_ProjectWarranty_StateId FOREIGN KEY (StateId) REFERENCES States(Id),
	CreatedAt            DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
	UpdatedAt            DATETIME2 NULL                             -- ✅ FIX 2: UpdatedAt
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
        [Id]              INT IDENTITY(1,1) NOT NULL,
        [DeviceId]        NVARCHAR(50) NOT NULL,
        [DeviceName]      NVARCHAR(100) NOT NULL,
        [ProjectId]       INT NOT NULL,
        [Location]        NVARCHAR(200) NULL,
        [DeviceType]      NVARCHAR(50) NOT NULL DEFAULT 'Fingerprint',
        [IpAddress]       NVARCHAR(100) NULL,
        [SerialNumber]    NVARCHAR(20) NULL,
        [FirmwareVersion] NVARCHAR(50) NULL,
        [Notes]           NVARCHAR(500) NULL,
        [IsActive]        BIT NOT NULL DEFAULT 1,
        [CreatedAt]       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),   -- ✅ FIX 3: GETUTCDATE()
        [UpdatedAt]       DATETIME2 NULL,                            -- ✅ FIX 2: UpdatedAt
        [LastActivity]    DATETIME2 NULL,
        
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
        [Id]           INT IDENTITY(1,1) NOT NULL,
        [UserId]       INT NOT NULL,
        [TokenId]      NVARCHAR(255) NOT NULL,
        [RefreshToken] NVARCHAR(255) NOT NULL,
        [DeviceInfo]   NVARCHAR(500) NULL,
        [IpAddress]    NVARCHAR(45) NULL,
        [UserAgent]    NVARCHAR(500) NULL,
        [IsActive]     BIT NOT NULL DEFAULT 1,
        [CreatedAt]    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt]    DATETIME2 NULL,                               -- ✅ FIX 2: UpdatedAt
        [ExpiresAt]    DATETIME2 NOT NULL,
        [LastActivity] DATETIME2 NULL,
        [LoggedOutAt]  DATETIME2 NULL,
        
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
-- 4. ESTADOS BASE (DATOS INICIALES)
-- ✅ FIX 4: Se insertan automáticamente los estados
--    para no depender del archivo de mock data
-- =====================================================

PRINT '📋 Insertando estados base...';

-- Estados de tipo Person (para EmployeeInfo e InternUsers)
IF NOT EXISTS (SELECT * FROM States WHERE StateName = 'Activo' AND StateType = 'Person')
    INSERT INTO States (StateName, StateType, Description) VALUES ('Activo', 'Person', 'Persona activa en el sistema');

IF NOT EXISTS (SELECT * FROM States WHERE StateName = 'Inactivo' AND StateType = 'Person')
    INSERT INTO States (StateName, StateType, Description) VALUES ('Inactivo', 'Person', 'Persona inactiva en el sistema');

-- Estados de tipo Project (para Projects, ProjectMaintenance, ProjectWarranty)
IF NOT EXISTS (SELECT * FROM States WHERE StateName = 'En progreso' AND StateType = 'Project')
    INSERT INTO States (StateName, StateType, Description) VALUES ('En progreso', 'Project', 'Proyecto en ejecución');

IF NOT EXISTS (SELECT * FROM States WHERE StateName = 'Completado' AND StateType = 'Project')
    INSERT INTO States (StateName, StateType, Description) VALUES ('Completado', 'Project', 'Proyecto finalizado exitosamente');

IF NOT EXISTS (SELECT * FROM States WHERE StateName = 'Cancelado' AND StateType = 'Project')
    INSERT INTO States (StateName, StateType, Description) VALUES ('Cancelado', 'Project', 'Proyecto cancelado');

PRINT '✅ Estados base insertados correctamente';
GO

-- =====================================================
-- 5. CREACIÓN DE ÍNDICES OPTIMIZADOS
-- =====================================================

PRINT '🔍 Creando índices optimizados...';

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
-- 6. VALIDACIONES Y RESTRICCIONES
-- =====================================================

PRINT '🔒 Aplicando validaciones y restricciones...';

-- 1. TABLA STATES
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

-- ✅ FIX 4: Solo se permiten los StateType definidos
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_States_StateType_Valid')
BEGIN
    ALTER TABLE States
    ADD CONSTRAINT CK_States_StateType_Valid
    CHECK (StateType IN ('Person', 'Project'));
END

-- 2. TABLA ROLES
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Roles_RoleName_NotEmpty')
BEGIN
    ALTER TABLE Roles
    ADD CONSTRAINT CK_Roles_RoleName_NotEmpty CHECK (LEN(LTRIM(RTRIM(RoleName))) > 0);
END

-- 3. TABLA INTERNUSERS
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
	CHECK (DocumentExpire IS NULL OR DocumentExpire >= CAST(GETUTCDATE() AS DATE));  -- ✅ FIX 3
END

-- ✅ FIX 4: InternUsers solo puede tener estados de tipo 'Person'
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_InternUsers_StateType')
BEGIN
    ALTER TABLE InternUsers
    ADD CONSTRAINT CK_InternUsers_StateType
    CHECK (StateId IN (SELECT Id FROM States WHERE StateType = 'Person'));
END

-- 4. TABLA PROJECTS
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

-- ✅ FIX 4: Projects solo puede tener estados de tipo 'Project'
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Projects_StateType')
BEGIN
    ALTER TABLE Projects
    ADD CONSTRAINT CK_Projects_StateType
    CHECK (StateId IN (SELECT Id FROM States WHERE StateType = 'Project'));
END

-- 5. TABLA DOCUMENTTYPE
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_DocumentType_DocumentName_NotEmpty')
BEGIN
    ALTER TABLE DocumentType
    ADD CONSTRAINT CK_DocumentType_DocumentName_NotEmpty CHECK (LEN(LTRIM(RTRIM(DocumentName))) > 0);
END

-- 6. TABLA EMPLOYEEINFO
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
    ADD CONSTRAINT CK_EmployeeInfo_Birthday_Past CHECK (Birthday < CAST(GETUTCDATE() AS DATE));  -- ✅ FIX 3
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_Birthday_Reasonable')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_Birthday_Reasonable 
    CHECK (Birthday >= DATEADD(YEAR, -100, GETUTCDATE()) AND Birthday <= DATEADD(YEAR, -16, GETUTCDATE()));  -- ✅ FIX 3
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_PhoneNumber_Format')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_PhoneNumber_Format 
    CHECK (PhoneNumber IS NULL OR PhoneNumber LIKE '[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9][0-9]');
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_IBAN_Format')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_IBAN_Format 
    CHECK (IBAN IS NULL OR (IBAN LIKE 'CR[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' AND LEN(IBAN) = 22));
END

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_CostPerHour_NonNegative' AND parent_object_id = OBJECT_ID('EmployeeInfo'))
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_CostPerHour_NonNegative CHECK (CostPerHour IS NULL OR CostPerHour >= 0);
END

-- ✅ FIX 4: EmployeeInfo solo puede tener estados de tipo 'Person'
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_StateType')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_StateType
    CHECK (StateId IN (SELECT Id FROM States WHERE StateType = 'Person'));
END

-- 7. TABLA ASSISTANCE
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

-- 8. TABLA PROJECTSASSIGNS
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ProjectsAssigns_Dates_Logical')
BEGIN
    ALTER TABLE ProjectsAssigns
    ADD CONSTRAINT CK_ProjectsAssigns_Dates_Logical CHECK (EndDate IS NULL OR EndDate >= AssignDate);
END

-- 9. TABLA AUDITREGISTER
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

-- 10. TABLA FINGERPRINT
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_FingerPrint_Template_NotEmpty')
BEGIN
    ALTER TABLE FingerPrint
    ADD CONSTRAINT CK_FingerPrint_Template_NotEmpty CHECK (DATALENGTH(TemplateFingerPrint) > 0);
END

-- 11. TABLA PROJECTMAINTENANCE
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

-- ✅ FIX 4: ProjectMaintenance solo puede tener estados de tipo 'Project'
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ProjectMaintenance_StateType')
BEGIN
    ALTER TABLE ProjectMaintenance
    ADD CONSTRAINT CK_ProjectMaintenance_StateType
    CHECK (StateId IN (SELECT Id FROM States WHERE StateType = 'Project'));
END

-- 12. TABLA PROJECTWARRANTY
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

-- ✅ FIX 4: ProjectWarranty solo puede tener estados de tipo 'Project'
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ProjectWarranty_StateType')
BEGIN
    ALTER TABLE ProjectWarranty
    ADD CONSTRAINT CK_ProjectWarranty_StateType
    CHECK (StateId IN (SELECT Id FROM States WHERE StateType = 'Project'));
END

PRINT '✅ Todas las validaciones aplicadas correctamente';

-- =====================================================
-- 7. PROCEDIMIENTOS ALMACENADOS
-- =====================================================

PRINT '📋 Creando procedimientos almacenados...';

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_CleanupExpiredSessions]') AND type in (N'P'))
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[sp_CleanupExpiredSessions]
    AS
    BEGIN
        SET NOCOUNT ON;
        
        UPDATE [dbo].[Sessions]
        SET [IsActive] = 0,
            [LoggedOutAt] = GETUTCDATE()
        WHERE [ExpiresAt] < GETUTCDATE()
        AND [IsActive] = 1;
        
        DELETE FROM [dbo].[Sessions]
        WHERE [CreatedAt] < DATEADD(DAY, -30, GETUTCDATE())
        AND [IsActive] = 0;
        
        PRINT ''Limpieza de sesiones completada'';
    END
    ');
    PRINT '✅ Procedimiento sp_CleanupExpiredSessions creado';
END

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
-- 8. MIGRACIÓN PARA BASES DE DATOS EXISTENTES
-- =====================================================

PRINT '🔄 Verificando y aplicando migraciones para bases de datos existentes...';

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

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_InternUsers_DocumentTypeId')
BEGIN
    ALTER TABLE InternUsers 
    ADD CONSTRAINT FK_InternUsers_DocumentTypeId 
    FOREIGN KEY (DocumentTypeId) REFERENCES DocumentType(Id);
    PRINT '✅ FK DocumentTypeId agregada a InternUsers';
END

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
-- 9. VERIFICACIÓN FINAL
-- =====================================================

PRINT '';
PRINT '========================================';
PRINT 'VERIFICACIÓN FINAL DE LA CONFIGURACIÓN';
PRINT '========================================';

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

PRINT '';
PRINT '📋 ESTADOS INSERTADOS:';
SELECT StateName, StateType, Description FROM [dbo].[States] ORDER BY StateType, StateName;

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

PRINT '';
PRINT '📋 PROCEDIMIENTOS ALMACENADOS:';
SELECT 
    name AS ProcedureName,
    create_date AS CreatedDate
FROM sys.procedures 
WHERE name LIKE 'sp_%'
ORDER BY name;

-- =====================================================
-- 10. RESUMEN FINAL
-- =====================================================

PRINT '';
PRINT '========================================';
PRINT '✅ CONFIGURACIÓN COMPLETADA EXITOSAMENTE';
PRINT '========================================';
PRINT '📊 RESUMEN:';
PRINT '   - Base de datos WTB_DB inicializada';
PRINT '   - 14 tablas creadas con UpdatedAt en todas';
PRINT '   - Fechas estandarizadas a GETUTCDATE()';
PRINT '   - Estados base insertados (Person y Project)';
PRINT '   - StateType validado por tabla';
PRINT '   - Índices optimizados para consultas frecuentes';
PRINT '   - Validaciones de integridad aplicadas';
PRINT '   - Procedimientos almacenados creados';
PRINT '';
PRINT '🔒 VALIDACIONES INCLUIDAS:';
PRINT '   - Formato de email válido';
PRINT '   - Formato de teléfono CR (nnnn-nnnn)';
PRINT '   - Formato de IBAN CR (22 caracteres)';
PRINT '   - Validaciones de fechas lógicas';
PRINT '   - Validaciones de campos no vacíos';
PRINT '   - Validaciones de costos no negativos';
PRINT '   - StateType correcto por tabla (Person / Project)';
PRINT '';
PRINT '🚀 La base de datos está lista para usar!';
GO