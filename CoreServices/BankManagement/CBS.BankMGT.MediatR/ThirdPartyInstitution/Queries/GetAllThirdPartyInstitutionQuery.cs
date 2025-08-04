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
    public class GetAllThirdPartyInstitutionQuery : IRequest<ServiceResponse<List<CorrespondingBankDto>>>
    {
    }
}
