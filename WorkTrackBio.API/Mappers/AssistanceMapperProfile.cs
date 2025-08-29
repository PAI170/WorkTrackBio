using AutoMapper;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.Assistance;

namespace WorkTrackBio.API.Mappers
{
    /// <summary>
    /// Perfil de AutoMapper para Assistance
    /// </summary>
    public class AssistanceMapperProfile : Profile
    {
        public AssistanceMapperProfile()
        {
            // Model -> DTO
            CreateMap<Assistance, AssistanceDataTransferObject>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => 
                    src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : string.Empty))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => 
                    src.Project != null ? src.Project.ProjectName : string.Empty));

            // Create DTO -> Model
            CreateMap<CreateAssistanceDataTransferObject, Assistance>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CheckInDateOnly, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore());

            // Update DTO -> Model (para actualizaciones parciales)
            CreateMap<UpdateAssistanceDataTransferObject, Assistance>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CheckInDateOnly, opt => opt.Ignore())
                .ForMember(dest => dest.Employee, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore());
        }
    }
}
