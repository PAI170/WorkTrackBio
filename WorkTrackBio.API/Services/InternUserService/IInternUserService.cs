using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.InternUser;

namespace WorkTrackBio.API.Services.InternUserService
{
    public interface IInternUserService
    {
        Task<ApiResponse<IEnumerable<InternUserDataTransferObject>>> GetAllInternUsersAsync();
        Task<ApiResponse<InternUserDataTransferObject>> GetInternUserByIdAsync(int id);
        Task<ApiResponse<InternUserDataTransferObject>> GetInternUserByEmailAsync(string email);
        Task<ApiResponse<IEnumerable<InternUserDataTransferObject>>> GetInternUsersByRoleAsync(int roleId);
        Task<ApiResponse<IEnumerable<InternUserDataTransferObject>>> GetInternUsersByStateAsync(int stateId);
        Task<ApiResponse<InternUserDataTransferObject>> CreateInternUserAsync(CreateInternUserDataTransferObject createDto);
        Task<ApiResponse<InternUserDataTransferObject>> UpdateInternUserAsync(UpdateInternUserDataTransferObject updateDto);
        Task<ApiResponse<bool>> DeleteInternUserAsync(int id);
    }
}

