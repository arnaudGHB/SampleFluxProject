using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// OrganizationalUnit
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class OrganizationalUnitController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// OrganizationalUnit
        /// </summary>
        /// <param name="mediator"></param>
        public OrganizationalUnitController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get OrganizationalUnit By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("OrganizationalUnit/{id}", Name = "GetOrganizationalUnit")]
        [Produces("application/json", "application/xml", Type = typeof(OrganizationalUnitDto))]
        public async Task<IActionResult> GetOrganizationalUnit(string id)
        {
            var getOrganizationalUnitQuery = new GetOrganizationalUnitQuery { Id = id };
            var result = await _mediator.Send(getOrganizationalUnitQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All OrganizationalUnit
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("OrganizationalUnit")]
        [Produces("application/json", "application/xml", Type = typeof(List<OrganizationalUnitDto>))]
        public async Task<IActionResult> GetOrganizationalUnit()
        {
            var getAllOrganizationalUnitQuery = new GetAllOrganizationalUnitQuery { };
            var result = await _mediator.Send(getAllOrganizationalUnitQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a OrganizationalUnit
        /// </summary>
        /// <param name="addOrganizationalUnitCommand"></param>
        /// <returns></returns>
        [HttpPost("OrganizationalUnit")]
        [Produces("application/json", "application/xml", Type = typeof(OrganizationalUnitDto))]
        public async Task<IActionResult> AddOrganizationalUnit(AddOrganizationalUnitCommand addOrganizationalUnitCommand)
        {
            var result = await _mediator.Send(addOrganizationalUnitCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update OrganizationalUnit By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateOrganizationalUnitCommand"></param>
        /// <returns></returns>
        [HttpPut("OrganizationalUnit/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(OrganizationalUnitDto))]
        public async Task<IActionResult> UpdateOrganizationalUnit(string Id, UpdateOrganizationalUnitCommand updateOrganizationalUnitCommand)
        {
            updateOrganizationalUnitCommand.Id = Id;
            var result = await _mediator.Send(updateOrganizationalUnitCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete OrganizationalUnit By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("OrganizationalUnit/{Id}")]
        public async Task<IActionResult> DeleteOrganizationalUnit(string Id)
        {
            var deleteOrganizationalUnitCommand = new DeleteOrganizationalUnitCommand { Id = Id };
            var result = await _mediator.Send(deleteOrganizationalUnitCommand);
            return ReturnFormattedResponse(result);
        }
    }
}