 
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
    public class CashMovementTrackingConfigurationController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// AccountType
        /// </summary>
        /// <param name="mediator"></param>
        public CashMovementTrackingConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// GEt CashMovementTrackingConfiguration By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("CashMovementTrackingConfiguration/GetCashMovementTrackingConfigurationQuery/{id}", Name = "GetCashMovementTrackingConfigurationQuery")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetCashMovementTrackingConfigurationQuery(string id)
        {
            var getCashReplenishmentQuery = new GetCashMovementTrackingConfigurationQuery { Id = id };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }




        /// <summary>
        /// Get All CashMovementTrackingConfiguration
        /// </summary>
        /// <returns></returns>
        [HttpGet("CashMovementTrackingConfiguration/GetAllCashMovementTrackingConfigurationQuery", Name = "GetAllCashMovementTrackingConfigurationQuery")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetAllCashMovementTrackingConfigurationQuery()
        {
            var getCashReplenishmentQuery = new GetAllCashMovementTrackingConfigurationQuery { };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Create  CashMovementTrackingConfigurationCommand
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("CashMovementTrackingConfiguration/AddCashMovementTrackingConfigurationCommand", Name = "AddCashMovementTrackingConfigurationCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddCashMovementTrackingConfigurationCommand(AddCashMovementTrackingConfigurationCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Update UpdateCashMovementTrackingConfigurationCommand By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateCommand"></param>
        /// <returns></returns>
        [HttpPut("CashMovementTrackingConfiguration/UpdateCashMovementTrackingConfigurationCommand/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UpdateCashMovementTrackingConfigurationCommand(string Id, UpdateCashMovementTrackingConfigurationCommand updateCommand)
        {
            updateCommand.Id = Id;
            var result = await _mediator.Send(updateCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete CashMovementTrackingConfiguration By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("CashMovementTrackingConfiguration/{Id}")]
        public async Task<IActionResult> DeleteAccountCartegory(string Id)
        {
            var deleteAccountCartegoryCommand = new DeleteCashMovementTrackingConfigurationCommand { Id = Id };
            var result = await _mediator.Send(deleteAccountCartegoryCommand);
            return ReturnFormattedResponse(result);
        }

    }
}