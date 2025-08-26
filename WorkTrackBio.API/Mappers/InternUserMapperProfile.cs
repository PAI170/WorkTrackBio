using AutoMapper;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.InternUser;

namespace WorkTrackBio.API.Mappers
{
    public class InternUserMapperProfile : Profile
    {
        public InternUserMapperProfile()
        {
            // Mapeo de InternUser (Entity) a InternUserDataTransferObject (Response DTO)
            CreateMap<InternUser, InternUserDataTransferObject>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.RolId, opt => opt.MapFrom(src => src.RolId))
                .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.StateId))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
                .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLogin))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.DocumentNumber))
                .ForMember(dest => dest.DocumentTypeId, opt => opt.MapFrom(src => src.DocumentTypeId))
                .ForMember(dest => dest.DocumentExpire, opt => opt.MapFrom(src => src.DocumentExpire));

            // Mapeo de CreateInternUserDataTransferObject a InternUser (Entity)
            CreateMap<CreateInternUserDataTransferObject, InternUser>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.RolId, opt => opt.MapFrom(src => src.RolId))
                .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.StateId))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.DocumentNumber))
                .ForMember(dest => dest.DocumentTypeId, opt => opt.MapFrom(src => src.DocumentTypeId))
                .ForMember(dest => dest.DocumentExpire, opt => opt.MapFrom(src => src.DocumentExpire))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Se establece en el Repository
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore()) // Se establece en el Repository
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore()) // Se establece en el Repository
                .ForMember(dest => dest.LastLogin, opt => opt.Ignore()) // Se establece como null por defecto
                .ForMember(dest => dest.Role, opt => opt.Ignore()) // No se mapea la entidad relacionada
                .ForMember(dest => dest.State, opt => opt.Ignore()) // No se mapea la entidad relacionada
                .ForMember(dest => dest.DocumentType, opt => opt.Ignore()); // No se mapea la entidad relacionada

            // Mapeo de UpdateInternUserDataTransferObject a InternUser (Entity)
            CreateMap<UpdateInternUserDataTransferObject, InternUser>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.RolId, opt => opt.MapFrom(src => src.RolId))
                .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.StateId))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.DocumentNumber))
                .ForMember(dest => dest.DocumentTypeId, opt => opt.MapFrom(src => src.DocumentTypeId))
                .ForMember(dest => dest.DocumentExpire, opt => opt.MapFrom(src => src.DocumentExpire))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // No se actualiza en updates
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore()) // No se actualiza en updates
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore()) // No se actualiza en updates
                .ForMember(dest => dest.LastLogin, opt => opt.Ignore()) // No se actualiza en updates
                .ForMember(dest => dest.Role, opt => opt.Ignore()) // No se mapea la entidad relacionada
                .ForMember(dest => dest.State, opt => opt.Ignore()) // No se mapea la entidad relacionada
                .ForMember(dest => dest.DocumentType, opt => opt.Ignore()); // No se mapea la entidad relacionada

            // Mapeo bidireccional opcional (si se necesita en el futuro)
            CreateMap<InternUserDataTransferObject, InternUser>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastLogin, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.State, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentType, opt => opt.Ignore());
        }
    }
}

