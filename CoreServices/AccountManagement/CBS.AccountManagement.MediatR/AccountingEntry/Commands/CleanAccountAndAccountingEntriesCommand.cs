using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class CleanAccountAndAccountingEntriesCommand: IRequest<ServiceResponse<bool>>
    {

        public List<string> BranchIds { get; set; }    
    }
 
}