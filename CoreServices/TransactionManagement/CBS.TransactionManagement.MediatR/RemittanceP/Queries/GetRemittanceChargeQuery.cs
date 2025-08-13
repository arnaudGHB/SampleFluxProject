using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.RemittanceP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific CashReplenishment by its unique identifier.
    /// </summary>
    public class GetRemittanceChargeQuery : IRequest<ServiceResponse<RemittanceChargeDto>>
    {
        public string RemittanceType { get; set; }
        public decimal Amount { get; set; }
        public string SenderAccountNumber { get; set; }
        public string ChargeType { get; set; }
        public string TransfterType { get; set; }
    }
}
