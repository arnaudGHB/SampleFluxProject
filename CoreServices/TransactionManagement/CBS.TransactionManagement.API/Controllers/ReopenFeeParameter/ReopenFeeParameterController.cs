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
    /// ReopenFeeParameter
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ReopenFeeParameterController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// ReopenFeeParameter 
        /// </summary>
        /// <param name="mediator"></param>
        public ReopenFeeParameterController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get ReopenFeeParameter  By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ReopenFeeParameter/{id}", Name = "GetReopenFeeParameter ")]
        [Produces("application/json", "application/xml", Type = typeof(ReopenFeeParameterDto))]
        public async Task<IActionResult> GetReopenFeeParameter (string id)
        {
            var getReopenFeeParameterQuery = new GetReopenFeeParameterQuery { Id = id };
            var result = await _mediator.Send(getReopenFeeParameterQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All ReopenFeeParameter 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("ReopenFeeParameter")]
        [Produces("application/json", "application/xml", Type = typeof(List<ReopenFeeParameterDto>))]
        public async Task<IActionResult> GetReopenFeeParameter()
        {
            var getAllReopenFeeParameterQuery = new GetAllReopenFeeParameterQuery { };
            var result = await _mediator.Send(getAllReopenFeeParameterQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a ReopenFeeParameter 
        /// </summary>
        /// <param name="addReopenFeeParameter Command"></param>
        /// <returns></returns>
        [HttpPost("ReopenFeeParameter")]
        [Produces("application/json", "application/xml", Type = typeof(ReopenFeeParameterDto))]
        public async Task<IActionResult> AddReopenFeeParameter (AddReopenFeeParameterCommand addReopenFeeParameterCommand)
        {
                var result = await _mediator.Send(addReopenFeeParameterCommand);
                return ReturnFormattedResponse(result);               
        }
        /// <summary>
        /// Update ReopenFeeParameter  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateReopenFeeParameter Command"></param>
        /// <returns></returns>
        [HttpPut("ReopenFeeParameter/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(ReopenFeeParameterDto))]
        public async Task<IActionResult> UpdateReopenFeeParameter (string Id, UpdateReopenFeeParameterCommand updateReopenFeeParameterCommand)
        {
            updateReopenFeeParameterCommand.Id = Id;
            var result = await _mediator.Send(updateReopenFeeParameterCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete ReopenFeeParameter  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("ReopenFeeParameter/{Id}")]
        public async Task<IActionResult> DeleteReopenFeeParameter (string Id)
        {
            var deleteReopenFeeParameterCommand = new DeleteReopenFeeParameterCommand { Id = Id };
            var result = await _mediator.Send(deleteReopenFeeParameterCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
