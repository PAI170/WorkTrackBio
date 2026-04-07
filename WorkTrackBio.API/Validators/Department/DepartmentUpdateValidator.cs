using FluentValidation;
using WorkTrackBio.API.DataTransferObjects.Catalog;

namespace WorkTrackBio.API.Validators.Department
{
    public class DepartmentUpdateValidator : AbstractValidator<DepartmentUpdateDto>
    {
        public DepartmentUpdateValidator()
        {
            RuleFor(x => x.DepartmentName)
                .NotEmpty().WithMessage("El nombre del departamento es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar 100 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("La descripción no puede superar 255 caracteres.")
                .When(x => x.Description != null);
        }
    }
}