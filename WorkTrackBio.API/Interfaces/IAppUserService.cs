using WorkTrackBio.API.DataTransferObjects.AppUser;

namespace WorkTrackBio.API.Interfaces
{
    public interface IAppUserService
    {
        Task<IEnumerable<AppUserListDto>> GetAllAsync();
        Task<AppUserDetailDto> GetByIdAsync(int id);
        Task<AppUserDetailDto> CreateAsync(AppUserCreateDto dto);
        Task<AppUserDetailDto> UpdateAsync(int id, AppUserUpdateDto dto);
        Task ResetPasswordAsync(int id, AppUserResetPasswordDto dto);
        Task UnlockAsync(int id);
        Task DeleteAsync(int id);
    }
}