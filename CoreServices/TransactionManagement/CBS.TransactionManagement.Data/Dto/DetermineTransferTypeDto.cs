using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto
{
    public class DetermineTransferTypeDto
    {
        public string TransferType { get; set; }
        public bool IsInterBranch { get; set; }
    }
}
