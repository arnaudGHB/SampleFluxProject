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
    /// Division
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class DivisionController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Division
        /// </summary>
        /// <param name="mediator"></param>
        public DivisionController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Division By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Division/{id}", Name = "GetDivision")]
        [Produces("application/json", "application/xml", Type = typeof(DivisionDto))]
        public async Task<IActionResult> GetDivision(string id)
        {
            var getDivisionQuery = new GetDivisionQuery { Id = id };
            var result = await _mediator.Send(getDivisionQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Divisions
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Divisions")]
        [Produces("application/json", "application/xml", Type = typeof(List<DivisionDto>))]
        public async Task<IActionResult> GetDivisions()
        {
            var getAllDivisionQuery = new GetAllDivisionQuery { };
            var result = await _mediator.Send(getAllDivisionQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Division
        /// </summary>
        /// <param name="addDivisionCommand"></param>
        /// <returns></returns>
        [HttpPost("Division")]
        [Produces("application/json", "application/xml", Type = typeof(DivisionDto))]
        public async Task<IActionResult> AddDivision(AddDivisionCommand addDivisionCommand)
        {
            var result = await _mediator.Send(addDivisionCommand);
            return ReturnFormattedResponse(result);
            
        }
        /// <summary>
        /// Update Division By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateDivisionCommand"></param>
        /// <returns></returns>
        [HttpPut("Division/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(DivisionDto))]
        public async Task<IActionResult> UpdateDivision(string Id, UpdateDivisionCommand updateDivisionCommand)
        {
            updateDivisionCommand.Id = Id;
            var result = await _mediator.Send(updateDivisionCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Division By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Division/{Id}")]
        public async Task<IActionResult> DeleteDivision(string Id)
        {
            var deleteDivisionCommand = new DeleteDivisionCommand { Id = Id };
            var result = await _mediator.Send(deleteDivisionCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
