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
    /// ThirdPartyBranche
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ThirdPartyBrancheController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// ThirdPartyBranche
        /// </summary>
        /// <param name="mediator"></param>
        public ThirdPartyBrancheController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Create a ThirdPartyBranche
        /// </summary>
        /// <param name="addThirdPartyBrancheCommand"></param>
        /// <returns></returns>
        [HttpPost("ThirdPartyBranche")]
        [Produces("application/json", "application/xml", Type = typeof(CorrespondingBankBranchDto))]
        public async Task<IActionResult> AddThirdPartyBranche(AddThirdPartyBrancheCommand addThirdPartyBrancheCommand)
        {
            var result = await _mediator.Send(addThirdPartyBrancheCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Get ThirdPartyBranche By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ThirdPartyBranche/{id}", Name = "GetThirdPartyBranche")]
        [Produces("application/json", "application/xml", Type = typeof(CorrespondingBankBranchDto))]
        public async Task<IActionResult> GetThirdPartyBranche(string id)
        {
            var getThirdPartyBrancheQuery = new GetThirdPartyBrancheQuery { Id = id };
            var result = await _mediator.Send(getThirdPartyBrancheQuery);
            return ReturnFormattedResponse(result);
        }
 
        
        /// <summary>
        /// Get All ThirdPartyBranche
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("ThirdPartyBranche")]
        [Produces("application/json", "application/xml", Type = typeof(List<CorrespondingBankBranchDto>))]
        public async Task<IActionResult> GetThirdPartyBranches()
        {
            var getAllThirdPartyBrancheQuery = new GetAllThirdPartyBrancheQuery { };
            var result = await _mediator.Send(getAllThirdPartyBrancheQuery);
            return Ok(result);
        }
 
 
        /// <summary>
        /// Delete ThirdPartyBranche By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("ThirdPartyBranche/{Id}")]
        [Authorize]
        public async Task<IActionResult> DeleteThirdPartyBranche(string Id)
        {
            var deleteThirdPartyBrancheCommand = new DeleteThirdPartyBrancheCommand { Id = Id };
            var result = await _mediator.Send(deleteThirdPartyBrancheCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Update ThirdPartyBranche By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateThirdPartyBrancheCommand"></param>
        /// <returns></returns>
        [HttpPut("ThirdPartyBranche/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(CorrespondingBankBranchDto))]
        public async Task<IActionResult> UpdateThirdPartyBranche(string Id, UpdateThirdPartyBrancheCommand updateThirdPartyBrancheCommand)
        {
            updateThirdPartyBrancheCommand.Id = Id;
            var result = await _mediator.Send(updateThirdPartyBrancheCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
