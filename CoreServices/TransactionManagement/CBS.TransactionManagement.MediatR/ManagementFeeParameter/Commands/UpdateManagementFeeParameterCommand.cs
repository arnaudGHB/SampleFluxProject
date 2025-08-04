using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to update a ManagementFeeParameter.
    /// </summary>
    public class UpdateManagementFeeParameterCommand : IRequest<ServiceResponse<ManagementFeeParameterDto>>
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public decimal ManagementFeeFlat { get; set; }
        public decimal ManagementFeeRate { get; set; }
        public string ManagementFeeFrequency { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }

    }

}
