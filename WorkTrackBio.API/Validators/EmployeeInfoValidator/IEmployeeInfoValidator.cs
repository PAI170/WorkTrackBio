using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.EmployeeInfo;

namespace WorkTrackBio.API.Validators.EmployeeInfoValidator
{
    public interface IEmployeeInfoValidator
    {
        Task<ValidationResult> ValidateCreateAsync(CreateEmployeeInfoDataTransferObject createDto);
        Task<ValidationResult> ValidateUpdateAsync(UpdateEmployeeInfoDataTransferObject updateDto);
        ValidationResult ValidateId(int id);
        ValidationResult ValidateDocumentNumber(string documentNumber);
        ValidationResult ValidateNames(string firstName, string lastName);
        ValidationResult ValidatePhoneNumber(string? phoneNumber);
        ValidationResult ValidateDates(DateOnly? documentExpire, DateOnly? birthday);
        ValidationResult ValidateCostPerHour(decimal? costPerHour);
        Task<ValidationResult> ValidateEmployeeExistsAsync(int id);
        Task<ValidationResult> ValidateDocumentNumberUniqueAsync(string documentNumber, int documentTypeId, int? excludeId = null);
        Task<ValidationResult> ValidateStateExistsAsync(int stateId);
        Task<ValidationResult> ValidateDocumentTypeExistsAsync(int documentTypeId);
    }
}
