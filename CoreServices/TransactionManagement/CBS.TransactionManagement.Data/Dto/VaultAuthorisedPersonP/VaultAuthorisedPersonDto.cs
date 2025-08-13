using CBS.TransactionManagement.Data.Dto.VaultP;
using CBS.TransactionManagement.Data.Entity.VaultOperationP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.CashVaultP
{
    public class VaultAuthorisedPersonDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string VaultId { get; set; }
        public string BranchId { get; set; }
        public string BranchCode { get; set; }
        public string Date { get; set; }
        public bool IsActive { get; set; }
        public VaultDto Vault { get; set; }
    }
}
