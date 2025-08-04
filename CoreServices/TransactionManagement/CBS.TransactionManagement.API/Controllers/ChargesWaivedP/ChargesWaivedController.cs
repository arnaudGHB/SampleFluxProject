using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Commands.ChargesWaivedP;
using CBS.TransactionManagement.Data.Dto.ChargesWaivedP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Queries.ChargesWaivedP;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.ChargesWaivedP
{
   
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ChargesWaivedController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// ChargesWaived 
        /// </summary>
        /// <param name="mediator"></param>
        public ChargesWaivedController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get ChargesWaived  By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ChargesWaived/{id}", Name = "GetChargesWaived")]
        [Produces("application/json", "application/xml", Type = typeof(ChargesWaivedDto))]
        public async Task<IActionResult> GetChargesWaived(string id)
        {
            var getChargesWaivedQuery = new GetChargesWaivedQuery { Id = id };
            var result = await _mediator.Send(getChargesWaivedQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All ChargesWaived 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("ChargesWaived")]
        [Produces("application/json", "application/xml", Type = typeof(List<ChargesWaivedDto>))]
        public async Task<IActionResult> GetChargesWaived()
        {
            var getAllChargesWaivedQuery = new GetAllChargesWaivedQuery { };
            var result = await _mediator.Send(getAllChargesWaivedQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Create a ChargesWaived 
        /// </summary>
        /// <param name="addChargesWaived Command"></param>
        /// <returns></returns>
        [HttpPost("ChargesWaived")]
        [Produces("application/json", "application/xml", Type = typeof(ChargesWaivedDto))]
        public async Task<IActionResult> AddChargesWaived(AddChargesWaivedCommand addChargesWaivedCommand)
        {
            var result = await _mediator.Send(addChargesWaivedCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Update ChargesWaived  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateChargesWaived Command"></param>
        /// <returns></returns>
        [HttpPut("ChargesWaived/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(ChargesWaivedDto))]
        public async Task<IActionResult> UpdateChargesWaived(string Id, UpdateChargesWaivedCommand updateChargesWaivedCommand)
        {
            updateChargesWaivedCommand.Id = Id;
            var result = await _mediator.Send(updateChargesWaivedCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete ChargesWaived  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("ChargesWaived/{Id}")]
        public async Task<IActionResult> DeleteChargesWaived(string Id)
        {
            var deleteChargesWaivedCommand = new DeleteChargesWaivedCommand { Id = Id };
            var result = await _mediator.Send(deleteChargesWaivedCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
