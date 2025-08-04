using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// AccountType
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AccountTypeController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// AccountType
        /// </summary>
        /// <param name="mediator"></param>
        public AccountTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get AccountType By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountType/{id}", Name = "GetAccountType")]
        [Produces("application/json", "application/xml", Type = typeof(AccountTypeDto))]
        public async Task<IActionResult> GetAccountType(string id)
        {
            var getAccountTypeQuery = new GetAccountTypeQuery { Id = id };
            var result = await _mediator.Send(getAccountTypeQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get OperationEventAttribute By OperationAccountTypeId Query
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountType/GetOperationEventAttributes/{id}", Name = "GetOperationEventAttributes")]
        [Produces("application/json", "application/xml", Type = typeof(OperationEventAttributesDto))]
        public async Task<IActionResult> GetOperationEventAttributes(string id)
        {
            var getAccountTypeQuery = new GetOperationEventAttributeByOperationAccountTypeIdQuery { OperationAccountTypeId = id };
            var result = await _mediator.Send(getAccountTypeQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get AccountType By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountType/AccountRubrique/{id}", Name = "GetAccountRubrique")]
        [Produces("application/json", "application/xml", Type = typeof(AccountRubricResponseDto))]
        public async Task<IActionResult> GetAccountRubrique(string id)
        {
            var getAccountTypeQuery = new GetAccountTypeQuery { Id = id };
            var result = await _mediator.Send(getAccountTypeQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All AccountTypes
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("AccountTypes")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountTypeDto>))]
        public async Task<IActionResult> GetAccountTypes()
        {
            var getAllAccountTypeQuery = new GetAllAccountTypeQuery { };
            var result = await _mediator.Send(getAllAccountTypeQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get All AccountRubrique
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("AccountTypes/AccountRubrique")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountRubricResponseDto>))]
        public async Task<IActionResult> AccountRubrique()
        {
            var getAllAccountRubriqueQuery = new GetAllAccountRubriqueQuery { };
            var result = await _mediator.Send(getAllAccountRubriqueQuery);
            return Ok(result);
        }

        /// <summary>
        /// Create a AccountType for any banking product
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("AccountType")]
        [Produces("application/json", "application/xml", Type = typeof(ProductAccount))]
        public async Task<IActionResult> AddAccountType(AddProductAccountingBookCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a AccountType for any System Expenditure
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("AccountType/AccountTypeForSystem")]
        [Produces("application/json", "application/xml", Type = typeof(AccountTypeDto))]
        public async Task<IActionResult> AddAccountTypeForSys(AddAccountTypeForSystemCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All System AccountTypes
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("AccountTypes/AllSystemAccountTypes")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountTypeDto>))]
        public async Task<IActionResult> GetAllSystemAccountTypes()
        {
            var getAllAccountTypeQuery = new GetAllSystemAccountTypeQuery { };
            var result = await _mediator.Send(getAllAccountTypeQuery);
            return Ok(result);
        }

        /// <summary>
        /// Create a AccountType for any banking product
        /// </summary>
        /// <param name="addAccountTypeCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountType/AccountRubrique")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccountType(AddProductAccountRubriqueCommand addAccountTypeCommand)
        {
            var result = await _mediator.Send(addAccountTypeCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a AccountRubric for any banking product
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPut("AccountType/AccountRubrique")]
        [Produces("application/json", "application/xml", Type = typeof(AccountRubricResponseDto))]
        public async Task<IActionResult> AddAccountType(UpdateAccountRubriqueCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update AccountType By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateAccountTypeCommand"></param>
        /// <returns></returns>
        [HttpPut("AccountType/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(AccountTypeDto))]
        public async Task<IActionResult> UpdateAccountType(string Id, UpdateAccountTypeCommand updateAccountTypeCommand)
        {
            var result = await _mediator.Send(updateAccountTypeCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete AccountType By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("AccountType/{Id}")]
        public async Task<IActionResult> DeleteAccountType(string Id)
        {
            var deleteAccountTypeCommand = new DeleteAccountTypeCommand { Id = Id };
            var result = await _mediator.Send(deleteAccountTypeCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete AccountRubrique By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="deleteAccountRubriqueCommand"></param>
        /// <returns></returns>
        [HttpDelete("AccountType/AccountRubrique/{Id}")]
        public async Task<IActionResult> DeleteAccountRubrique(string Id, DeleteAccountRubriqueCommand deleteAccountRubriqueCommand)
        {
            var result = await _mediator.Send(deleteAccountRubriqueCommand);
            return ReturnFormattedResponse(result);
        }
    }
}