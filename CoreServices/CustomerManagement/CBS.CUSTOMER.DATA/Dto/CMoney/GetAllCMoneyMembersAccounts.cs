
using CBS.CUSTOMER.DATA.Entity.CMoneyP;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.CUSTOMER.DATA.Dto.CMoney
{
    public class GetAllCMoneyMembersAccounts
    {

        public string Id { get; set; }
        public string CustomerId { get; set; } // e.g., "0011234567"
        public string BranchCode { get; set; } // e.g., "001"
        public string LoginId { get; set; } // e.g., "0014567"
        public string PhoneNumber { get; set; }
        public string PIN { get; set; } // 4-digit PIN
        public string BranchId { get; set; }
        public DateTime ActivationDate { get; set; }
        public bool IsActive { get; set; }
        public string DefaultPin { get; set; }
        public bool HasChangeDefaultPin { get; set; }
        public DateTime LastSubcriptionRenewalDate { get; set; }
        public bool IsSubcribed { get; set; }
        public decimal LastPaymentAmount { get; set; }
        public string? DeactivationReason { get; set; }
        public string? SecretQuestion { get; set; }
        public string? SecretAnswer { get; set; }
        public string ActivatingBranchId { get; set; }
        public string ActivatingBranchCode { get; set; }
        public string ActivatingBranchName { get; set; }
        public string ActivatedBy { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
        public DateTime LastPaymentDate { get; set; }
        public virtual ICollection<CMoneySubscriptionDetail> CMoneySubscriptionDetails { get; set; }
        public int FailedAttempts { get; set; }
        public virtual Customer Customer { get; set; }

        public PaginationMetadata PaginationMetadata { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
    public class PaginationMetadata
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int Skip { get; set; }
        public int TotalPages { get; set; }
    }
  
}
