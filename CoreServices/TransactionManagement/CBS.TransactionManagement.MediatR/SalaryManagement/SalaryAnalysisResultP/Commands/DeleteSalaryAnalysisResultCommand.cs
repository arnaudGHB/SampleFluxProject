using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Commands
{
    // Command
    public class DeleteSalaryAnalysisResultCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; } // The unique identifier of the salary analysis result to delete
    }
}
