using AutoMapper;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.State;

namespace WorkTrackBio.API.Mappers
{
    public class StateMapperProfile : Profile
    {
        public StateMapperProfile()
        {
            CreateMap<State, StateDataTransferObject>();
            CreateMap<CreateStateDataTransferObject, State>();
            CreateMap<UpdateStateDataTransferObject, State>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
