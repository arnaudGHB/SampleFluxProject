using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class TrailBalanceUploudController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// TrailBalanceUploud
        /// </summary>
        /// <param name="mediator"></param>
        public TrailBalanceUploudController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get TrailBalanceUploud By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("TrailBalanceUploud/{id}", Name = "GetTrailBalanceUploud")]
        [Produces("application/json", "application/xml", Type = typeof(TrailBalanceUploudDto))]
        public async Task<IActionResult> GetTrailBalanceUploud(string id)
        {
            var getTrailBalanceUploudQuery = new GetTrailBalanceUploudQuery { Id = id };
            var result = await _mediator.Send(getTrailBalanceUploudQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All TrailBalanceUplouds
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("TrailBalanceUplouds")]
        [Produces("application/json", "application/xml", Type = typeof(List<TrailBalanceUploudDto>))]
        public async Task<IActionResult> GetTrailBalanceUplouds()
        {
            var getAllTrailBalanceUploudQuery = new GetAllTrailBalanceUploudQuery { };
            var result = await _mediator.Send(getAllTrailBalanceUploudQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a TrailBalanceUploud
        /// </summary>
        /// <param name="addTrailBalanceUploudCommand"></param>
        /// <returns></returns>
        [HttpPost("TrailBalanceUploud")]
        [Produces("application/json", "application/xml", Type = typeof(TrailBalanceUploudDto))]
        public async Task<IActionResult> AddTrailBalanceUploud(AddTrailBalanceUploudCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update TrailBalanceUploud By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateTrailBalanceUploudCommand"></param>
        /// <returns></returns>
        [HttpPut("TrailBalanceUploud/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(TrailBalanceUploudDto))]
        public async Task<IActionResult> UpdateTrailBalanceUploud(string Id, UpdateTrailBalanceUploudCommand updateTrailBalanceUploudCommand)
        {
            updateTrailBalanceUploudCommand.Id = Id;
            var result = await _mediator.Send(updateTrailBalanceUploudCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete TrailBalanceUploud By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("TrailBalanceUploud/{Id}")]
        public async Task<IActionResult> DeleteTrailBalanceUploud(string Id)
        {
            var deleteTrailBalanceUploudCommand = new DeleteTrailBalanceUploudCommand { Id = Id };
            var result = await _mediator.Send(deleteTrailBalanceUploudCommand);
            return ReturnFormattedResponse(result);
        }
    }
}