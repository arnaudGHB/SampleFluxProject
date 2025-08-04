using CBS.BankMGT.Data;
using CBS.BankMGT.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.MediatR.Queries
{
    public class GetAllBankingZoneQuery : IRequest<ServiceResponse<List<BankingZoneDto>>>
    {
    }
}
