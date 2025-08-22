using AutoMapper;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.DocumentType;

namespace WorkTrackBio.API.Mappers
{
    public class DocumentTypeMapperProfile : Profile
    {
        public DocumentTypeMapperProfile()
        {
            CreateMap<DocumentType, DocumentTypeDataTransferObject>();
            CreateMap<CreateDocumentTypeDataTransferObject, DocumentType>();
            CreateMap<UpdateDocumentTypeDataTransferObject, DocumentType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
