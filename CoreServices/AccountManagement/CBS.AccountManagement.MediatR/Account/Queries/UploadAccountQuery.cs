using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

namespace CBS.AccountManagement.MediatR.Queries
{

    public class UploadAccountQuery : IRequest<ServiceResponse<List<AccountUploadDto>>>
    {
        public List<AccountDto> Accounts { get; set; }

    }
}
