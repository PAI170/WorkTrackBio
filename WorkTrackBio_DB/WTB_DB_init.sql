IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'WTB_DB')
BEGIN
	CREATE DATABASE WTB_DB;
END
GO

USE WTB_DB;
GO

--CREATE STATES TABLE
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'States')
BEGIN
	CREATE TABLE States(
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	StateName NVARCHAR(50) NOT NULL UNIQUE,
	StateType NVARCHAR(50) NOT NULL,
	Description NVARCHAR(50) NULL
	);
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
	LastLogin DATETIME2 NULL
	);
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
END
GO

--INDEX CREATION
USE WTB_DB;
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_States_StateName' AND object_id = OBJECT_ID('States'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_States_StateName
    ON States (StateName, StateType);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Roles_RoleName' AND object_id = OBJECT_ID('Roles'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_Roles_RoleName
    ON Roles (RoleName);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InternUsers_Email' AND object_id = OBJECT_ID('InternUsers'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_InternUsers_Email
    ON InternUsers (Email);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InternUsers_RolId' AND object_id = OBJECT_ID('InternUsers'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_InternUsers_RolId
    ON InternUsers (RolId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Projects_ProjectName' AND object_id = OBJECT_ID('Projects'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_Projects_ProjectName
    ON Projects (ProjectName);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Projects_StateId' AND object_id = OBJECT_ID('Projects'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Projects_StateId
    ON Projects (StateId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DocumentType_DocumentName' AND object_id = OBJECT_ID('DocumentType'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_DocumentType_DocumentName
    ON DocumentType (DocumentName);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeInfo_DocumentNumber' AND object_id = OBJECT_ID('EmployeeInfo'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_EmployeeInfo_DocumentNumber
    ON EmployeeInfo (DocumentNumber);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeInfo_StateId' AND object_id = OBJECT_ID('EmployeeInfo'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_EmployeeInfo_StateId
    ON EmployeeInfo (StateId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeInfo_DocumentTypeId' AND object_id = OBJECT_ID('EmployeeInfo'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_EmployeeInfo_DocumentTypeId
    ON EmployeeInfo (DocumentTypeId);
END
GO

-- NUEVO: Índice para búsquedas por nombre completo
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeInfo_FullName' AND object_id = OBJECT_ID('EmployeeInfo'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_EmployeeInfo_FullName
    ON EmployeeInfo (FirstName, LastName);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assistance_EmployeeProjectCheckIn' AND object_id = OBJECT_ID('Assistance'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Assistance_EmployeeProjectCheckIn
    ON Assistance (EmployeeId, ProjectId, CheckIn DESC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assistance_ProjectId' AND object_id = OBJECT_ID('Assistance'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Assistance_ProjectId
    ON Assistance (ProjectId);
END
GO

-- NUEVO: Índice para consultas por fecha de asistencia (muy común en reportes)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Assistance_CheckInDate' AND object_id = OBJECT_ID('Assistance'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Assistance_CheckInDate
    ON Assistance (CheckInDateOnly DESC) -- ¡Aquí usas la nueva columna computada!
    INCLUDE (EmployeeId, ProjectId, TotalHours);
    PRINT 'Index IX_Assistance_CheckInDate created on Assistance table.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectsAssigns_EmployeeProjectAssign' AND object_id = OBJECT_ID('ProjectsAssigns'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProjectsAssigns_EmployeeProjectAssign
    ON ProjectsAssigns (EmployeeId, ProjectId, AssignDate DESC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectsAssigns_ProjectId' AND object_id = OBJECT_ID('ProjectsAssigns'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProjectsAssigns_ProjectId
    ON ProjectsAssigns (ProjectId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditRegister_AssistanceId' AND object_id = OBJECT_ID('AuditRegister'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AuditRegister_AssistanceId
    ON AuditRegister (AssistanceId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditRegister_AdminId' AND object_id = OBJECT_ID('AuditRegister'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AuditRegister_AdminId
    ON AuditRegister (AdminId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditRegister_ActionDate' AND object_id = OBJECT_ID('AuditRegister'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AuditRegister_ActionDate
    ON AuditRegister (ActionDate DESC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FingerPrint_EmployeeId' AND object_id = OBJECT_ID('FingerPrint'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_FingerPrint_EmployeeId
    ON FingerPrint (EmployeeId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectMaintenance_IdProject' AND object_id = OBJECT_ID('ProjectMaintenance'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProjectMaintenance_IdProject
    ON ProjectMaintenance (IdProject);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectMaintenance_MadeById' AND object_id = OBJECT_ID('ProjectMaintenance'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProjectMaintenance_MadeById
    ON ProjectMaintenance (MadeById);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectWarranty_IdProject' AND object_id = OBJECT_ID('ProjectWarranty'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProjectWarranty_IdProject
    ON ProjectWarranty (IdProject);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectWarranty_MadeById' AND object_id = OBJECT_ID('ProjectWarranty'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProjectWarranty_MadeById
    ON ProjectWarranty (MadeById);
END
GO

-- ================================
-- VALIDACIONES Y RESTRICCIONES BÁSICAS
-- ================================
USE WTB_DB;
GO

-- 1. TABLA STATES - Validaciones básicas de formato
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_States_StateName_NotEmpty')
BEGIN
    ALTER TABLE States
    ADD CONSTRAINT CK_States_StateName_NotEmpty CHECK (LEN(LTRIM(RTRIM(StateName))) > 0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_States_StateType_NotEmpty')
BEGIN
    ALTER TABLE States
    ADD CONSTRAINT CK_States_StateType_NotEmpty CHECK (LEN(LTRIM(RTRIM(StateType))) > 0);
END
GO

-- 2. TABLA ROLES - Validación de nombre no vacío
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Roles_RoleName_NotEmpty')
BEGIN
    ALTER TABLE Roles
    ADD CONSTRAINT CK_Roles_RoleName_NotEmpty CHECK (LEN(LTRIM(RTRIM(RoleName))) > 0);
END
GO

-- 3. TABLA INTERNUSERS - Validaciones de formato
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_InternUsers_Email_Format')
BEGIN
    ALTER TABLE InternUsers
    ADD CONSTRAINT CK_InternUsers_Email_Format 
    CHECK (Email LIKE '%@%.%' AND LEN(Email) >= 5 AND Email NOT LIKE '%@%@%');
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_InternUsers_FirstName_NotEmpty')
BEGIN
    ALTER TABLE InternUsers
    ADD CONSTRAINT CK_InternUsers_FirstName_NotEmpty CHECK (LEN(LTRIM(RTRIM(FirstName))) > 0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_InternUsers_LastName_NotEmpty')
BEGIN
    ALTER TABLE InternUsers
    ADD CONSTRAINT CK_InternUsers_LastName_NotEmpty CHECK (LEN(LTRIM(RTRIM(LastName))) > 0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_InternUsers_PasswordHash_NotEmpty')
BEGIN
    ALTER TABLE InternUsers
    ADD CONSTRAINT CK_InternUsers_PasswordHash_NotEmpty CHECK (LEN(LTRIM(RTRIM(PasswordHash))) >= 32);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_InternUsers_PasswordSalt_NotEmpty')
BEGIN
    ALTER TABLE InternUsers
    ADD CONSTRAINT CK_InternUsers_PasswordSalt_NotEmpty CHECK (LEN(LTRIM(RTRIM(PasswordSalt))) >= 8);
END
GO

-- 4. TABLA PROJECTS - Validaciones básicas
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Projects_ProjectName_NotEmpty')
BEGIN
    ALTER TABLE Projects
    ADD CONSTRAINT CK_Projects_ProjectName_NotEmpty CHECK (LEN(LTRIM(RTRIM(ProjectName))) > 0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Projects_Dates_Logical')
BEGIN
    ALTER TABLE Projects
    ADD CONSTRAINT CK_Projects_Dates_Logical CHECK (EndDate IS NULL OR StartDate IS NULL OR EndDate >= StartDate);
END
GO

-- 5. TABLA DOCUMENTTYPE - Validación de nombre
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_DocumentType_DocumentName_NotEmpty')
BEGIN
    ALTER TABLE DocumentType
    ADD CONSTRAINT CK_DocumentType_DocumentName_NotEmpty CHECK (LEN(LTRIM(RTRIM(DocumentName))) > 0);
END
GO

-- 6. TABLA EMPLOYEEINFO - Validaciones de formato y lógica
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_DocumentNumber_NotEmpty')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_DocumentNumber_NotEmpty CHECK (LEN(LTRIM(RTRIM(DocumentNumber))) > 0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_FirstName_NotEmpty')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_FirstName_NotEmpty CHECK (LEN(LTRIM(RTRIM(FirstName))) > 0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_LastName_NotEmpty')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_LastName_NotEmpty CHECK (LEN(LTRIM(RTRIM(LastName))) > 0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_Birthday_Past')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_Birthday_Past CHECK (Birthday < CAST(GETDATE() AS DATE));
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_Birthday_Reasonable')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_Birthday_Reasonable 
    CHECK (Birthday >= DATEADD(YEAR, -100, GETDATE()) AND Birthday <= DATEADD(YEAR, -16, GETDATE()));
END
GO

-- NUEVA: Validación de teléfono formato Costa Rica (nnnn-nnnn)
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_PhoneNumber_Format')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_PhoneNumber_Format 
    CHECK (PhoneNumber IS NULL OR PhoneNumber LIKE '[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9][0-9]');
END
GO

-- NUEVA: Validación de IBAN formato Costa Rica (CR + 20 dígitos)
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_IBAN_Format')
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_IBAN_Format 
    CHECK (IBAN IS NULL OR (IBAN LIKE 'CR[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' AND LEN(IBAN) = 22));
END
GO

-- 7. TABLA ASSISTANCE - Validaciones de integridad temporal y datos
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Assistance_CheckOut_After_CheckIn')
BEGIN
    ALTER TABLE Assistance
    ADD CONSTRAINT CK_Assistance_CheckOut_After_CheckIn CHECK (CheckOut IS NULL OR CheckOut >= CheckIn);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Assistance_TotalHours_NonNegative')
BEGIN
    ALTER TABLE Assistance
    ADD CONSTRAINT CK_Assistance_TotalHours_NonNegative CHECK (TotalHours IS NULL OR TotalHours >= 0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Assistance_RegisterType_Valid')
BEGIN
    ALTER TABLE Assistance
    ADD CONSTRAINT CK_Assistance_RegisterType_Valid 
    CHECK (RegisterType IN ('CheckIn', 'CheckOut', 'Manual'));
END
GO

-- 8. TABLA PROJECTSASSIGNS - Validación de fechas
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ProjectsAssigns_Dates_Logical')
BEGIN
    ALTER TABLE ProjectsAssigns
    ADD CONSTRAINT CK_ProjectsAssigns_Dates_Logical CHECK (EndDate IS NULL OR EndDate >= AssignDate);
END
GO

-- 9. TABLA AUDITREGISTER - Validaciones de contenido
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_AuditRegister_ActionType_NotEmpty')
BEGIN
    ALTER TABLE AuditRegister
    ADD CONSTRAINT CK_AuditRegister_ActionType_NotEmpty CHECK (LEN(LTRIM(RTRIM(ActionType))) > 0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_AuditRegister_DetailChange_NotEmpty')
BEGIN
    ALTER TABLE AuditRegister
    ADD CONSTRAINT CK_AuditRegister_DetailChange_NotEmpty CHECK (LEN(LTRIM(RTRIM(DetailChange))) > 0);
END
GO

-- 10. TABLA FINGERPRINT - Validación de datos biométricos
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_FingerPrint_Template_NotEmpty')
BEGIN
    ALTER TABLE FingerPrint
    ADD CONSTRAINT CK_FingerPrint_Template_NotEmpty CHECK (DATALENGTH(TemplateFingerPrint) > 0);
END
GO

-- 11. TABLA PROJECTMAINTENANCE - Validaciones de costos y contenido
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ProjectMaintenance_Cost_NonNegative')
BEGIN
    ALTER TABLE ProjectMaintenance
    ADD CONSTRAINT CK_ProjectMaintenance_Cost_NonNegative CHECK (MaintenanceCost IS NULL OR MaintenanceCost >= 0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ProjectMaintenance_Description_NotEmpty')
BEGIN
    ALTER TABLE ProjectMaintenance
    ADD CONSTRAINT CK_ProjectMaintenance_Description_NotEmpty CHECK (LEN(LTRIM(RTRIM(MaintenanceDescription))) > 0);
END
GO

-- 12. TABLA PROJECTWARRANTY - Validaciones de costos y contenido
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ProjectWarranty_Cost_NonNegative')
BEGIN
    ALTER TABLE ProjectWarranty
    ADD CONSTRAINT CK_ProjectWarranty_Cost_NonNegative CHECK (WarrantyCost IS NULL OR WarrantyCost >= 0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_ProjectWarranty_Description_NotEmpty')
BEGIN
    ALTER TABLE ProjectWarranty
    ADD CONSTRAINT CK_ProjectWarranty_Description_NotEmpty CHECK (LEN(LTRIM(RTRIM(WarrantyDescription))) > 0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_EmployeeInfo_CostPerHour_NonNegative' AND parent_object_id = OBJECT_ID('EmployeeInfo'))
BEGIN
    ALTER TABLE EmployeeInfo
    ADD CONSTRAINT CK_EmployeeInfo_CostPerHour_NonNegative CHECK (CostPerHour IS NULL OR CostPerHour >= 0);
    PRINT 'CHECK CONSTRAINT CK_EmployeeInfo_CostPerHour_NonNegative added to EmployeeInfo table.';
END
ELSE
BEGIN
    PRINT 'CHECK CONSTRAINT CK_EmployeeInfo_CostPerHour_NonNegative already exists in EmployeeInfo table.';
END
GO

PRINT '✅ Base de datos WTB_DB inicializada correctamente';
PRINT '✅ Todas las tablas creadas con relaciones FK';
PRINT '✅ Índices optimizados para consultas frecuentes';
PRINT '✅ Validaciones básicas de integridad aplicadas';
PRINT '📋 Validaciones agregadas:';
PRINT '   - Formato de email válido';
PRINT '   - Formato de teléfono CR (nnnn-nnnn)';
PRINT '   - Formato de IBAN CR (22 caracteres)';
PRINT '   - Validaciones de fechas lógicas';
PRINT '   - Validaciones de campos no vacíos';
PRINT '   - Validaciones de costos no negativos';
GO