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
    public class WithdrawalLimitsController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// WithdrawalLimits
        /// </summary>
        /// <param name="mediator"></param>
        public WithdrawalLimitsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get WithdrawalLimits By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("WithdrawalLimits/{id}", Name = "GetWithdrawalLimits")]
        [Produces("application/json", "application/xml", Type = typeof(WithdrawalParameterDto))]
        public async Task<IActionResult> GetWithdrawalLimits(string id)
        {
            var getWithdrawalLimitsQuery = new GetWithdrawalLimitsQuery { Id = id };
            var result = await _mediator.Send(getWithdrawalLimitsQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All WithdrawalLimits
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("WithdrawalLimits")]
        [Produces("application/json", "application/xml", Type = typeof(List<WithdrawalParameterDto>))]
        public async Task<IActionResult> GetWithdrawalLimits()
        {
            var getAllWithdrawalLimitsQuery = new GetAllWithdrawalLimitsQuery { };
            var result = await _mediator.Send(getAllWithdrawalLimitsQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a WithdrawalLimits
        /// </summary>
        /// <param name="addWithdrawalLimitsCommand"></param>
        /// <returns></returns>
        [HttpPost("WithdrawalLimits")]
        [Produces("application/json", "application/xml", Type = typeof(WithdrawalParameterDto))]
        public async Task<IActionResult> AddWithdrawalLimits(AddWithdrawalLimitsCommand addWithdrawalLimitsCommand)
        {
                var result = await _mediator.Send(addWithdrawalLimitsCommand);
                return ReturnFormattedResponse(result);          
            
        }
        /// <summary>
        /// Update WithdrawalLimits By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateWithdrawalLimitsCommand"></param>
        /// <returns></returns>
        [HttpPut("WithdrawalLimits/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(WithdrawalParameterDto))]
        public async Task<IActionResult> UpdateWithdrawalLimits(string Id, UpdateWithdrawalLimitsCommand updateWithdrawalLimitsCommand)
        {
            updateWithdrawalLimitsCommand.Id = Id;
            var result = await _mediator.Send(updateWithdrawalLimitsCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete WithdrawalLimits By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("WithdrawalLimits/{Id}")]
        public async Task<IActionResult> DeleteWithdrawalLimits(string Id)
        {
            var deleteWithdrawalLimitsCommand = new DeleteWithdrawalLimitsCommand { Id = Id };
            var result = await _mediator.Send(deleteWithdrawalLimitsCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
