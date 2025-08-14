using FluentValidation;
using WTB.API.Models.DTOs.ProjectAssign;

namespace WTB.API.Validators.ProjectAssign
{
    /// <summary>
    /// Validador para el filtro de asignaciones empleado-proyecto
    /// </summary>
    public class ProjectAssignFilterValidator : AbstractValidator<ProjectAssignFilterDto>
    {
        public ProjectAssignFilterValidator()
        {
            // Validación de términos de búsqueda
            When(x => !string.IsNullOrEmpty(x.SearchTerm), () =>
            {
                RuleFor(x => x.SearchTerm)
                    .MinimumLength(2)
                    .WithMessage("El término de búsqueda debe tener al menos 2 caracteres")
                    .MaximumLength(100)
                    .WithMessage("El término de búsqueda no puede exceder 100 caracteres");
            });

            // Validación de IDs
            When(x => x.EmployeeId.HasValue, () =>
            {
                RuleFor(x => x.EmployeeId)
                    .GreaterThan(0)
                    .WithMessage("El ID del empleado debe ser mayor que cero");
            });

            When(x => x.ProjectId.HasValue, () =>
            {
                RuleFor(x => x.ProjectId)
                    .GreaterThan(0)
                    .WithMessage("El ID del proyecto debe ser mayor que cero");
            });

            // Validación de fechas
            When(x => x.AssignedFrom.HasValue && x.AssignedTo.HasValue, () =>
            {
                RuleFor(x => x.AssignedFrom)
                    .LessThanOrEqualTo(x => x.AssignedTo)
                    .WithMessage("La fecha de inicio debe ser anterior o igual a la fecha de fin");
            });

            When(x => x.EndDateFrom.HasValue && x.EndDateTo.HasValue, () =>
            {
                RuleFor(x => x.EndDateFrom)
                    .LessThanOrEqualTo(x => x.EndDateTo)
                    .WithMessage("La fecha de fin desde debe ser anterior o igual a la fecha de fin hasta");
            });

            // Validación de rangos de horas
            When(x => x.MinHoursWorked.HasValue && x.MaxHoursWorked.HasValue, () =>
            {
                RuleFor(x => x.MinHoursWorked)
                    .LessThanOrEqualTo(x => x.MaxHoursWorked)
                    .WithMessage("Las horas mínimas deben ser menores o iguales a las horas máximas");
            });

            // Validación de paginación
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("La página debe ser mayor que cero");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("El tamaño de página debe estar entre 1 y 100");

            // Validación de ordenamiento
            When(x => !string.IsNullOrEmpty(x.SortBy), () =>
            {
                RuleFor(x => x.SortBy)
                    .Must(BeValidSortField)
                    .WithMessage("Campo de ordenamiento no válido. Campos válidos: employeename, projectname, assigndate, enddate");
            });
        }

        /// <summary>
        /// Valida que el campo de ordenamiento sea válido
        /// </summary>
        private bool BeValidSortField(string? sortBy)
        {
            if (string.IsNullOrEmpty(sortBy)) return false;
            
            var validFields = new[] { "employeename", "projectname", "assigndate", "enddate" };
            return validFields.Contains(sortBy.ToLower());
        }
    }
}
