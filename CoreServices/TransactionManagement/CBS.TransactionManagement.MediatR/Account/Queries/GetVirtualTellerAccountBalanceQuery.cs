using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class GetVirtualTellerAccountBalanceQuery : IRequest<ServiceResponse<AccountDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Transaction to be retrieved.
        /// </summary>
        public string QueryParameter { get; set; }//MobileMoneyMTN or MobileMoneyORANGE
        public string BrnachId { get; set; }

    }
}
