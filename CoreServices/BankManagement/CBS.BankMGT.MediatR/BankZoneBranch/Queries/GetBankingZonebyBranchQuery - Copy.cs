using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data.Entity;
using CBS.BankMGT.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific BankZoneBranch by its unique identifier.
    /// </summary>
    public class GetBankingZonebyBankBranchQuery : IRequest<ServiceResponse<List<LocalBrancheZoneInfo>>>
    {
        public string Type { get; set; }
        public string Id { get; set; }
       
    }

}
