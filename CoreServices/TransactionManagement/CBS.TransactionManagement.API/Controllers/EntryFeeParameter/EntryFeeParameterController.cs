using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers
{
    /// <summary>
    /// EntryFeeParameter
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class EntryFeeParameterController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// EntryFeeParameter 
        /// </summary>
        /// <param name="mediator"></param>
        public EntryFeeParameterController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get EntryFeeParameter  By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("EntryFeeParameter/{id}", Name = "GetEntryFeeParameter ")]
        [Produces("application/json", "application/xml", Type = typeof(EntryFeeParameterDto))]
        public async Task<IActionResult> GetEntryFeeParameter (string id)
        {
            var getEntryFeeParameterQuery = new GetEntryFeeParameterQuery { Id = id };
            var result = await _mediator.Send(getEntryFeeParameterQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All EntryFeeParameter 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("EntryFeeParameter")]
        [Produces("application/json", "application/xml", Type = typeof(List<EntryFeeParameterDto>))]
        public async Task<IActionResult> GetEntryFeeParameter()
        {
            var getAllEntryFeeParameterQuery = new GetAllEntryFeeParameterQuery { };
            var result = await _mediator.Send(getAllEntryFeeParameterQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a EntryFeeParameter 
        /// </summary>
        /// <param name="addEntryFeeParameter Command"></param>
        /// <returns></returns>
        [HttpPost("EntryFeeParameter")]
        [Produces("application/json", "application/xml", Type = typeof(EntryFeeParameterDto))]
        public async Task<IActionResult> AddEntryFeeParameter (AddEntryFeeParameterCommand addEntryFeeParameterCommand)
        {
                var result = await _mediator.Send(addEntryFeeParameterCommand);
                return ReturnFormattedResponse(result);               
        }
        /// <summary>
        /// Update EntryFeeParameter  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateEntryFeeParameter Command"></param>
        /// <returns></returns>
        [HttpPut("EntryFeeParameter/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(EntryFeeParameterDto))]
        public async Task<IActionResult> UpdateEntryFeeParameter (string Id, UpdateEntryFeeParameterCommand updateEntryFeeParameterCommand)
        {
            updateEntryFeeParameterCommand.Id = Id;
            var result = await _mediator.Send(updateEntryFeeParameterCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete EntryFeeParameter  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("EntryFeeParameter/{Id}")]
        public async Task<IActionResult> DeleteEntryFeeParameter (string Id)
        {
            var deleteEntryFeeParameterCommand = new DeleteEntryFeeParameterCommand { Id = Id };
            var result = await _mediator.Send(deleteEntryFeeParameterCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
