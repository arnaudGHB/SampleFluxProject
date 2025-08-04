using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CBS.TransactionManagement.Data.Dto.ReversalRequestP;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries.ReversalRequestP;
using CBS.TransactionManagement.MediatR.Commands.ReversalRequestP;

namespace CBS.TransactionManagement.API.Controllers.TransactionReversalP
{

    /// <summary>
    /// ReversalRequest
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class TransactionReversalController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// ReversalRequest 
        /// </summary>
        /// <param name="mediator"></param>
        public TransactionReversalController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get ReversalRequest  By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ReversalRequest/{id}", Name = "GetReversalRequest ")]
        [Produces("application/json", "application/xml", Type = typeof(ReversalRequestDto))]
        public async Task<IActionResult> GetReversalRequest(string id)
        {
            var getReversalRequestQuery = new GetReversalRequestQuery { Id = id };
            var result = await _mediator.Send(getReversalRequestQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All ReversalRequest by dates
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("ReversalRequest/All")]
        [Produces("application/json", "application/xml", Type = typeof(List<ReversalRequestDto>))]
        public async Task<IActionResult> GetReversalRequest(GetAllReversalRequestQuery ReversalRequestQuery)
        {
            var result = await _mediator.Send(ReversalRequestQuery);
            return ReturnFormattedResponse(result);
        }
        
        /// <summary>
        /// Create a ReversalRequest 
        /// </summary>
        /// <param name="addReversalRequest Command"></param>
        /// <returns></returns>
        [HttpPost("ReversalRequest/Request")]
        [Produces("application/json", "application/xml", Type = typeof(ReversalRequestDto))]
        public async Task<IActionResult> AddReversalRequest(AddReversalRequestCommand addReversalRequestCommand)
        {
            var result = await _mediator.Send(addReversalRequestCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// validate a Reversal Request 
        /// </summary>
        /// <param name="validationReversalRequestCommand Command"></param>
        /// <returns></returns>
        [HttpPost("ReversalRequest/Validate")]
        [Produces("application/json", "application/xml", Type = typeof(ReversalRequestDto))]
        public async Task<IActionResult> ValidateReversalRequest(ValidationReversalRequestCommand validationReversalRequestCommand)
        {
            var result = await _mediator.Send(validationReversalRequestCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Approve a Reversal Request 
        /// </summary>
        /// <param name="approvedReversalRequestCommand Command"></param>
        /// <returns></returns>
        [HttpPost("ReversalRequest/Approved")]
        [Produces("application/json", "application/xml", Type = typeof(ReversalRequestDto))]
        public async Task<IActionResult> ApproveReversalRequest(ApprovedReversalRequestCommand approvedReversalRequestCommand)
        {
            var result = await _mediator.Send(approvedReversalRequestCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Cash treatment of a Reversal Request 
        /// </summary>
        /// <param name="cashComploetionOfReversalCommand Command"></param>
        /// <returns></returns>
        [HttpPost("ReversalRequest/TreatRequest")]
        [Produces("application/json", "application/xml", Type = typeof(ReversalRequestDto))]
        public async Task<IActionResult> AApproveReversalRequest(CashCompletionOfReversalCommand cashComploetionOfReversalCommand)
        {
            var result = await _mediator.Send(cashComploetionOfReversalCommand);
            return ReturnFormattedResponse(result);
        }
       
        /// <summary>
        /// Delete ReversalRequest  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("ReversalRequest/{Id}")]
        public async Task<IActionResult> DeleteReversalRequest(string Id)
        {
            var deleteReversalRequestCommand = new DeleteReversalRequestCommand { Id = Id };
            var result = await _mediator.Send(deleteReversalRequestCommand);
            return ReturnFormattedResponse(result);
        }
    }

}
