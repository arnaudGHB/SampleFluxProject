using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.BankingZoneMGT.MediatR.Commands;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CBS.BankMGT.API.Controllers
{
    /// <summary>
    /// BankingZone
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class BankingZoneController : BaseController
    {
        public IMediator _mediator { get; set; }
        private IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// BankingZone
        /// </summary>
        /// <param name="mediator"></param>
        public BankingZoneController(IMediator mediator, IWebHostEnvironment webHostEnvironment = null)
        {
            _mediator = mediator;
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// Get BankingZone By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BankingZone/{id}", Name = "GetBankingZone")]
        [Produces("application/json", "application/xml", Type = typeof(BankingZoneDto))]
        public async Task<IActionResult> GetBankingZone(string id)
        {
            var getBankingZoneQuery = new GetBankingZoneQuery { Id = id };
            var result = await _mediator.Send(getBankingZoneQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All BankingZones
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("BankingZones")]
        [Produces("application/json", "application/xml", Type = typeof(List<BankingZoneDto>))]
        public async Task<IActionResult> GetBankingZones()
        {
            var getAllBankingZoneQuery = new GetAllBankingZoneQuery { };
            var result = await _mediator.Send(getAllBankingZoneQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a BankingZone
        /// </summary>
        /// <param name="addBankingZoneCommand"></param>
        /// <returns></returns>
        [HttpPost("BankingZone")]
        [Produces("application/json", "application/xml", Type = typeof(BankingZoneDto))]
        public async Task<IActionResult> AddBankingZone(AddBankingZoneCommand addBankingZoneCommand)
        {
            var result = await _mediator.Send(addBankingZoneCommand);
            return ReturnFormattedResponse(result);
            
        }
 

        /// <summary>
        /// Update BankingZone By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateBankingZoneCommand"></param>
        /// <returns></returns>
        [HttpPut("BankingZone/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(BankingZoneDto))]
        public async Task<IActionResult> UpdateBankingZone(string Id, UpdateBankingZoneCommand updateBankingZoneCommand)
        {
            updateBankingZoneCommand.Id = Id;
            var result = await _mediator.Send(updateBankingZoneCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete BankingZone By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("BankingZone/{Id}")]
        public async Task<IActionResult> DeleteBankingZone(string Id)
        {
            var deleteBankingZoneCommand = new DeleteBankingZoneCommand { Id = Id };
            var result = await _mediator.Send(deleteBankingZoneCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
