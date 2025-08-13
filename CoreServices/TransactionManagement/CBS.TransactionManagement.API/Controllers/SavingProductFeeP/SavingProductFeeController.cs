using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data.Dto.SavingProductFeeP;
using CBS.TransactionManagement.MediatR.SavingProductFeeP.Commands;
using CBS.TransactionManagement.MediatR.SavingProductFeeP.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.SavingProductFeeP
{
    //SavingProductFee
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class SavingProductFeeController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// SavingProductFee
        /// </summary>
        /// <param name="mediator"></param>
        public SavingProductFeeController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get SavingProductFee By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("SavingProductFee/{Id}", Name = "GetSavingProductFee")]
        [Produces("application/json", "application/xml", Type = typeof(SavingProductFeeDto))]
        public async Task<IActionResult> GetSavingProductFee(string Id)
        {
            var getSavingProductFeeQuery = new GetSavingProductFeeQuery { Id = Id };
            var result = await _mediator.Send(getSavingProductFeeQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All SavingProductFees
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("SavingProductFees")]
        [Produces("application/json", "application/xml", Type = typeof(List<SavingProductFeeDto>))]
        public async Task<IActionResult> GetSavingProductFees()
        {
            var getAllSavingProductFeeQuery = new GetAllSavingProductFeeQuery { };
            var result = await _mediator.Send(getAllSavingProductFeeQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a SavingProductFee
        /// </summary>
        /// <param name="addSavingProductFeeCommand"></param>
        /// <returns></returns>
        [HttpPost("SavingProductFee")]
        [Produces("application/json", "application/xml", Type = typeof(SavingProductFeeDto))]
        public async Task<IActionResult> AddSavingProductFee(AddSavingProductFeeCommand addSavingProductFeeCommand)
        {
            var result = await _mediator.Send(addSavingProductFeeCommand);
            return ReturnFormattedResponse(result);

        }

        /// <summary>
        /// Update SavingProductFee By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateSavingProductFeeCommand"></param>
        /// <returns></returns>
        [HttpPut("SavingProductFee/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(SavingProductFeeDto))]
        public async Task<IActionResult> UpdateSavingProductFee(string Id, UpdateSavingProductFeeCommand updateSavingProductFeeCommand)
        {
            updateSavingProductFeeCommand.Id = Id;
            var result = await _mediator.Send(updateSavingProductFeeCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete SavingProductFee By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("SavingProductFee/{Id}")]
        public async Task<IActionResult> DeleteSavingProductFee(string Id)
        {
            var deleteSavingProductFeeCommand = new DeleteSavingProductFeeCommand { Id = Id };
            var result = await _mediator.Send(deleteSavingProductFeeCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
