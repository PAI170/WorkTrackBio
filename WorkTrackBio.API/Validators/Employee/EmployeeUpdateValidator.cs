using FluentValidation;
using WorkTrackBio.API.DataTransferObjects.Employee;

namespace WorkTrackBio.API.Validators.Employee
{
    public class EmployeeUpdateValidator : AbstractValidator<EmployeeUpdateDto>
    {
        public EmployeeUpdateValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede superar 50 caracteres.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .MaximumLength(50).WithMessage("El apellido no puede superar 50 caracteres.");

            RuleFor(x => x.PersonalEmail)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.")
                .MaximumLength(100).WithMessage("El correo no puede superar 100 caracteres.");

            RuleFor(x => x.Birthday)
                .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.")
                .Must(birthday => birthday <= DateOnly.FromDateTime(DateTime.Today.AddYears(-18)))
                .WithMessage("El empleado debe tener al menos 18 años.");

            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("El número de documento es obligatorio.")
                .MaximumLength(50).WithMessage("El número de documento no puede superar 50 caracteres.");

            RuleFor(x => x.DocumentExpire)
                .NotEmpty().WithMessage("La fecha de vencimiento del documento es obligatoria.")
                .Must(expire => expire >= DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("El documento de identidad está vencido.");

            RuleFor(x => x.HireDate)
                .NotEmpty().WithMessage("La fecha de contratación es obligatoria.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("El género es obligatorio.")
                .MaximumLength(20).WithMessage("El género no puede superar 20 caracteres.");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("El cargo es obligatorio.")
                .MaximumLength(100).WithMessage("El cargo no puede superar 100 caracteres.");

            RuleFor(x => x.Nationality)
                .NotEmpty().WithMessage("La nacionalidad es obligatoria.")
                .MaximumLength(50).WithMessage("La nacionalidad no puede superar 50 caracteres.");

            RuleFor(x => x.MaritalStatus)
                .NotEmpty().WithMessage("El estado civil es obligatorio.")
                .MaximumLength(50).WithMessage("El estado civil no puede superar 50 caracteres.");

            RuleFor(x => x.NumberOfChildren)
                .InclusiveBetween(0, 20).WithMessage("El número de hijos debe estar entre 0 y 20.");

            RuleFor(x => x.StateId)
                .GreaterThan(0).WithMessage("El estado es obligatorio.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("El departamento es obligatorio.");

            RuleFor(x => x.DocumentTypeId)
                .GreaterThan(0).WithMessage("El tipo de documento es obligatorio.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("El teléfono no puede superar 20 caracteres.")
                .When(x => x.PhoneNumber != null);

            RuleFor(x => x.Address)
                .MaximumLength(255).WithMessage("La dirección no puede superar 255 caracteres.")
                .When(x => x.Address != null);

            RuleFor(x => x.IBAN)
                .MaximumLength(50).WithMessage("El IBAN no puede superar 50 caracteres.")
                .When(x => x.IBAN != null);

            RuleFor(x => x.Salary.PaymentFrequency)
                .IsInEnum().WithMessage("La frecuencia de pago no es válida.");

            RuleFor(x => x.Salary.Currency)
                .MaximumLength(3).WithMessage("La moneda debe tener máximo 3 caracteres.")
                .When(x => x.Salary.Currency != null);
        }
    }
}