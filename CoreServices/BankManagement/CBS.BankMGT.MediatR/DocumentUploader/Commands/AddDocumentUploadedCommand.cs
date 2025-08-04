using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Division.
    /// </summary>
    public class AddDocumentUploadedCommand : IRequest<ServiceResponse<DocumentUploadedDto>>
    {
        public IFormFileCollection FormFiles { get; set; }
        public string RootPath { get; set; }
        public string OperationID { get; set; }
        public string DocumentType { get; set; }
        public string ServiceType { get; set; }
        public string BaseURL { get; set; }
    }
    
}
