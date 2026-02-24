using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.Role;

namespace WorkTrackBio.API.Validators.RoleValidator
{
    public interface IRoleValidator
    {
        Task<ValidationResult> ValidateCreateAsync(CreateRoleDataTransferObject createDto);
        Task<ValidationResult> ValidateUpdateAsync(UpdateRoleDataTransferObject updateDto);
        ValidationResult ValidateId(int id);
        ValidationResult ValidateRoleName(string roleName);
        Task<ValidationResult> ValidateRoleExistsAsync(int id);
        Task<ValidationResult> ValidateRoleNameUniqueAsync(string roleName, int? excludeId = null);
    }
}
