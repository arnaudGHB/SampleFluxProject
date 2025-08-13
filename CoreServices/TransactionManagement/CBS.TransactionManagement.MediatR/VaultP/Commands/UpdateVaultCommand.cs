using CBS.TransactionManagement.Data.Dto.OTP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.VaultP
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class UpdateVaultCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BranchId { get; set; }
        public string BranchCode { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string? Diamention { get; set; }
        public decimal MaximumCapacity { get; set; }
        public bool IsActive { get; set; }
    }

}
