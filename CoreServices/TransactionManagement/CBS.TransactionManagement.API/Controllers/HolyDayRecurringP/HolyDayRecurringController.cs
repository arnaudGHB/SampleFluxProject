
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data.Dto.HolyDayRecurringP;
using CBS.TransactionManagement.MediatR.HolyDayRecurringP.Commands;
using CBS.TransactionManagement.MediatR.HolyDayRecurringP.Queries;
using CBS.TransactionManagement.MediatR.RecurringRecurringP.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.HolyDayRecurringRecurring
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class HolyDayRecurringController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// HolyDayRecurring
        /// </summary>
        /// <param name="mediator"></param>
        public HolyDayRecurringController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get HolyDayRecurring By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("HolyDayRecurring/{Id}", Name = "GetHolyDayRecurring")]
        [Produces("application/json", "application/xml", Type = typeof(HolyDayRecurringDto))]
        public async Task<IActionResult> GetHolyDayRecurring(string Id)
        {
            var getHolyDayRecurringQuery = new GetHolyDayRecurringQuery { Id = Id };
            var result = await _mediator.Send(getHolyDayRecurringQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All HolyDayRecurrings
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("HolyDayRecurrings")]
        [Produces("application/json", "application/xml", Type = typeof(List<HolyDayRecurringDto>))]
        public async Task<IActionResult> GetHolyDayRecurrings(GetAllHolyDayRecurringQuery getAllHolyDayRecurringQuery)
        {
            var result = await _mediator.Send(getAllHolyDayRecurringQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a HolyDayRecurring
        /// </summary>
        /// <param name="addHolyDayRecurringCommand"></param>
        /// <returns></returns>
        [HttpPost("HolyDayRecurring")]
        [Produces("application/json", "application/xml", Type = typeof(HolyDayRecurringDto))]
        public async Task<IActionResult> AddHolyDayRecurring(AddHolyDayRecurringCommand addHolyDayRecurringCommand)
        {
            var result = await _mediator.Send(addHolyDayRecurringCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update HolyDayRecurring By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateHolyDayRecurringCommand"></param>
        /// <returns></returns>
        [HttpPut("HolyDayRecurring/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(HolyDayRecurringDto))]
        public async Task<IActionResult> UpdateHolyDayRecurring(string Id, UpdateHolyDayRecurringCommand updateHolyDayRecurringCommand)
        {
            updateHolyDayRecurringCommand.Id = Id;
            var result = await _mediator.Send(updateHolyDayRecurringCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete HolyDayRecurring By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("HolyDayRecurring/{Id}")]
        public async Task<IActionResult> DeleteHolyDayRecurring(string Id)
        {
            var deleteHolyDayRecurringCommand = new DeleteHolyDayRecurringCommand { Id = Id };
            var result = await _mediator.Send(deleteHolyDayRecurringCommand);
            return ReturnFormattedResponse(result);
        }
    }
}