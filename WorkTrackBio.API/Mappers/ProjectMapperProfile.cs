using AutoMapper;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.Project;

namespace WorkTrackBio.API.Mappers
{
    public class ProjectMapperProfile : Profile
    {
        public ProjectMapperProfile()
        {
            CreateMap<Project, ProjectDataTransferObject>()
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State.StateName));  

            CreateMap<CreateProjectDataTransferObject, Project>();     
                 
            CreateMap<UpdateProjectDataTransferObject, Project>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
