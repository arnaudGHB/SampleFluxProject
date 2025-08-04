using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Queries;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers
{

    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class SubTellerCashReplenishmentController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// CashReplenishment 
        /// </summary>
        /// <param name="mediator"></param>
        public SubTellerCashReplenishmentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get CashReplenishment By Id
        /// </summary>
        /// <param id="id"></param>
        /// <returns></returns>
        [HttpGet("SubTellerCashReplenishment/Request/{id}", Name = "GetCashReplenishmentById")]
        [Produces("application/json", "application/xml", Type = typeof(SubTellerCashReplenishmentDto))]
        public async Task<IActionResult> GetCashReplenishment (string id)
        {
            var getCashReplenishmentQuery = new GetCashReplenishmentQuery { Id = id };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get all CashReplenishment that are pending
        /// </summary>
        /// <param id="id"></param>
        /// <returns></returns>
        [HttpGet("SubTellerCashReplenishment/Request/Pending", Name = "GetAllCashReplenishmentSubTellerPendingQuery")]
        [Produces("application/json", "application/xml", Type = typeof(SubTellerCashReplenishmentDto))]
        public async Task<IActionResult> GetAllCashReplenishmentSubTellerPendingQuery()
        {
            var getCashReplenishmentQuery = new GetAllCashReplenishmentSubTellerPendingQuery {};
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }
        //
        /// <summary>
        /// Get All CashReplenishments 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("SubTellerCashReplenishment/GetAllRequest")]
        [Produces("application/json", "application/xml", Type = typeof(List<SubTellerCashReplenishmentDto>))]
        public async Task<IActionResult> GetCashReplenishment(GetAllCashReplenishmentQuery getAllCashReplenishmentQuery)
        {
            var result = await _mediator.Send(getAllCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }
        
        /// <summary>
        /// Sub-teller cash requisition to topup till account
        /// </summary>
        /// <param name="addCashReplenishmentCommand"></param>
        /// <returns></returns>
        [HttpPost("SubTellerCashReplenishment/Request")]
        [Produces("application/json", "application/xml", Type = typeof(SubTellerCashReplenishmentDto))]
        public async Task<IActionResult> AddCashReplenishment (AddCashReplenishmentSubTellerCommand addCashReplenishmentCommand)
        {
                var result = await _mediator.Send(addCashReplenishmentCommand);
                return ReturnFormattedResponse(result);               
        }
        /// <summary>
        /// Get All CashReplenishments by branch
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("SubTellerCashReplenishment/GetRequestByBranch")]
        [Produces("application/json", "application/xml", Type = typeof(List<SubTellerCashReplenishmentDto>))]
        public async Task<IActionResult> GetRequestByBranch(GetAllCashReplenishmentByBranchQuery cashReplenishmentByBranchQuery)
        {
            var result = await _mediator.Send(cashReplenishmentByBranchQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Sub-teller cash requisition validation.
        /// </summary>
        /// <param name="validateCashReplenishment"></param>
        /// <returns></returns>
        [HttpPut("SubTellerCashReplenishment/RequestValidation/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(SubTellerCashReplenishmentDto))]
        public async Task<IActionResult> ValidateCashReplenishmentSubTellerCommand(string Id, ValidateCashReplenishmentSubTellerCommand validateCashReplenishment)
        {
            validateCashReplenishment.Id = Id;
            var result = await _mediator.Send(validateCashReplenishment);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete CashReplenishment  By Id
        /// </summary>
        /// <param id="id"></param>
        /// <returns></returns>
        [HttpDelete("SubTellerCashReplenishment/Request/{id}")]
        public async Task<IActionResult> DeleteCashReplenishment (string id)
        {
            var deleteCashReplenishmentCommand = new DeleteCashReplenishmentCommand { Id = id };
            var result = await _mediator.Send(deleteCashReplenishmentCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
