using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new CloseFeeParameter.
    /// </summary>
    public class AddCloseFeeParameterCommand : IRequest<ServiceResponse<CloseFeeParameterDto>>
    {
        public string ProductId { get; set; }
        public decimal CloseFeeFlat { get; set; }
        public decimal CloseFeeRate { get; set; }
        public string BankId { get; set; }
        public decimal HeadOfficeShare { get; set; }
        public decimal PartnerShare { get; set; }
        public decimal SourceBrachOfficeShare { get; set; }
        public decimal DestinationBranchOfficeShare { get; set; }
    }

}
