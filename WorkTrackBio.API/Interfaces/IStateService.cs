using WorkTrackBio.API.DataTransferObjects.Catalog;

namespace WorkTrackBio.API.Interfaces
{
    public interface IStateService
    {
        Task<IEnumerable<StateResponseDto>> GetAllAsync();
        Task<StateResponseDto> GetByIdAsync(int id);
        Task<StateResponseDto> CreateAsync(StateCreateDto dto);
        Task<StateResponseDto> UpdateAsync(int id, StateUpdateDto dto);
        Task DeleteAsync(int id);
    }
}