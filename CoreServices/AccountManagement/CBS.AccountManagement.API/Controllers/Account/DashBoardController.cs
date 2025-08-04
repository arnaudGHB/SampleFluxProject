using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers.Account
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class DashBoardController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Dashbaord
        /// </summary>
        /// <param name="mediator"></param>
        public DashBoardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///  Dashbaord
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Dashbaord")]
        [Produces("application/json", "application/xml", Type = typeof(DashbaordDto))]
        public async Task<IActionResult> GetDashbaordDto()
        {
            var getAllAccountQuery = new GetDashbaordQuery { };
            var result = await _mediator.Send(getAllAccountQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        ///  DashboardStatisticsDto
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("DashboardStatisticsDto")]
        [Produces("application/json", "application/xml", Type = typeof(List<DashboardStatisticsDto>))]
        public async Task<IActionResult> GetDashboardStatisticsQuery(GetDashboardStatisticsQuery model)
        {
 
            var result = await _mediator.Send(model);
            return ReturnFormattedResponse(result);
        }
    }
}