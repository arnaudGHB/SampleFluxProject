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
    /// AccountCartegory
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AccountCartegoryController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// AccountCartegory
        /// </summary>
        /// <param name="mediator"></param>
        public AccountCartegoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get AccountCartegory By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountCartegory/{id}", Name = "GetAccountCartegory")]
        [Produces("application/json", "application/xml", Type = typeof(AccountCartegoryDto))]
        public async Task<IActionResult> GetAccountCartegory(string id)
        {
            var getAccountCartegoryQuery = new GetAccountCategoryQuery { Id = id };
            var result = await _mediator.Send(getAccountCartegoryQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All AccountCartegorys
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("AccountCartegories")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountCartegoryDto>))]
        public async Task<IActionResult> GetAccountCartegories()
        {
            var getAllAccountCartegoryQuery = new GetAllAccountCategoryQuery { };
            var result = await _mediator.Send(getAllAccountCartegoryQuery);
            return Ok(result);
        }

        /// <summary>
        /// Create a AccountCartegory
        /// </summary>
        /// <param name="addAccountCartegoryCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountCartegory")]
        [Produces("application/json", "application/xml", Type = typeof(AccountCartegoryDto))]
        public async Task<IActionResult> AddAccountCartegory(AddAccountCategoryCommand addAccountCartegoryCommand)
        {
            var result = await _mediator.Send(addAccountCartegoryCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a list of AccountCartegory
        /// </summary>
        /// <param name="addAccountCartegoryCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountCartegories/CreateListOfItem")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountCartegoryDto>))]
        public async Task<IActionResult> AddAccountCartegory(AddAccountCartegoriesCommand addAccountCartegoryCommand)
        {
            var result = await _mediator.Send(addAccountCartegoryCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Upload all existing AccountCartegory
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("AccountCartegory/UploadFiles/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountCartegoryDto>))]
        public async Task<IActionResult> UploadOperationEvent(IFormFile file)
        {
            var getAllOperationEventQuery = new UploadAccountCategoriesQuery { AccountCategories = new AccountCartegoryModel().Upload(file) };
            var result = await _mediator.Send(getAllOperationEventQuery);
            return Ok(result);
        }

        /// <summary>
        /// Update AccountCartegory By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateAccountCartegoryCommand"></param>
        /// <returns></returns>
        [HttpPut("AccountCartegory/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(AccountCartegoryDto))]
        public async Task<IActionResult> UpdateAccountCartegory(string Id, UpdateAccountCartegoryCommand updateAccountCartegoryCommand)
        {
            updateAccountCartegoryCommand.Id = Id;
            var result = await _mediator.Send(updateAccountCartegoryCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete AccountCartegory By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("AccountCartegory/{Id}")]
        public async Task<IActionResult> DeleteAccountCartegory(string Id)
        {
            var deleteAccountCartegoryCommand = new DeleteAccountCategoryCommand { Id = Id };
            var result = await _mediator.Send(deleteAccountCartegoryCommand);
            return ReturnFormattedResponse(result);
        }
    }
}