using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Queries
{
    // Query Command
    public class GetSalaryAnalysisResultQueryByFileUploadId : IRequest<ServiceResponse<SalaryAnalysisResultDto>>
    {
        public string FileUploadId { get; set; } // The unique identifier of the salary analysis file
    }
}
