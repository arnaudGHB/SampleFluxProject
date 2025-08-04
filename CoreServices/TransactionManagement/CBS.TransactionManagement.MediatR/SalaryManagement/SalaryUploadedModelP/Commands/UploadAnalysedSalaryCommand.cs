using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Commands
{
    /// <summary>
    /// Represents a command to add a new ReopenFeeParameter.
    /// </summary>
    public class UploadAnalysedSalaryCommand : IRequest<ServiceResponse<bool>>
    {
        public IFormFile AttachedFiles { get; set; } // The uploaded file
        public string UploadFileId { get; set; }
    }

}
