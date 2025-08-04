using CBS.AccountManagement.API.Model;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// AccountClass
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AccountClassController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// AccountClass
        /// </summary>
        /// <param name="mediator"></param>
        public AccountClassController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get AccountClass By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountClass/{id}", Name = "GetAccountClass")]
        [Produces("application/json", "application/xml", Type = typeof(AccountClassDto))]
        public async Task<IActionResult> GetAccountClass(string id)
        {
            var getAccountClassQuery = new GetAccountClassQuery { Id = id };
            var result = await _mediator.Send(getAccountClassQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All AccountClasss
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("AccountClasss")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountClassDto>))]
        public async Task<IActionResult> GetAccountClasss()
        {
            var getAllAccountClassQuery = new GetAllAccountClassesQuery { };
            var result = await _mediator.Send(getAllAccountClassQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get All AccountClasss
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("AccountClassCartegory/{accountNumber}")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountClassCategoryDto>))]
        public async Task<IActionResult> GetAccountClassCategoryDto(string accountNumber)
        {
            var getAllAccountClassQuery = new GetAccountCategoryByAccountNumberQuery { AccountNumber = accountNumber };
            var result = await _mediator.Send(getAllAccountClassQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a AccountClass
        /// </summary>
        /// <param name="addAccountClassCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountClass")]
        [Produces("application/json", "application/xml", Type = typeof(AccountClassDto))]
        public async Task<IActionResult> AddAccountClass(AddAccountClassCommand addAccountClassCommand)
        {
            var result = await _mediator.Send(addAccountClassCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Upload all existing AccountClass
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("AccountClass/UploadFiles/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountClassDto>))]
        public async Task<IActionResult> UploadOperationEvent(IFormFile file)
        {
            var getAllOperationEventQuery = new UploadAccountClassesQuery { AccountClasses = new AccountClassModel().Upload(file) };
            var result = await _mediator.Send(getAllOperationEventQuery);
            return Ok(result);
        }

        /// <summary>
        /// Update AccountClass By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateAccountClassCommand"></param>
        /// <returns></returns>
        [HttpPut("AccountClass/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(AccountClassDto))]
        public async Task<IActionResult> UpdateAccountClass(string Id, UpdateAccountClassCommand updateAccountClassCommand)
        {
            updateAccountClassCommand.Id = Id;
            var result = await _mediator.Send(updateAccountClassCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete AccountClass By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("AccountClass/{Id}")]
        public async Task<IActionResult> DeleteAccountClass(string Id)
        {
            var deleteAccountClassCommand = new DeleteAccountClassCommand { Id = Id };
            var result = await _mediator.Send(deleteAccountClassCommand);
            return ReturnFormattedResponse(result);
        }
    }
}