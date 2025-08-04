using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class CloseOfDayEventCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Reference of Transaction
        /// </summary>
        public string? TransactionReferenceId { get; set; } // Loan contract reference
        /// <summary>
        /// EventCode { "NORMAL_OOD", "NEGATIVE_OOD","POSITIVE_OOD","NORMAL_COD","NEGATIVE_COD", "POSITIVE_COD"}
        /// </summary>
        public string? EventCode { get; set; }  // Generated from rule
        /// <summary>
        /// Primary Amount
        /// </summary>

        public decimal Amount { get; set; }
        /// <summary>
        /// Secondary Amount
        /// </summary>
        public decimal AmountDifference { get; set; }
        public DateTime TransactionDate { get; set; }
        public string getEventDescription()
        {
            if (this.EventCode.Equals("OOD"))
            {
                return "Opening Of Day Event";
            }
            else if (this.EventCode.Equals("NEGATIVE_OOD"))
            {
                return "Negative Opening Of Day Event";
            }
            else if (this.EventCode.Equals("POSITIVE_OOD"))
            {
                return "Positive Opening Of Day Event";
            }
            else if (this.EventCode.Equals("NEGATIVE_COD"))
            {
                return "Negative Closing Of Day Event";
            }
            else if (this.EventCode.Equals("POSITIVE_COD"))
            {
                return "Positive Closing Of Day Event";
            }
            else
            {
                return " Closing Or Opening  Of Day Event";
            }
        }

    }
}
