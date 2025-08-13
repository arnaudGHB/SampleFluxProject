using CBS.TransactionManagement.Data.Entity.CashVaultP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.VaultOperationP
{

    public class VaultOperation : BaseEntity
    {
        public string Id { get; set; }
        public string VaultId { get; set; }
        public string OperationType { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal CashInHand { get; set; }
        public decimal CashInVault { get; set; }
        public string? InitializationNote { get; set; }
        public string DoneBy { get; set; }
        public string BranchId { get; set; }
        public string Reference { get; set; }
        public virtual Vault Vault { get; set; }
    }

}
