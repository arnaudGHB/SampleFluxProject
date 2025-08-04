using System.ComponentModel.DataAnnotations;

namespace CBS.AccountManagement.Data
{
 
    public class Teller : BaseEntity
    {
        public string Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        [Required]
        public string AccountId { get; set; }

        [Required]
        public string BranchId { get; set; }
        public string BankId { get; set; }
    }
}