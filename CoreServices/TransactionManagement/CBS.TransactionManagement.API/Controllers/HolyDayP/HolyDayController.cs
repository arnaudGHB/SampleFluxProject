
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data.Dto.HolyDayP;
using CBS.TransactionManagement.MediatR.HolyDayMediaR.HolyDayP.Commands;
using CBS.TransactionManagement.MediatR.HolyDayP.Commands;
using CBS.TransactionManagement.MediatR.HolyDayP.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.HolyDayP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class HolyDayController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// HolyDay
        /// </summary>
        /// <param name="mediator"></param>
        public HolyDayController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get HolyDay By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("HolyDay/{Id}", Name = "GetHolyDay")]
        [Produces("application/json", "application/xml", Type = typeof(HolyDayDto))]
        public async Task<IActionResult> GetHolyDay(string Id)
        {
            var getHolyDayQuery = new GetHolyDayQuery { Id = Id };
            var result = await _mediator.Send(getHolyDayQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All HolyDays
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("HolyDays")]
        [Produces("application/json", "application/xml", Type = typeof(List<HolyDayDto>))]
        public async Task<IActionResult> GetHolyDays(GetAllHolyDayQuery getAllHolyDayQuery)
        {
            var result = await _mediator.Send(getAllHolyDayQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a HolyDay
        /// </summary>
        /// <param name="addHolyDayCommand"></param>
        /// <returns></returns>
        [HttpPost("HolyDay")]
        [Produces("application/json", "application/xml", Type = typeof(HolyDayDto))]
        public async Task<IActionResult> AddHolyDay(AddHolyDayCommand addHolyDayCommand)
        {
            var result = await _mediator.Send(addHolyDayCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update HolyDay By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateHolyDayCommand"></param>
        /// <returns></returns>
        [HttpPut("HolyDay/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(HolyDayDto))]
        public async Task<IActionResult> UpdateHolyDay(string Id, UpdateHolyDayCommand updateHolyDayCommand)
        {
            updateHolyDayCommand.Id = Id;
            var result = await _mediator.Send(updateHolyDayCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete HolyDay By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("HolyDay/{Id}")]
        public async Task<IActionResult> DeleteHolyDay(string Id)
        {
            var deleteHolyDayCommand = new DeleteHolyDayCommand { Id = Id };
            var result = await _mediator.Send(deleteHolyDayCommand);
            return ReturnFormattedResponse(result);
        }
    }
}