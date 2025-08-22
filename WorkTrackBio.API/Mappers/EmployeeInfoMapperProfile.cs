using AutoMapper;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.EmployeeInfo;

namespace WorkTrackBio.API.Mappers
{
    public class EmployeeInfoMapperProfile : Profile
    {
        public EmployeeInfoMapperProfile()
        {
            CreateMap<EmployeeInfo, EmployeeInfoDataTransferObject>()
                .ForMember(dest => dest.DocumentTypeName, opt => opt.MapFrom(src => src.DocumentType.DocumentName))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State.StateName));

            CreateMap<CreateEmployeeInfoDataTransferObject, EmployeeInfo>()
                .ForMember(dest => dest.RegisterDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateEmployeeInfoDataTransferObject, EmployeeInfo>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RegisterDate, opt => opt.Ignore());
        }
    }
}
