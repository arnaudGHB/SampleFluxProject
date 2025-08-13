using CBS.TransactionManagement.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Handlers;
using CBS.TransactionManagement.Queries;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CBS.TransactionManagement.MediatR.GAV.Query;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.MediatR.Queries;
using CBS.TransactionManagement.Data.Dto.Resource;
using CBS.TransactionManagement.MediatR.Commands;
using CBS.TransactionManagement.DailyStatisticBoard.Queries;
using CBS.TransactionManagement.Data.Dto.DailyStatisticBoard;

namespace CBS.TransactionManagement.API.Controllers.DailyStatisticBoard
{
    /// <summary>
    /// Account
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class GeneralDailyDashboardController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Account
        /// </summary>
        /// <param name="mediator"></param>

        private readonly ILogger<GeneralDailyDashboardController> _logger;
        public GeneralDailyDashboardController(IMediator mediator, ILogger<GeneralDailyDashboardController> logger = null)
        {
            _mediator = mediator;
            _logger = logger;
        }

        

        /// <summary>
        /// Retrieve all General Daily Dashboard entries within a specified date range.
        /// </summary>
        /// <param name="getAllGeneralDailyDashboardQuery">The query containing the date range (DateFrom and DateTo).</param>
        /// <returns>A list of GeneralDailyDashboardDto within the specified date range.</returns>
        [HttpPost("Dashboard/GetAllGeneralDailyDashboard", Name = "GetAllGeneralDailyDashboard")]
        [Produces("application/json", "application/xml", Type = typeof(List<GeneralDailyDashboardDto>))]
        public async Task<IActionResult> GetAllGeneralDailyDashboard([FromBody] GetAllGeneralDailyDashboardQuery getAllGeneralDailyDashboardQuery)
        {
            // Send the query to the mediator and retrieve the result.
            var result = await _mediator.Send(getAllGeneralDailyDashboardQuery);

            // Format the response based on the result (e.g., status code and content).
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieves account statistics, including total balances, active and inactive accounts, 
        /// and balances without blocked amounts, filtered by branch if a BranchId is specified.
        /// </summary>
        /// <param name="getAllAccountStatisticsQuery">The query parameters for retrieving account statistics,
        /// which may include a BranchId for branch-specific filtering and a QueryParameter for additional options.</param>
        /// <returns>An AccountStatisticsDto object containing account statistics data.</returns>
        [HttpPost("Dashboard/GetAllAccountsDashboard", Name = "GetAllAccountStatisticsQuery")]
        [Produces("application/json", "application/xml", Type = typeof(ServiceResponse<AccountStatisticsDto>))]
        public async Task<IActionResult> GetAllAccountStatisticsQuery([FromBody] GetAllAccountStatisticsQuery getAllAccountStatisticsQuery)
        {
            // Send the query to the mediator to process the request and get account statistics.
            var result = await _mediator.Send(getAllAccountStatisticsQuery);

            // Return the formatted response based on the result (e.g., status code and content).
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieve the General Daily Dashboard data for a specific branch within a specified date range.
        /// </summary>
        /// <param name="generalDailyDashboardByBranchQuery">The query containing the branch ID and date range (DateFrom and DateTo).</param>
        /// <returns>A GeneralDailyDashboardDto with dashboard details for the specified branch and date range.</returns>
        [HttpPost("Dashboard/GetGeneralDailyDashboardByBranch", Name = "GetGeneralDailyDashboardByBranch")]
        [Produces("application/json", "application/xml", Type = typeof(GeneralDailyDashboardDto))]
        public async Task<IActionResult> GetGeneralDailyDashboardByBranch([FromBody] GeneralDailyDashboardByBranchQuery generalDailyDashboardByBranchQuery)
        {
            // Send the query to the mediator and retrieve the result.
            var result = await _mediator.Send(generalDailyDashboardByBranchQuery);

            // Format the response based on the result (e.g., status code and content).
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieve the General Daily Dashboard data for all branches within a specified date range.
        /// </summary>
        /// <param name="getAllGeneralDailyDashboardSingleQuery">The query containing the date range (DateFrom and DateTo).</param>
        /// <returns>A GeneralDailyDashboardDto with dashboard summaries for all branches within the date range.</returns>
        [HttpPost("Dashboard/GetAllGeneralDailySummaryDashboard", Name = "GetAllGeneralDailyDashboardSingleQuery")]
        [Produces("application/json", "application/xml", Type = typeof(GeneralDailyDashboardDto))]
        public async Task<IActionResult> GetAllGeneralDailySummaryDashboard([FromBody] GetAllGeneralDailyDashboardSingleQuery getAllGeneralDailyDashboardSingleQuery)
        {
            // Send the query to the mediator and retrieve the result.
            var result = await _mediator.Send(getAllGeneralDailyDashboardSingleQuery);

            // Format the response based on the result (e.g., status code and content).
            return ReturnFormattedResponse(result);
        }
    }
}
