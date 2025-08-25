using WorkTrackBio.API.DataTransferObjects.InternUser;

namespace WorkTrackBio.API.Services.InternUserService
{
    public interface IInternUserService
    {
        Task<IEnumerable<InternUserDataTransferObject>> GetAllInternUsersAsync();
        Task<InternUserDataTransferObject?> GetInternUserByIdAsync(int id);
        Task<InternUserDataTransferObject?> GetInternUserByEmailAsync(string email);
        Task<IEnumerable<InternUserDataTransferObject>> GetInternUsersByRoleAsync(int roleId);
        Task<IEnumerable<InternUserDataTransferObject>> GetInternUsersByStateAsync(int stateId);
        Task<bool> InternUserExistsAsync(int id);
        Task<bool> InternUserEmailExistsAsync(string email);
        Task<InternUserDataTransferObject> CreateInternUserAsync(CreateInternUserDataTransferObject createDto);
        Task<InternUserDataTransferObject?> UpdateInternUserAsync(UpdateInternUserDataTransferObject updateDto);
        Task<bool> DeleteInternUserAsync(int id);
    }
}
