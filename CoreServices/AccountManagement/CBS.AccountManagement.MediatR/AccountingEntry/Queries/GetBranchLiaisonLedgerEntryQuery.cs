using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetBranchLiaisonLedgerEntryQuery : SystemQuery,IRequest<ServiceResponse<List<BranchLiaisonLedgerEntry>>>
    {
  

    }
 
}