using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new TransferLimits.
    /// </summary>
    public class AddTransferLimitsCommand : IRequest<ServiceResponse<TransferParameterDto>>
    {
        public string ProductId { get; set; }
        public string TransferType { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal TransferFeeRate { get; set; }
        public decimal TransferFeeFlat { get; set; }
        public string BankId { get; set; }
        public decimal CamCCULShare { get; set; }
        public decimal FluxAndPTMShare { get; set; }
        public decimal HeadOfficeShare { get; set; }
        public decimal SourceBrachOfficeShare { get; set; }
        public decimal DestinationBranchOfficeShare { get; set; }
        public decimal SourceBrachOfficeShareCMoney { get; set; }
        public decimal DestinationBranchOfficeShareCMoney { get; set; }

        public decimal CamCCULShareCMoney { get; set; }
        public decimal FluxAndPTMShareCMoney { get; set; }
        public decimal HeadOfficeShareCMoney { get; set; }
        public string? Path { get; set; }

    }

}
