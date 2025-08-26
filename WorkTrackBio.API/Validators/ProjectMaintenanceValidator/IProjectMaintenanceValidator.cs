using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.ProjectMaintenance;

namespace WorkTrackBio.API.Validators.ProjectMaintenanceValidator
{
    public interface IProjectMaintenanceValidator
    {
        Task<ValidationResult> ValidateCreateAsync(CreateProjectMaintenanceDataTransferObject createDto);
        Task<ValidationResult> ValidateUpdateAsync(UpdateProjectMaintenanceDataTransferObject updateDto);
        ValidationResult ValidateId(int id);
        ValidationResult ValidateProjectId(int projectId);
        ValidationResult ValidateEmployeeId(int employeeId);
        ValidationResult ValidateStateId(int stateId);
        ValidationResult ValidateMaintenanceDescription(string description);
        ValidationResult ValidateMaintenanceCost(decimal? cost);
        ValidationResult ValidateAdditionalInfo(string? additionalInfo);
    }
}
