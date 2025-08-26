using AutoMapper;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.ProjectMaintenance;

namespace WorkTrackBio.API.Mappers
{
    public class ProjectMaintenanceMapperProfile : Profile
    {
        public ProjectMaintenanceMapperProfile()
        {
            // Mapeo de ProjectMaintenance a ProjectMaintenanceDataTransferObject
            CreateMap<ProjectMaintenance, ProjectMaintenanceDataTransferObject>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.ProjectName : null))
                .ForMember(dest => dest.MadeByName, opt => opt.MapFrom(src => src.MadeBy != null ? $"{src.MadeBy.FirstName} {src.MadeBy.LastName}" : null))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State != null ? src.State.StateName : null));

            // Mapeo de CreateProjectMaintenanceDataTransferObject a ProjectMaintenance
            CreateMap<CreateProjectMaintenanceDataTransferObject, ProjectMaintenance>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.MaintenanceDate, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.MadeBy, opt => opt.Ignore())
                .ForMember(dest => dest.State, opt => opt.Ignore());

            // Mapeo de UpdateProjectMaintenanceDataTransferObject a ProjectMaintenance
            CreateMap<UpdateProjectMaintenanceDataTransferObject, ProjectMaintenance>()
                .ForMember(dest => dest.MaintenanceDate, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.MadeBy, opt => opt.Ignore())
                .ForMember(dest => dest.State, opt => opt.Ignore());
        }
    }
}
