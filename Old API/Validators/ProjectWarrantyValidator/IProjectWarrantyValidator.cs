using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.ProjectWarranty;

namespace WorkTrackBio.API.Validators.ProjectWarrantyValidator
{
    public interface IProjectWarrantyValidator
    {
        Task<ValidationResult> ValidateCreateAsync(CreateProjectWarrantyDataTransferObject createDto);
        Task<ValidationResult> ValidateUpdateAsync(int id, UpdateProjectWarrantyDataTransferObject updateDto);
        Task<ValidationResult> ValidateProjectExistsAsync(int projectId);
        Task<ValidationResult> ValidateEmployeeExistsAsync(int employeeId);
        Task<ValidationResult> ValidateStateExistsAsync(int stateId);
    }
}
