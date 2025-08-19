-- ================================
-- WTB_DB TEST DATA INSERTION
-- ================================

USE WTB_DB;
GO

-- Step 1: Insert States (Estados del sistema)
-- These are the different states that entities can have in the system
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
-- Different roles for internal system users
INSERT INTO Roles (RoleName, Description) VALUES
('Administrator', 'Administrador del sistema con acceso completo'),
('HR Manager', 'Gerente de recursos humanos'),
('Project Manager', 'Gerente de proyectos'),
('Supervisor', 'Supervisor de campo'),
('Operator', 'Operador básico del sistema');
GO

-- Step 3: Insert Document Types (Tipos de documentos de Costa Rica)
-- Document types used in Costa Rica for employee identification
INSERT INTO DocumentType (DocumentName, Description) VALUES
('Cedula de Identidad', 'Cédula de identidad costarricense'),
('Pasaporte', 'Pasaporte internacional'),
('DIMEX', 'Documento de Identidad Migratoria para Extranjeros'),
('Permiso de Trabajo', 'Permiso de trabajo para extranjeros');
GO

-- Step 4: Insert Internal Users (Usuarios internos del sistema)
-- Internal system users with different roles
INSERT INTO InternUsers (Email, FirstName, LastName, PasswordHash, PasswordSalt, RolId, StateId) VALUES
('admin@wtb.co.cr', 'Carlos', 'Rodríguez', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 'salt123abc', 1, 1),
('hr@wtb.co.cr', 'María', 'González', 'b3a8e0e1f9ab1bfe3a36f231f676f78bb30a519d2b21e6c530c0eee8ebb4a5d0', 'salt456def', 2, 1),
('pm1@wtb.co.cr', 'José', 'Vargas', 'c2356069e9d1e79ca924378153cfbbfb4d4416b1f99d41a2940bfdb66c5319db', 'salt789ghi', 3, 1),
('supervisor@wtb.co.cr', 'Ana', 'Mora', 'd4735e3a265e16eee03f59718b9b5d03019c07d8b6c51f90da3a666eec13ab35', 'saltjklmno', 4, 1);
GO

-- Step 5: Insert Projects (Proyectos de la empresa)
-- Different types of projects the company handles
INSERT INTO Projects (ProjectName, StartDate, EndDate, StateId) VALUES
('CCTV Installation - Mall San Pedro', '2024-01-15', '2024-03-30', 5), -- Completed
('Web Design - Hotel Presidente', '2024-02-01', NULL, 4), -- In Progress
('Alarm System - Banco Nacional', '2024-01-20', '2024-04-15', 5), -- Completed
('CCTV Installation - Universidad UCR', '2024-03-01', NULL, 4), -- In Progress
('Web Design - Restaurant Machu Picchu', '2024-02-15', '2024-05-20', 5), -- Completed
('Alarm System - Oficinas Ministerio', '2024-03-15', NULL, 6); -- On Hold
GO

-- Step 6: Insert Employee Information (Información de empleados)
-- Employee data with Costa Rican document formats
INSERT INTO EmployeeInfo (DocumentNumber, DocumentTypeId, DocumentExpire, FirstName, LastName, PhoneNumber, EmergencyContact, EmergencyContactPhoneNumber, Birthday, CostPerHour, StateId, Address, IBAN) VALUES
('1-1234-5678', 1, '2029-12-31', 'Roberto', 'Jiménez', '2456-7890', 'Elena Jiménez', '2456-7891', '1985-06-15', 3500.00, 1, 'San José, Barrio Escalante', 'CR05015202001234567890'),
('2-2345-6789', 1, '2028-11-30', 'Sofía', 'Herrera', '2567-8901', 'Miguel Herrera', '2567-8902', '1990-03-22', 3200.00, 1, 'Cartago, Centro', 'CR05015202002345678901'),
('P123456789', 2, '2027-08-15', 'Diego', 'Ramírez', '2678-9012', 'Carmen Ramírez', '2678-9013', '1988-11-08', 3800.00, 1, 'Alajuela, Centro', 'CR05015202003456789012'),
('DIM-12345678', 3, '2026-05-20', 'Isabella', 'Morales', '2789-0123', 'Pedro Morales', '2789-0124', '1992-09-14', 3000.00, 1, 'Heredia, Mercedes', 'CR05015202004567890123'),
('PT-987654', 4, '2025-12-10', 'Fernando', 'Castro', '2890-1234', 'Lucía Castro', '2890-1235', '1987-01-30', 3600.00, 1, 'Puntarenas, Centro', 'CR05015202005678901234'),
('1-9876-5432', 1, '2030-06-25', 'Gabriela', 'Solís', '2901-2345', 'Juan Solís', '2901-2346', '1991-07-12', 3300.00, 1, 'San José, Pavas', 'CR05015202006789012345'),
('2-8765-4321', 1, '2029-09-18', 'Andrés', 'Navarro', '2012-3456', 'Rosa Navarro', '2012-3457', '1989-04-05', 3700.00, 1, 'Cartago, Paraíso', 'CR05015202007890123456'),
('DIM-87654321', 3, '2026-03-12', 'Valentina', 'Ortega', '2123-4567', 'Mario Ortega', '2123-4568', '1993-12-28', 2900.00, 1, 'Limón, Centro', 'CR05015202008901234567');
GO

-- Step 7: Insert Project Assignments (Asignaciones de proyectos)
-- Assign employees to different projects (some employees work on multiple projects)
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
(8, 4, '2024-03-10', NULL), -- Valentina

-- Restaurant Web Design (Completed)
(4, 5, '2024-02-15', '2024-05-20'), -- Isabella (works on multiple projects)
(5, 5, '2024-02-20', '2024-05-20'), -- Fernando (works on multiple projects)

-- Ministerio Alarm (On Hold)
(6, 6, '2024-03-15', NULL), -- Gabriela (works on multiple projects)
(7, 6, '2024-03-20', NULL); -- Andrés (works on multiple projects)
GO

-- Step 8: Insert Fingerprint Data (Datos de huellas dactilares)
-- Simulated fingerprint templates (in real life these would be actual biometric data)
INSERT INTO FingerPrint (EmployeeId, TemplateFingerPrint) VALUES
(1, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CB), -- Roberto
(2, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CC), -- Sofía
(3, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CD), -- Diego
(4, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CE), -- Isabella
(5, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5CF), -- Fernando
(6, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5D0), -- Gabriela
(7, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5D1), -- Andrés
(8, 0x89504E470D0A1A0A0000000D49484452000000640000006408060000007017A5D2); -- Valentina
GO

-- Step 9: Insert Assistance Records (Registros de asistencia)
-- Time tracking records for employees on different projects
INSERT INTO Assistance (EmployeeId, ProjectId, CheckIn, CheckOut, TotalHours, RegisterType, Notes) VALUES
-- Roberto working on Mall San Pedro CCTV
(1, 1, '2024-01-15 08:00:00', '2024-01-15 17:00:00', 8.0, 'CheckOut', 'Instalación inicial de cámaras en planta baja'),
(1, 1, '2024-01-16 08:30:00', '2024-01-16 17:30:00', 8.0, 'CheckOut', 'Configuración del sistema de grabación'),
(1, 1, '2024-01-17 08:00:00', '2024-01-17 16:00:00', 7.0, 'CheckOut', 'Pruebas del sistema completo'),

-- Roberto working on Banco Nacional Alarm
(1, 3, '2024-01-22 09:00:00', '2024-01-22 18:00:00', 8.0, 'CheckOut', 'Instalación de sensores perimetrales'),
(1, 3, '2024-01-23 08:45:00', '2024-01-23 17:45:00', 8.0, 'CheckOut', 'Configuración de central de alarmas'),

-- Sofía working on Mall San Pedro CCTV
(2, 1, '2024-01-20 08:00:00', '2024-01-20 17:00:00', 8.5, 'CheckOut', 'Cableado estructurado segundo piso'),
(2, 1, '2024-01-21 08:15:00', '2024-01-21 17:15:00', 8.0, 'CheckOut', 'Instalación cámaras exteriores'),

-- Sofía working on UCR CCTV
(2, 4, '2024-03-01 08:00:00', '2024-03-01 17:30:00', 8.5, 'CheckOut', 'Levantamiento de requerimientos UCR'),
(2, 4, '2024-03-02 08:30:00', '2024-03-02 17:00:00', 7.5, 'CheckOut', 'Diseño de layout de cámaras'),

-- Diego working on multiple projects
(3, 1, '2024-02-01 08:00:00', '2024-02-01 16:30:00', 7.5, 'CheckOut', 'Programación de cámaras IP'),
(3, 4, '2024-03-05 09:00:00', '2024-03-05 18:15:00', 8.25, 'CheckOut', 'Instalación inicial UCR - Facultad Ingeniería'),

-- Isabella working on web projects
(4, 2, '2024-02-01 09:00:00', '2024-02-01 18:00:00', 8.0, 'CheckOut', 'Análisis de requerimientos web Hotel Presidente'),
(4, 5, '2024-02-15 08:30:00', '2024-02-15 17:30:00', 8.0, 'CheckOut', 'Diseño UI/UX Restaurant Machu Picchu'),

-- Fernando working on web projects
(5, 2, '2024-02-05 08:00:00', '2024-02-05 17:45:00', 8.75, 'CheckOut', 'Desarrollo backend Hotel Presidente'),
(5, 5, '2024-02-20 09:15:00', '2024-02-20 18:00:00', 7.75, 'CheckOut', 'Implementación sistema reservas restaurant'),

-- Gabriela working on alarm systems
(6, 3, '2024-01-25 08:00:00', '2024-01-25 17:00:00', 8.0, 'CheckOut', 'Instalación detectores de movimiento'),
(6, 6, '2024-03-15 08:30:00', '2024-03-15 17:30:00', 8.0, 'CheckOut', 'Evaluación sitio Ministerio'),

-- Andrés working on alarm systems
(7, 3, '2024-02-10 08:15:00', '2024-02-10 17:15:00', 8.0, 'CheckOut', 'Configuración panel central Banco Nacional'),
(7, 6, '2024-03-20 09:00:00', '2024-03-20 18:00:00', 8.0, 'CheckOut', 'Diseño sistema seguridad Ministerio'),

-- Valentina working on UCR
(8, 4, '2024-03-10 08:00:00', '2024-03-10 16:45:00', 7.75, 'CheckOut', 'Soporte técnico instalación UCR'),

-- Some current check-ins (employees currently working)
(2, 4, '2024-07-31 08:00:00', NULL, NULL, 'CheckIn', 'Continuando trabajo UCR - edificio administrativo'),
(4, 2, '2024-07-31 09:00:00', NULL, NULL, 'CheckIn', 'Desarrollo módulo reservas Hotel Presidente');
GO

-- Step 10: Insert Audit Records (Registros de auditoría)
-- Audit trail for assistance modifications
INSERT INTO AuditRegister (AssistanceId, ActionType, DetailChange, AdminId) VALUES
(1, 'CREATE', 'Registro de asistencia creado: Roberto Jiménez - Mall San Pedro CCTV - 2024-01-15', 1),
(2, 'CREATE', 'Registro de asistencia creado: Roberto Jiménez - Mall San Pedro CCTV - 2024-01-16', 1),
(5, 'UPDATE', 'Modificación de horas: cambio de 7.5 a 8.0 horas por aprobación supervisor', 3),
(8, 'CREATE', 'Registro de asistencia creado: Sofía Herrera - UCR CCTV - 2024-03-01', 2),
(12, 'UPDATE', 'Corrección de hora de salida: 17:30 a 18:00 por tiempo extra aprobado', 1);
GO

-- Step 11: Insert Project Maintenance Records (Registros de mantenimiento)
-- Maintenance activities for completed projects
INSERT INTO ProjectMaintenance (IdProject, MaintenanceDescription, MadeById, MaintenanceCost, AdditionalInfo, StateId) VALUES
(1, 'Limpieza y calibración de cámaras CCTV Mall San Pedro', 1, 75000.00, 'Mantenimiento preventivo trimestral', 1),
(3, 'Revisión y prueba de sensores alarma Banco Nacional', 6, 45000.00, 'Mantenimiento semestral programado', 1),
(5, 'Actualización de certificados SSL sitio web Restaurant', 4, 25000.00, 'Renovación anual de seguridad', 1);
GO

-- Step 12: Insert Project Warranty Records (Registros de garantía)
-- Warranty services provided for projects
INSERT INTO ProjectWarranty (IdProject, WarrantyDescription, MadeById, WarrantyCost, StateId) VALUES
(1, 'Reemplazo de cámara defectuosa en entrada principal Mall San Pedro', 1, 120000.00, 1),
(3, 'Ajuste de sensibilidad en detector sector norte Banco Nacional', 7, NULL, 1),
(5, 'Corrección de bug en módulo de pagos Restaurant Machu Picchu', 5, NULL, 1);
GO

-- ================================
-- VERIFICATION QUERIES
-- ================================

PRINT '✅ Datos de prueba insertados exitosamente';
PRINT '';
PRINT '📊 RESUMEN DE DATOS INSERTADOS:';

SELECT 'States' as Tabla, COUNT(*) as Registros FROM States
UNION ALL
SELECT 'Roles', COUNT(*) FROM Roles
UNION ALL
SELECT 'DocumentType', COUNT(*) FROM DocumentType
UNION ALL
SELECT 'InternUsers', COUNT(*) FROM InternUsers
UNION ALL
SELECT 'Projects', COUNT(*) FROM Projects
UNION ALL
SELECT 'EmployeeInfo', COUNT(*) FROM EmployeeInfo
UNION ALL
SELECT 'ProjectsAssigns', COUNT(*) FROM ProjectsAssigns
UNION ALL
SELECT 'FingerPrint', COUNT(*) FROM FingerPrint
UNION ALL
SELECT 'Assistance', COUNT(*) FROM Assistance
UNION ALL
SELECT 'AuditRegister', COUNT(*) FROM AuditRegister
UNION ALL
SELECT 'ProjectMaintenance', COUNT(*) FROM ProjectMaintenance
UNION ALL
SELECT 'ProjectWarranty', COUNT(*) FROM ProjectWarranty;

PRINT '';
PRINT '👥 EMPLEADOS QUE TRABAJAN EN MÚLTIPLES PROYECTOS:';

SELECT 
    e.FirstName + ' ' + e.LastName as Empleado,
    COUNT(DISTINCT pa.ProjectId) as CantidadProyectos,
    STRING_AGG(p.ProjectName, ', ') as Proyectos
FROM EmployeeInfo e
JOIN ProjectsAssigns pa ON e.Id = pa.EmployeeId
JOIN Projects p ON pa.ProjectId = p.Id
GROUP BY e.Id, e.FirstName, e.LastName
HAVING COUNT(DISTINCT pa.ProjectId) > 1
ORDER BY CantidadProyectos DESC;

PRINT '';
PRINT '📈 HORAS TRABAJADAS POR PROYECTO:';

SELECT 
    p.ProjectName as Proyecto,
    COUNT(a.Id) as RegistrosAsistencia,
    SUM(ISNULL(a.TotalHours, 0)) as TotalHoras,
    COUNT(DISTINCT a.EmployeeId) as EmpleadosAsignados
FROM Projects p
LEFT JOIN Assistance a ON p.Id = a.ProjectId
GROUP BY p.Id, p.ProjectName
ORDER BY TotalHoras DESC;

-- =====================================================
-- DATOS DE PRUEBA PARA DISPOSITIVOS BIOMÉTRICOS
-- =====================================================

PRINT '';
PRINT '📱 Insertando datos de prueba para dispositivos biométricos...';

-- Insertar dispositivos de prueba para diferentes proyectos
IF NOT EXISTS (SELECT * FROM [dbo].[Devices] WHERE [DeviceId] = 'DEV001')
BEGIN
    INSERT INTO [dbo].[Devices] ([DeviceId], [DeviceName], [ProjectId], [Location], [DeviceType], [IpAddress], [SerialNumber], [FirmwareVersion], [Notes]) VALUES 
        ('DEV001', 'Lector Principal - Edificio A', 1, 'Entrada Principal - Edificio A', 'Fingerprint', '192.168.1.100', 'SN001234567', 'v2.1.5', 'Dispositivo principal para empleados - Mall San Pedro'),
        ('DEV002', 'Lector Secundario - Edificio B', 1, 'Entrada Secundaria - Edificio B', 'Fingerprint', '192.168.1.101', 'SN001234568', 'v2.1.5', 'Dispositivo para visitantes - Mall San Pedro'),
        ('DEV003', 'Lector RFID - Estacionamiento', 1, 'Entrada Estacionamiento', 'RFID', '192.168.1.102', 'SN001234569', 'v1.8.2', 'Control de acceso vehicular - Mall San Pedro'),
        ('DEV004', 'Lector Facial - Recepción', 2, 'Recepción Principal', 'Facial', '192.168.1.103', 'SN001234570', 'v3.0.1', 'Identificación facial para ejecutivos - Hotel Presidente'),
        ('DEV005', 'Lector Backup - Almacén', 2, 'Entrada Almacén', 'Fingerprint', '192.168.1.104', 'SN001234571', 'v2.1.5', 'Dispositivo de respaldo - Hotel Presidente'),
        ('DEV006', 'Lector Principal - Banco', 3, 'Entrada Principal Banco', 'Fingerprint', '192.168.1.105', 'SN001234572', 'v2.1.5', 'Control de acceso principal - Banco Nacional'),
        ('DEV007', 'Lector Secundario - Banco', 3, 'Entrada Empleados Banco', 'Fingerprint', '192.168.1.106', 'SN001234573', 'v2.1.5', 'Acceso personal autorizado - Banco Nacional'),
        ('DEV008', 'Lector UCR - Facultad Ingeniería', 4, 'Entrada Facultad Ingeniería', 'Fingerprint', '192.168.1.107', 'SN001234574', 'v2.1.5', 'Control de acceso estudiantes - UCR'),
        ('DEV009', 'Lector UCR - Biblioteca', 4, 'Entrada Biblioteca Central', 'Fingerprint', '192.168.1.108', 'SN001234575', 'v2.1.5', 'Acceso a biblioteca - UCR'),
        ('DEV010', 'Lector Restaurant - Entrada', 5, 'Entrada Principal Restaurant', 'Fingerprint', '192.168.1.109', 'SN001234576', 'v2.1.5', 'Control de acceso personal - Restaurant Machu Picchu'),
        ('DEV011', 'Lector Ministerio - Recepción', 6, 'Recepción Ministerio', 'Fingerprint', '192.168.1.110', 'SN001234577', 'v2.1.5', 'Control de acceso visitantes - Ministerio'),
        ('DEV012', 'Lector Ministerio - Personal', 6, 'Entrada Personal Ministerio', 'Fingerprint', '192.168.1.111', 'SN001234578', 'v2.1.5', 'Acceso personal autorizado - Ministerio');
    
    PRINT '✅ Datos de prueba para Devices insertados exitosamente';
END
ELSE
BEGIN
    PRINT 'ℹ️ Los datos de prueba para Devices ya existen';
END

-- =====================================================
-- VERIFICACIÓN FINAL COMPLETA
-- =====================================================

PRINT '';
PRINT '========================================';
PRINT 'VERIFICACIÓN FINAL COMPLETA';
PRINT '========================================';

-- Verificar todas las tablas incluyendo Devices
PRINT '';
PRINT '📋 RESUMEN COMPLETO DE DATOS:';

SELECT 'States' as Tabla, COUNT(*) as Registros FROM States
UNION ALL
SELECT 'Roles', COUNT(*) FROM Roles
UNION ALL
SELECT 'DocumentType', COUNT(*) FROM DocumentType
UNION ALL
SELECT 'InternUsers', COUNT(*) FROM InternUsers
UNION ALL
SELECT 'Projects', COUNT(*) FROM Projects
UNION ALL
SELECT 'EmployeeInfo', COUNT(*) FROM EmployeeInfo
UNION ALL
SELECT 'ProjectsAssigns', COUNT(*) FROM ProjectsAssigns
UNION ALL
SELECT 'FingerPrint', COUNT(*) FROM FingerPrint
UNION ALL
SELECT 'Assistance', COUNT(*) FROM Assistance
UNION ALL
SELECT 'AuditRegister', COUNT(*) FROM AuditRegister
UNION ALL
SELECT 'ProjectMaintenance', COUNT(*) FROM ProjectMaintenance
UNION ALL
SELECT 'ProjectWarranty', COUNT(*) FROM ProjectWarranty
UNION ALL
SELECT 'Devices', COUNT(*) FROM Devices;

PRINT '';
PRINT '📱 DISPOSITIVOS POR PROYECTO:';

SELECT 
    p.ProjectName as Proyecto,
    COUNT(d.Id) as CantidadDispositivos,
    STRING_AGG(d.DeviceType, ', ') as TiposDispositivos
FROM Projects p
LEFT JOIN Devices d ON p.Id = d.ProjectId
GROUP BY p.Id, p.ProjectName
ORDER BY CantidadDispositivos DESC;

PRINT '';
PRINT '🔒 TIPOS DE DISPOSITIVOS INSTALADOS:';

SELECT 
    DeviceType as TipoDispositivo,
    COUNT(*) as Cantidad,
    STRING_AGG(DeviceName, '; ') as Dispositivos
FROM Devices
GROUP BY DeviceType
ORDER BY Cantidad DESC;

PRINT '';
PRINT '✅ TODOS LOS DATOS DE PRUEBA HAN SIDO INSERTADOS EXITOSAMENTE!';
PRINT '🚀 La base de datos está completamente configurada y poblada con datos de prueba';

GO