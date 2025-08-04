using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new ManagementFeeParameter.
    /// </summary>
    public class AddManagementFeeParameterCommand : IRequest<ServiceResponse<ManagementFeeParameterDto>>
    {
        public string ProductId { get; set; }
        public decimal ManagementFeeFlat { get; set; }
        public decimal ManagementFeeRate { get; set; }
        public string ManagementFeeFrequency { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }

    }

}
