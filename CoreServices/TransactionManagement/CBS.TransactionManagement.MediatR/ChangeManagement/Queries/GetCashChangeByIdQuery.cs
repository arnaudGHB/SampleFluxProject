using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.DailyTellerP.Queries
{
    /// <summary>
    /// Query to retrieve a specific cash change operation by its unique ID.
    /// </summary>
    public class GetCashChangeByIdQuery : IRequest<ServiceResponse<CashChangeHistoryDto>>
    {
        public string Id { get; set; } // Unique ID of the cash change operation
    }



}
