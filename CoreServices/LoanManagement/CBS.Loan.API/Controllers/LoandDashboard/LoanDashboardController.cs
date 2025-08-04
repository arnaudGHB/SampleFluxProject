
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.AlertProfileP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.MediatR.AlertProfileMediaR.Commands;
using CBS.NLoan.MediatR.AlertProfileMediaR.Queries;
using CBS.NLoan.MediatR.LoanMediaR.Queries;
using CBS.TransactionManagement.Data.Dto.DailyStatisticBoard;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.AlertProfileP.LoandDashboard
{
    /// <summary>
    /// Controller for handling loan dashboard-related operations.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class LoanDashboardController : BaseController
    {
        /// <summary>
        /// The mediator used to handle requests in the controller.
        /// </summary>
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoanDashboardController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for handling requests.</param>
        public LoanDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves loan dashboard data for a specified date range and optional branch filter.
        /// </summary>
        /// <param name="getAllGeneralDailyDashboardQuery">
        /// The query containing the date range (`DateFrom` and `DateTo`).
        /// Optionally, the query may include parameters for filtering by branch or other criteria.
        /// </param>
        /// <returns>
        /// Returns a list of <see cref="LoanMainDashboardDto"/> objects representing the loan dashboard data 
        /// for the specified date range and optional branch filter.
        /// </returns>
        [HttpPost("Dashboard/GetLoanDashboardQuery", Name = "GetLoanDashboardQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanMainDashboardDto>))]
        public async Task<IActionResult> GetLoanDashboardQuery([FromBody] GetLoanDashboardQuery getAllGeneralDailyDashboardQuery)
        {
            // Send the query to the mediator to handle the request and retrieve the result.
            var result = await _mediator.Send(getAllGeneralDailyDashboardQuery);

            // Format and return the response based on the result status and content.
            return ReturnFormattedResponse(result);
        }
    }

}
