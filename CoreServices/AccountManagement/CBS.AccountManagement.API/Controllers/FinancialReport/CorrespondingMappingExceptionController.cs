using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// StatementModel
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class CorrespondingMappingExceptionController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// CorrespondingMappingException
        /// </summary>
        /// <param name="mediator"></param>
        public CorrespondingMappingExceptionController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Create a CorrespondingMappingException
        /// </summary>
        /// <param name="AddCorrespondingMappingCommand"></param>
        /// <returns></returns>
        [HttpPost("CorrespondingMappingException/Create")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddCorrespondingMapping(AddCommandCorrespondingMappingException addStatementModelCommand)
        {
            var result = await _mediator.Send(addStatementModelCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get CorrespondingMappingException By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("CorrespondingMappingException/{id}", Name = "GetCorrespondingMappingExceptionById")]
        [Produces("application/json", "application/xml", Type = typeof(CorrespondingMappingExceptionDto))]
        public async Task<IActionResult> GetCorrespondingMappingException(string id)
        {
            var getStatementModelQuery = new GetCorrespondingMappingExceptionQuery { Id = id };
            var result = await _mediator.Send(getStatementModelQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All CorrespondingMappingException
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("CorrespondingMappingException")]
        [Produces("application/json", "application/xml", Type = typeof(List<CorrespondingMappingExceptionDto>))]
        public async Task<IActionResult> GetAllCorrespondingMappingException()
        {
            var getAllStatementModelQuery = new GetCorrespondingMappingExceptionQuery { };
            var result = await _mediator.Send(getAllStatementModelQuery);
            return Ok(result);
        }

        /// <summary>
        /// Update CorrespondingMappingException By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateStatementModelCommand"></param>
        /// <returns></returns>
        [HttpPut("CorrespondingMappingException/Update/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UpdateCorrespondingMappingException(string Id, UpdateCorrespondingAccountCommand updateStatementModelCommand)
        {
            updateStatementModelCommand.Id = Id;
            var result = await _mediator.Send(updateStatementModelCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete CorrespondingMappingException By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("CorrespondingMappingException/Delete/{Id}")]
        public async Task<IActionResult> DeleteCorrespondingMappingException(string Id)
        {
            var deleteStatementModelCommand = new DeleteCorrespondingMappingExceptionCommand { Id = Id };
            var result = await _mediator.Send(deleteStatementModelCommand);
            return ReturnFormattedResponse(result);
        }
    }
}