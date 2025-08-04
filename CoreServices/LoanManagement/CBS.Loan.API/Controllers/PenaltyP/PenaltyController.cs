//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.PenaltyP;
using CBS.NLoan.MediatR.PenaltyMediaR.Commands;
using CBS.NLoan.MediatR.PenaltyMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.PenaltyP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class PenaltyController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Penalty
        /// </summary>
        /// <param name="mediator"></param>
        public PenaltyController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Penalty By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Penalty/{Id}", Name = "GetPenalty")]
        [Produces("application/json", "application/xml", Type = typeof(PenaltyDto))]
        public async Task<IActionResult> GetPenalty(string Id)
        {
            var getPenaltyQuery = new GetPenaltyQuery { Id = Id };
            var result = await _mediator.Send(getPenaltyQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Penaltys
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Penalties")]
        [Produces("application/json", "application/xml", Type = typeof(List<PenaltyDto>))]
        public async Task<IActionResult> GetPenaltys()
        {
            var getAllPenaltyQuery = new GetAllPenaltyQuery { };
            var result = await _mediator.Send(getAllPenaltyQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Penalty
        /// </summary>
        /// <param name="addPenaltyCommand"></param>
        /// <returns></returns>
        [HttpPost("Penalty")]
        [Produces("application/json", "application/xml", Type = typeof(PenaltyDto))]
        public async Task<IActionResult> AddPenalty(AddPenaltyCommand addPenaltyCommand)
        {
            var result = await _mediator.Send(addPenaltyCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update Penalty By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updatePenaltyCommand"></param>
        /// <returns></returns>
        [HttpPut("Penalty/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(PenaltyDto))]
        public async Task<IActionResult> UpdatePenalty(string Id, UpdatePenaltyCommand updatePenaltyCommand)
        {
            updatePenaltyCommand.Id = Id;
            var result = await _mediator.Send(updatePenaltyCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Penalty By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("Penalty/{Id}")]
        public async Task<IActionResult> DeletePenalty(string Id)
        {
            var deletePenaltyCommand = new DeletePenaltyCommand { Id = Id };
            var result = await _mediator.Send(deletePenaltyCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
