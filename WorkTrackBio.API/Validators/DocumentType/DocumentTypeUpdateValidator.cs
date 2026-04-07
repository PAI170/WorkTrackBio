using FluentValidation;
using WorkTrackBio.API.DataTransferObjects.Catalog;

namespace WorkTrackBio.API.Validators.DocumentType
{
    public class DocumentTypeUpdateValidator : AbstractValidator<DocumentTypeUpdateDto>
    {
        public DocumentTypeUpdateValidator()
        {
            RuleFor(x => x.DocumentName)
                .NotEmpty().WithMessage("El nombre del tipo de documento es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede superar 50 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("La descripción no puede superar 255 caracteres.")
                .When(x => x.Description != null);
        }
    }
}