
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Entity;
using CBS.NLoan.MediatR.EnumData;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class EnumAggregatesController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// EnumAggregates
        /// </summary>
        /// <param name="mediator"></param>
        public EnumAggregatesController(IMediator mediator)
        {
            _mediator = mediator;
        }
       
        /// <summary>
        /// Get All EnumAggregatess
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("EnumAggregates")]
        [Produces("application/json", "application/xml", Type = typeof(List<EnumDto>))]
        public async Task<IActionResult> GetEnumAggregatess()
        {
            var getAllEnumAggregatesQuery = new GetAllEnumDataCommand { };
            var result = await _mediator.Send(getAllEnumAggregatesQuery);
            return Ok(result);
        }
       
    }
}
