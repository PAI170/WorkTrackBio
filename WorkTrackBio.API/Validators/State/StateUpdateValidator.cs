using FluentValidation;
using WorkTrackBio.API.DataTransferObjects.Catalog;

namespace WorkTrackBio.API.Validators.State
{
    public class StateUpdateValidator : AbstractValidator<StateUpdateDto>
    {
        public StateUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El Id debe ser mayor a 0.");

            RuleFor(x => x.StateName)
                .NotEmpty().WithMessage("El nombre del estado es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede superar 50 caracteres.");

            RuleFor(x => x.StateType)
                .IsInEnum().WithMessage("El tipo de estado no es válido.");

            RuleFor(x => x.Description)
                .MaximumLength(50).WithMessage("La descripción no puede superar 50 caracteres.")
                .When(x => x.Description != null);
        }
    }
}