using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Dto.CustomerAccountDto
{
    public class SavingProductDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public string InterestAccrualFrequency { get; set; }//Daily, Monthly, Yearly, Qaurterly
        public string PostingFrequency { get; set; }
        public string AccountType { get; set; }

    }
}
