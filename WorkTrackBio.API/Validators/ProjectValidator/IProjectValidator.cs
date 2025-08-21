using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.Project;

namespace WorkTrackBio.API.Validators.ProjectValidator
{
    public interface IProjectValidator
    {
        Task<ValidationResult> ValidateCreateAsync(CreateProjectDataTransferObject createDto);
        Task<ValidationResult> ValidateUpdateAsync(UpdateProjectDataTransferObject updateDto);
        ValidationResult ValidateId(int id);
        ValidationResult ValidateProjectName(string projectName);
        ValidationResult ValidateDates(DateOnly? startDate, DateOnly? endDate);
        Task<ValidationResult> ValidateProjectExistsAsync(int id);
        Task<ValidationResult> ValidateProjectNameUniqueAsync(string projectName, int? excludeId = null);
        Task<ValidationResult> ValidateStateExistsAsync(int stateId);
    }
}
