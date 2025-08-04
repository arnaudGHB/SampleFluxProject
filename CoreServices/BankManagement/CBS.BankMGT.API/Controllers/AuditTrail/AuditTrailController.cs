using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.AuditTrailP.Queries;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CBS.BankMGT.API.Controllers
{
    /// <summary>
    /// AuditTrail
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AuditTrailController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// AuditTrail
        /// </summary>
        /// <param name="mediator"></param>
        public AuditTrailController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get AuditTrail By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AuditTrail/{id}", Name = "GetAuditTrail")]
        [Produces("application/json", "application/xml", Type = typeof(AuditTrailDto))]
        public async Task<IActionResult> GetAuditTrail(string id)
        {
            var getAuditTrailQuery = new GetAuditTrailQuery { Id = id };
            var result = await _mediator.Send(getAuditTrailQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get AuditTrail for a particular user By username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("AuditTrail/get-by-user-name/{username}", Name = "GetAuditTrailByUserName")]
        [Produces("application/json", "application/xml", Type = typeof(AuditTrailDto))]
        public async Task<IActionResult> GetAuditTrailByUserName(string username)
        {
            var getAuditTrailQuery = new GetAuditTrailByUserQuery() { UserName = username };
            var result = await _mediator.Send(getAuditTrailQuery);
            return ReturnFormattedResponse(result);
        }
        [HttpGet("AuditTrail/pagging")]
        public async Task<IActionResult> GetPagedAuditTrails([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetPagedAuditTrailQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get paginated, filtered, and sorted AuditTrails for DataTables.
        /// </summary>
        /// <param name="query">Query containing DataTable options and date filters.</param>
        /// <returns>Paginated and filtered list of AuditTrail entries.</returns>
        [HttpPost("AuditTrail/datatable-pagging")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAuditTrailsForDataTable([FromBody] GetAuditTrailsDataTableQuery query)
        {
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All AuditTrails
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("AuditTrails")]
        [Produces("application/json", "application/xml", Type = typeof(List<AuditTrailDto>))]
        public async Task<IActionResult> GetAuditTrails()
        {
            var getAllAuditTrailQuery = new GetAllAuditTrailQuery { };
            var result = await _mediator.Send(getAllAuditTrailQuery);
            return Ok(result);
        }
        /// <summary>
        /// Get trails between two dates with date format (dd-MM-yyy).
        /// both for Date from and Date To
        /// </summary>
        /// <param name="getAuditTrailBetweenDatesQuery"></param>
        /// <returns></returns>
        [HttpPost("GetAuditTrailBetweenDates")]
        [Produces("application/json", "application/xml", Type = typeof(AuditTrailDto))]
        public async Task<IActionResult> GetAuditTrailBetweenDates(GetAuditTrailBetweenDatesQuery getAuditTrailBetweenDatesQuery)
        {
            var result = await _mediator.Send(getAuditTrailBetweenDatesQuery);
            return ReturnFormattedResponse(result);
            
        }
        /// <summary>
        /// Create a AuditTrail
        /// </summary>
        /// <param name="addAuditTrailCommand"></param>
        /// <returns></returns>
        [HttpPost("AuditTrail")]
        [Produces("application/json", "application/xml", Type = typeof(AuditTrailDto))]
        [AllowAnonymous]
        public async Task<IActionResult> AddAuditTrail(AddAuditTrailCommand addAuditTrailCommand)
        {
            
            addAuditTrailCommand.IPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            var result = await _mediator.Send(addAuditTrailCommand);
            return ReturnFormattedResponse(result);

        }

        /// <summary>
        /// Delete AuditTrail By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("AuditTrail/{Id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAuditTrail(string Id)
        {
            var deleteAuditTrailCommand = new DeleteAuditTrailCommand { Id = Id };
            var result = await _mediator.Send(deleteAuditTrailCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
