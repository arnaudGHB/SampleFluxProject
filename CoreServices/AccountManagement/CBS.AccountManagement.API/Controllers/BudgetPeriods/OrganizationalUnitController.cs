using CBS.APICaller.Helper;
using CBS.AccountManagement.API.Model;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CBS.APICaller.Helper.LoginModel.Authenthication;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// BudgetPeriods
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class BudgetPeriodsController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// BudgetPeriods
        /// </summary>
        /// <param name="mediator"></param>
        public BudgetPeriodsController(IMediator mediator)
        {
            _mediator = mediator;
        }



        /// <summary>
        /// Get All BudgetPeriods
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("BudgetPeriods")]
        [Produces("application/json", "application/xml", Type = typeof(List<BudgetPeriodDto>))]
        public async Task<IActionResult> GetBudgetPeriods()
        {
            var getAllBudgetPeriodsQuery = new GetAllBudgetPeriodQuery { };
            var result = await _mediator.Send(getAllBudgetPeriodsQuery);
            return ReturnFormattedResponse(result);
        }
    
      

        /// <summary>
        /// Create a BudgetPeriods
        /// </summary>
        /// <param name="addBudgetPeriodsCommand"></param>
        /// <returns></returns>
        [HttpPost("BudgetPeriods")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetPeriodDto))]
        public async Task<IActionResult> AddBudgetPeriods(AddBudgetPeriodCommand addBudgetPeriodsCommand)
        {
            var result = await _mediator.Send(addBudgetPeriodsCommand);
            return ReturnFormattedResponse(result);
        }


      
    }
}