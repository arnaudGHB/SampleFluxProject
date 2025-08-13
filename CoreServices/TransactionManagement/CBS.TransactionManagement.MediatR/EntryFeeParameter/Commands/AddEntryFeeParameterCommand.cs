using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new EntryFeeParameter.
    /// </summary>
    public class AddEntryFeeParameterCommand : IRequest<ServiceResponse<EntryFeeParameterDto>>
    {
        public string ProductId { get; set; }
        public decimal EntryFeeRate { get; set; }
        public decimal EntryFeeFlat { get; set; }
        public string BankId { get; set; }
        public decimal HeadOfficeShare { get; set; }
        public decimal PartnerShare { get; set; }
        public decimal SourceBrachOfficeShare { get; set; }
        public decimal DestinationBranchOfficeShare { get; set; }
    }

}
