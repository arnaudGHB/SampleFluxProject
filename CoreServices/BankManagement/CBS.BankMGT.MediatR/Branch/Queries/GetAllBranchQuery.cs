using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    public class GetAllBranchQuery : IRequest<ServiceResponse<List<BranchDto>>>
    {
    }
}
