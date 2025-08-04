using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data.Dto
{
    public class FundingLineDto:BaseEntity
    {
        public string Id { get; set; }
        public string CurrencyID { get; set; }
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Purpose { get; set; }
        public double Amount { get; set; }
    }
}