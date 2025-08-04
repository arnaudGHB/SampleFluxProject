//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.TaxP;
using CBS.NLoan.MediatR.TaxMediaR.Commands;
using CBS.NLoan.MediatR.TaxMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.TaxP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class TaxController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Tax
        /// </summary>
        /// <param name="mediator"></param>
        public TaxController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Tax By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Tax/{Id}", Name = "GetTax")]
        [Produces("application/json", "application/xml", Type = typeof(TaxDto))]
        public async Task<IActionResult> GetTax(string Id)
        {
            var getTaxQuery = new GetTaxQuery { Id = Id };
            var result = await _mediator.Send(getTaxQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Taxs
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Taxs")]
        [Produces("application/json", "application/xml", Type = typeof(List<TaxDto>))]
        public async Task<IActionResult> GetTaxs()
        {
            var getAllTaxQuery = new GetAllTaxQuery { };
            var result = await _mediator.Send(getAllTaxQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Tax
        /// </summary>
        /// <param name="addTaxCommand"></param>
        /// <returns></returns>
        [HttpPost("Tax")]
        [Produces("application/json", "application/xml", Type = typeof(TaxDto))]
        public async Task<IActionResult> AddTax(AddTaxCommand addTaxCommand)
        {
            var result = await _mediator.Send(addTaxCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update Tax By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateTaxCommand"></param>
        /// <returns></returns>
        [HttpPut("Tax/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(TaxDto))]
        public async Task<IActionResult> UpdateTax(string Id, UpdateTaxCommand updateTaxCommand)
        {
            updateTaxCommand.Id = Id;
            var result = await _mediator.Send(updateTaxCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Tax By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("Tax/{Id}")]
        public async Task<IActionResult> DeleteTax(string Id)
        {
            var deleteTaxCommand = new DeleteTaxCommand { Id = Id };
            var result = await _mediator.Send(deleteTaxCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
