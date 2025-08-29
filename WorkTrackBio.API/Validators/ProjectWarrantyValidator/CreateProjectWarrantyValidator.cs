using FluentValidation;
using WorkTrackBio.API.DataTransferObjects.ProjectWarranty;

namespace WorkTrackBio.API.Validators.ProjectWarrantyValidator
{
    /// <summary>
    /// Validador de FluentValidation para CreateProjectWarrantyDataTransferObject
    /// </summary>
    public class CreateProjectWarrantyValidator : AbstractValidator<CreateProjectWarrantyDataTransferObject>
    {
        public CreateProjectWarrantyValidator()
        {
            RuleFor(x => x.IdProject)
                .GreaterThan(0)
                .WithMessage("El ID del proyecto debe ser mayor que 0");

            RuleFor(x => x.WarrantyDate)
                .NotEmpty()
                .WithMessage("La fecha de garantía es obligatoria")
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                .WithMessage("La fecha de garantía no puede ser en el futuro");

            RuleFor(x => x.WarrantyDescription)
                .NotEmpty()
                .WithMessage("La descripción de la garantía es obligatoria")
                .MaximumLength(500)
                .WithMessage("La descripción de la garantía no puede exceder 500 caracteres");

            RuleFor(x => x.MadeById)
                .GreaterThan(0)
                .WithMessage("El ID del empleado debe ser mayor que 0");

            RuleFor(x => x.WarrantyCost)
                .GreaterThanOrEqualTo(0)
                .When(x => x.WarrantyCost.HasValue)
                .WithMessage("El costo de la garantía no puede ser negativo");

            RuleFor(x => x.StateId)
                .GreaterThan(0)
                .WithMessage("El ID del estado debe ser mayor que 0");
        }
    }
}
