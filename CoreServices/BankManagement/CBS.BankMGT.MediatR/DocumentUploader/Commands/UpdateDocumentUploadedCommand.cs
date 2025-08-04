using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CBS.BankMGT.MediatR.Commands
{
  
    public class UpdateDocumentUploadedCommand : IRequest<ServiceResponse<DocumentUploadedDto>>
    {
        public string Id { get; set; }
        public IFormFileCollection FormFiles { get; set; }
        public string RootPath { get; set; }
        public string OperationID { get; set; }
        public string DocumentType { get; set; }
        public string BaseURL { get; set; }
    }

}
