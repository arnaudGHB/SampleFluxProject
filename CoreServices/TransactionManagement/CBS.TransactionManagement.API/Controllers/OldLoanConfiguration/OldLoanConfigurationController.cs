using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.API.Controllers;
using CBS.TransactionManagement.Data.Dto.OldLoanConfiguration;
using CBS.TransactionManagement.OldLoanConfiguration.Commands;
using CBS.TransactionManagement.OldLoanConfiguration.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.OldLoanConfiguration.API.Controllers
{
    /// <summary>
    /// OldLoanConfigurationController to handle Old Loan Accounting Mapping related operations
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class OldLoanConfigurationController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Initializes a new instance of the OldLoanConfigurationController class.
        /// </summary>
        /// <param name="mediator">The mediator interface used to handle requests and commands</param>
        public OldLoanConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets the OldLoanAccountingMaping by its Id.
        /// </summary>
        /// <param name="id">The unique identifier of the OldLoanAccountingMaping</param>
        /// <returns>Returns the OldLoanAccountingMapingDto</returns>
        /// <response code="200">If the OldLoanAccountingMaping is successfully retrieved</response>
        /// <response code="404">If the OldLoanAccountingMaping with the given Id is not found</response>
        [HttpGet("OldLoanAccountingMaping/{id}", Name = "GetOldLoanAccountingMaping")]
        [Produces("application/json", "application/xml", Type = typeof(OldLoanAccountingMapingDto))]
        public async Task<IActionResult> GetOldLoanAccountingMaping(string id)
        {
            // Query to get the OldLoanAccountingMaping by Id
            var getOldLoanAccountingMapingQuery = new GetOldLoanAccountingMapingQuery { Id = id };
            var result = await _mediator.Send(getOldLoanAccountingMapingQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Gets all OldLoanAccountingMaping records.
        /// </summary>
        /// <returns>Returns a list of OldLoanAccountingMapingDto</returns>
        /// <response code="200">Returns the list of OldLoanAccountingMaping records</response>
        [HttpGet("OldLoanAccountingMaping")]
        [Produces("application/json", "application/xml", Type = typeof(List<OldLoanAccountingMapingDto>))]
        public async Task<IActionResult> GetOldLoanAccountingMaping()
        {
            // Query to get all OldLoanAccountingMaping records
            var getAllOldLoanAccountingMapingQuery = new GetAllOldLoanAccountingMapingQuery { };
            var result = await _mediator.Send(getAllOldLoanAccountingMapingQuery);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new OldLoanAccountingMaping record.
        /// </summary>
        /// <param name="addOldLoanAccountingMapingCommand">The command to add a new OldLoanAccountingMaping</param>
        /// <returns>Returns the newly created OldLoanAccountingMapingDto</returns>
        /// <response code="201">If the OldLoanAccountingMaping is successfully created</response>
        /// <response code="400">If there is a validation error or the data is invalid</response>
        [HttpPost("OldLoanAccountingMaping")]
        [Produces("application/json", "application/xml", Type = typeof(OldLoanAccountingMapingDto))]
        public async Task<IActionResult> AddOldLoanAccountingMaping(AddOldLoanAccountingMapingCommand addOldLoanAccountingMapingCommand)
        {
            // Command to add a new OldLoanAccountingMaping
            var result = await _mediator.Send(addOldLoanAccountingMapingCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Updates an existing OldLoanAccountingMaping by its Id.
        /// </summary>
        /// <param name="Id">The unique identifier of the OldLoanAccountingMaping to update</param>
        /// <param name="updateOldLoanAccountingMapingCommand">The command to update the OldLoanAccountingMaping</param>
        /// <returns>Returns the updated OldLoanAccountingMapingDto</returns>
        /// <response code="200">If the OldLoanAccountingMaping is successfully updated</response>
        /// <response code="404">If the OldLoanAccountingMaping with the given Id is not found</response>
        [HttpPut("OldLoanAccountingMaping/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(OldLoanAccountingMapingDto))]
        public async Task<IActionResult> UpdateOldLoanAccountingMaping(string Id, UpdateOldLoanAccountingMapingCommand updateOldLoanAccountingMapingCommand)
        {
            // Set the Id in the command and send it to update the OldLoanAccountingMaping
            updateOldLoanAccountingMapingCommand.Id = Id;
            var result = await _mediator.Send(updateOldLoanAccountingMapingCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Deletes an OldLoanAccountingMaping by its Id.
        /// </summary>
        /// <param name="Id">The unique identifier of the OldLoanAccountingMaping to delete</param>
        /// <returns>Returns an IActionResult indicating the outcome</returns>
        /// <response code="200">If the OldLoanAccountingMaping is successfully deleted</response>
        /// <response code="404">If the OldLoanAccountingMaping with the given Id is not found</response>
        [HttpDelete("OldLoanAccountingMaping/{Id}")]
        public async Task<IActionResult> DeleteOldLoanAccountingMaping(string Id)
        {
            // Command to delete an OldLoanAccountingMaping by Id
            var deleteOldLoanAccountingMapingCommand = new DeleteOldLoanAccountingMapingCommand { Id = Id };
            var result = await _mediator.Send(deleteOldLoanAccountingMapingCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
