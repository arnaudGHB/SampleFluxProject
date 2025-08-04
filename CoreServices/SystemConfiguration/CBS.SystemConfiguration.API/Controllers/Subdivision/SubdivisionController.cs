using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.MediatR.Commands;
using CBS.SystemConfiguration.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.SystemConfiguration.API.Controllers
{
    /// <summary>
    /// Subdivision
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    //[Authorize]
    public class SubdivisionController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Subdivision
        /// </summary>
        /// <param name="mediator"></param>
        public SubdivisionController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Subdivision By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Subdivision/{id}", Name = "GetSubdivision")]
        [Produces("application/json", "application/xml", Type = typeof(SubdivisionDto))]
        public async Task<IActionResult> GetSubdivision(string id)
        {
            var getSubdivisionQuery = new GetSubdivisionQuery { Id = id };
            var result = await _mediator.Send(getSubdivisionQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Subdivisions by divisionId
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("SubdivisionByDivisionId/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<SubdivisionDto>))]
        public async Task<IActionResult> GetSubdivisions(string id)
        {
            var getAllSubdivisionQuery = new GetAllSubdivisionByDivisionIdQuery { Id =id};
            var result = await _mediator.Send(getAllSubdivisionQuery);
            return Ok(result);
        }
        /// <summary>
        /// Get All Subdivisions
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Subdivisions")]
        [Produces("application/json", "application/xml", Type = typeof(List<SubdivisionDto>))]
        public async Task<IActionResult> GetSubdivisions()
        {
            var getAllSubdivisionQuery = new GetAllSubdivisionQuery { };
            var result = await _mediator.Send(getAllSubdivisionQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Subdivision
        /// </summary>
        /// <param name="addSubdivisionCommand"></param>
        /// <returns></returns>
        [HttpPost("Subdivision")]
        [Produces("application/json", "application/xml", Type = typeof(SubdivisionDto))]
        public async Task<IActionResult> AddSubdivision(AddSubdivisionCommand addSubdivisionCommand)
        {
            var result = await _mediator.Send(addSubdivisionCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update Subdivision By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateSubdivisionCommand"></param>
        /// <returns></returns>
        [HttpPut("Subdivision/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(SubdivisionDto))]
        public async Task<IActionResult> UpdateSubdivision(string Id, UpdateSubdivisionCommand updateSubdivisionCommand)
        {
            updateSubdivisionCommand.Id = Id;
            var result = await _mediator.Send(updateSubdivisionCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Subdivision By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Subdivision/{Id}")]
        public async Task<IActionResult> DeleteSubdivision(string Id)
        {
            var deleteSubdivisionCommand = new DeleteSubdivisionCommand { Id = Id };
            var result = await _mediator.Send(deleteSubdivisionCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
