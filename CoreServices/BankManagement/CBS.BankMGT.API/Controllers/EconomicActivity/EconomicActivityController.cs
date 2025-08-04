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
    /// EconomicActivity
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class EconomicActivityController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// EconomicActivity
        /// </summary>
        /// <param name="mediator"></param>
        public EconomicActivityController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get EconomicActivity By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("EconomicActivity/{id}", Name = "GetEconomicActivity")]
        [Produces("application/json", "application/xml", Type = typeof(EconomicActivityDto))]
        public async Task<IActionResult> GetEconomicActivity(string id)
        {
            var getEconomicActivityQuery = new GetEconomicActivityQuery { Id = id };
            var result = await _mediator.Send(getEconomicActivityQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All EconomicActivitys
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("EconomicActivitys")]
        [Produces("application/json", "application/xml", Type = typeof(List<EconomicActivityDto>))]
        public async Task<IActionResult> GetEconomicActivitys()
        {
            var getAllEconomicActivityQuery = new GetAllEconomicActivityQuery { };
            var result = await _mediator.Send(getAllEconomicActivityQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a EconomicActivity
        /// </summary>
        /// <param name="addEconomicActivityCommand"></param>
        /// <returns></returns>
        [HttpPost("EconomicActivity")]
        [Produces("application/json", "application/xml", Type = typeof(EconomicActivityDto))]
        public async Task<IActionResult> AddEconomicActivity(AddEconomicActivityCommand addEconomicActivityCommand)
        {
            var result = await _mediator.Send(addEconomicActivityCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update EconomicActivity By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateEconomicActivityCommand"></param>
        /// <returns></returns>
        [HttpPut("EconomicActivity/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(EconomicActivityDto))]
        public async Task<IActionResult> UpdateEconomicActivity(string Id, UpdateEconomicActivityCommand updateEconomicActivityCommand)
        {
            updateEconomicActivityCommand.Id = Id;
            var result = await _mediator.Send(updateEconomicActivityCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete EconomicActivity By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("EconomicActivity/{Id}")]
        public async Task<IActionResult> DeleteEconomicActivity(string Id)
        {
            var deleteEconomicActivityCommand = new DeleteEconomicActivityCommand { Id = Id };
            var result = await _mediator.Send(deleteEconomicActivityCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
