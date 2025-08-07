# 📋 VALIDACIÓN DE DOCUMENTOS POR TIPO

## 🎯 FUNCIONALIDAD

El sistema valida automáticamente el formato del número de documento según el tipo seleccionado. Esta validación se realiza en **FluentValidation** para mantener la lógica de negocio centralizada.

## 📝 FORMATOS SOPORTADOS

### 1. **Cédula de Identidad (ID: 1)**
- **Formatos:** `n-nnnn-nnnn` o `nnnnnnnnn`
- **Ejemplos:** `1-1234-5678` o `112345678`
- **Descripción:** Cédula de identidad costarricense (9 dígitos, no puede empezar con 0)

### 2. **Pasaporte (ID: 2)**
- **Formato:** `[A-Z]{1,2}[0-9]{6,8}`
- **Ejemplos:** `A1234567`, `CR12345678`
- **Descripción:** Pasaporte internacional

### 3. **DIMEX (ID: 3)**
- **Formato:** `DIM-nnnnnnnn`
- **Ejemplo:** `DIM-12345678`
- **Descripción:** Documento de Identidad Migratoria para Extranjeros

### 4. **Permiso de Trabajo (ID: 4)**
- **Formato:** `PT-nnnnnn`
- **Ejemplo:** `PT-123456`
- **Descripción:** Permiso de trabajo para extranjeros

## 📞 FORMATOS DE TELÉFONO

### **Teléfonos Costarricenses**
- **Formatos:** `nnnn-nnnn` o `nnnnnnnn`
- **Ejemplos:** `2456-7890` o `24567890`
- **Descripción:** Números de teléfono de Costa Rica (8 dígitos totales)

## 🔧 IMPLEMENTACIÓN

### Enum de Tipos de Documento:
```csharp
public enum DocumentTypeEnum
{
    CedulaIdentidad = 1,
    Pasaporte = 2,
    DIMEX = 3,
    PermisoTrabajo = 4
}
```

### Validación Dinámica:
```csharp
// La validación cambia según el DocumentTypeId
RuleFor(x => x)
    .Must(x => BeValidDocumentNumberForType(x.DocumentNumber, x.DocumentTypeId))
    .WithMessage(x => GetDocumentValidationMessage(x.DocumentTypeId));
```

## ✅ EJEMPLOS DE VALIDACIÓN

### Casos Válidos:
```json
{
    "documentTypeId": 1,
    "documentNumber": "1-1234-5678"  // ✅ Válido - Cédula con guiones
}

{
    "documentTypeId": 1,
    "documentNumber": "112345678"    // ✅ Válido - Cédula sin guiones
}

{
    "documentTypeId": 2,
    "documentNumber": "A1234567"     // ✅ Válido - Pasaporte
}

{
    "documentTypeId": 3,
    "documentNumber": "DIM-12345678" // ✅ Válido - DIMEX
}

{
    "documentTypeId": 4,
    "documentNumber": "PT-123456"    // ✅ Válido - Permiso trabajo
}

// Teléfonos válidos
{
    "phoneNumber": "2456-7890"       // ✅ Válido - Con guión
}

{
    "phoneNumber": "24567890"        // ✅ Válido - Sin guión
}
```

### Casos Inválidos:
```json
{
    "documentTypeId": 1,
    "documentNumber": "A1234567"     // ❌ Formato de pasaporte en cédula
}

{
    "documentTypeId": 2,
    "documentNumber": "1-1234-5678"  // ❌ Formato de cédula en pasaporte
}

{
    "documentTypeId": 3,
    "documentNumber": "PT-123456"    // ❌ Formato de permiso en DIMEX
}
```

## 🔄 MENSAJES DE ERROR CONTEXTUALES

El sistema devuelve mensajes específicos según el tipo:

- **Cédula:** "El formato de la cédula debe ser: n-nnnn-nnnn (ej: 1-1234-5678)"
- **Pasaporte:** "El formato del pasaporte debe ser: 1-2 letras seguidas de 6-8 números (ej: A1234567, CR12345678)"
- **DIMEX:** "El formato del DIMEX debe ser: DIM-nnnnnnnn (ej: DIM-12345678)"
- **Permiso:** "El formato del permiso de trabajo debe ser: PT-nnnnnn (ej: PT-123456)"

## 🛠️ MANTENIMIENTO

Para agregar nuevos tipos de documento:

1. Actualizar la tabla `DocumentType` en la BD
2. Agregar el nuevo tipo al enum `DocumentTypeEnum`
3. Implementar el método de validación específico
4. Actualizar los switch statements en el validador
5. Agregar mensaje de error contextual
