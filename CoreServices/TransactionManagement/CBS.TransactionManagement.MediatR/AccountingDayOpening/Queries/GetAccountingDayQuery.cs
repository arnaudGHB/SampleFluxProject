using CBS.TransactionManagement.Data.Dto.AccountingDayOpening;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries.AccountingDayOpening
{
    /// <summary>
    /// Represents a query to retrieve a specific TempAccount by its unique identifier.
    /// </summary>
    public class GetAccountingDayBYIdQuery : IRequest<ServiceResponse<AccountingDayDto>>
    {
        public string Id { get; set; }
    }
}
