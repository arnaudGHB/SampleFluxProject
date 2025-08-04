using CBS.NLoan.Data.Entity;
using CBS.NLoan.Helper.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.MediatR.EnumData
{
 
    public class GetAllEnumDataCommand : IRequest<ServiceResponse<EnumDto>>
    {
    }
}
