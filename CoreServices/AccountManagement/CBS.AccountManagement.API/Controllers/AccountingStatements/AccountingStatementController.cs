using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers.AccountingStatements
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountingStatementController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// AccountingStatement
        /// </summary>
        /// <param name="mediator"></param>
        public AccountingStatementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Generating a 4 column balance sheet  record By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("AccountingStatement/Get4ColumnTrialBalanceQuery", Name = "Get4ColumnTrialBalanceQuery")]
        [Produces("application/json", "application/xml", Type = typeof(TrialBalance4ColumnDto))]
        public async Task<IActionResult> Get4ColumnTrialBalanceQuery(Get4ColumnTrialBalanceQuery model)
        {
            var result = await _mediator.Send(model);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Generating a 6 column balance sheet  record By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("AccountingStatement/Get6ColumnTrialBalanceQuery", Name = "Get6ColumnTrialBalanceQuery")]
        [Produces("application/json", "application/xml", Type = typeof(TrialBalance6ColumnDto))]
        public async Task<IActionResult> Get4ColumnTrialBalanceQuery(Get6ColumnTrialBalanceQuery model)
        {
            var result = await _mediator.Send(model);
            return ReturnFormattedResponse(result);
        }
    }
}