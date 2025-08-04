using CBS.NLoan.Data.Dto.BankP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.BankP.Command
{
    public class BranchPICallCommand : IRequest<ServiceResponse<BranchDto>>
    {
        public string BranchId { get; set; }
    }
}
