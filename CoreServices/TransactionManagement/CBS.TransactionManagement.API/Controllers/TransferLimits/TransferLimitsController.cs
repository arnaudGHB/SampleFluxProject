using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Commands;
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
    public class TransferLimitsController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// TransferLimits
        /// </summary>
        /// <param name="mediator"></param>
        public TransferLimitsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get TransferLimits By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("TransferLimits/{id}", Name = "GetTransferLimits")]
        [Produces("application/json", "application/xml", Type = typeof(TransferParameterDto))]
        public async Task<IActionResult> GetTransferLimits(string id)
        {
            var getTransferLimitsQuery = new GetTransferLimitsQuery { Id = id };
            var result = await _mediator.Send(getTransferLimitsQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All TransferLimits
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("TransferLimits")]
        [Produces("application/json", "application/xml", Type = typeof(List<TransferParameterDto>))]
        public async Task<IActionResult> GetTransferLimitss()
        {
            var getAllTransferLimitsQuery = new GetAllTransferLimitsQuery { };
            var result = await _mediator.Send(getAllTransferLimitsQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a TransferLimits
        /// </summary>
        /// <param name="addTransferLimitsCommand"></param>
        /// <returns></returns>
        [HttpPost("TransferLimits")]
        [Produces("application/json", "application/xml", Type = typeof(TransferParameterDto))]
        public async Task<IActionResult> AddTransferLimits(AddTransferLimitsCommand addTransferLimitsCommand)
        {
                var result = await _mediator.Send(addTransferLimitsCommand);
                return ReturnFormattedResponse(result);          
            
        }
        /// <summary>
        /// Update TransferLimits By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateTransferLimitsCommand"></param>
        /// <returns></returns>
        [HttpPut("TransferLimits/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(TransferParameterDto))]
        public async Task<IActionResult> UpdateTransferLimits(string Id, UpdateTransferLimitsCommand updateTransferLimitsCommand)
        {
            updateTransferLimitsCommand.Id = Id;
            var result = await _mediator.Send(updateTransferLimitsCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete TransferLimits By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("TransferLimits/{Id}")]
        public async Task<IActionResult> DeleteTransferLimits(string Id)
        {
            var deleteTransferLimitsCommand = new DeleteTransferLimitsCommand { Id = Id };
            var result = await _mediator.Send(deleteTransferLimitsCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
