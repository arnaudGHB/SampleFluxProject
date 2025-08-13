using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper.Model;
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
    public class ConfigController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Config 
        /// </summary>
        /// <param name="mediator"></param>
        public ConfigController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Config  By id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("Config/{id}", Name = "GetConfigQuery")]
        [Produces("application/json", "application/xml", Type = typeof(ConfigDto))]
        public async Task<IActionResult> GetConfig(string id)
        {
            var getConfigQuery = new GetConfigQuery { Id = id };
            var result = await _mediator.Send(getConfigQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Config 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Configs")]
        [Produces("application/json", "application/xml", Type = typeof(List<ConfigDto>))]
        public async Task<IActionResult> GetConfig()
        {
            var getAllConfigQuery = new GetAllConfigQuery { };
            var result = await _mediator.Send(getAllConfigQuery);
            return Ok(result);
        }

        [HttpGet("Config/EnumData")]
        [Produces("application/json", "application/xml", Type = typeof(EnumData))]
        public async Task<IActionResult> getAllEnumData()
        {
            var GetAllEnumDataCommand = new GetAllEnumDataCommand { };
            var result = await _mediator.Send(GetAllEnumDataCommand);
            return Ok(result);
        }

        /// <summary>
        /// start the year
        /// </summary>
        [HttpPost("Config")]
        [Produces("application/json", "application/xml", Type = typeof(ConfigDto))]
        public async Task<IActionResult> OpenTheYear(AddConfigCommand OpeningOfYearCommand)
        {
            var result = await _mediator.Send(OpeningOfYearCommand);
            return ReturnFormattedResponse(result);
        }
        
        /// <summary>
        /// Update Config  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateConfigCommand"></param>
        /// <returns></returns>
        [HttpPut("Config/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(ConfigDto))]
        public async Task<IActionResult> UpdateConfig(string Id, UpdateConfigCommand updateConfigCommand)
        {
            updateConfigCommand.Id = Id;
            var result = await _mediator.Send(updateConfigCommand);
            return ReturnFormattedResponse(result);
        }
        
    }
}
