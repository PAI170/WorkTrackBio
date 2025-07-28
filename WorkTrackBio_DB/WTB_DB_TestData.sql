USE WTB_DB;
GO

-- INSERT TEST DATA

INSERT INTO States (StateName, StateType, Description) VALUES
('Active', 'Employee', 'Employee is currently active.'),
('Inactive', 'Employee', 'Employee is inactive.'),
('On Leave', 'Employee', 'Employee is on leave.'),
('Active', 'Project', 'Project is currently active.'),
('Completed', 'Project', 'Project has been completed.'),
('Pending', 'Project', 'Project is pending to start.'),
('Cancelled', 'Project', 'Project has been cancelled.'),
('Active', 'InternUser', 'Internal user is active.'),
('Disabled', 'InternUser', 'Internal user account is disabled.'),
('In Progress', 'Maintenance', 'Maintenance is in progress.'),
('Finished', 'Maintenance', 'Maintenance completed.'),
('Approved', 'Warranty', 'Warranty claim approved.'),
('Rejected', 'Warranty', 'Warranty claim rejected.');
GO

DECLARE @ActiveEmployeeStateId INT = (SELECT Id FROM States WHERE StateName = 'Active' AND StateType = 'Employee');
DECLARE @InactiveEmployeeStateId INT = (SELECT Id FROM States WHERE StateName = 'Inactive' AND StateType = 'Employee');
DECLARE @ActiveProjectStateId INT = (SELECT Id FROM States WHERE StateName = 'Active' AND StateType = 'Project');
DECLARE @CompletedProjectStateId INT = (SELECT Id FROM States WHERE StateName = 'Completed' AND StateType = 'Project');
DECLARE @ActiveInternUserStateId INT = (SELECT Id FROM States WHERE StateName = 'Active' AND StateType = 'InternUser');
DECLARE @FinishedMaintenanceStateId INT = (SELECT Id FROM States WHERE StateName = 'Finished' AND StateType = 'Maintenance');
DECLARE @ApprovedWarrantyStateId INT = (SELECT Id FROM States WHERE StateName = 'Approved' AND StateType = 'Warranty');

INSERT INTO DocumentType (DocumentName, Description) VALUES
('Cédula de Identidad', 'National identification card.'),
('DIMEX', 'Foreign Resident Identification Document.'),
('Pasaporte', 'International travel document.'),
('Permiso de Trabajo', 'Work permit for foreign nationals.');
GO

DECLARE @CedulaId INT = (SELECT Id FROM DocumentType WHERE DocumentName = 'Cédula de Identidad');
DECLARE @DimexId INT = (SELECT Id FROM DocumentType WHERE DocumentName = 'DIMEX');
DECLARE @PasaporteId INT = (SELECT Id FROM DocumentType WHERE DocumentName = 'Pasaporte');
DECLARE @PermisoTrabajoId INT = (SELECT Id FROM DocumentType WHERE DocumentName = 'Permiso de Trabajo');

INSERT INTO Roles (RoleName, Description) VALUES
('SuperAdmin', 'Full access to all system functionalities.'),
('ProjectManager', 'Manages projects and oversees assigned employees.'),
('ITSupport', 'Provides IT support for system operations.');
GO

DECLARE @SuperAdminRoleId INT = (SELECT Id FROM Roles WHERE RoleName = 'SuperAdmin');
DECLARE @ProjectManagerRoleId INT = (SELECT Id FROM Roles WHERE RoleName = 'ProjectManager');


INSERT INTO InternUsers (Email, FirstName, LastName, PasswordHash, PasswordSalt, RolId, CreationDate, StateId, LastLogin) VALUES
('admin@worktrackbio.com', 'System', 'Admin', 'hashedpassword1', 'salt1', @SuperAdminRoleId, GETDATE(), @ActiveInternUserStateId, GETDATE()),
('manager@worktrackbio.com', 'Project', 'Manager', 'hashedpassword2', 'salt2', @ProjectManagerRoleId, GETDATE(), @ActiveInternUserStateId, NULL);
GO

DECLARE @AdminUserId INT = (SELECT Id FROM InternUsers WHERE Email = 'admin@worktrackbio.com');
DECLARE @ManagerUserId INT = (SELECT Id FROM InternUsers WHERE Email = 'manager@worktrackbio.com');

INSERT INTO EmployeeInfo (DocumentNumber, DocumentTypeId, DocumentExpire, FirstName, LastName, PhoneNumber, Birthday, RegisterDate, StateId, Address, IBAN) VALUES
('101230456', @CedulaId, NULL, 'Ana', 'Hernandez', '8888-1111', '1990-01-15', GETDATE(), @ActiveEmployeeStateId, 'Street 1, City A', 'CR0100000000000000000001'),
('707890123', @DimexId, '2027-06-30', 'Carlos', 'Mejia', '8888-2222', '1985-04-22', GETDATE(), @ActiveEmployeeStateId, 'Street 2, City B', 'CR0100000000000000000002'),
('012345678', @PasaporteId, '2029-01-01', 'Laura', 'Vasquez', '8888-3333', '1992-07-10', GETDATE(), @ActiveEmployeeStateId, 'Street 3, City C', 'CR0100000000000000000003'),
('456789012', @PermisoTrabajoId, '2026-03-15', 'Miguel', 'Sanchez', '8888-4444', '1980-11-05', GETDATE(), @ActiveEmployeeStateId, 'Street 4, City D', 'CR0100000000000000000004'),
('901234567', @CedulaId, NULL, 'Sofia', 'Rodriguez', '8888-5555', '1993-09-28', GETDATE(), @ActiveEmployeeStateId, 'Street 5, City E', 'CR0100000000000000000005');
GO


DECLARE @AnaHernandezId INT = (SELECT Id FROM EmployeeInfo WHERE DocumentNumber = '101230456');
DECLARE @CarlosMejiaId INT = (SELECT Id FROM EmployeeInfo WHERE DocumentNumber = '707890123');
DECLARE @LauraVasquezId INT = (SELECT Id FROM EmployeeInfo WHERE DocumentNumber = '012345678');
DECLARE @MiguelSanchezId INT = (SELECT Id FROM EmployeeInfo WHERE DocumentNumber = '456789012');
DECLARE @SofiaRodriguezId INT = (SELECT Id FROM EmployeeInfo WHERE DocumentNumber = '901234567');


INSERT INTO Projects (ProjectName, StartDate, EndDate, StateId) VALUES
('Office CCTV Install Phase 1', '2025-07-01', '2025-07-31', @ActiveProjectStateId),
('Client X Network Support', '2025-07-10', NULL, @ActiveProjectStateId),
('New Website for Corp Z', '2025-06-01', '2025-06-30', @CompletedProjectStateId),
('Data Center Migration Y', '2025-07-15', '2025-08-15', @ActiveProjectStateId),
('Residential Alarm System', '2025-07-20', NULL, @ActiveProjectStateId);
GO

DECLARE @CCTVInstallId INT = (SELECT Id FROM Projects WHERE ProjectName = 'Office CCTV Install Phase 1');
DECLARE @ClientXNetworkId INT = (SELECT Id FROM Projects WHERE ProjectName = 'Client X Network Support');
DECLARE @WebsiteCorpZId INT = (SELECT Id FROM Projects WHERE ProjectName = 'New Website for Corp Z');
DECLARE @DataCenterMigrationId INT = (SELECT Id FROM Projects WHERE ProjectName = 'Data Center Migration Y');
DECLARE @ResidentialAlarmId INT = (SELECT Id FROM Projects WHERE ProjectName = 'Residential Alarm System');


INSERT INTO ProjectsAssigns (EmployeeId, ProjectId, AssignDate, EndDate, IsActive) VALUES

(@AnaHernandezId, @CCTVInstallId, '2025-07-01', NULL, 1),
(@AnaHernandezId, @DataCenterMigrationId, '2025-07-15', NULL, 1),


(@CarlosMejiaId, @ClientXNetworkId, '2025-07-10', NULL, 1),
(@CarlosMejiaId, @ResidentialAlarmId, '2025-07-20', NULL, 1),


(@LauraVasquezId, @ClientXNetworkId, '2025-07-10', NULL, 1),


(@MiguelSanchezId, @DataCenterMigrationId, '2025-07-15', NULL, 1),


(@SofiaRodriguezId, @ResidentialAlarmId, '2025-07-20', NULL, 1);
GO


INSERT INTO FingerPrint (EmployeeId, TemplateFingerPrint, Dedo, IssueDate) VALUES
(@AnaHernandezId, 0x0102030405060708090A0B0C0D0E0F10, 'Right Thumb', GETDATE()),
(@CarlosMejiaId, 0x1112131415161718191A1B1C1D1E1F20, 'Right Index', GETDATE()),
(@LauraVasquezId, 0x2122232425262728292A2B2C2D2E2F30, 'Left Thumb', GETDATE()),
(@MiguelSanchezId, 0x3132333435363738393A3B3C3D3E3F40, 'Right Middle', GETDATE()),
(@SofiaRodriguezId, 0x4142434445464748494A4B4C4D4E4F50, 'Left Index', GETDATE());
GO


INSERT INTO Assistance (EmployeeId, ProjectId, CheckIn, CheckOut, TotalHours, RegisterType, IsManual, AdjustedByAdminId, AdjustmentDate, AdjustmentReason) VALUES
(@AnaHernandezId, @CCTVInstallId, '2025-07-22 08:00:00', '2025-07-22 17:00:00', 9.00, 'CheckOut', 0, NULL, NULL, NULL),
(@AnaHernandezId, @CCTVInstallId, '2025-07-23 08:30:00', '2025-07-23 17:30:00', 9.00, 'CheckOut', 0, NULL, NULL, NULL),

(@AnaHernandezId, @DataCenterMigrationId, '2025-07-24 09:00:00', '2025-07-24 18:00:00', 9.00, 'CheckOut', 0, NULL, NULL, NULL);


INSERT INTO Assistance (EmployeeId, ProjectId, CheckIn, CheckOut, TotalHours, RegisterType, IsManual, AdjustedByAdminId, AdjustmentDate, AdjustmentReason) VALUES
(@CarlosMejiaId, @ClientXNetworkId, '2025-07-22 08:15:00', '2025-07-22 17:15:00', 9.00, 'CheckOut', 0, NULL, NULL, NULL),
(@CarlosMejiaId, @ClientXNetworkId, '2025-07-23 08:00:00', '2025-07-23 17:00:00', 9.00, 'CheckOut', 0, NULL, NULL, NULL),

(@CarlosMejiaId, @ResidentialAlarmId, '2025-07-24 10:00:00', '2025-07-24 19:00:00', 9.00, 'CheckOut', 0, NULL, NULL, NULL);


INSERT INTO Assistance (EmployeeId, ProjectId, CheckIn, CheckOut, TotalHours, RegisterType, IsManual, AdjustedByAdminId, AdjustmentDate, AdjustmentReason) VALUES
(@LauraVasquezId, @ClientXNetworkId, '2025-07-22 09:00:00', '2025-07-22 18:00:00', 9.00, 'CheckOut', 0, NULL, NULL, NULL),
(@LauraVasquezId, @ClientXNetworkId, '2025-07-23 09:15:00', '2025-07-23 18:15:00', 9.00, 'CheckOut', 0, NULL, NULL, NULL);


INSERT INTO Assistance (EmployeeId, ProjectId, CheckIn, CheckOut, TotalHours, RegisterType, IsManual, AdjustedByAdminId, AdjustmentDate, AdjustmentReason) VALUES
(@MiguelSanchezId, @DataCenterMigrationId, '2025-07-22 07:45:00', '2025-07-22 16:45:00', 9.00, 'CheckOut', 0, NULL, NULL, NULL),
(@MiguelSanchezId, @DataCenterMigrationId, '2025-07-23 08:00:00', '2025-07-23 17:00:00', 9.00, 'CheckOut', 0, NULL, NULL, NULL);


INSERT INTO Assistance (EmployeeId, ProjectId, CheckIn, CheckOut, TotalHours, RegisterType, IsManual, AdjustedByAdminId, AdjustmentDate, AdjustmentReason) VALUES
(@SofiaRodriguezId, @ResidentialAlarmId, '2025-07-22 09:30:00', '2025-07-22 18:30:00', 9.00, 'CheckOut', 0, NULL, NULL, NULL),
(@SofiaRodriguezId, @ResidentialAlarmId, '2025-07-23 09:00:00', '2025-07-23 17:00:00', 8.00, 'CheckOut', 0, NULL, NULL, NULL);
GO


DECLARE @AdjustedAssistanceIdForAudit INT = (SELECT Id FROM Assistance WHERE EmployeeId = @AnaHernandezId AND CheckIn = '2025-07-24 09:00:00');


INSERT INTO ProjectMaintenance (IdProject, MaintenanceDate, MaintenanceDescription, MadeById, MaintenanceCost, AdditionalInfo, StateId) VALUES
(@CCTVInstallId, '2025-07-20 10:00:00', 'Replaced faulty camera on 3rd floor.', @AnaHernandezId, 150.75, 'Camera model XZ200', @FinishedMaintenanceStateId),
(@ClientXNetworkId, '2025-07-24 14:00:00', 'Router firmware update.', @CarlosMejiaId, 50.00, NULL, @FinishedMaintenanceStateId);
GO


INSERT INTO ProjectWarranty (IdProject, WarrantyDate, WarrantyDescription, MadeById, WarrantyCost, StateId) VALUES
(@CCTVInstallId, '2025-07-21 09:00:00', 'Client reported flickering image on monitor. Re-calibrated.', @AnaHernandezId, 0.00, @ApprovedWarrantyStateId);
GO


INSERT INTO AuditRegister (AssistanceId, ActionType, DetailChange, ActionDate, AdminId) VALUES
(@AdjustedAssistanceIdForAudit, 'Manual CheckIn Adjustment', 'Corrected missing CheckIn for Ana Hernandez. Employee forgot.', GETDATE(), @AdminUserId);


INSERT INTO AuditRegister (AssistanceId, ActionType, DetailChange, ActionDate, AdminId) VALUES
(NULL, 'Project Creation', 'New project "Residential Alarm System" created by Admin.', GETDATE(), @AdminUserId);
GO