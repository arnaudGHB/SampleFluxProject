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
    /// Organization
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class OrganizationController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Organization
        /// </summary>
        /// <param name="mediator"></param>
        public OrganizationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Organization By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Organization/{id}", Name = "GetOrganization")]
        [Produces("application/json", "application/xml", Type = typeof(OrganizationDto))]
        public async Task<IActionResult> GetOrganization(string id)
        {
            var getOrganizationQuery = new GetOrganizationQuery { Id = id };
            var result = await _mediator.Send(getOrganizationQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Organizations
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Organizations")]
        [Produces("application/json", "application/xml", Type = typeof(List<OrganizationDto>))]
        public async Task<IActionResult> GetOrganizations()
        {
            var getAllOrganizationQuery = new GetAllOrganizationQuery { };
            var result = await _mediator.Send(getAllOrganizationQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Organization
        /// </summary>
        /// <param name="addOrganizationCommand"></param>
        /// <returns></returns>
        [HttpPost("Organization")]
        [Produces("application/json", "application/xml", Type = typeof(OrganizationDto))]
        public async Task<IActionResult> AddOrganization(AddOrganizationCommand addOrganizationCommand)
        {
            var result = await _mediator.Send(addOrganizationCommand);
            return ReturnFormattedResponse(result);
            
        }
        /// <summary>
        /// Update Organization By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateOrganizationCommand"></param>
        /// <returns></returns>
        [HttpPut("Organization/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(OrganizationDto))]
        public async Task<IActionResult> UpdateOrganization(string Id, UpdateOrganizationCommand updateOrganizationCommand)
        {
            updateOrganizationCommand.Id = Id;
            var result = await _mediator.Send(updateOrganizationCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Organization By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Organization/{Id}")]
        public async Task<IActionResult> DeleteOrganization(string Id)
        {
            var deleteOrganizationCommand = new DeleteOrganizationCommand { Id = Id };
            var result = await _mediator.Send(deleteOrganizationCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
