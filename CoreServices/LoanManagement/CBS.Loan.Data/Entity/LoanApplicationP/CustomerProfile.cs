using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class CustomerProfile : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string CustomerProfileId { get; set; }
        public string LoanProductId { get; set; }
        public virtual LoanProduct? LoanProduct { get; set; }
    }
}
