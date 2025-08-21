-- ================================
-- WTB_DB TEST DATA INSERTION - CORREGIDO
-- ================================

USE WTB_DB;
GO

-- Step 1: Insert States (Estados del sistema)
INSERT INTO States (StateName, StateType, Description) VALUES
('Active', 'General', 'Estado activo del sistema'),
('Inactive', 'General', 'Estado inactivo del sistema'),
('Suspended', 'General', 'Estado suspendido temporalmente'),
('In Progress', 'Project', 'Proyecto en progreso'),
('Completed', 'Project', 'Proyecto completado'),
('On Hold', 'Project', 'Proyecto en pausa'),
('Pending', 'Employee', 'Empleado pendiente de aprobación'),
('Terminated', 'Employee', 'Empleado dado de baja');
GO

-- Step 2: Insert Roles (Roles de usuarios internos)
INSERT INTO Roles (RoleName, Description) VALUES
('Administrator', 'Administrador del sistema con acceso completo'),
('HR Manager', 'Gerente de recursos humanos'),
('Project Manager', 'Gerente de proyectos'),
('Supervisor', 'Supervisor de campo'),
('Operator', 'Operador básico del sistema');
GO

-- Step 3: Insert Document Types (Tipos de documentos de Costa Rica)
INSERT INTO DocumentType (DocumentName, Description) VALUES
('Cedula de Identidad', 'Cédula de identidad costarricense'),
('Pasaporte', 'Pasaporte internacional'),
('DIMEX', 'Documento de Identidad Migratoria para Extranjeros'),
('Permiso de Trabajo', 'Permiso de trabajo para extranjeros');
GO

-- Step 4: Insert Internal Users (Usuarios internos del sistema)
-- NOTA: Agregamos CreationDate y StateId explícitamente
INSERT INTO InternUsers (Email, FirstName, LastName, PasswordHash, PasswordSalt, RolId, CreationDate, StateId) VALUES
('admin@wtb.co.cr', 'Carlos', 'Rodríguez', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'salt123abc', 1, GETDATE(), 1),
('hr@wtb.co.cr', 'María', 'González', 'b3a8e0e1f9ab1bfe3a36f231f676f78bb30a519d2b21e6c530c0eee8ebb4a5d0', 'salt456def', 2, GETDATE(), 1),
('pm1@wtb.co.cr', 'José', 'Vargas', 'c2356069e9d1e79ca924378153cfbbfb4d4416b1f99d41a2940bfdb66c5319db', 'salt789ghi', 3, GETDATE(), 1),
('supervisor@wtb.co.cr', 'Ana', 'Mora', 'd4735e3a265e16eee03f59718b9b5d03019c07d8b6c51f90da3a666eec13ab35', 'saltjklmno', 4, GETDATE(), 1);
GO

-- Step 5: Insert Projects (Proyectos de la empresa)
INSERT INTO Projects (ProjectName, StartDate, EndDate, StateId) VALUES
('CCTV Installation - Mall San Pedro', '2024-01-15', '2024-03-30', 5), -- Completed
('Web Design - Hotel Presidente', '2024-02-01', NULL, 4), -- In Progress
('Alarm System - Banco Nacional', '2024-01-20', '2024-04-15', 5), -- Completed
('CCTV Installation - Universidad UCR', '2024-03-01', NULL, 4), -- In Progress
('Web Design - Restaurant Machu Picchu', '2024-02-15', '2024-05-20', 5), -- Completed
('Alarm System - Oficinas Ministerio', '2024-03-15', NULL, 6); -- On Hold
GO

-- Step 6: Insert Employee Information (Información de empleados)
-- NOTA: Agregamos RegisterDate explícitamente
INSERT INTO EmployeeInfo (DocumentNumber, DocumentTypeId, DocumentExpire, FirstName, LastName, PhoneNumber, EmergencyContact, EmergencyContactPhoneNumber, Birthday, RegisterDate, CostPerHour, StateId, Address, IBAN) VALUES
('1-1234-5678', 1, '2029-12-31', 'Roberto', 'Jiménez', '2456-7890', 'Elena Jiménez', '2456-7891', '1985-06-15', GETDATE(), 3500.00, 1, 'San José, Barrio Escalante', 'CR05015202001234567890'),
('2-2345-6789', 1, '2028-11-30', 'Sofía', 'Herrera', '2567-8901', 'Miguel Herrera', '2567-8902', '1990-03-22', GETDATE(), 3200.00, 1, 'Cartago, Centro', 'CR05015202002345678901'),
('P123456789', 2, '2027-08-15', 'Diego', 'Ramírez', '2678-9012', 'Carmen Ramírez', '2678-9013', '1988-11-08', GETDATE(), 3800.00, 1, 'Alajuela, Centro', 'CR05015202003456789012'),
('DIM-12345678', 3, '2026-05-20', 'Isabella', 'Morales', '2789-0123', 'Pedro Morales', '2789-0124', '1992-09-14', GETDATE(), 3000.00, 1, 'Heredia, Mercedes', 'CR05015202004567890123'),
('PT-987654', 4, '2025-12-10', 'Fernando', 'Castro', '2890-1234', 'Lucía Castro', '2890-1235', '1987-01-30', GETDATE(), 3600.00, 1, 'Puntarenas, Centro', 'CR05015202005678901234'),
('1-9876-5432', 1, '2030-06-25', 'Gabriela', 'Solís', '2901-2345', 'Juan Solís', '2901-2346', '1991-07-12', GETDATE(), 3300.00, 1, 'San José, Pavas', 'CR05015202006789012345'),
('2-8765-4321', 1, '2029-09-18', 'Andrés', 'Navarro', '2012-3456', 'Rosa Navarro', '2012-3457', '1989-04-05', GETDATE(), 3700.00, 1, 'Cartago, Paraíso', 'CR05015202007890123456'),
('DIM-87654321', 3, '2026-03-12', 'Valentina', 'Ortega', '2123-4567', 'Mario Ortega', '2123-4568', '1993-12-28', GETDATE(), 2900.00, 1, 'Limón, Centro', 'CR05015202008901234567');
GO

-- Step 7: Insert Project Assignments (Asignaciones de proyectos)
INSERT INTO ProjectsAssigns (EmployeeId, ProjectId, AssignDate, EndDate) VALUES
-- Mall San Pedro CCTV (Completed)
(1, 1, '2024-01-15', '2024-03-30'), -- Roberto
(2, 1, '2024-01-20', '2024-03-30'), -- Sofía
(3, 1, '2024-02-01', '2024-03-30'), -- Diego

-- Hotel Presidente Web Design (In Progress)
(4, 2, '2024-02-01', NULL), -- Isabella
(5, 2, '2024-02-05', NULL), -- Fernando

-- Banco Nacional Alarm (Completed)
(1, 3, '2024-01-20', '2024-04-15'), -- Roberto (works on multiple projects)
(6, 3, '2024-01-25', '2024-04-15'), -- Gabriela
(7, 3, '2024-02-10', '2024-04-15'), -- Andrés

-- UCR CCTV (In Progress)
(2, 4, '2024-03-01', NULL), -- Sofía (works on multiple projects)
(3, 4, '2024-03-05', NULL), -- Diego (works on multiple projects)
(8, 4, '2024-03-10', NULL); -- Valentina
GO

-- Step 8: Insert Fingerprint Data (Datos de huellas dactilares)
-- NOTA: Agregamos IssueDate explícitamente
INSERT INTO FingerPrint (EmployeeId, TemplateFingerPrint, IssueDate) VALUES
(1, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CB, GETDATE()), -- Roberto
(2, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CC, GETDATE()), -- Sofía
(3, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CD, GETDATE()), -- Diego
(4, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CE, GETDATE()), -- Isabella
(5, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CF, GETDATE()), -- Fernando
(6, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5D0, GETDATE()), -- Gabriela
(7, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5D1, GETDATE()), -- Andrés
(8, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5D2, GETDATE()); -- Valentina
GO

-- Step 9: Insert Assistance Records (Registros de asistencia)
-- NOTA: Agregamos CreatedDate explícitamente
INSERT INTO Assistance (EmployeeId, ProjectId, CreatedDate, CheckIn, CheckOut, TotalHours, RegisterType, Notes) VALUES
-- Roberto working on Mall San Pedro CCTV
(1, 1, GETDATE(), '2024-01-15 08:00:00', '2024-01-15 17:00:00', 8.0, 'CheckOut', 'Instalación inicial de cámaras en planta baja'),
(1, 1, GETDATE(), '2024-01-16 08:30:00', '2024-01-16 17:30:00', 8.0, 'CheckOut', 'Configuración del sistema de grabación'),
(1, 1, GETDATE(), '2024-01-17 08:00:00', '2024-01-17 16:00:00', 7.0, 'CheckOut', 'Pruebas del sistema completo'),

-- Roberto working on Banco Nacional Alarm
(1, 3, GETDATE(), '2024-01-22 09:00:00', '2024-01-22 18:00:00', 8.0, 'CheckOut', 'Instalación de sensores perimetrales'),
(1, 3, GETDATE(), '2024-01-23 08:45:00', '2024-01-23 17:45:00', 8.0, 'CheckOut', 'Configuración de central de alarmas'),

-- Sofía working on Mall San Pedro CCTV
(2, 1, GETDATE(), '2024-01-20 08:00:00', '2024-01-20 17:00:00', 8.5, 'CheckOut', 'Cableado estructurado segundo piso'),
(2, 1, GETDATE(), '2024-01-21 08:15:00', '2024-01-21 17:15:00', 8.0, 'CheckOut', 'Instalación cámaras exteriores'),

-- Sofía working on UCR CCTV
(2, 4, GETDATE(), '2024-03-01 08:00:00', '2024-03-01 17:30:00', 8.5, 'CheckOut', 'Levantamiento de requerimientos UCR'),
(2, 4, GETDATE(), '2024-03-02 08:30:00', '2024-03-02 17:00:00', 7.5, 'CheckOut', 'Diseño de layout de cámaras'),

-- Diego working on multiple projects
(3, 1, GETDATE(), '2024-02-01 08:00:00', '2024-02-01 16:30:00', 7.5, 'CheckOut', 'Programación de cámaras IP'),
(3, 4, GETDATE(), '2024-03-05 09:00:00', '2024-03-05 18:15:00', 8.25, 'CheckOut', 'Instalación inicial UCR - Facultad Ingeniería'),

-- Isabella working on web projects
(4, 2, GETDATE(), '2024-02-01 09:00:00', '2024-02-01 18:00:00', 8.0, 'CheckOut', 'Análisis de requerimientos web Hotel Presidente'),
(4, 5, GETDATE(), '2024-02-15 08:30:00', '2024-02-15 17:30:00', 8.0, 'CheckOut', 'Diseño UI/UX Restaurant Machu Picchu'),

-- Fernando working on web projects
(5, 2, GETDATE(), '2024-02-05 08:00:00', '2024-02-05 17:45:00', 8.75, 'CheckOut', 'Desarrollo backend Hotel Presidente'),
(5, 5, GETDATE(), '2024-02-20 09:15:00', '2024-02-20 18:00:00', 7.75, 'CheckOut', 'Implementación sistema reservas restaurant'),

-- Gabriela working on alarm systems
(6, 3, GETDATE(), '2024-01-25 08:00:00', '2024-01-25 17:00:00', 8.0, 'CheckOut', 'Instalación detectores de movimiento'),
(6, 6, GETDATE(), '2024-03-15 08:30:00', '2024-03-15 17:30:00', 8.0, 'CheckOut', 'Evaluación sitio Ministerio'),

-- Andrés working on alarm systems
(7, 3, GETDATE(), '2024-02-10 08:15:00', '2024-02-10 17:15:00', 8.0, 'CheckOut', 'Configuración panel central Banco Nacional'),
(7, 6, GETDATE(), '2024-03-20 09:00:00', '2024-03-20 18:00:00', 8.0, 'CheckOut', 'Diseño sistema seguridad Ministerio'),

-- Valentina working on UCR
(8, 4, GETDATE(), '2024-03-10 08:00:00', '2024-03-10 16:45:00', 7.75, 'CheckOut', 'Soporte técnico instalación UCR'),

-- Some current check-ins (employees currently working)
(2, 4, GETDATE(), '2024-07-31 08:00:00', NULL, NULL, 'CheckIn', 'Continuando trabajo UCR - edificio administrativo'),
(4, 2, GETDATE(), '2024-07-31 09:00:00', NULL, NULL, 'CheckIn', 'Desarrollo módulo reservas Hotel Presidente');
GO

-- Step 10: Insert Audit Records (Registros de auditoría)
-- NOTA: Agregamos ActionDate explícitamente
INSERT INTO AuditRegister (AssistanceId, ActionType, DetailChange, ActionDate, AdminId) VALUES
(1, 'CREATE', 'Registro de asistencia creado: Roberto Jiménez - Mall San Pedro CCTV - 2024-01-15', GETDATE(), 1),
(2, 'CREATE', 'Registro de asistencia creado: Roberto Jiménez - Mall San Pedro CCTV - 2024-01-16', GETDATE(), 1),
(5, 'UPDATE', 'Modificación de horas: cambio de 7.5 a 8.0 horas por aprobación supervisor', GETDATE(), 3),
(8, 'CREATE', 'Registro de asistencia creado: Sofía Herrera - UCR CCTV - 2024-03-01', GETDATE(), 2),
(12, 'UPDATE', 'Corrección de hora de salida: 17:30 a 18:00 por tiempo extra aprobado', GETDATE(), 1);
GO

-- Step 11: Insert Project Maintenance Records (Registros de mantenimiento)
-- NOTA: Agregamos MaintenanceDate explícitamente
INSERT INTO ProjectMaintenance (IdProject, MaintenanceDescription, MadeById, MaintenanceCost, AdditionalInfo, StateId, MaintenanceDate) VALUES
(1, 'Limpieza y calibración de cámaras CCTV Mall San Pedro', 1, 75000.00, 'Mantenimiento preventivo trimestral', 1, GETDATE()),
(3, 'Revisión y prueba de sensores alarma Banco Nacional', 6, 45000.00, 'Mantenimiento semestral programado', 1, GETDATE()),
(5, 'Actualización de certificados SSL sitio web Restaurant', 4, 25000.00, 'Renovación anual de seguridad', 1, GETDATE());
GO

-- Step 12: Insert Project Warranty Records (Registros de garantía)
-- NOTA: Agregamos WarrantyDate explícitamente
INSERT INTO ProjectWarranty (IdProject, WarrantyDescription, MadeById, WarrantyCost, StateId, WarrantyDate) VALUES
(1, 'Reemplazo de cámara defectuosa en entrada principal Mall San Pedro', 1, 120000.00, 1, GETDATE()),
(3, 'Ajuste de sensibilidad en detector sector norte Banco Nacional', 7, NULL, 1, GETDATE()),
(5, 'Corrección de bug en módulo de pagos Restaurant Machu Picchu', 5, NULL, 1, GETDATE());
GO

-- Step 13: Insert Devices (Dispositivos biométricos)
-- NOTA: Agregamos CreatedDate e IsActive explícitamente
INSERT INTO Devices (DeviceId, DeviceName, ProjectId, Location, DeviceType, IpAddress, SerialNumber, FirmwareVersion, Notes, IsActive, CreatedDate) VALUES
('FP001', 'Fingerprint Reader - Mall San Pedro', 1, 'Entrada Principal', 'Fingerprint', '192.168.1.100', 'SN001234', 'v2.1.0', 'Lector principal entrada', 1, GETDATE()),
('FP002', 'Fingerprint Reader - Hotel Presidente', 2, 'Recepción', 'Fingerprint', '192.168.1.101', 'SN001235', 'v2.1.0', 'Lector recepción hotel', 1, GETDATE()),
('FP003', 'Fingerprint Reader - Banco Nacional', 3, 'Entrada Empleados', 'Fingerprint', '192.168.1.102', 'SN001236', 'v2.1.0', 'Control acceso empleados', 1, GETDATE()),
('FP004', 'Fingerprint Reader - UCR', 4, 'Facultad Ingeniería', 'Fingerprint', '192.168.1.103', 'SN001237', 'v2.1.0', 'Control acceso estudiantes', 1, GETDATE()),
('FP005', 'Fingerprint Reader - Restaurant', 5, 'Entrada Personal', 'Fingerprint', '192.168.1.104', 'SN001238', 'v2.1.0', 'Control acceso personal', 1, GETDATE()),
('FP006', 'Fingerprint Reader - Ministerio', 6, 'Entrada Principal', 'Fingerprint', '192.168.1.105', 'SN001239', 'v2.1.0', 'Control acceso visitantes', 1, GETDATE());
GO

-- Step 14: Insert Sessions (Sesiones de usuario)
-- NOTA: Agregamos CreatedAt y ExpiresAt explícitamente
INSERT INTO Sessions (UserId, TokenId, RefreshToken, DeviceInfo, IpAddress, UserAgent, IsActive, CreatedAt, ExpiresAt) VALUES
(1, 'token_admin_001', 'refresh_admin_001', 'Windows 11 - Chrome', '192.168.1.50', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 1, GETDATE(), DATEADD(day, 1, GETDATE())),
(2, 'token_hr_001', 'refresh_hr_001', 'Windows 10 - Edge', '192.168.1.51', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 1, GETDATE(), DATEADD(day, 1, GETDATE())),
(3, 'token_pm_001', 'refresh_pm_001', 'macOS - Safari', '192.168.1.52', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36', 1, GETDATE(), DATEADD(day, 1, GETDATE())),
(4, 'token_supervisor_001', 'refresh_supervisor_001', 'Windows 11 - Firefox', '192.168.1.53', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101', 1, GETDATE(), DATEADD(day, 1, GETDATE()));
GO

PRINT '✅ Script de datos de prueba corregido ejecutado exitosamente!';
PRINT '📊 Todos los campos obligatorios han sido incluidos';
PRINT '🔗 Las Foreign Keys respetan el orden correcto de inserción';
GO
