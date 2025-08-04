using CBS.AccountManagement.API.Model;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.OperationEventNameAttributes.Queries;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// OperationEventAttribute
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class OperationEventAttributeController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// OperationEventAttribute
        /// </summary>
        /// <param name="mediator"></param>
        public OperationEventAttributeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get OperationEventAttribute By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("OperationEventAttribute/{id}", Name = "GetOperationEventAttribute")]
        [Produces("application/json", "application/xml", Type = typeof(OperationEventAttributesDto))]
        public async Task<IActionResult> GetOperationEventAttribute(string id)
        {
            var getOperationEventAttributeQuery = new GetOperationEventAttributesQuery { Id = id };
            var result = await _mediator.Send(getOperationEventAttributeQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All OperationEventAttributes
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("OperationEventAttributes")]
        [Produces("application/json", "application/xml", Type = typeof(List<OperationEventAttributesDto>))]
        public async Task<IActionResult> GetOperationEventAttributes()
        {
            var getAllOperationEventAttributeQuery = new GetAllOperationEventAttributesQuery { };
            var result = await _mediator.Send(getAllOperationEventAttributeQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get All OperationEventAttributes
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("OperationEventAttributes/GetOperationEventAttributesByOperationEventId/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<OperationEventAttributesDto>))]
        public async Task<IActionResult> GetOperationEventAttributesByOperationEventId(string id)
        {
            var getAllOperationEventAttributeQuery = new GetAllOperationEventAttributesByOperationEventIdQuery { OperationEventId = id };
            var result = await _mediator.Send(getAllOperationEventAttributeQuery);
            return Ok(result);
        }

        /// <summary>
        /// Upload all existing OperationEventAttributes
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("OperationEventAttributes/UploadFiles/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<OperationEventAttributesDto>))]
        public async Task<IActionResult> UploadOperationEventAttributes(IFormFile file)
        {
            var getAllOperationEventQuery = new UploadOperationEventAttributesQuery { OperationEventAttributes = new OperationEventAttributesModel().Upload(file) };
            var result = await _mediator.Send(getAllOperationEventQuery);
            return Ok(result);
        }

        /// <summary>
        /// Create a OperationEventAttribute
        /// </summary>
        /// <param name="addOperationEventAttributeCommand"></param>
        /// <returns></returns>
        [HttpPost("OperationEventAttribute")]
        [Produces("application/json", "application/xml", Type = typeof(OperationEventAttributesDto))]
        public async Task<IActionResult> AddOperationEventAttribute(AddOperationEventAttributesCommand addOperationEventAttributeCommand)
        {
            var result = await _mediator.Send(addOperationEventAttributeCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update OperationEventAttribute By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateOperationEventAttributeCommand"></param>
        /// <returns></returns>
        [HttpPut("OperationEventAttribute/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(OperationEventAttributesDto))]
        public async Task<IActionResult> UpdateOperationEventAttribute(string Id, UpdateOperationEventAttributesCommand updateOperationEventAttributeCommand)
        {
            updateOperationEventAttributeCommand.Id = Id;
            var result = await _mediator.Send(updateOperationEventAttributeCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete OperationEventAttribute By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("OperationEventAttribute/{Id}")]
        public async Task<IActionResult> DeleteOperationEventAttribute(string Id)
        {
            var deleteOperationEventAttributeCommand = new DeleteOperationEventAttributesCommand { Id = Id };
            var result = await _mediator.Send(deleteOperationEventAttributeCommand);
            return ReturnFormattedResponse(result);
        }
    }
}