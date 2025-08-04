using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class AddCommandCorrespondingMappingException : IRequest<ServiceResponse<bool>>
    {

        public string DocumentReferenceCodeId { get; set; }
        public string AccountNumber { get; set; }
        public string ChartOfAccountId { get; set; }
        public string Cartegory { get; set; }
        public string BalanceType { get; set; }
        public bool IsActive { get; set; }

    }
}
