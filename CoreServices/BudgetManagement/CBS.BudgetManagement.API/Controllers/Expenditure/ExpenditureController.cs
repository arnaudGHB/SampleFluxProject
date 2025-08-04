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
    /// Controller for managing Expenditure operations.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ExpenditureController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for ExpenditureController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public ExpenditureController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Expenditure by Id.
        /// </summary>
        /// <param name="id">The Id of the Expenditure.</param>
        /// <returns>An ExpenditureDto object.</returns>
        [HttpGet("Expenditure/{id}", Name = "GetExpenditure")]
        [Produces("application/json", "application/xml", Type = typeof(ExpenditureDto))]
        public async Task<IActionResult> GetExpenditure(string id)
        {
            var query = new GetExpenditureQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all Expenditures.
        /// </summary>
        /// <returns>A list of ExpenditureDto objects.</returns>
        [HttpGet("Expenditure")]
        [Produces("application/json", "application/xml", Type = typeof(List<ExpenditureDto>))]
        public async Task<IActionResult> GetAllExpenditures()
        {
            var query = new GetAllExpenditureQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

 

        /// <summary>
        /// Create a new Expenditure.
        /// </summary>
        /// <param name="command">The command to add a new Expenditure.</param>
        /// <returns>The created ExpenditureDto object.</returns>
        [HttpPost("Expenditure")]
        [Produces("application/json", "application/xml", Type = typeof(ExpenditureDto))]
        public async Task<IActionResult> AddExpenditure(AddExpenditureCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing Expenditure by Id.
        /// </summary>
        /// <param name="Id">The Id of the Expenditure to be updated.</param>
        /// <param name="command">The command containing the updated Expenditure details.</param>
        /// <returns>The updated ExpenditureDto object.</returns>
        [HttpPut("Expenditure/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(ExpenditureDto))]
        public async Task<IActionResult> UpdateExpenditure(string Id, UpdateExpenditureCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an Expenditure by Id.
        /// </summary>
        /// <param name="Id">The Id of the Expenditure to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("Expenditure/{Id}")]
        public async Task<IActionResult> DeleteExpenditure(string Id)
        {
            var command = new DeleteExpenditureCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
