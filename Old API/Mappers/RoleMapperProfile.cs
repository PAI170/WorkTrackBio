using AutoMapper;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.Role;

namespace WorkTrackBio.API.Mappers
{
    public class RoleMapperProfile : Profile
    {
        public RoleMapperProfile()
        {
            CreateMap<Role, RoleDataTransferObject>();
            CreateMap<CreateRoleDataTransferObject, Role>();
            CreateMap<UpdateRoleDataTransferObject, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
