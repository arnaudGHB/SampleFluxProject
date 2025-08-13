using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.AccountingDayOpening
{
    public class CloseOrOpenAccountingDayResultDto
    {
        public string BranchId { get; set; }
        public string BranchCode { get; set; }  // New property for BranchCode
        public string BranchName { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
  
}
