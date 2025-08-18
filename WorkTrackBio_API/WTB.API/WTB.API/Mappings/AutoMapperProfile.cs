using AutoMapper;
using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.Employee;
using WTB.API.Models.DTOs.Project;
using WTB.API.Models.DTOs.Assistance;
using WTB.API.Models.DTOs.InternUser;
using WTB.API.Models.DTOs.ProjectAssign;
using WTB.API.Models.DTOs.ProjectMaintenance;
using WTB.API.Models.DTOs.ProjectWarranty;
using WTB.API.Models.DTOs.Common;

namespace WTB.API.Mappings
{
    /// <summary>
    /// Perfil principal de AutoMapper para mapeos entre entidades y DTOs
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ===============================
            // MAPEOS DE EMPLEADOS
            // ===============================
            
            CreateMap<EmployeeInfo, CreateEmployeeDto>().ReverseMap();
            CreateMap<EmployeeInfo, UpdateEmployeeDto>().ReverseMap();
            CreateMap<EmployeeInfo, EmployeeResponseDto>();
            CreateMap<EmployeeInfo, EmployeeDetailsDto>();
            CreateMap<EmployeeInfo, EmployeeListDto>();
            CreateMap<EmployeeInfo, EmployeeFilterDto>();

            // ===============================
            // MAPEOS DE PROYECTOS
            // ===============================
            
            CreateMap<Projects, CreateProjectDto>().ReverseMap();
            CreateMap<Projects, UpdateProjectDto>().ReverseMap();
            CreateMap<Projects, ProjectResponseDto>();
            CreateMap<Projects, ProjectDetailsDto>();
            CreateMap<Projects, ProjectListDto>();
            CreateMap<Projects, ProjectFilterDto>();

            // ===============================
            // MAPEOS DE ASISTENCIA
            // ===============================
            
            CreateMap<Assistance, CheckInDto>().ReverseMap();
            CreateMap<Assistance, CheckOutDto>().ReverseMap();
            CreateMap<Assistance, UpdateAssistanceDto>().ReverseMap();
            CreateMap<Assistance, AssistanceResponseDto>();
            CreateMap<Assistance, AssistanceDetailsDto>();
            CreateMap<Assistance, AssistanceListDto>();
            CreateMap<Assistance, AssistanceFilterDto>();

            // ===============================
            // MAPEOS DE USUARIOS INTERNOS
            // ===============================
            
            CreateMap<InternUsers, CreateInternUserDto>().ReverseMap();
            CreateMap<InternUsers, UpdateInternUserDto>().ReverseMap();
            CreateMap<InternUsers, InternUserResponseDto>();
            CreateMap<InternUsers, InternUserDetailsDto>();
            CreateMap<InternUsers, InternUserListDto>();
            CreateMap<InternUsers, InternUserFilterDto>();

            // ===============================
            // MAPEOS DE ASIGNACIONES DE PROYECTOS
            // ===============================
            
            CreateMap<ProjectsAssigns, CreateProjectAssignDto>().ReverseMap();
            CreateMap<ProjectsAssigns, UpdateProjectAssignDto>().ReverseMap();
            CreateMap<ProjectsAssigns, ProjectAssignResponseDto>()
                .ForMember(dest => dest.EmployeeFullName, opt => opt.MapFrom(src => src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : "N/A"))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.ProjectName : "N/A"))
                .ForMember(dest => dest.ProjectState, opt => opt.MapFrom(src => src.Project != null && src.Project.State != null ? src.Project.State.StateName : "N/A"))
                .ForMember(dest => dest.ProjectEndDate, opt => opt.MapFrom(src => src.Project != null ? src.Project.EndDate : null))
                .ForMember(dest => dest.ProjectIsActive, opt => opt.MapFrom(src => src.Project != null && src.Project.State != null ? src.Project.State.StateName == "Active" : false));

            CreateMap<ProjectsAssigns, ProjectAssignListDto>()
                .ForMember(dest => dest.EmployeeFullName, opt => opt.MapFrom(src => src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : "N/A"))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.ProjectName : "N/A"));

            CreateMap<ProjectsAssigns, ProjectAssignDetailsDto>()
                .ForMember(dest => dest.EmployeeFullName, opt => opt.MapFrom(src => src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : "N/A"))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.ProjectName : "N/A"));

            CreateMap<ProjectsAssigns, ProjectAssignFilterDto>();

            // ===============================
            // MAPEOS DE MANTENIMIENTO DE PROYECTOS
            // ===============================
            
            CreateMap<ProjectMaintenance, CreateProjectMaintenanceDto>().ReverseMap();
            CreateMap<ProjectMaintenance, ProjectMaintenanceResponseDto>();
            CreateMap<ProjectMaintenance, ProjectMaintenanceDetailsDto>();
            CreateMap<ProjectMaintenance, ProjectMaintenanceListDto>();

            // ===============================
            // MAPEOS DE GARANTÍA DE PROYECTOS
            // ===============================
            
            CreateMap<ProjectWarranty, CreateProjectWarrantyDto>().ReverseMap();
            CreateMap<ProjectWarranty, ProjectWarrantyResponseDto>();
            CreateMap<ProjectWarranty, ProjectWarrantyDetailsDto>();
            CreateMap<ProjectWarranty, ProjectWarrantyListDto>();

            // ===============================
            // MAPEOS COMUNES
            // ===============================
            
            CreateMap<States, LookupDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.StateName));

            CreateMap<Roles, LookupDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.RoleName));

            CreateMap<DocumentType, LookupDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DocumentName));
        }
    }
}
