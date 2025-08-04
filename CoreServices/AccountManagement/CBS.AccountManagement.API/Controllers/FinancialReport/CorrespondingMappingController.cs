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
    public class CorrespondingMappingController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// CorrespondingMapping
        /// </summary>
        /// <param name="mediator"></param>
        public CorrespondingMappingController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Create a CorrespondingMapping
        /// </summary>
        /// <param name="AddCorrespondingMappingCommand"></param>
        /// <returns></returns>
        [HttpPost("CorrespondingMapping/Create")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddCorrespondingMapping(AddCommandCorrespondingMapping addStatementModelCommand)
        {
            var result = await _mediator.Send(addStatementModelCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get CorrespondingMapping By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("CorrespondingMapping/{id}", Name = "GetCorrespondingMappingById")]
        [Produces("application/json", "application/xml", Type = typeof(CorrespondingMappingDto))]
        public async Task<IActionResult> GetCorrespondingMapping(string id)
        {
            var getStatementModelQuery = new GetCorrespondingMappingQuery { Id = id };
            var result = await _mediator.Send(getStatementModelQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All CorrespondingMapping
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("CorrespondingMapping")]
        [Produces("application/json", "application/xml", Type = typeof(List<CorrespondingMappingDto>))]
        public async Task<IActionResult> GetAllCorrespondingMapping()
        {
            var getAllStatementModelQuery = new GetAllCorrespondingMappingQuery { };
            var result = await _mediator.Send(getAllStatementModelQuery);
            return Ok(result);
        }

        /// <summary>
        /// Update CorrespondingMapping By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateStatementModelCommand"></param>
        /// <returns></returns>
        [HttpPut("CorrespondingMapping/Update/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UpdateCorrespondingMapping(string Id, UpdateCorrespondingAccountCommand updateStatementModelCommand)
        {
            updateStatementModelCommand.Id = Id;
            var result = await _mediator.Send(updateStatementModelCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete CorrespondingMapping By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("CorrespondingMapping/Delete/{Id}")]
        public async Task<IActionResult> DeleteCorrespondingMapping(string Id)
        {
            var deleteStatementModelCommand = new DeleteCorrespondingMappingCommand { Id = Id };
            var result = await _mediator.Send(deleteStatementModelCommand);
            return ReturnFormattedResponse(result);
        }
    }
}