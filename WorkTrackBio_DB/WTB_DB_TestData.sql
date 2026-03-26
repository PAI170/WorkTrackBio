USE WTB_DB;
GO

-- ================================
-- STEP 1: ESTADOS
-- ================================
-- ✅ No se insertan aquí.
-- El script WTB_DB_init_v2.sql ya los inserta automáticamente:
--   'Activo'      → Person
--   'Inactivo'    → Person
--   'En progreso' → Project
--   'Completado'  → Project
--   'Cancelado'   → Project
PRINT 'ℹ️ Estados omitidos - ya insertados por init script v2';
GO

-- ================================
-- STEP 2: ROLES
-- ================================
-- ✅ Solo se insertan si no existen para evitar duplicados
IF NOT EXISTS (SELECT * FROM Roles WHERE RoleName = 'Administrator')
    INSERT INTO Roles (RoleName, Description) VALUES ('Administrator', 'Administrador del sistema con acceso completo');

IF NOT EXISTS (SELECT * FROM Roles WHERE RoleName = 'HR Manager')
    INSERT INTO Roles (RoleName, Description) VALUES ('HR Manager', 'Gerente de recursos humanos');

IF NOT EXISTS (SELECT * FROM Roles WHERE RoleName = 'Project Manager')
    INSERT INTO Roles (RoleName, Description) VALUES ('Project Manager', 'Gerente de proyectos');

IF NOT EXISTS (SELECT * FROM Roles WHERE RoleName = 'Supervisor')
    INSERT INTO Roles (RoleName, Description) VALUES ('Supervisor', 'Supervisor de campo');

IF NOT EXISTS (SELECT * FROM Roles WHERE RoleName = 'Operator')
    INSERT INTO Roles (RoleName, Description) VALUES ('Operator', 'Operador básico del sistema');

PRINT '✅ Roles insertados';
GO

-- ================================
-- STEP 3: DOCUMENT TYPES
-- ================================
IF NOT EXISTS (SELECT * FROM DocumentType WHERE DocumentName = 'Cedula de Identidad')
    INSERT INTO DocumentType (DocumentName, Description) VALUES ('Cedula de Identidad', 'Cédula de identidad costarricense');

IF NOT EXISTS (SELECT * FROM DocumentType WHERE DocumentName = 'Pasaporte')
    INSERT INTO DocumentType (DocumentName, Description) VALUES ('Pasaporte', 'Pasaporte internacional');

IF NOT EXISTS (SELECT * FROM DocumentType WHERE DocumentName = 'DIMEX')
    INSERT INTO DocumentType (DocumentName, Description) VALUES ('DIMEX', 'Documento de Identidad Migratoria para Extranjeros');

IF NOT EXISTS (SELECT * FROM DocumentType WHERE DocumentName = 'Permiso de Trabajo')
    INSERT INTO DocumentType (DocumentName, Description) VALUES ('Permiso de Trabajo', 'Permiso de trabajo para extranjeros');

PRINT '✅ Tipos de documento insertados';
GO

-- ================================
-- STEP 4: INTERN USERS
-- ================================
-- ✅ FIX: CreationDate → CreatedAt
-- ✅ FIX: GETDATE() → GETUTCDATE()
-- ✅ FIX: StateId usa 'Activo' que es de tipo 'Person' ✓
DECLARE @StateActivoPerson INT = (SELECT Id FROM States WHERE StateName = 'Activo' AND StateType = 'Person');
DECLARE @RolAdmin INT          = (SELECT Id FROM Roles WHERE RoleName = 'Administrator');
DECLARE @RolHR INT             = (SELECT Id FROM Roles WHERE RoleName = 'HR Manager');
DECLARE @RolPM INT             = (SELECT Id FROM Roles WHERE RoleName = 'Project Manager');
DECLARE @RolSupervisor INT     = (SELECT Id FROM Roles WHERE RoleName = 'Supervisor');

INSERT INTO InternUsers (Email, FirstName, LastName, PasswordHash, PasswordSalt, RolId, CreatedAt, StateId) VALUES
('admin@wtb.co.cr',      'Carlos', 'Rodríguez', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'salt123abc', @RolAdmin,      GETUTCDATE(), @StateActivoPerson),
('hr@wtb.co.cr',         'María',  'González',  'b3a8e0e1f9ab1bfe3a36f231f676f78bb30a519d2b21e6c530c0eee8ebb4a5d0', 'salt456def', @RolHR,         GETUTCDATE(), @StateActivoPerson),
('pm1@wtb.co.cr',        'José',   'Vargas',    'c2356069e9d1e79ca924378153cfbbfb4d4416b1f99d41a2940bfdb66c5319db', 'salt789ghi', @RolPM,         GETUTCDATE(), @StateActivoPerson),
('supervisor@wtb.co.cr', 'Ana',    'Mora',      'd4735e3a265e16eee03f59718b9b5d03019c07d8b6c51f90da3a666eec13ab35', 'saltjklmno', @RolSupervisor, GETUTCDATE(), @StateActivoPerson);

PRINT '✅ Usuarios internos insertados';
GO

-- ================================
-- STEP 5: PROJECTS
-- ================================
-- ✅ FIX: StateId usa nombres en español y StateType 'Project' ✓
DECLARE @StateEnProgreso INT = (SELECT Id FROM States WHERE StateName = 'En progreso' AND StateType = 'Project');
DECLARE @StateCompletado INT = (SELECT Id FROM States WHERE StateName = 'Completado'  AND StateType = 'Project');
DECLARE @StateCancelado  INT = (SELECT Id FROM States WHERE StateName = 'Cancelado'   AND StateType = 'Project');

INSERT INTO Projects (ProjectName, StartDate, EndDate, StateId, CreatedAt) VALUES
('CCTV Installation - Mall San Pedro',      '2024-01-15', '2024-03-30', @StateCompletado,  GETUTCDATE()),
('Web Design - Hotel Presidente',           '2024-02-01', NULL,         @StateEnProgreso,  GETUTCDATE()),
('Alarm System - Banco Nacional',           '2024-01-20', '2024-04-15', @StateCompletado,  GETUTCDATE()),
('CCTV Installation - Universidad UCR',     '2024-03-01', NULL,         @StateEnProgreso,  GETUTCDATE()),
('Web Design - Restaurant Machu Picchu',    '2024-02-15', '2024-05-20', @StateCompletado,  GETUTCDATE()),
('Alarm System - Oficinas Ministerio',      '2024-03-15', NULL,         @StateCancelado,   GETUTCDATE());

PRINT '✅ Proyectos insertados';
GO

-- ================================
-- STEP 6: EMPLOYEE INFO
-- ================================
-- ✅ FIX: RegisterDate → CreatedAt
-- ✅ FIX: GETDATE() → GETUTCDATE()
-- ✅ FIX: StateId usa 'Activo' que es de tipo 'Person' ✓
DECLARE @StateActivoPerson INT  = (SELECT Id FROM States WHERE StateName = 'Activo' AND StateType = 'Person');
DECLARE @DocCedula         INT  = (SELECT Id FROM DocumentType WHERE DocumentName = 'Cedula de Identidad');
DECLARE @DocPasaporte      INT  = (SELECT Id FROM DocumentType WHERE DocumentName = 'Pasaporte');
DECLARE @DocDIMEX          INT  = (SELECT Id FROM DocumentType WHERE DocumentName = 'DIMEX');
DECLARE @DocPermiso        INT  = (SELECT Id FROM DocumentType WHERE DocumentName = 'Permiso de Trabajo');

INSERT INTO EmployeeInfo (DocumentNumber, DocumentTypeId, DocumentExpire, FirstName, LastName, PhoneNumber, EmergencyContact, EmergencyContactPhoneNumber, Birthday, CostPerHour, StateId, Address, IBAN, CreatedAt) VALUES
('1-1234-5678',   @DocCedula,    '2029-12-31', 'Roberto',   'Jiménez', '2456-7890', 'Elena Jiménez',  '2456-7891', '1985-06-15', 3500.00, @StateActivoPerson, 'San José, Barrio Escalante', 'CR05015202001234567890', GETUTCDATE()),
('2-2345-6789',   @DocCedula,    '2028-11-30', 'Sofía',     'Herrera', '2567-8901', 'Miguel Herrera', '2567-8902', '1990-03-22', 3200.00, @StateActivoPerson, 'Cartago, Centro',            'CR05015202002345678901', GETUTCDATE()),
('P123456789',    @DocPasaporte, '2027-08-15', 'Diego',     'Ramírez', '2678-9012', 'Carmen Ramírez', '2678-9013', '1988-11-08', 3800.00, @StateActivoPerson, 'Alajuela, Centro',           'CR05015202003456789012', GETUTCDATE()),
('DIM-12345678',  @DocDIMEX,     '2026-05-20', 'Isabella',  'Morales', '2789-0123', 'Pedro Morales',  '2789-0124', '1992-09-14', 3000.00, @StateActivoPerson, 'Heredia, Mercedes',          'CR05015202004567890123', GETUTCDATE()),
('PT-987654',     @DocPermiso,   '2025-12-10', 'Fernando',  'Castro',  '2890-1234', 'Lucía Castro',   '2890-1235', '1987-01-30', 3600.00, @StateActivoPerson, 'Puntarenas, Centro',         'CR05015202005678901234', GETUTCDATE()),
('1-9876-5432',   @DocCedula,    '2030-06-25', 'Gabriela',  'Solís',   '2901-2345', 'Juan Solís',     '2901-2346', '1991-07-12', 3300.00, @StateActivoPerson, 'San José, Pavas',            'CR05015202006789012345', GETUTCDATE()),
('2-8765-4321',   @DocCedula,    '2029-09-18', 'Andrés',    'Navarro', '2012-3456', 'Rosa Navarro',   '2012-3457', '1989-04-05', 3700.00, @StateActivoPerson, 'Cartago, Paraíso',           'CR05015202007890123456', GETUTCDATE()),
('DIM-87654321',  @DocDIMEX,     '2026-03-12', 'Valentina', 'Ortega',  '2123-4567', 'Mario Ortega',   '2123-4568', '1993-12-28', 2900.00, @StateActivoPerson, 'Limón, Centro',              'CR05015202008901234567', GETUTCDATE());

PRINT '✅ Empleados insertados';
GO

-- ================================
-- STEP 7: PROJECT ASSIGNMENTS
-- ================================
-- ✅ FIX: GETDATE() → GETUTCDATE()
INSERT INTO ProjectsAssigns (EmployeeId, ProjectId, AssignDate, EndDate) VALUES
-- Mall San Pedro CCTV (Completado)
(1, 1, '2024-01-15', '2024-03-30'),
(2, 1, '2024-01-20', '2024-03-30'),
(3, 1, '2024-02-01', '2024-03-30'),
-- Hotel Presidente Web Design (En progreso)
(4, 2, '2024-02-01', NULL),
(5, 2, '2024-02-05', NULL),
-- Banco Nacional Alarm (Completado)
(1, 3, '2024-01-20', '2024-04-15'),
(6, 3, '2024-01-25', '2024-04-15'),
(7, 3, '2024-02-10', '2024-04-15'),
-- UCR CCTV (En progreso)
(2, 4, '2024-03-01', NULL),
(3, 4, '2024-03-05', NULL),
(8, 4, '2024-03-10', NULL);

PRINT '✅ Asignaciones de proyectos insertadas';
GO

-- ================================
-- STEP 8: FINGERPRINTS
-- ================================
-- ✅ FIX: IssueDate → CreatedAt
-- ✅ FIX: GETDATE() → GETUTCDATE()
INSERT INTO FingerPrint (EmployeeId, TemplateFingerPrint, CreatedAt) VALUES
(1, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CB, GETUTCDATE()),
(2, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CC, GETUTCDATE()),
(3, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CD, GETUTCDATE()),
(4, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CE, GETUTCDATE()),
(5, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CF, GETUTCDATE()),
(6, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5D0, GETUTCDATE()),
(7, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5D1, GETUTCDATE()),
(8, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5D2, GETUTCDATE());

PRINT '✅ Huellas dactilares insertadas';
GO

-- ================================
-- STEP 9: ASSISTANCE RECORDS
-- ================================
-- ✅ FIX: CreatedDate → CreatedAt
-- ✅ FIX: GETDATE() → GETUTCDATE()
INSERT INTO Assistance (EmployeeId, ProjectId, CreatedAt, CheckIn, CheckOut, TotalHours, RegisterType, Notes) VALUES
-- Roberto - Mall San Pedro CCTV
(1, 1, GETUTCDATE(), '2024-01-15 08:00:00', '2024-01-15 17:00:00', 8.0,  'CheckOut', 'Instalación inicial de cámaras en planta baja'),
(1, 1, GETUTCDATE(), '2024-01-16 08:30:00', '2024-01-16 17:30:00', 8.0,  'CheckOut', 'Configuración del sistema de grabación'),
(1, 1, GETUTCDATE(), '2024-01-17 08:00:00', '2024-01-17 16:00:00', 7.0,  'CheckOut', 'Pruebas del sistema completo'),
-- Roberto - Banco Nacional Alarm
(1, 3, GETUTCDATE(), '2024-01-22 09:00:00', '2024-01-22 18:00:00', 8.0,  'CheckOut', 'Instalación de sensores perimetrales'),
(1, 3, GETUTCDATE(), '2024-01-23 08:45:00', '2024-01-23 17:45:00', 8.0,  'CheckOut', 'Configuración de central de alarmas'),
-- Sofía - Mall San Pedro CCTV
(2, 1, GETUTCDATE(), '2024-01-20 08:00:00', '2024-01-20 17:00:00', 8.5,  'CheckOut', 'Cableado estructurado segundo piso'),
(2, 1, GETUTCDATE(), '2024-01-21 08:15:00', '2024-01-21 17:15:00', 8.0,  'CheckOut', 'Instalación cámaras exteriores'),
-- Sofía - UCR CCTV
(2, 4, GETUTCDATE(), '2024-03-01 08:00:00', '2024-03-01 17:30:00', 8.5,  'CheckOut', 'Levantamiento de requerimientos UCR'),
(2, 4, GETUTCDATE(), '2024-03-02 08:30:00', '2024-03-02 17:00:00', 7.5,  'CheckOut', 'Diseño de layout de cámaras'),
-- Diego - múltiples proyectos
(3, 1, GETUTCDATE(), '2024-02-01 08:00:00', '2024-02-01 16:30:00', 7.5,  'CheckOut', 'Programación de cámaras IP'),
(3, 4, GETUTCDATE(), '2024-03-05 09:00:00', '2024-03-05 18:15:00', 8.25, 'CheckOut', 'Instalación inicial UCR - Facultad Ingeniería'),
-- Isabella - proyectos web
(4, 2, GETUTCDATE(), '2024-02-01 09:00:00', '2024-02-01 18:00:00', 8.0,  'CheckOut', 'Análisis de requerimientos web Hotel Presidente'),
(4, 5, GETUTCDATE(), '2024-02-15 08:30:00', '2024-02-15 17:30:00', 8.0,  'CheckOut', 'Diseño UI/UX Restaurant Machu Picchu'),
-- Fernando - proyectos web
(5, 2, GETUTCDATE(), '2024-02-05 08:00:00', '2024-02-05 17:45:00', 8.75, 'CheckOut', 'Desarrollo backend Hotel Presidente'),
(5, 5, GETUTCDATE(), '2024-02-20 09:15:00', '2024-02-20 18:00:00', 7.75, 'CheckOut', 'Implementación sistema reservas restaurant'),
-- Gabriela - sistemas de alarma
(6, 3, GETUTCDATE(), '2024-01-25 08:00:00', '2024-01-25 17:00:00', 8.0,  'CheckOut', 'Instalación detectores de movimiento'),
(6, 6, GETUTCDATE(), '2024-03-15 08:30:00', '2024-03-15 17:30:00', 8.0,  'CheckOut', 'Evaluación sitio Ministerio'),
-- Andrés - sistemas de alarma
(7, 3, GETUTCDATE(), '2024-02-10 08:15:00', '2024-02-10 17:15:00', 8.0,  'CheckOut', 'Configuración panel central Banco Nacional'),
(7, 6, GETUTCDATE(), '2024-03-20 09:00:00', '2024-03-20 18:00:00', 8.0,  'CheckOut', 'Diseño sistema seguridad Ministerio'),
-- Valentina - UCR
(8, 4, GETUTCDATE(), '2024-03-10 08:00:00', '2024-03-10 16:45:00', 7.75, 'CheckOut', 'Soporte técnico instalación UCR'),
-- CheckIn activos (empleados trabajando actualmente)
(2, 4, GETUTCDATE(), '2024-07-31 08:00:00', NULL, NULL, 'CheckIn', 'Continuando trabajo UCR - edificio administrativo'),
(4, 2, GETUTCDATE(), '2024-07-31 09:00:00', NULL, NULL, 'CheckIn', 'Desarrollo módulo reservas Hotel Presidente');

PRINT '✅ Registros de asistencia insertados';
GO

-- ================================
-- STEP 10: AUDIT REGISTER
-- ================================
-- ✅ FIX: GETDATE() → GETUTCDATE()
INSERT INTO AuditRegister (AssistanceId, ActionType, DetailChange, ActionDate, AdminId, CreatedAt) VALUES
(1, 'CREATE', 'Registro de asistencia creado: Roberto Jiménez - Mall San Pedro CCTV - 2024-01-15',      GETUTCDATE(), 1, GETUTCDATE()),
(2, 'CREATE', 'Registro de asistencia creado: Roberto Jiménez - Mall San Pedro CCTV - 2024-01-16',      GETUTCDATE(), 1, GETUTCDATE()),
(5, 'UPDATE', 'Modificación de horas: cambio de 7.5 a 8.0 horas por aprobación supervisor',             GETUTCDATE(), 3, GETUTCDATE()),
(8, 'CREATE', 'Registro de asistencia creado: Sofía Herrera - UCR CCTV - 2024-03-01',                   GETUTCDATE(), 2, GETUTCDATE()),
(12,'UPDATE', 'Corrección de hora de salida: 17:30 a 18:00 por tiempo extra aprobado',                  GETUTCDATE(), 1, GETUTCDATE());

PRINT '✅ Registros de auditoría insertados';
GO

-- ================================
-- STEP 11: PROJECT MAINTENANCE
-- ================================
-- ✅ FIX: GETDATE() → GETUTCDATE()
-- ✅ FIX: StateId usa 'En progreso' que es de tipo 'Project' ✓
DECLARE @StateEnProgreso INT = (SELECT Id FROM States WHERE StateName = 'En progreso' AND StateType = 'Project');

INSERT INTO ProjectMaintenance (IdProject, MaintenanceDescription, MadeById, MaintenanceCost, AdditionalInfo, StateId, MaintenanceDate, CreatedAt) VALUES
(1, 'Limpieza y calibración de cámaras CCTV Mall San Pedro', 1, 75000.00, 'Mantenimiento preventivo trimestral',  @StateEnProgreso, GETUTCDATE(), GETUTCDATE()),
(3, 'Revisión y prueba de sensores alarma Banco Nacional',   6, 45000.00, 'Mantenimiento semestral programado',   @StateEnProgreso, GETUTCDATE(), GETUTCDATE()),
(5, 'Actualización de certificados SSL sitio web Restaurant',4, 25000.00, 'Renovación anual de seguridad',        @StateEnProgreso, GETUTCDATE(), GETUTCDATE());

PRINT '✅ Registros de mantenimiento insertados';
GO

-- ================================
-- STEP 12: PROJECT WARRANTY
-- ================================
-- ✅ FIX: GETDATE() → GETUTCDATE()
-- ✅ FIX: StateId usa 'En progreso' que es de tipo 'Project' ✓
DECLARE @StateEnProgreso INT = (SELECT Id FROM States WHERE StateName = 'En progreso' AND StateType = 'Project');

INSERT INTO ProjectWarranty (IdProject, WarrantyDescription, MadeById, WarrantyCost, StateId, WarrantyDate, CreatedAt) VALUES
(1, 'Reemplazo de cámara defectuosa en entrada principal Mall San Pedro', 1, 120000.00,  @StateEnProgreso, GETUTCDATE(), GETUTCDATE()),
(3, 'Ajuste de sensibilidad en detector sector norte Banco Nacional',     7, NULL,        @StateEnProgreso, GETUTCDATE(), GETUTCDATE()),
(5, 'Corrección de bug en módulo de pagos Restaurant Machu Picchu',      5, NULL,        @StateEnProgreso, GETUTCDATE(), GETUTCDATE());

PRINT '✅ Registros de garantía insertados';
GO

-- ================================
-- STEP 13: DEVICES
-- ================================
-- ✅ FIX: GETDATE() → GETUTCDATE()
INSERT INTO Devices (DeviceId, DeviceName, ProjectId, Location, DeviceType, IpAddress, SerialNumber, FirmwareVersion, Notes, IsActive, CreatedAt) VALUES
('FP001', 'Fingerprint Reader - Mall San Pedro', 1, 'Entrada Principal',   'Fingerprint', '192.168.1.100', 'SN001234', 'v2.1.0', 'Lector principal entrada',     1, GETUTCDATE()),
('FP002', 'Fingerprint Reader - Hotel Presidente',2,'Recepción',           'Fingerprint', '192.168.1.101', 'SN001235', 'v2.1.0', 'Lector recepción hotel',       1, GETUTCDATE()),
('FP003', 'Fingerprint Reader - Banco Nacional',  3,'Entrada Empleados',   'Fingerprint', '192.168.1.102', 'SN001236', 'v2.1.0', 'Control acceso empleados',     1, GETUTCDATE()),
('FP004', 'Fingerprint Reader - UCR',             4,'Facultad Ingeniería', 'Fingerprint', '192.168.1.103', 'SN001237', 'v2.1.0', 'Control acceso estudiantes',   1, GETUTCDATE()),
('FP005', 'Fingerprint Reader - Restaurant',      5,'Entrada Personal',    'Fingerprint', '192.168.1.104', 'SN001238', 'v2.1.0', 'Control acceso personal',      1, GETUTCDATE()),
('FP006', 'Fingerprint Reader - Ministerio',      6,'Entrada Principal',   'Fingerprint', '192.168.1.105', 'SN001239', 'v2.1.0', 'Control acceso visitantes',    1, GETUTCDATE());

PRINT '✅ Dispositivos insertados';
GO

-- ================================
-- STEP 14: SESSIONS
-- ================================
-- ✅ FIX: GETDATE() → GETUTCDATE()
INSERT INTO Sessions (UserId, TokenId, RefreshToken, DeviceInfo, IpAddress, UserAgent, IsActive, CreatedAt, ExpiresAt) VALUES
(1, 'token_admin_001',      'refresh_admin_001',      'Windows 11 - Chrome',  '192.168.1.50', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',              1, GETUTCDATE(), DATEADD(DAY, 1, GETUTCDATE())),
(2, 'token_hr_001',         'refresh_hr_001',         'Windows 10 - Edge',    '192.168.1.51', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',              1, GETUTCDATE(), DATEADD(DAY, 1, GETUTCDATE())),
(3, 'token_pm_001',         'refresh_pm_001',         'macOS - Safari',       '192.168.1.52', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36',        1, GETUTCDATE(), DATEADD(DAY, 1, GETUTCDATE())),
(4, 'token_supervisor_001', 'refresh_supervisor_001', 'Windows 11 - Firefox', '192.168.1.53', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101',        1, GETUTCDATE(), DATEADD(DAY, 1, GETUTCDATE()));

PRINT '✅ Sesiones insertadas';
GO

-- ================================
-- VERIFICACIÓN FINAL
-- ================================
PRINT '';
PRINT '========================================';
PRINT '📊 VERIFICACIÓN DE DATOS INSERTADOS';
PRINT '========================================';

SELECT 'States'             AS Tabla, COUNT(*) AS Registros FROM States
UNION ALL
SELECT 'Roles',                        COUNT(*) FROM Roles
UNION ALL
SELECT 'DocumentType',                 COUNT(*) FROM DocumentType
UNION ALL
SELECT 'InternUsers',                  COUNT(*) FROM InternUsers
UNION ALL
SELECT 'Projects',                     COUNT(*) FROM Projects
UNION ALL
SELECT 'EmployeeInfo',                 COUNT(*) FROM EmployeeInfo
UNION ALL
SELECT 'ProjectsAssigns',              COUNT(*) FROM ProjectsAssigns
UNION ALL
SELECT 'FingerPrint',                  COUNT(*) FROM FingerPrint
UNION ALL
SELECT 'Assistance',                   COUNT(*) FROM Assistance
UNION ALL
SELECT 'AuditRegister',                COUNT(*) FROM AuditRegister
UNION ALL
SELECT 'ProjectMaintenance',           COUNT(*) FROM ProjectMaintenance
UNION ALL
SELECT 'ProjectWarranty',              COUNT(*) FROM ProjectWarranty
UNION ALL
SELECT 'Devices',                      COUNT(*) FROM Devices
UNION ALL
SELECT 'Sessions',                     COUNT(*) FROM Sessions;

PRINT '';
PRINT '✅ Script de datos de prueba v2 ejecutado exitosamente!';
PRINT '📌 Recuerda: ejecuta primero WTB_DB_init_v2.sql antes de este script';
GO