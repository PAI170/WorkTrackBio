using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.State;

namespace WorkTrackBio.API.Validators.StateValidator
{
    public interface IStateValidator
    {
        Task<ValidationResult> ValidateCreateAsync(CreateStateDataTransferObject createDto);
        Task<ValidationResult> ValidateUpdateAsync(UpdateStateDataTransferObject updateDto);
        ValidationResult ValidateId(int id);
        ValidationResult ValidateStateName(string stateName);
        ValidationResult ValidateStateType(string stateType);
        Task<ValidationResult> ValidateStateExistsAsync(int id);
        Task<ValidationResult> ValidateStateNameUniqueAsync(string stateName, int? excludeId = null);
    }
}
