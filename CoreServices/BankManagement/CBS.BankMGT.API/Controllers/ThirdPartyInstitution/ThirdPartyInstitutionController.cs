using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data.Entity;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CBS.BankMGT.API.Controllers
{
    /// <summary>
    /// ThirdPartyInstitution
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ThirdPartyInstitutionController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// ThirdPartyInstitution
        /// </summary>
        /// <param name="mediator"></param>
        public ThirdPartyInstitutionController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Create a ThirdPartyInstitution
        /// </summary>
        /// <param name="addThirdPartyInstitutionCommand"></param>
        /// <returns></returns>
        [HttpPost("ThirdPartyInstitution")]
        [Produces("application/json", "application/xml", Type = typeof(CorrespondingBankDto))]
        public async Task<IActionResult> AddThirdPartyInstitution(AddThirdPartyInstitutionCommand addThirdPartyInstitutionCommand)
        {
            var result = await _mediator.Send(addThirdPartyInstitutionCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update ThirdPartyInstitution By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateThirdPartyInstitutionCommand"></param>
        /// <returns></returns>
        [HttpPut("ThirdPartyInstitution/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(CorrespondingBankDto))]
        public async Task<IActionResult> UpdateThirdPartyInstitution(string Id, UpdateThirdPartyInstitutionCommand updateThirdPartyInstitutionCommand)
        {
            updateThirdPartyInstitutionCommand.Id = Id;
            var result = await _mediator.Send(updateThirdPartyInstitutionCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get ThirdPartyInstitution By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ThirdPartyInstitution/{id}", Name = "GetThirdPartyInstitution")]
        [Produces("application/json", "application/xml", Type = typeof(CorrespondingBankDto))]
        public async Task<IActionResult> GetThirdPartyInstitution(string id)
        {
            var getThirdPartyInstitutionQuery = new GetThirdPartyInstitutionQuery { Id = id };
            var result = await _mediator.Send(getThirdPartyInstitutionQuery);
            return ReturnFormattedResponse(result);
        }
 
        
        /// <summary>
        /// Get All ThirdPartyInstitutions
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("ThirdPartyInstitutions")]
        [Produces("application/json", "application/xml", Type = typeof(List<CorrespondingBankDto>))]
        public async Task<IActionResult> GetThirdPartyInstitutions()
        {
            var getAllThirdPartyInstitutionQuery = new GetAllThirdPartyInstitutionQuery { };
            var result = await _mediator.Send(getAllThirdPartyInstitutionQuery);
            return Ok(result);
        }
 
 
        /// <summary>
        /// Delete ThirdPartyInstitution By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("ThirdPartyInstitution/{Id}")]
        [Authorize]
        public async Task<IActionResult> DeleteThirdPartyInstitution(string Id)
        {
            var deleteThirdPartyInstitutionCommand = new DeleteThirdPartyInstitutionCommand { Id = Id };
            var result = await _mediator.Send(deleteThirdPartyInstitutionCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
