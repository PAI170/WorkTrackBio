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
	RolName NVARCHAR(50) NOT NULL UNIQUE,
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
	Birthday DATE NOT NULL,
	RegisterDate DATETIME2 DEFAULT GETDATE() NOT NULL,
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
	CheckIn DATETIME2 NOT NULL,
	CheckOut DATETIME2 NULL,
	TotalHours DECIMAL(5,2) NULL,
	RegisterType NVARCHAR(50) NOT NULL
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
    ON Roles (RolName);
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

-- CHECK CONSTRAINTS CREATION
USE WTB_DB;
GO

-- 1. Tabla 'States'

ALTER TABLE States
ADD CONSTRAINT CK_States_StateType_NotEmpty CHECK (LEN(StateType) > 0);
GO

ALTER TABLE Roles
ADD CONSTRAINT CK_Roles_RolName_NotEmpty CHECK (LEN(RolName) > 0);
GO

ALTER TABLE InternUsers
ADD CONSTRAINT CK_InternUsers_Email_NotEmpty CHECK (LEN(Email) > 0);

ALTER TABLE Projects
ADD CONSTRAINT CK_Projects_Dates CHECK (EndDate IS NULL OR StartDate IS NULL OR EndDate >= StartDate);
GO

ALTER TABLE Projects
ADD CONSTRAINT CK_Projects_ProjectName_NotEmpty CHECK (LEN(ProjectName) > 0);
GO

ALTER TABLE DocumentType
ADD CONSTRAINT CK_DocumentType_DocumentName_NotEmpty CHECK (LEN(DocumentName) > 0);
GO

ALTER TABLE EmployeeInfo
ADD CONSTRAINT CK_EmployeeInfo_DocumentNumber_NotEmpty CHECK (LEN(DocumentNumber) > 0);
GO

ALTER TABLE EmployeeInfo
ADD CONSTRAINT CK_EmployeeInfo_Birthday_Past CHECK (Birthday < GETDATE());
GO

ALTER TABLE EmployeeInfo
ADD CONSTRAINT CK_EmployeeInfo_RegisterDate_PastOrPresent CHECK (RegisterDate <= GETDATE());
GO

ALTER TABLE Assistance
ADD CONSTRAINT CK_Assistance_CheckDates CHECK (CheckOut IS NULL OR CheckOut >= CheckIn);
GO

ALTER TABLE Assistance
ADD CONSTRAINT CK_Assistance_TotalHours_NonNegative CHECK (TotalHours IS NULL OR TotalHours >= 0);
GO

ALTER TABLE Assistance
ADD CONSTRAINT CK_Assistance_RegisterType_Valid CHECK (RegisterType IN ('CheckIn', 'CheckOut', 'Manual'));
GO

ALTER TABLE ProjectsAssigns
ADD CONSTRAINT CK_ProjectsAssigns_Dates CHECK (EndDate IS NULL OR AssignDate IS NULL OR EndDate >= AssignDate);
GO

ALTER TABLE AuditRegister
ADD CONSTRAINT CK_AuditRegister_ActionType_NotEmpty CHECK (LEN(ActionType) > 0);
GO

ALTER TABLE FingerPrint
ADD CONSTRAINT CK_FingerPrint_Template_NotEmpty CHECK (DATALENGTH(TemplateFingerPrint) > 0);
GO

ALTER TABLE FingerPrint
ADD CONSTRAINT CK_FingerPrint_IssueDate_PastOrPresent CHECK (IssueDate <= GETDATE());
GO

ALTER TABLE ProjectMaintenance
ADD CONSTRAINT CK_ProjectMaintenance_Cost_NonNegative CHECK (MaintenanceCost IS NULL OR MaintenanceCost >= 0);
GO

ALTER TABLE ProjectMaintenance
ADD CONSTRAINT CK_ProjectMaintenance_Description_NotEmpty CHECK (LEN(MaintenanceDescription) > 0);
GO

ALTER TABLE ProjectMaintenance
ADD CONSTRAINT CK_ProjectMaintenance_Date_PastOrPresent CHECK (MaintenanceDate <= GETDATE());
GO

ALTER TABLE ProjectWarranty
ADD CONSTRAINT CK_ProjectWarranty_Cost_NonNegative CHECK (WarrantyCost IS NULL OR WarrantyCost >= 0);
GO

ALTER TABLE ProjectWarranty
ADD CONSTRAINT CK_ProjectWarranty_Description_NotEmpty CHECK (LEN(WarrantyDescription) > 0);
GO

ALTER TABLE ProjectWarranty
ADD CONSTRAINT CK_ProjectWarranty_Date_PastOrPresent CHECK (WarrantyDate <= GETDATE());
GO