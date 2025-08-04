using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.CashReplenishmentPrimaryTellerP.Commands;
using CBS.TransactionManagement.MediatR.CashReplenishmentPrimaryTellerP.Queries;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Queries;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Commands;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers
{

    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class PrimaryTellerCashReplenishmentController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// CashReplenishment 
        /// </summary>
        /// <param name="mediator"></param>
        public PrimaryTellerCashReplenishmentController(IMediator mediator)
        {
            _mediator = mediator;
        }
       
        /// <summary>
        /// Add cash to primary teller for the continue running of daily operations
        /// </summary>
        /// <returns>CashReplenishmentPrimaryTellerDto</returns>
        /// <response code="200">Returns the newly created cash replenishment</response>
        [HttpPost("PrimaryTellerCashReplenishment/Topup")]
        [Produces("application/json", "application/xml", Type = typeof(CashReplenishmentPrimaryTellerDto))]
        public async Task<IActionResult> CashReplenishment(ValidationCashReplenishmentPrimaryTellerCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// primary-teller cash requisition to topup cashin hand
        /// </summary>
        /// <param name="addCashReplenishmentCommand"></param>
        /// <returns></returns>
        [HttpPost("PrimaryTellerCashReplenishment/Request")]
        [Produces("application/json", "application/xml", Type = typeof(CashReplenishmentPrimaryTellerDto))]
        public async Task<IActionResult> AddCashReplenishment(AddCashReplenishmentPrimaryTellerCommand addCashReplenishmentCommand)
        {
            var result = await _mediator.Send(addCashReplenishmentCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get CashReplenishment By Id
        /// </summary>
        /// <param id="id"></param>
        /// <returns></returns>
        [HttpGet("PrimaryTellerCashReplenishment/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(CashReplenishmentPrimaryTellerDto))]
        public async Task<IActionResult> GetCashReplenishment(string id)
        {
            var getCashReplenishmentQuery = new GetCashReplenishmentPrimaryTellerQuery { Id = id };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get all CashReplenishment that are pending for provision in operation.
        /// </summary>
        /// <param id="id"></param>
        /// <returns></returns>
        [HttpGet("PrimaryTellerCashReplenishment/AllPendingProvisions")]
        [Produces("application/json", "application/xml", Type = typeof(CashReplenishmentPrimaryTellerDto))]
        public async Task<IActionResult> GetAllCashReplenishmentPrimaryTellerPendingQuery()
        {
            var getCashReplenishmentQuery = new GetAllCashReplenishmentPrimaryTellerPendingQuery {};
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }
        //GetAllCashReplenishmentPrimaryTellerPendingQuery
        /// <summary>
        /// Get All CashReplenishments 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("PrimaryTellerCashReplenishment/GetAllRequest")]
        [Produces("application/json", "application/xml", Type = typeof(List<CashReplenishmentPrimaryTellerDto>))]
        public async Task<IActionResult> GetCashReplenishment(GetAllCashReplenishmentPrimaryTellerQuery primaryTellerQuery)
        {
            var result = await _mediator.Send(primaryTellerQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All CashReplenishments  by branch
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("PrimaryTellerCashReplenishment/ReplenishmentByBranch")]
        [Produces("application/json", "application/xml", Type = typeof(List<CashReplenishmentPrimaryTellerDto>))]
        public async Task<IActionResult> GetRequestByBranch(GetAllCashReplenishmentPrimaryTellerByBranchQuery  primaryTellerByBranchQuery)
        {
            var result = await _mediator.Send(primaryTellerByBranchQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete CashReplenishment  By Id
        /// </summary>
        /// <param id="id"></param>
        /// <returns></returns>
        [HttpDelete("PrimaryTellerCashReplenishment/{id}")]
        public async Task<IActionResult> DeleteCashReplenishment(string id)
        {
            var deleteCashReplenishmentCommand = new DeleteCashReplenishmentCommand { Id = id };
            var result = await _mediator.Send(deleteCashReplenishmentCommand);
            return ReturnFormattedResponse(result);
        }

    }
}
