using FluentValidation;
using WTB.API.Models.DTOs.ProjectAssign;

namespace WTB.API.Validators.ProjectAssign
{
    /// <summary>
    /// Validador para la creación de asignaciones empleado-proyecto
    /// </summary>
    public class CreateProjectAssignValidator : AbstractValidator<CreateProjectAssignDto>
    {
        public CreateProjectAssignValidator()
        {
            // Validación del empleado
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un empleado válido")
                .Must(BeValidEmployee)
                .WithMessage("El empleado seleccionado no existe o no está activo");

            // Validación del proyecto
            RuleFor(x => x.ProjectId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un proyecto válido")
                .Must(BeValidProject)
                .WithMessage("El proyecto seleccionado no existe o no está activo");

            // Validación de fecha de asignación
            RuleFor(x => x.AssignDate)
                .NotEmpty()
                .WithMessage("La fecha de asignación es obligatoria")
                .Must(BeValidAssignDate)
                .WithMessage("La fecha de asignación no puede ser en el futuro")
                .Must((dto, assignDate) => BeValidAssignDateForProject(assignDate, dto.ProjectId))
                .WithMessage("La fecha de asignación debe estar dentro del período del proyecto");

            // Validación de fecha de fin (si se proporciona)
            When(x => x.EndDate.HasValue, () =>
            {
                RuleFor(x => x.EndDate)
                    .Must((dto, endDate) => endDate > dto.AssignDate)
                    .WithMessage("La fecha de fin debe ser posterior a la fecha de asignación")
                    .Must((dto, endDate) => BeValidEndDateForProject(endDate, dto.ProjectId))
                    .WithMessage("La fecha de fin debe estar dentro del período del proyecto");
            });

            // Validación de duplicados
            RuleFor(x => x)
                .Must(NotBeDuplicateAssignment)
                .WithMessage("El empleado ya está asignado a este proyecto")
                .WithName("Assignment");

            // TODO: Validaciones que requieren acceso a datos:
            // - El empleado no debe estar asignado al mismo proyecto simultáneamente
            // - El proyecto debe estar activo y no completado
            // - El empleado debe estar activo
            // - Verificar límites de empleados por proyecto
            // Estas validaciones se implementarán en el servicio
        }

        /// <summary>
        /// Valida que el empleado sea válido
        /// </summary>
        private bool BeValidEmployee(int employeeId)
        {
            // TODO: Implementar validación real con acceso a base de datos
            // return await _context.EmployeeInfo.AnyAsync(e => e.Id == employeeId && e.StateId == 1);
            
            // Mock temporal - empleados válidos
            var validEmployees = new[] { 1, 2, 3, 4, 5 }; // IDs de empleados activos
            return validEmployees.Contains(employeeId);
        }

        /// <summary>
        /// Valida que el proyecto sea válido
        /// </summary>
        private bool BeValidProject(int projectId)
        {
            // TODO: Implementar validación real
            var validProjects = new[] { 1, 2, 3 }; // IDs de proyectos activos
            return validProjects.Contains(projectId);
        }

        /// <summary>
        /// Valida que la fecha de asignación sea válida
        /// </summary>
        private bool BeValidAssignDate(DateTime assignDate)
        {
            return assignDate <= DateTime.UtcNow;
        }

        /// <summary>
        /// Valida que la fecha de asignación esté dentro del período del proyecto
        /// </summary>
        private bool BeValidAssignDateForProject(DateTime assignDate, int projectId)
        {
            // TODO: Implementar validación real consultando fechas del proyecto
            // var project = await _context.Projects.FindAsync(projectId);
            // return assignDate >= project.StartDate && assignDate <= project.EndDate;
            
            // Mock temporal - asumir que el proyecto está activo
            return true;
        }

        /// <summary>
        /// Valida que la fecha de fin esté dentro del período del proyecto
        /// </summary>
        private bool BeValidEndDateForProject(DateTime? endDate, int projectId)
        {
            if (!endDate.HasValue) return true;
            
            // TODO: Implementar validación real
            // var project = await _context.Projects.FindAsync(projectId);
            // return endDate <= project.EndDate;
            
            return true;
        }

        /// <summary>
        /// Valida que no haya asignación duplicada
        /// </summary>
        private bool NotBeDuplicateAssignment(CreateProjectAssignDto dto)
        {
            // TODO: Implementar validación real
            // return !await _context.ProjectsAssigns.AnyAsync(pa => 
            //     pa.EmployeeId == dto.EmployeeId && 
            //     pa.ProjectId == dto.ProjectId && 
            //     pa.EndDate == null);
            
            // Mock temporal - asumir que no hay duplicados
            return true;
        }
    }
}
