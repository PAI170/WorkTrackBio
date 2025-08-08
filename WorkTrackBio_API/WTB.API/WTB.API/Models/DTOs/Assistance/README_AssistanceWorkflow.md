# 🕐 DTOs de Assistance - Control de Tiempo

## 📋 **FLUJO DE TRABAJO**

### **1. Check-In (Entrada al trabajo)**
```csharp
CheckInDto
├── EmployeeId       - ID del empleado
├── ProjectId        - ID del proyecto a trabajar
├── CheckIn          - Fecha/hora de entrada
└── Notes            - Notas opcionales
```

### **2. Check-Out (Salida del trabajo)**
```csharp
CheckOutDto
├── AssistanceId     - ID del registro de asistencia
├── CheckOut         - Fecha/hora de salida
└── Notes            - Notas opcionales del checkout
```

### **3. Correcciones Administrativas**
```csharp
UpdateAssistanceDto
├── Id               - ID del registro
├── EmployeeId       - ID del empleado
├── ProjectId        - ID del proyecto
├── CheckIn          - Fecha/hora entrada (corregida)
├── CheckOut         - Fecha/hora salida (corregida)
├── TotalHours       - Horas totales trabajadas
├── Notes            - Notas administrativas
└── RegisterType     - Tipo de registro (Manual, etc.)
```

## 🔄 **PROCESO TÍPICO**

```
1. 📱 Empleado hace CHECK-IN  →  CheckInDto
2. ⏰ Sistema registra entrada
3. 💼 Empleado trabaja en el proyecto
4. 📱 Empleado hace CHECK-OUT  →  CheckOutDto
5. 📊 Sistema calcula horas automáticamente
6. 👨‍💼 Admin puede hacer correcciones  →  UpdateAssistanceDto
```

## ⚡ **VALIDACIONES ESPECÍFICAS**

### **CheckIn:**
- ✅ Horario laboral: 5:00 AM - 11:00 PM
- ✅ Máximo 7 días atrás (correcciones)
- ✅ Máximo 30 min diferencia tiempo real
- ✅ Sin check-in activo previo

### **CheckOut:**
- ✅ Horario extendido: 5:00 AM - 11:59 PM
- ✅ Máximo 1 día atrás
- ✅ Debe existir check-in previo
- ✅ CheckOut > CheckIn

### **Update (Admin):**
- ✅ Rango extendido: 30 días
- ✅ Consistencia horas vs tiempo
- ✅ Validaciones de negocio flexibles
- ✅ Auditoría de cambios

## 📊 **DTOs DE RESPUESTA**

### **AssistanceResponseDto** - Respuesta estándar con cálculos
### **AssistanceListDto** - Para listados optimizados  
### **AssistanceDetailsDto** - Vista completa con relaciones
### **AssistanceFilterDto** - Para búsquedas y reportes

## 🔐 **SEGURIDAD Y VALIDACIONES**

- **Caracteres seguros** en notas (sin HTML/SQL injection)
- **Rangos de tiempo lógicos** 
- **Validación de existencia** de empleados y proyectos
- **Control de sesiones activas** (no doble check-in)
- **Auditoría completa** de cambios administrativos

## 🎯 **CASOS DE USO**

1. **Check-in normal**: Empleado llega y registra entrada
2. **Check-out normal**: Empleado termina y registra salida  
3. **Corrección administrativa**: Admin ajusta tiempos incorrectos
4. **Reportes de tiempo**: Consultas con filtros avanzados
5. **Auditoría**: Seguimiento de cambios y modificaciones
