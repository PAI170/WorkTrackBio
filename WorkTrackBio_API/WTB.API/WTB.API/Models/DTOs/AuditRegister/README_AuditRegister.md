# 🔍 DTOs de AuditRegister - Sistema de Auditoría

## 📋 **DESCRIPCIÓN GENERAL**

El sistema de `AuditRegister` permite rastrear y registrar todos los cambios administrativos realizados en las asistencias del sistema. Cada modificación, corrección o ajuste queda documentado para auditoría y cumplimiento.

## 🔄 **FLUJO DE AUDITORÍA**

### **1. Creación de Registro de Auditoría**
```csharp
CreateAuditRegisterDto
├── AssistanceId       - ID de la asistencia modificada
├── ActionType         - Tipo de acción realizada
├── DetailChange       - Detalle específico del cambio
├── AdminId            - ID del administrador que realizó el cambio
└── ActionDate         - Fecha/hora de la acción (opcional, por defecto UTC)
```

### **2. Tipos de Acción Comunes**
- **"TIME_CORRECTION"** - Corrección de horarios
- **"MANUAL_ENTRY"** - Entrada manual de asistencia
- **"ADMIN_OVERRIDE"** - Sobrescritura administrativa
- **"DATA_FIX"** - Corrección de datos
- **"SYSTEM_ADJUSTMENT"** - Ajuste del sistema

## 📊 **DTOs IMPLEMENTADOS**

### **CreateAuditRegisterDto**
- **Propósito:** Crear nuevos registros de auditoría
- **Validaciones:** 
  - AssistanceId y AdminId deben ser números válidos
  - ActionType máximo 50 caracteres
  - DetailChange máximo 1000 caracteres
  - ActionDate opcional (por defecto UTC actual)

### **AuditRegisterDetailsDto**
- **Propósito:** Mostrar información completa del registro
- **Incluye:** 
  - Datos del registro de auditoría
  - Información de la asistencia relacionada
  - Datos del administrador que realizó el cambio

### **AuditRegisterListDto**
- **Propósito:** Listados optimizados para performance
- **Campos:** Información esencial para tablas y reportes

### **AuditRegisterResponseDto**
- **Propósito:** Respuesta estándar de la API
- **Uso:** Para operaciones CRUD básicas

### **AuditRegisterFilterDto**
- **Propósito:** Filtros avanzados para búsquedas
- **Filtros disponibles:**
  - **Por fecha:** StartDate, EndDate
  - **Por acción:** ActionType
  - **Por asistencia:** AssistanceId, EmployeeId, ProjectId
  - **Por administrador:** AdminId
  - **Texto libre:** SearchText
  - **Paginación:** PageSize, PageNumber

## 🎯 **CASOS DE USO**

### **1. Corrección de Horarios**
```json
{
    "assistanceId": 123,
    "actionType": "TIME_CORRECTION",
    "detailChange": "Corrección de check-in de 8:00 AM a 7:45 AM por llegada temprana",
    "adminId": 456
}
```

### **2. Entrada Manual de Asistencia**
```json
{
    "assistanceId": 124,
    "actionType": "MANUAL_ENTRY",
    "detailChange": "Registro manual de asistencia para empleado que olvidó marcar entrada",
    "adminId": 456
}
```

### **3. Búsqueda de Cambios por Empleado**
```json
{
    "employeeId": 789,
    "startDate": "2024-01-01T00:00:00Z",
    "endDate": "2024-01-31T23:59:59Z",
    "pageSize": 50,
    "pageNumber": 1
}
```

## 🔐 **SEGURIDAD Y VALIDACIONES**

- **Auditoría completa:** Todos los cambios quedan registrados
- **Traza de responsabilidad:** Se registra quién hizo cada cambio
- **Timestamps precisos:** Fechas en UTC para consistencia
- **Validación de datos:** Campos requeridos y longitudes máximas
- **Integridad referencial:** AssistanceId y AdminId deben existir

## 📈 **REPORTES Y CONSULTAS**

### **Reportes Comunes:**
1. **Cambios por administrador** - Para responsabilidad
2. **Cambios por período** - Para auditorías mensuales
3. **Cambios por tipo de acción** - Para análisis de patrones
4. **Cambios por empleado** - Para seguimiento individual
5. **Cambios por proyecto** - Para control de proyectos

### **Consultas de Auditoría:**
- **Cumplimiento:** Verificar que todos los cambios estén documentados
- **Responsabilidad:** Identificar quién realizó cada modificación
- **Patrones:** Analizar tipos de cambios más comunes
- **Tendencias:** Identificar períodos con más modificaciones

## 🛠️ **INTEGRACIÓN CON EL SISTEMA**

### **Automático:**
- El sistema crea registros automáticamente en operaciones críticas
- Se registran cambios en asistencia, empleados y proyectos

### **Manual:**
- Los administradores pueden crear registros manuales
- Útil para cambios realizados fuera del sistema

### **API Endpoints:**
- `POST /api/audit` - Crear registro de auditoría
- `GET /api/audit` - Listar con filtros
- `GET /api/audit/{id}` - Obtener detalles
- `GET /api/audit/reports` - Reportes especializados

## 📝 **EJEMPLOS DE IMPLEMENTACIÓN**

### **Crear Registro de Auditoría:**
```csharp
var auditDto = new CreateAuditRegisterDto(
    AssistanceId: 123,
    ActionType: "TIME_CORRECTION",
    DetailChange: "Corrección de horario por error del sistema",
    AdminId: 456
);

var result = await _auditService.CreateAuditRegisterAsync(auditDto);
```

### **Filtrar Registros:**
```csharp
var filter = new AuditRegisterFilterDto(
    StartDate: DateTime.Today.AddDays(-30),
    ActionType: "TIME_CORRECTION",
    PageSize: 100
);

var audits = await _auditService.GetAuditRegistersAsync(filter);
```

## 🔄 **PRÓXIMOS PASOS**

1. **Implementar validadores** con FluentValidation
2. **Crear controlador** AuditRegisterController
3. **Implementar servicios** de negocio
4. **Agregar endpoints** de reportes especializados
5. **Integrar con sistema** de notificaciones
