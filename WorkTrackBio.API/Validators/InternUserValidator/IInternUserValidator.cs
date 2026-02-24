using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.InternUser;

namespace WorkTrackBio.API.Validators.InternUserValidator
{
    public interface IInternUserValidator
    {

        Task<ValidationResult> ValidateCreateAsync(CreateInternUserDataTransferObject createDto);

        Task<ValidationResult> ValidateUpdateAsync(UpdateInternUserDataTransferObject updateDto);

        Task<ValidationResult> ValidateEmailAsync(string email, int? excludeUserId = null);

        Task<ValidationResult> ValidateRoleExistsAsync(int roleId);

        Task<ValidationResult> ValidateStateExistsAsync(int stateId);

        Task<ValidationResult> ValidateDocumentAsync(string? documentNumber, int? documentTypeId, DateOnly? documentExpire = null);

        ValidationResult ValidateDocumentExpiration(DateOnly documentExpire);
    }
}
