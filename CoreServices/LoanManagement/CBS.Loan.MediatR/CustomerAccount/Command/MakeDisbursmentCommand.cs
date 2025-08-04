using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.MediatR.CustomerP.Command
{
 
    public class MakeDisbursmentCommand : IRequest<ServiceResponse<bool>>
    {
        public decimal Amount { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public string CustomerId { get; set; }
        public string LoanId { get; set; }
        public string Note { get; set; }
        public bool IsNormal { get; set; }
        public string LoanApplicationType { get; set; }
        public decimal RestructuredBalance { get; set; }
        public decimal RequestedAmount { get; set; }
        public bool IsChargeInclussive { get; set; }
        public string LoanProductId { get; set; }
        public OldLoanPayment? OldLoanPayment { get; set; }
        public List<ChargCollection>? ChargCollections { get; set; }
    }
    public class ChargCollection
    {
        public decimal Amount { get; set; }
        public string? EventCode { get; set; }
        public string? ChargeName { get; set; }
    }
}
