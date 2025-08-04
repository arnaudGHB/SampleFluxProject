using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.API.Controllers;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.TransferM.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.TransferM.API.Controllers
{
    /// <summary>
    /// Transaction
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class TransferController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Transfer 
        /// </summary>
        /// <param name="mediator"></param>
        public TransferController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Transfer By Id
        /// </summary>
        /// <param id="id"></param>
        /// <returns></returns>
        [HttpGet("Transfer/{id}", Name = "GetTransferById")]
        [Produces("application/json", "application/xml", Type = typeof(TransferDto))]
        public async Task<IActionResult> GetTransfer (string id)
        {
            var getTransferQuery = new GetTransferQuery { Id = id };
            var result = await _mediator.Send(getTransferQuery);
            return ReturnFormattedResponse(result);
        }
        
        /// <summary>
        /// Get All Transfers 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Transfers")]
        [Produces("application/json", "application/xml", Type = typeof(List<TransferDto>))]
        public async Task<IActionResult> GetTransfer()
        {
            var getAllTransferQuery = new GetAllTransferQuery { };
            var result = await _mediator.Send(getAllTransferQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Pending Transfers 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Transfer/Pending")]
        [Produces("application/json", "application/xml", Type = typeof(List<TransferDto>))]
        public async Task<IActionResult> GetTransferPending()
        {
            var getAllPendingTransfer = new GetAllPendingTransferQuery { };
            var result = await _mediator.Send(getAllPendingTransfer);
            return ReturnFormattedResponse(result);
        }
    }
}
