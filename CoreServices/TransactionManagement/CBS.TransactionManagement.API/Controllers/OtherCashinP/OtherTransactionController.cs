using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data.Dto.OtherCashInP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.otherCashIn.Commands;
using CBS.TransactionManagement.otherCashIn.Queries;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.OtherCashinP
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class OtherTransactionController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// OtherTransaction 
        /// </summary>
        /// <param name="mediator"></param>
        public OtherTransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get OtherTransaction  By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("OtherTransaction/{id}", Name = "GetOtherTransaction ")]
        [Produces("application/json", "application/xml", Type = typeof(OtherTransactionDto))]
        public async Task<IActionResult> GetOtherTransaction(string id)
        {
            var getOtherTransactionQuery = new GetOtherTransactionQuery { Id = id };
            var result = await _mediator.Send(getOtherTransactionQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All OtherTransaction 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("OtherTransaction")]
        [Produces("application/json", "application/xml", Type = typeof(List<OtherTransactionDto>))]
        public async Task<IActionResult> GetOtherTransaction()
        {
            var getAllOtherTransactionQuery = new GetAllOtherTransactionQuery { };
            var result = await _mediator.Send(getAllOtherTransactionQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a OtherTransaction 
        /// </summary>
        /// <param name="addOtherTransaction Command"></param>
        /// <returns></returns>
        [HttpPost("OtherTransaction")]
        [Produces("application/json", "application/xml", Type = typeof(OtherTransactionDto))]
        public async Task<IActionResult> AddOtherTransaction(AddOtherTransactionCommand addOtherTransactionCommand)
        {
            var result = await _mediator.Send(addOtherTransactionCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Create a OtherTransaction Mobile Money
        /// </summary>
        /// <param name="addOtherTransactionMobileMoneyCommand Command"></param>
        /// <returns></returns>
        [HttpPost("OtherTransaction/MobileMoney")]
        [Produces("application/json", "application/xml", Type = typeof(OtherTransactionDto))]
        public async Task<IActionResult> AddOtherTransactionMobileMoneyCommand(AddOtherTransactionMobileMoneyCommand addOtherTransactionMobileMoneyCommand)
        {
            var result = await _mediator.Send(addOtherTransactionMobileMoneyCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Create a OtherTransaction Mobile Money
        /// </summary>
        /// <param name="addNoneCashMobileMoney Command"></param>
        /// <returns></returns>
        [HttpPost("OtherTransaction/MobileMoney-None-Cash-Cash-In")]
        [Produces("application/json", "application/xml", Type = typeof(OtherTransactionDto))]
        public async Task<IActionResult> AddNoneCashMobileMoneyCommand(AddNoneCashMobileMoneyCommand addNoneCashMobileMoney)
        {
            var result = await _mediator.Send(addNoneCashMobileMoney);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Update OtherTransaction  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateOtherTransaction Command"></param>
        /// <returns></returns>
        [HttpPut("OtherTransaction/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(OtherTransactionDto))]
        public async Task<IActionResult> UpdateOtherTransaction(string Id, UpdateOtherTransactionCommand updateOtherTransactionCommand)
        {
            updateOtherTransactionCommand.Id = Id;
            var result = await _mediator.Send(updateOtherTransactionCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete OtherTransaction  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("OtherTransaction/{Id}")]
        public async Task<IActionResult> DeleteOtherTransaction(string Id)
        {
            var deleteOtherTransactionCommand = new DeleteOtherTransactionCommand { Id = Id };
            var result = await _mediator.Send(deleteOtherTransactionCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
