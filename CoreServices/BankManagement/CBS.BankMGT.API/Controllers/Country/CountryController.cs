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
    /// Country
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class CountryController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Country
        /// </summary>
        /// <param name="mediator"></param>
        public CountryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Country By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Country/{id}", Name = "GetCountry")]
        [Produces("application/json", "application/xml", Type = typeof(CountryDto))]
        public async Task<IActionResult> GetCountry(string id)
        {
            var getCountryQuery = new GetCountryQuery { Id = id };
            var result = await _mediator.Send(getCountryQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Countrys
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Countrys")]
        [Produces("application/json", "application/xml", Type = typeof(List<CountryDto>))]
        public async Task<IActionResult> GetCountrys()
        {
            var getAllCountryQuery = new GetAllCountryQuery { };
            var result = await _mediator.Send(getAllCountryQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Country
        /// </summary>
        /// <param name="addCountryCommand"></param>
        /// <returns></returns>
        [HttpPost("Country")]
        [Produces("application/json", "application/xml", Type = typeof(CountryDto))]
        public async Task<IActionResult> AddCountry(AddCountryCommand addCountryCommand)
        {
            var result = await _mediator.Send(addCountryCommand);
            return ReturnFormattedResponse(result);
            
        }
        /// <summary>
        /// Update Country By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateCountryCommand"></param>
        /// <returns></returns>
        [HttpPut("Country/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(CountryDto))]
        public async Task<IActionResult> UpdateCountry(string Id, UpdateCountryCommand updateCountryCommand)
        {
            updateCountryCommand.Id = Id;
            var result = await _mediator.Send(updateCountryCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Country By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Country/{Id}")]
        public async Task<IActionResult> DeleteCountry(string Id)
        {
            var deleteCountryCommand = new DeleteCountryCommand { Id = Id };
            var result = await _mediator.Send(deleteCountryCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
