//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.AlertProfileP;
using CBS.NLoan.MediatR.AlertProfileMediaR.Commands;
using CBS.NLoan.MediatR.AlertProfileMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.AlertProfileP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AlertProfileController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// AlertProfile
        /// </summary>
        /// <param name="mediator"></param>
        public AlertProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get AlertProfile By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AlertProfile/{Id}", Name = "GetAlertProfile")]
        [Produces("application/json", "application/xml", Type = typeof(AlertProfileDto))]
        public async Task<IActionResult> GetAlertProfile(string Id)
        {
            var getAlertProfileQuery = new GetAlertProfileQuery { Id = Id };
            var result = await _mediator.Send(getAlertProfileQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All AlertProfiles
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("AlertProfiles")]
        [Produces("application/json", "application/xml", Type = typeof(List<AlertProfileDto>))]
        public async Task<IActionResult> GetAlertProfiles()
        {
            var getAllAlertProfileQuery = new GetAllAlertProfileQuery { };
            var result = await _mediator.Send(getAllAlertProfileQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a AlertProfile
        /// </summary>
        /// <param name="addAlertProfileCommand"></param>
        /// <returns></returns>
        [HttpPost("AlertProfile")]
        [Produces("application/json", "application/xml", Type = typeof(AlertProfileDto))]
        public async Task<IActionResult> AddAlertProfile(AddAlertProfileCommand addAlertProfileCommand)
        {
            var result = await _mediator.Send(addAlertProfileCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update AlertProfile By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateAlertProfileCommand"></param>
        /// <returns></returns>
        [HttpPut("AlertProfile/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(AlertProfileDto))]
        public async Task<IActionResult> UpdateAlertProfile(string Id, UpdateAlertProfileCommand updateAlertProfileCommand)
        {
            updateAlertProfileCommand.Id = Id;
            var result = await _mediator.Send(updateAlertProfileCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete AlertProfile By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("AlertProfile/{Id}")]
        public async Task<IActionResult> DeleteAlertProfile(string Id)
        {
            var deleteAlertProfileCommand = new DeleteAlertProfileCommand { Id = Id };
            var result = await _mediator.Send(deleteAlertProfileCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
