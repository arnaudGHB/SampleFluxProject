using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.TellerP.Commands;
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
    public class TellerOperationsController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// TellerHistoryHistory 
        /// </summary>
        /// <param name="mediator"></param>
        public TellerOperationsController(IMediator mediator)
        {
            _mediator = mediator;
        }
      
        /// <summary>
        /// Get All TellerHistorys 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("TellerOperations")]
        [Produces("application/json", "application/xml", Type = typeof(List<PrimaryTellerProvisioningHistoryDto>))]
        public async Task<IActionResult> GetTellerHistory()
        {
            var getAllTellerHistoryQuery = new GetAllTellerProvisioningQuery { };
            var result = await _mediator.Send(getAllTellerHistoryQuery);
            return Ok(result);
        }

        /// <summary>
        /// EOD by accountant
        /// </summary>
        /// <param name="getAllTellerOperationsQuery"></param>
        /// <returns></returns>
        [HttpPost("TellerOperations/DailyOperations", Name = "GetAllTellerOperationsQuery")]
        [Produces("application/json", "application/xml", Type = typeof(TellerDto))]
        public async Task<IActionResult> DailyOperations(GetAllTellerOperationsQuery getAllTellerOperationsQuery)
        {
            var result = await _mediator.Send(getAllTellerOperationsQuery);
            return ReturnFormattedResponse(result);
        }
    }
}
