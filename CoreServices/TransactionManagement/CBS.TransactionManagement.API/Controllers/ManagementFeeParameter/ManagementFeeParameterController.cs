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
    /// ManagementFeeParameter
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ManagementFeeParameterController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// ManagementFeeParameter 
        /// </summary>
        /// <param name="mediator"></param>
        public ManagementFeeParameterController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get ManagementFeeParameter  By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ManagementFeeParameter/{id}", Name = "GetManagementFeeParameter ")]
        [Produces("application/json", "application/xml", Type = typeof(ManagementFeeParameterDto))]
        public async Task<IActionResult> GetManagementFeeParameter (string id)
        {
            var getManagementFeeParameterQuery = new GetManagementFeeParameterQuery { Id = id };
            var result = await _mediator.Send(getManagementFeeParameterQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All ManagementFeeParameter 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("ManagementFeeParameter")]
        [Produces("application/json", "application/xml", Type = typeof(List<ManagementFeeParameterDto>))]
        public async Task<IActionResult> GetManagementFeeParameter()
        {
            var getAllManagementFeeParameterQuery = new GetAllManagementFeeParameterQuery { };
            var result = await _mediator.Send(getAllManagementFeeParameterQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a ManagementFeeParameter 
        /// </summary>
        /// <param name="addManagementFeeParameter Command"></param>
        /// <returns></returns>
        [HttpPost("ManagementFeeParameter")]
        [Produces("application/json", "application/xml", Type = typeof(ManagementFeeParameterDto))]
        public async Task<IActionResult> AddManagementFeeParameter (AddManagementFeeParameterCommand addManagementFeeParameterCommand)
        {
                var result = await _mediator.Send(addManagementFeeParameterCommand);
                return ReturnFormattedResponse(result);               
        }
        /// <summary>
        /// Update ManagementFeeParameter  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateManagementFeeParameter Command"></param>
        /// <returns></returns>
        [HttpPut("ManagementFeeParameter/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(ManagementFeeParameterDto))]
        public async Task<IActionResult> UpdateManagementFeeParameter (string Id, UpdateManagementFeeParameterCommand updateManagementFeeParameterCommand)
        {
            updateManagementFeeParameterCommand.Id = Id;
            var result = await _mediator.Send(updateManagementFeeParameterCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete ManagementFeeParameter  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("ManagementFeeParameter/{Id}")]
        public async Task<IActionResult> DeleteManagementFeeParameter (string Id)
        {
            var deleteManagementFeeParameterCommand = new DeleteManagementFeeParameterCommand { Id = Id };
            var result = await _mediator.Send(deleteManagementFeeParameterCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
