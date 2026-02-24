using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.Assistance;

namespace WorkTrackBio.API.Validators.AssistanceValidator
{
    public interface IAssistanceValidator
    {
        Task<ValidationResult> ValidateCreateAsync(CreateAssistanceDataTransferObject createDto);

        Task<ValidationResult> ValidateUpdateAsync(int id, UpdateAssistanceDataTransferObject updateDto);

        Task<ValidationResult> ValidateEmployeeExistsAsync(int employeeId);

        Task<ValidationResult> ValidateProjectExistsAsync(int projectId);

        Task<ValidationResult> ValidateCheckInOutLogicAsync(int employeeId, DateTime? checkOut);
    }
}
