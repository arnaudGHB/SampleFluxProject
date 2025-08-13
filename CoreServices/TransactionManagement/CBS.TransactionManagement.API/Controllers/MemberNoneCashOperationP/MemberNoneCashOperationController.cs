using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.API.Controllers;
using CBS.TransactionManagement.Data.Dto.MemberNoneCashOperationP;
using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands;
using CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Commands;
using CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Queries;
using CBS.TransactionManagement.MediatR.MobileMoney.Commands;
using CBS.TransactionManagement.MediatR.MobileMoney.Queries;
using CBS.TransactionManagement.MediatR.RemittanceP.Commands;
using CBS.TransactionManagement.MediatR.RemittanceP.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.MemberNoneCashOperationP.Controllers
{
    /// <summary>
    /// Controller for managing Member None Cash Operations.
    /// Provides endpoints for creating, retrieving, validating, and deleting operations.
    /// Supports querying operations based on status, branch, and other filters.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class MemberNoneCashOperationController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for MemberNoneCashOperationController.
        /// Injects the IMediator service to handle requests.
        /// </summary>
        /// <param name="mediator">IMediator instance to handle queries and commands</param>
        public MemberNoneCashOperationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a specific Member None Cash Operation by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the operation.</param>
        /// <returns>Returns the operation details in JSON or XML format.</returns>
        [HttpGet("MemberNoneCashOperation/{id}", Name = "GetMemberNoneCashOperationById")]
        [Produces("application/json", "application/xml", Type = typeof(MemberNoneCashOperationDto))]
        public async Task<IActionResult> GetMemberNoneCashOperationById(string id)
        {
            var query = new GetMemberNoneCashOperationByIdQuery { Id=id};
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves all Member None Cash Operations based on query criteria.
        /// Allows filtering by status and branch.
        /// </summary>
        /// <param name="query">Query object containing the filter criteria.</param>
        /// <returns>A list of operations matching the criteria.</returns>
        [HttpGet("MemberNoneCashOperations", Name = "GetAllMemberNoneCashOperations")]
        [Produces("application/json", "application/xml", Type = typeof(List<MemberNoneCashOperationDto>))]
        public async Task<IActionResult> GetAllMemberNoneCashOperations([FromQuery] string BranchId, [FromQuery] string Status)
        {
            GetAllMemberNoneCashOperationsQuery query = new GetAllMemberNoneCashOperationsQuery(Status,BranchId);
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
       
        /// <summary>
        /// Creates a new Member None Cash Operation.
        /// </summary>
        /// <param name="command">Command object containing operation details.</param>
        /// <returns>Returns the created operation details.</returns>
        [HttpPost("MemberNoneCashOperation")]
        [Produces("application/json", "application/xml", Type = typeof(MemberNoneCashOperationDto))]
        public async Task<IActionResult> AddMemberNoneCashOperation([FromBody] AddMemberNoneCashOperationCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Validates a Member None Cash Operation.
        /// Updates the status based on the validation outcome.
        /// </summary>
        /// <param name="id">The unique identifier of the operation to validate.</param>
        /// <param name="command">Command object containing validation details.</param>
        /// <returns>Returns the validation result.</returns>
        [HttpPut("MemberNoneCashOperation/Validate/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(MemberNoneCashOperationDto))]
        public async Task<IActionResult> ValidateMemberNoneCashOperation(string id, [FromBody] ValidateMemberNoneCashOperationCommand command)
        {
            command.OperationId = id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Deletes a Member None Cash Operation by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the operation to delete.</param>
        /// <returns>Returns a boolean indicating success or failure.</returns>
        [HttpDelete("MemberNoneCashOperation/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> DeleteMemberNoneCashOperation(string id)
        {
            var command = new DeleteMemberNoneCashOperationCommand(id);
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

    }
}
