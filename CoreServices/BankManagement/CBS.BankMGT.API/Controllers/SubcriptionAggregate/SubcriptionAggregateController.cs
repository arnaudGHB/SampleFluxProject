using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.BankMGT.API.Controllers
{
    /// <summary>
    /// SubcriptionAggregate
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class SubcriptionAggregateController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// SubcriptionAggregate
        /// </summary>
        /// <param name="mediator"></param>
        public SubcriptionAggregateController(IMediator mediator)
        {
            _mediator = mediator;
        }
       
        /// <summary>
        /// Get All SubcriptionAggregates
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("SubcriptionAggregates")]
        [Produces("application/json", "application/xml", Type = typeof(SubcriptionAggregateDto))]
        public async Task<IActionResult> GetSubcriptionAggregates()
        {
            var getAllSubcriptionAggregateQuery = new GetAllSubcriptionAggregatQuery { };
            var result = await _mediator.Send(getAllSubcriptionAggregateQuery);
            return Ok(result);
        }
        
    }
}
