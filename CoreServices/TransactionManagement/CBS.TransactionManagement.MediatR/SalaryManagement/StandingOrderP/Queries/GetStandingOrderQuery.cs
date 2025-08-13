using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific CashReplenishment by its unique identifier.
    /// </summary>
    public class GetStandingOrderQuery : IRequest<ServiceResponse<StandingOrderDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the CashReplenishment to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
