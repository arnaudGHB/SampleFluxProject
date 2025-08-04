using AutoMapper;
using CBS.AccountManagement.Data;

using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class DocumentReferenceCodeProfile : Profile
    {
        public DocumentReferenceCodeProfile()
        {
            CreateMap<AddCommandDocumentReferenceCode, DocumentReferenceCodeDto>().ReverseMap();
            CreateMap<UpdateCommandDocumentReferenceCode, DocumentReferenceCode>();
            //CreateMap<AddDocumentCommand, TrialBalanceReference>();
            //CreateMap<UpdateTrialBalanceReferenceCommand, TrialBalanceReferenceDto>();
        }
    }

    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<Document, DocumentDto>().ReverseMap();
            CreateMap<UpdateDocumentCommand, DocumentDto>();
            CreateMap<AddDocumentCommand, DocumentDto>();
            CreateMap<AddDocumentCommand, Document>();
        }
    }

    public class DocumentTypeProfile : Profile
    {
        public DocumentTypeProfile()
        {
            CreateMap<DocumentType, DocumentTypeDto>().ReverseMap();
            CreateMap<UpdateDocumentTypeCommand, DocumentTypeDto>();
            CreateMap<AddDocumentTypeCommand, DocumentTypeDto>();
            CreateMap<AddDocumentTypeCommand, DocumentType>();
            //CreateMap<UpdateTrialBalanceReferenceCommand, TrialBalanceReferenceDto>();
        }
    }

    public class CorrespondingMappingProfile : Profile
    {
        public CorrespondingMappingProfile()
        {
            CreateMap<CorrespondingMapping, CorrespondingMappingDto>().ReverseMap();
            CreateMap<AddCommandCorrespondingMapping, CorrespondingMapping>();
            CreateMap<UpdateCommandCorrespondingMapping, CorrespondingMapping>();
            //CreateMap<UpdateTrialBalanceReferenceCommand, TrialBalanceReferenceDto>();
        }
    }

    public class CorrespondingMappingExceptionProfile : Profile
    {
        public CorrespondingMappingExceptionProfile()
        {
            CreateMap<CorrespondingMappingException, CorrespondingMappingExceptionDto>().ReverseMap();
            CreateMap<AddCommandCorrespondingMappingException, CorrespondingMappingException>();
           //CreateMap<UpdateCommandCorrespondingMappingException, CorrespondingMappingException>();
            //CreateMap<UpdateTrialBalanceReferenceCommand, TrialBalanceReferenceDto>();
        }
    }
}