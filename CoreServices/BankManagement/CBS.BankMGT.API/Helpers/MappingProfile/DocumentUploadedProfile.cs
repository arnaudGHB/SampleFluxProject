using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class DocumentUploadedProfile: Profile
    {
        public DocumentUploadedProfile()
        {
            CreateMap<DocumentUploaded, DocumentUploadedDto>().ReverseMap();
            CreateMap<AddDocumentUploadedCommand, DocumentUploaded>();
            CreateMap<UpdateDocumentUploadedCommand, DocumentUploaded>();
        }
    }
}
