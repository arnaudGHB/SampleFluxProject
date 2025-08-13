using CBS.TransactionManagement.Data.Entity.VaultOperationP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.CashVaultP
{
    public class Vault:BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string BranchId { get; set; }
        public string BranchCode { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string? Diamention { get; set; }
        public decimal MaximumCapacity { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal PreviouseBalance { get; set; }
        public string? EnryptedBalance { get; set; }
        public bool IsActive { get; set; }
        public decimal LastOperationAmount { get; set; }
        public string? LastOperation { get; set; }

        // Closing Notes and Coins Counts
        public int ClosingNote10000 { get; set; }
        public int ClosingNote5000 { get; set; }
        public int ClosingNote2000 { get; set; }
        public int ClosingNote1000 { get; set; }
        public int ClosingNote500 { get; set; }
        public int ClosingCoin500 { get; set; }
        public int ClosingCoin100 { get; set; }
        public int ClosingCoin50 { get; set; }
        public int ClosingCoin25 { get; set; }
        public int ClosingCoin10 { get; set; }
        public int ClosingCoin5 { get; set; }
        public int ClosingCoin1 { get; set; }
        public virtual ICollection<VaultOperation> VaultOperations { get; set; }
        public virtual ICollection<VaultAuthorisedPerson> VaultAuthorisedPersons { get; set; }
        
    }
    
}
