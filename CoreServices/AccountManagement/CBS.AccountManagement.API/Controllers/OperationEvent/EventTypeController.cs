using CBS.AccountManagement.API.Model;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// OperationEvent
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class OperationEventController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// OperationEvent
        /// </summary>
        /// <param name="mediator"></param>
        public OperationEventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get OperationEvent By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("OperationEvent/{id}", Name = "GetOperationEvent")]
        [Produces("application/json", "application/xml", Type = typeof(OperationEventDto))]
        public async Task<IActionResult> GetOperationEvent(string id)
        {
            var getOperationEventQuery = new GetOperationEventQuery { Id = id };
            var result = await _mediator.Send(getOperationEventQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All OperationEvent
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("OperationEvent")]
        [Produces("application/json", "application/xml", Type = typeof(List<OperationEventDto>))]
        public async Task<IActionResult> GetOperationEvent()
        {
            var getAllOperationEventQuery = new GetAllOperationEventQuery { };
            var result = await _mediator.Send(getAllOperationEventQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Upload all existing OperationEvent
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("OperationEvent/UploadFiles/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<OperationEventDto>))]
        public async Task<IActionResult> UploadOperationEvent(IFormFile file)
        {
            var getAllOperationEventQuery = new UploadOperationEventQuery { OperationEvent = new OperationEventModel().Upload(file) };
            var result = await _mediator.Send(getAllOperationEventQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a OperationEvent
        /// </summary>
        /// <param name="addOperationEventCommand"></param>
        /// <returns></returns>
        [HttpPost("OperationEvent")]
        [Produces("application/json", "application/xml", Type = typeof(OperationEventDto))]
        public async Task<IActionResult> AddOperationEvent(AddOperationEventCommand addOperationEventCommand)
        {
            var result = await _mediator.Send(addOperationEventCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update OperationEvent By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateOperationEventCommand"></param>
        /// <returns></returns>
        [HttpPut("OperationEvent/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(OperationEventDto))]
        public async Task<IActionResult> UpdateOperationEvent(string Id, UpdateOperationEventCommand updateOperationEventCommand)
        {
            updateOperationEventCommand.Id = Id;
            var result = await _mediator.Send(updateOperationEventCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete OperationEvent By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("OperationEvent/{Id}")]
        public async Task<IActionResult> DeleteOperationEvent(string Id)
        {
            var deleteOperationEventCommand = new DeleteOperationEventCommand { Id = Id };
            var result = await _mediator.Send(deleteOperationEventCommand);
            return ReturnFormattedResponse(result);
        }
    }
}