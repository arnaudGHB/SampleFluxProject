 
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Handlers;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// AccountType
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class CashMovementTrackerController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// AccountType
        /// </summary>
        /// <param name="mediator"></param>
        public CashMovementTrackerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// GEt CashMovementTracker By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("CashMovementTracker/GetCashMovementTrackerQuery/{id}", Name = "GetCashMovementTrackerQuery")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetCashMovementTrackerQuery(string id)
        {
            var getCashReplenishmentQuery = new GetCashMovementTrackerQuery { Id = id };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }




        /// <summary>
        /// Get All CashMovementTracker
        /// </summary>
        /// <returns></returns>
        [HttpGet("CashMovementTracker/GetAllCashMovementTrackerQuery", Name = "GetAllCashMovementTrackerQuery")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetAllCashMovementTrackerQuery()
        {
            var getCashReplenishmentQuery = new GetAllCashMovementTrackerQuery { };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Create  CashMovementTrackerCommand
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("CashMovementTracker/AddCashMovementTrackerCommand", Name = "AddCashMovementTrackerCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddCashMovementTrackerCommand(AddCashMovementTrackerCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }



        /// <summary>
        /// Update UpdateCashMovementTrackerCommand By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateCommand"></param>
        /// <returns></returns>
        [HttpPut("CashMovementTracker/UpdateCashMovementTrackerCommand/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UpdateCashMovementTrackerCommand(string Id, UpdateCashMovementTrackerCommand updateCommand)
        {
            updateCommand.Id = Id;
            var result = await _mediator.Send(updateCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete CashMovementTracker By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("CashMovementTracker/{Id}")]
        public async Task<IActionResult> DeleteAccountCartegory(string Id)
        {
            var deleteAccountCartegoryCommand = new DeleteCashMovementTrackerCommand { Id = Id };
            var result = await _mediator.Send(deleteAccountCartegoryCommand);
            return ReturnFormattedResponse(result);
        }

    }
}