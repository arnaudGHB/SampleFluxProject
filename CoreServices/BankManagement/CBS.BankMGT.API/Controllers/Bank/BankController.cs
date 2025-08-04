using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CBS.BankMGT.API.Controllers
{
    /// <summary>
    /// Bank
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class BankController : BaseController
    {
        public IMediator _mediator { get; set; }
        private IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Bank
        /// </summary>
        /// <param name="mediator"></param>
        public BankController(IMediator mediator, IWebHostEnvironment webHostEnvironment = null)
        {
            _mediator = mediator;
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// Get Bank By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Bank/{id}", Name = "GetBank")]
        [Produces("application/json", "application/xml", Type = typeof(BankDto))]
        public async Task<IActionResult> GetBank(string id)
        {
            var getBankQuery = new GetBankQuery { Id = id };
            var result = await _mediator.Send(getBankQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Banks
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Banks")]
        [Produces("application/json", "application/xml", Type = typeof(List<BankDto>))]
        public async Task<IActionResult> GetBanks()
        {
            var getAllBankQuery = new GetAllBankQuery { };
            var result = await _mediator.Send(getAllBankQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Bank
        /// </summary>
        /// <param name="addBankCommand"></param>
        /// <returns></returns>
        [HttpPost("Bank")]
        [Produces("application/json", "application/xml", Type = typeof(BankDto))]
        public async Task<IActionResult> AddBank(AddBankCommand addBankCommand)
        {
            var result = await _mediator.Send(addBankCommand);
            return ReturnFormattedResponse(result);
            
        }

        /// <summary>
        /// Update Bank logo Call back URL
        /// </summary>
        /// <param name="addBankCommand"></param>
        /// <returns></returns>
        [HttpPost("Bank/UpdateBankLogoCallBack")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UpdateBankLogoCallBack(UpdateBankLogoCommand addBankCommand)
        {
            var result = await _mediator.Send(addBankCommand);
            return ReturnFormattedResponse(result);

        }

        /// <summary>
        /// Update Bank By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateBankCommand"></param>
        /// <returns></returns>
        [HttpPut("Bank/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(BankDto))]
        public async Task<IActionResult> UpdateBank(string Id, UpdateBankCommand updateBankCommand)
        {
            updateBankCommand.Id = Id;
            var result = await _mediator.Send(updateBankCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Bank By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Bank/{Id}")]
        public async Task<IActionResult> DeleteBank(string Id)
        {
            var deleteBankCommand = new DeleteBankCommand { Id = Id };
            var result = await _mediator.Send(deleteBankCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
