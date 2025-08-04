using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CBS.NLoan.MediatR.LoanP.Command
{
    public class LoanUploadCommand : IRequest<ServiceResponse<List<LoanUpload>>>
    {

        public IFormFile? formFile { get; set; }


    }
}
