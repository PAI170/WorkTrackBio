using AutoMapper;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.State;

namespace WorkTrackBio.API.Mappers
{
    public class StateMapperProfile : Profile
    {
        public StateMapperProfile()
        {
            // Model -> DTO
            CreateMap<State, StateDataTransferObject>();
            
            // DTO -> Model
            CreateMap<CreateStateDataTransferObject, State>();
            CreateMap<UpdateStateDataTransferObject, State>();
            
            // Para actualizaciones, ignorar el Id
            CreateMap<UpdateStateDataTransferObject, State>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
