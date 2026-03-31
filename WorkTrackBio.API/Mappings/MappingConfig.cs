using Mapster;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.Catalog;

namespace WorkTrackBio.API.Mappings
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {

            config.NewConfig<StateCreateDto, State>();

            config.NewConfig<StateUpdateDto, State>();

            config.NewConfig<State, StateResponseDto>()
                .Map(dest => dest.StateType, src => src.StateType.ToString());
        }
    }
}