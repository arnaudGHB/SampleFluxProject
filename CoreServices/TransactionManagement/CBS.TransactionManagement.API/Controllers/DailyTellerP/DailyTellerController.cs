using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.DailyTellerP.Commands;
using CBS.TransactionManagement.DailyTellerP.Queries;
using CBS.TransactionManagement.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.DailyTellerP
{
    /// <summary>
    /// DailyTeller
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class DailyTellerController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// DailyTeller 
        /// </summary>
        /// <param name="mediator"></param>
        public DailyTellerController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get DailyTeller  By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DailyTeller/{id}", Name = "GetDailyTeller ")]
        [Produces("application/json", "application/xml", Type = typeof(DailyTellerDto))]
        public async Task<IActionResult> GetDailyTeller(string id)
        {
            var getDailyTellerQuery = new GetDailyTellerQuery { Id = id };
            var result = await _mediator.Send(getDailyTellerQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get Daily Teller by user ID and teller type.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="tellerType">The type of the teller (e.g., Primary, Sub-Teller).</param>
        /// <returns>A response containing the daily teller details or an appropriate error message.</returns>
        [HttpGet("DailyTeller/User", Name = "GetDailyTellerByUserId")]
        [Produces("application/json", "application/xml", Type = typeof(DailyTellerDto))]
        public async Task<IActionResult> GetDailyTellerByUserIdQuery(string userid, [FromQuery] string tellerType)
        {
            var getDailyTellerQuery = new GetDailyTellerByUserIdQuery { UserId = userid, TellerType=tellerType };
            var result = await _mediator.Send(getDailyTellerQuery);
            return ReturnFormattedResponse(result);
        }
        //
        /// <summary>
        /// Get All DailyTeller by dates
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("DailyTeller/All")]
        [Produces("application/json", "application/xml", Type = typeof(List<DailyTellerDto>))]
        public async Task<IActionResult> GetDailyTeller(GetAllDailyTellerQuery dailyTellerQuery)
        {
            var result = await _mediator.Send(dailyTellerQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All DailyTeller by dates and branch
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("DailyTeller/Branch")]
        [Produces("application/json", "application/xml", Type = typeof(List<DailyTellerDto>))]
        public async Task<IActionResult> GetAllDailyTellerByBranchQuery(GetAllDailyTellerByBranchQuery dailyTellerByBranchQuery)
        {
            var result = await _mediator.Send(dailyTellerByBranchQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Create a DailyTeller 
        /// </summary>
        /// <param name="addDailyTeller Command"></param>
        /// <returns></returns>
        [HttpPost("DailyTeller")]
        [Produces("application/json", "application/xml", Type = typeof(DailyTellerDto))]
        public async Task<IActionResult> AddDailyTeller(AddDailyTellerCommand addDailyTellerCommand)
        {
            var result = await _mediator.Send(addDailyTellerCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Update DailyTeller  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateDailyTeller Command"></param>
        /// <returns></returns>
        [HttpPut("DailyTeller/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(DailyTellerDto))]
        public async Task<IActionResult> UpdateDailyTeller(string Id, UpdateDailyTellerCommand updateDailyTellerCommand)
        {
            updateDailyTellerCommand.Id = Id;
            var result = await _mediator.Send(updateDailyTellerCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete DailyTeller  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("DailyTeller/{Id}")]
        public async Task<IActionResult> DeleteDailyTeller(string Id)
        {
            var deleteDailyTellerCommand = new DeleteDailyTellerCommand { Id = Id };
            var result = await _mediator.Send(deleteDailyTellerCommand);
            return ReturnFormattedResponse(result);
        }
    }

}
