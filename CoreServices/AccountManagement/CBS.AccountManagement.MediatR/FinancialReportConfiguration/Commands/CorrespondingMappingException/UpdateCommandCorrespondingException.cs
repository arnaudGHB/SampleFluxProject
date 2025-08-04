using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class UpdateCommandCorrespondingException : IRequest<ServiceResponse<bool>>
    {
        public string  Id { get; set; }
        public string DocumentReferenceCodeId { get; set; }
        public string AccountNumber { get; set; }
        public string ChartOfAccountId { get; set; }
        public string Cartegory { get; set; }
        public string BalanceType { get; set; }
        public bool IsActive { get; set; }
    }


}
