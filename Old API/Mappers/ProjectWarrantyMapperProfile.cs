using AutoMapper;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.ProjectWarranty;

namespace WorkTrackBio.API.Mappers
{
    public class ProjectWarrantyMapperProfile : Profile
    {
        public ProjectWarrantyMapperProfile()
        {
            CreateMap<ProjectWarranty, ProjectWarrantyDataTransferObject>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => 
                    src.Project != null ? src.Project.ProjectName : string.Empty))
                .ForMember(dest => dest.MadeByName, opt => opt.MapFrom(src => 
                    src.MadeBy != null ? $"{src.MadeBy.FirstName} {src.MadeBy.LastName}" : string.Empty))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => 
                    src.State != null ? src.State.StateName : string.Empty));

            CreateMap<CreateProjectWarrantyDataTransferObject, ProjectWarranty>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.MadeBy, opt => opt.Ignore())
                .ForMember(dest => dest.State, opt => opt.Ignore());

            CreateMap<UpdateProjectWarrantyDataTransferObject, ProjectWarranty>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.MadeBy, opt => opt.Ignore())
                .ForMember(dest => dest.State, opt => opt.Ignore());
        }
    }
}
