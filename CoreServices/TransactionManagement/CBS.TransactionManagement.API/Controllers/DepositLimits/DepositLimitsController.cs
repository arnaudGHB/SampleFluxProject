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
    public class DepositLimitsController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// DepositLimits 
        /// </summary>
        /// <param name="mediator"></param>
        public DepositLimitsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get DepositLimits  By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DepositLimits/{id}", Name = "GetDepositLimits ")]
        [Produces("application/json", "application/xml", Type = typeof(CashDepositParameterDto))]
        public async Task<IActionResult> GetDepositLimits (string id)
        {
            var getDepositLimitQuery = new GetDepositLimitQuery { Id = id };
            var result = await _mediator.Send(getDepositLimitQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All DepositLimits 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("DepositLimits")]
        [Produces("application/json", "application/xml", Type = typeof(List<CashDepositParameterDto>))]
        public async Task<IActionResult> GetDepositLimits()
        {
            var getAllDepositLimitsQuery = new GetAllDepositLimitQuery { };
            var result = await _mediator.Send(getAllDepositLimitsQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a DepositLimits 
        /// </summary>
        /// <param name="addDepositLimits Command"></param>
        /// <returns></returns>
        [HttpPost("DepositLimits")]
        [Produces("application/json", "application/xml", Type = typeof(CashDepositParameterDto))]
        public async Task<IActionResult> AddDepositLimits (AddDepositLimitCommand addDepositLimitCommand)
        {
                var result = await _mediator.Send(addDepositLimitCommand);
                return ReturnFormattedResponse(result);               
        }
        /// <summary>
        /// Update DepositLimits  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateDepositLimits Command"></param>
        /// <returns></returns>
        [HttpPut("DepositLimits/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(CashDepositParameterDto))]
        public async Task<IActionResult> UpdateDepositLimits (string Id, UpdateDepositLimitCommand updateDepositLimitCommand)
        {
            updateDepositLimitCommand.Id = Id;
            var result = await _mediator.Send(updateDepositLimitCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete DepositLimits  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("DepositLimits/{Id}")]
        public async Task<IActionResult> DeleteDepositLimits (string Id)
        {
            var deleteDepositLimitCommand = new DeleteDepositLimitCommand { Id = Id };
            var result = await _mediator.Send(deleteDepositLimitCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
