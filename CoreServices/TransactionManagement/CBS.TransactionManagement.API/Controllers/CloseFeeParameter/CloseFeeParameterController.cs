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
    /// Transaction
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class CloseFeeParameterController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// CloseFeeParameter 
        /// </summary>
        /// <param name="mediator"></param>
        public CloseFeeParameterController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get CloseFeeParameter  By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("CloseFeeParameter/{id}", Name = "GetCloseFeeParameter ")]
        [Produces("application/json", "application/xml", Type = typeof(CloseFeeParameterDto))]
        public async Task<IActionResult> GetCloseFeeParameter (string id)
        {
            var getCloseFeeParameterQuery = new GetCloseFeeParameterQuery { Id = id };
            var result = await _mediator.Send(getCloseFeeParameterQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All CloseFeeParameter 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("CloseFeeParameter")]
        [Produces("application/json", "application/xml", Type = typeof(List<CloseFeeParameterDto>))]
        public async Task<IActionResult> GetCloseFeeParameter()
        {
            var getAllCloseFeeParameterQuery = new GetAllCloseFeeParameterQuery { };
            var result = await _mediator.Send(getAllCloseFeeParameterQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a CloseFeeParameter 
        /// </summary>
        /// <param name="addCloseFeeParameter Command"></param>
        /// <returns></returns>
        [HttpPost("CloseFeeParameter")]
        [Produces("application/json", "application/xml", Type = typeof(CloseFeeParameterDto))]
        public async Task<IActionResult> AddCloseFeeParameter (AddCloseFeeParameterCommand addCloseFeeParameterCommand)
        {
                var result = await _mediator.Send(addCloseFeeParameterCommand);
                return ReturnFormattedResponse(result);               
        }
        /// <summary>
        /// Update CloseFeeParameter  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateCloseFeeParameter Command"></param>
        /// <returns></returns>
        [HttpPut("CloseFeeParameter/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(CloseFeeParameterDto))]
        public async Task<IActionResult> UpdateCloseFeeParameter (string Id, UpdateCloseFeeParameterCommand updateCloseFeeParameterCommand)
        {
            updateCloseFeeParameterCommand.Id = Id;
            var result = await _mediator.Send(updateCloseFeeParameterCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete CloseFeeParameter  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("CloseFeeParameter/{Id}")]
        public async Task<IActionResult> DeleteCloseFeeParameter (string Id)
        {
            var deleteCloseFeeParameterCommand = new DeleteCloseFeeParameterCommand { Id = Id };
            var result = await _mediator.Send(deleteCloseFeeParameterCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
