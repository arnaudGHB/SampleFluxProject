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
    /// Currency
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class CurrencyController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Currency
        /// </summary>
        /// <param name="mediator"></param>
        public CurrencyController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Currency By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Currency/{id}", Name = "GetCurrency")]
        [Produces("application/json", "application/xml", Type = typeof(CurrencyDto))]
        public async Task<IActionResult> GetCurrency(string id)
        {
            var getCurrencyQuery = new GetCurrencyQuery { Id = id };
            var result = await _mediator.Send(getCurrencyQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Currencys
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Currencys")]
        [Produces("application/json", "application/xml", Type = typeof(List<CurrencyDto>))]
        public async Task<IActionResult> GetCurrencys()
        {
            var getAllCurrencyQuery = new GetAllCurrencyQuery { };
            var result = await _mediator.Send(getAllCurrencyQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Currency
        /// </summary>
        /// <param name="addCurrencyCommand"></param>
        /// <returns></returns>
        [HttpPost("Currency")]
        [Produces("application/json", "application/xml", Type = typeof(CurrencyDto))]
        public async Task<IActionResult> AddCurrency(AddCurrencyCommand addCurrencyCommand)
        {
            var result = await _mediator.Send(addCurrencyCommand);
            return ReturnFormattedResponse(result);
            
        }
        /// <summary>
        /// Update Currency By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateCurrencyCommand"></param>
        /// <returns></returns>
        [HttpPut("Currency/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(CurrencyDto))]
        public async Task<IActionResult> UpdateCurrency(string Id, UpdateCurrencyCommand updateCurrencyCommand)
        {
            updateCurrencyCommand.Id = Id;
            var result = await _mediator.Send(updateCurrencyCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Currency By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Currency/{Id}")]
        public async Task<IActionResult> DeleteCurrency(string Id)
        {
            var deleteCurrencyCommand = new DeleteCurrencyCommand { Id = Id };
            var result = await _mediator.Send(deleteCurrencyCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
