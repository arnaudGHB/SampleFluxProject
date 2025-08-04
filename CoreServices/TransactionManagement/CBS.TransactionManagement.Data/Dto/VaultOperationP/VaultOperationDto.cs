using CBS.TransactionManagement.Data.Dto.VaultP;
using CBS.TransactionManagement.Data.Entity.CashVaultP;
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.VaultOperationP
{

    public class VaultOperationDto
    {
        public string Id { get; set; }
        public string VaultId { get; set; }
        public string OperationType { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string DoneBy { get; set; }
        public string BranchId { get; set; }
        public string Reference  { get; set; }
        public VaultDto Vault { get; set; }
    }

}
