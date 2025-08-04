using CBS.BudgetManagement.API;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.MediatR.Commands;
using CBS.BudgetManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CBS.BudgetManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing FiscalYear operations.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class FiscalYearController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for FiscalYearController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public FiscalYearController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get FiscalYear by Id.
        /// </summary>
        /// <param name="id">The Id of the FiscalYear.</param>
        /// <returns>An FiscalYearDto object.</returns>
        [HttpGet("FiscalYear/{id}", Name = "GetFiscalYear")]
        [Produces("application/json", "application/xml", Type = typeof(FiscalYearDto))]
        public async Task<IActionResult> GetFiscalYear(string id)
        {
            var query = new GetFiscalYearQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all FiscalYears.
        /// </summary>
        /// <returns>A list of FiscalYearDto objects.</returns>
        [HttpGet("FiscalYear")]
        [Produces("application/json", "application/xml", Type = typeof(List<FiscalYearDto>))]
        public async Task<IActionResult> GetAllFiscalYears()
        {
            var query = new GetAllFiscalYearQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

 

        /// <summary>
        /// Create a new FiscalYear.
        /// </summary>
        /// <param name="command">The command to add a new FiscalYear.</param>
        /// <returns>The created FiscalYearDto object.</returns>
        [HttpPost("FiscalYear")]
        [Produces("application/json", "application/xml", Type = typeof(FiscalYearDto))]
        public async Task<IActionResult> AddFiscalYear(AddFiscalYearCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing FiscalYear by Id.
        /// </summary>
        /// <param name="Id">The Id of the FiscalYear to be updated.</param>
        /// <param name="command">The command containing the updated FiscalYear details.</param>
        /// <returns>The updated FiscalYearDto object.</returns>
        [HttpPut("FiscalYear/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(FiscalYearDto))]
        public async Task<IActionResult> UpdateFiscalYear(string Id, UpdateFiscalYearCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an FiscalYear by Id.
        /// </summary>
        /// <param name="Id">The Id of the FiscalYear to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("FiscalYear/{Id}")]
        public async Task<IActionResult> DeleteFiscalYear(string Id)
        {
            var command = new DeleteFiscalYearCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
