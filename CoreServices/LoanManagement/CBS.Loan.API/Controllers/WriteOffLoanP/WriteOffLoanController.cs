
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.WriteOffLoanP;
using CBS.NLoan.MediatR.WriteOffLoanMediaR.Commands;
using CBS.NLoan.MediatR.WriteOffLoanMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.WriteOffLoanP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class WriteOffLoanController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// WriteOffLoan
        /// </summary>
        /// <param name="mediator"></param>
        public WriteOffLoanController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get WriteOffLoan By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("WriteOffLoan/{Id}", Name = "GetWriteOffLoan")]
        [Produces("application/json", "application/xml", Type = typeof(WriteOffLoanDto))]
        public async Task<IActionResult> GetWriteOffLoan(string Id)
        {
            var getWriteOffLoanQuery = new GetWriteOffLoanQuery { Id = Id };
            var result = await _mediator.Send(getWriteOffLoanQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All WriteOffLoans
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("WriteOffLoans")]
        [Produces("application/json", "application/xml", Type = typeof(List<WriteOffLoanDto>))]
        public async Task<IActionResult> GetWriteOffLoans()
        {
            var getAllWriteOffLoanQuery = new GetAllWriteOffLoanQuery { };
            var result = await _mediator.Send(getAllWriteOffLoanQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a WriteOffLoan
        /// </summary>
        /// <param name="addWriteOffLoanCommand"></param>
        /// <returns></returns>
        [HttpPost("WriteOffLoan")]
        [Produces("application/json", "application/xml", Type = typeof(WriteOffLoanDto))]
        public async Task<IActionResult> AddWriteOffLoan(AddWriteOffLoanCommand addWriteOffLoanCommand)
        {
            var result = await _mediator.Send(addWriteOffLoanCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update WriteOffLoan By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateWriteOffLoanCommand"></param>
        /// <returns></returns>
        [HttpPut("WriteOffLoan/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(WriteOffLoanDto))]
        public async Task<IActionResult> UpdateWriteOffLoan(string Id, UpdateWriteOffLoanCommand updateWriteOffLoanCommand)
        {
            updateWriteOffLoanCommand.Id = Id;
            var result = await _mediator.Send(updateWriteOffLoanCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete WriteOffLoan By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("WriteOffLoan/{Id}")]
        public async Task<IActionResult> DeleteWriteOffLoan(string Id)
        {
            var deleteWriteOffLoanCommand = new DeleteWriteOffLoanCommand { Id = Id };
            var result = await _mediator.Send(deleteWriteOffLoanCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
