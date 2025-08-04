using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.MediatR.Commands;
using CBS.SystemConfiguration.MediatR.Queries;
using CBS.SystemConfiguration.MediatR.Queries;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.SystemConfiguration.API.Controllers
{
    /// <summary>
    /// Town
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class TownController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Town
        /// </summary>
        /// <param name="mediator"></param>
        public TownController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Town By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Town/{id}", Name = "GetTown")]
        [Produces("application/json", "application/xml", Type = typeof(TownDto))]
        public async Task<IActionResult> GetTown(string id)
        {
            var getTownQuery = new GetTownQuery { Id = id };
            var result = await _mediator.Send(getTownQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Towns
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Towns")]
        [Produces("application/json", "application/xml", Type = typeof(List<TownDto>))]
        public async Task<IActionResult> GetTowns()
        {
            var getAllTownQuery = new GetAllTownQuery { };
            var result = await _mediator.Send(getAllTownQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Town
        /// </summary>
        /// <param name="addTownCommand"></param>
        /// <returns></returns>
        [HttpPost("Town")]
        [Produces("application/json", "application/xml", Type = typeof(TownDto))]
        public async Task<IActionResult> AddTown(AddTownCommand addTownCommand)
        {
            var result = await _mediator.Send(addTownCommand);
            return ReturnFormattedResponse(result);
            
        }
        /// <summary>
        /// Update Town By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateTownCommand"></param>
        /// <returns></returns>
        [HttpPut("Town/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(TownDto))]
        public async Task<IActionResult> UpdateTown(string Id, UpdateTownCommand updateTownCommand)
        {
            updateTownCommand.Id = Id;
            var result = await _mediator.Send(updateTownCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Town By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Town/{Id}")]
        public async Task<IActionResult> DeleteTown(string Id)
        {
            var deleteTownCommand = new DeleteTownCommand { Id = Id };
            var result = await _mediator.Send(deleteTownCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// upload account from file
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        [HttpPost("ChartOfAccountManagementPosition/UploadAccountForm")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UploadMOdelAccount(IFormFile formData)
        {
            UploadTownsCommand UploadAccountCommand = new UploadTownsCommand();
            UploadAccountCommand.TownFile = UploadAccountCommand.UploadTownQueryModel(formData);

            var result = await _mediator.Send(UploadAccountCommand);
            return ReturnFormattedResponseObject(result);
        }
    }
}
