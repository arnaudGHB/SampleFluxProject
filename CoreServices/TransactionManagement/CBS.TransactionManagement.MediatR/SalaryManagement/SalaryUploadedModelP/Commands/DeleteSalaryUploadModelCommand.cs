using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Commands
{
    /// <summary>
    /// Represents a command to delete a SalaryUploadModel by its ID.
    /// </summary>
    public class DeleteSalaryUploadModelCommand : IRequest<ServiceResponse<bool>>
    {
        public string FileUploadId { get; set; } // The ID of the SalaryUploadModel to delete.
    }


}
