using FluentValidation;
using WorkTrackBio.API.DataTransferObjects.ProjectWarranty;

namespace WorkTrackBio.API.Validators.ProjectWarrantyValidator
{
    /// <summary>
    /// Validador de FluentValidation para UpdateProjectWarrantyDataTransferObject
    /// </summary>
    public class UpdateProjectWarrantyValidator : AbstractValidator<UpdateProjectWarrantyDataTransferObject>
    {
        public UpdateProjectWarrantyValidator()
        {
            RuleFor(x => x.IdProject)
                .GreaterThan(0)
                .When(x => x.IdProject.HasValue)
                .WithMessage("El ID del proyecto debe ser mayor que 0");

            RuleFor(x => x.WarrantyDate)
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                .When(x => x.WarrantyDate.HasValue)
                .WithMessage("La fecha de garantía no puede ser en el futuro");

            RuleFor(x => x.WarrantyDescription)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.WarrantyDescription))
                .WithMessage("La descripción de la garantía no puede exceder 500 caracteres");

            RuleFor(x => x.MadeById)
                .GreaterThan(0)
                .When(x => x.MadeById.HasValue)
                .WithMessage("El ID del empleado debe ser mayor que 0");

            RuleFor(x => x.WarrantyCost)
                .GreaterThanOrEqualTo(0)
                .When(x => x.WarrantyCost.HasValue)
                .WithMessage("El costo de la garantía no puede ser negativo");

            RuleFor(x => x.StateId)
                .GreaterThan(0)
                .When(x => x.StateId.HasValue)
                .WithMessage("El ID del estado debe ser mayor que 0");
        }
    }
}
