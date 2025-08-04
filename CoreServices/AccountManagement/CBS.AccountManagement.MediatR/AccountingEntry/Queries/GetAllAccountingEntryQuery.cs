using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllAccountingEntryQuery : SystemQuery,IRequest<ServiceResponse<List<AccountingEntryDto>>>
    {
  

    }
    public class SystemQuery
    {

        public DateTime ToDate { get; set; }


        public DateTime FromDate { get; set; }  

        public string? SearchOption { get; set; }

        public string? BranchId { get; set; }

        public string? AccountId { get; set; }

        public bool DateIsNull(DateTime ToDate)
        {
          return   ToDate.Equals( new DateTime());
        }
    }

}