using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers
{
    /// <summary>
    /// TempAccount
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class SavingProductController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// SavingProduct
        /// </summary>
        /// <param name="mediator"></param>
        public SavingProductController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get SavingProduct By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("SavingProduct/{id}", Name = "GetSavingProduct")]
        [Produces("application/json", "application/xml", Type = typeof(SavingProductDto))]
        public async Task<IActionResult> GetSavingProduct(string id)
        {
            var getSavingProductQuery = new GetSavingProductQuery { Id = id };
            var result = await _mediator.Send(getSavingProductQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All SavingProduct
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("SavingProduct")]
        [Produces("application/json", "application/xml", Type = typeof(List<SavingProductDto>))]
        public async Task<IActionResult> GetSavingProducts()
        {
            var getAllSavingProductQuery = new GetAllSavingProductQuery { };
            var result = await _mediator.Send(getAllSavingProductQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a SavingProduct
        /// </summary>
        /// <param name="addSavingProductCommand"></param>
        /// <returns></returns>
        [HttpPost("SavingProduct")]
        [Produces("application/json", "application/xml", Type = typeof(SavingProductDto))]
        public async Task<IActionResult> AddSavingProduct(AddSavingProductCommand addSavingProductCommand)
        {
                var result = await _mediator.Send(addSavingProductCommand);
                return ReturnFormattedResponse(result);          
            
        }
        /// <summary>
        /// Update SavingProduct By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateSavingProductCommand"></param>
        /// <returns></returns>
        [HttpPut("SavingProduct/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(SavingProductDto))]
        public async Task<IActionResult> UpdateSavingProduct(string Id, UpdateSavingProductCommand updateSavingProductCommand)
        {
            updateSavingProductCommand.Id = Id;
            var result = await _mediator.Send(updateSavingProductCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete SavingProduct By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("SavingProduct/{Id}")]
        public async Task<IActionResult> DeleteSavingProduct(string Id)
        {
            var deleteSavingProductCommand = new DeleteSavingProductCommand { Id = Id };
            var result = await _mediator.Send(deleteSavingProductCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
