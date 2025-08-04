using CBS.AccountManagement.MediatR.BankingOperation.Commands;
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
    public class BankingOperationController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// AccountType
        /// </summary>
        /// <param name="mediator"></param>
        public BankingOperationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// GEt DepositNotification By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BankingOperation/GetDepositNotificationQuery/{id}", Name = "GetDepositNotificationQuery")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetDepositNotificationQuery(string id)
        {
            var getCashReplenishmentQuery = new GetDepositNotificationQuery { Id = id };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// GetCashReplenishmentRequest By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BankingOperation/GetCashReplenishmentRequestById/{id}", Name = "GetCashReplenishmentRequest")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetCashReplenishmentRequest(string id)
        {
            var getCashReplenishmentQuery = new GetCashReplenishmentQuery { ReferenceId = id };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get CashReplenishment Request By RefereceId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BankingOperation/GetCashReplenishmentRequestByRefereceId/{id}", Name = "GetCashReplenishmentRequestByRefereceId")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetCashReplenishmentRequestByRefereceId(string id)
        {
            var getCashReplenishmentQuery = new GetCashReplenishmentByReferenceIdQuery { ReferenceId = id };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All CashReplenishmentRequest
        /// </summary>
        /// <returns></returns>
        [HttpGet("BankingOperation/GetAllCashRequestApprovalQuery", Name = "GetAllCashRequestApprovalQuery")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetAllCashRequestApprovalQuery()
        {
            var getCashReplenishmentQuery = new GetAllCashRequestApprovalQuery { };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All GetAllCashReplenishmentQueryAsBranch
        /// </summary>
        /// <returns></returns>
        [HttpGet("BankingOperation/GetAllCashReplenishmentQueryAsBranch/{BranchID}", Name = "GetAllCashReplenishmentQueryAsBranch")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetAllCashReplenishmentQueryAsBranch(bool BranchID)
        {
            var getCashReplenishmentQuery = new GetAllCashReplenishmentQueryAsBranch { CENSOR = BranchID };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All DepositNotification
        /// </summary>
        /// <returns></returns>
        [HttpGet("BankingOperation/GetAllDepositNotificationQuery", Name = "GetAllDepositNotificationQuery")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetAllDepositNotificationQuery()
        {
            var getCashReplenishmentQuery = new GetAllDepositNotificationQuery { };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All DepositNotification
        /// </summary>
        /// <returns></returns>
        [HttpGet("BankingOperation/GetAllDepositNotificationRedirectionQuery", Name = "GetAllDepositNotificationRedirectionQuery")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetAllDepositNotificationRedirectionQuery()
        {
            var getCashReplenishmentQuery = new GetAllDepositNotificationQueryAsBranch { };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All CashReplenishmentRequest
        /// </summary>
        /// <returns></returns>
        [HttpGet("BankingOperation/GetAllCashReplenishmentRequests", Name = "GetAllCashReplenishmentRequests")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetAllCashReplenishmentRequests()
        {
            var getCashReplenishmentQuery = new GetAllCashReplenishmentQuery { };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All CashReplenishmentRequest
        /// </summary>
        /// <returns></returns>BankingOperation/GetAllCashReplenishmentPendingRequests
        [HttpGet("BankingOperation/GetAllCashReplenishmentPendingRequests", Name = "GetAllCashReplenishmentPendingRequest")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetAllCashReplenishmentPendingRequest()
        {
            var getCashReplenishmentQuery = new GetAllCashReplenishmentPendingRequestQuery { BranchId = "" };
            var result = await _mediator.Send(getCashReplenishmentQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Credit a specifique Account record AddCashInfusionCommand
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("BankingOperation/RequestForCashReplenishment", Name = "AddCashInfusionCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddCashInfusionCommand(AddCashInfusionCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Credit a specifique Account record AddDepositNotificationCommand
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("BankingOperation/AddDepositNotificationCommand", Name = "AddDepositNotificationCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddDepositNotificationCommand(AddDepositNotificationCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update Deposit By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateCommand"></param>
        /// <returns></returns>
        [HttpPut("BankingOperation/UpdateDepositNotificationCommand/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UpdateDepositNotificationCommand(string Id, UpdateDepositNotificationCommand updateCommand)
        {
            updateCommand.Id = Id;
            var result = await _mediator.Send(updateCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update CashInfusionC By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateCommand"></param>
        /// <returns></returns>
        [HttpPut("BankingOperation/UpdateRequestForCashReplenishment/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UpdateCashInfusionCommand(string Id, UpdateCashInfusionCommand updateCommand)
        {
            updateCommand.Id = Id;
            var result = await _mediator.Send(updateCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Credit a specifique Account record AddCashInfusionCommand
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("BankingOperation/AddDepositApprovalCommand", Name = "AddDepositApprovalCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddDepositApprovalCommand(AddDepositApprovalCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Credit a specifique Account record AddCashInfusionCommand
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("BankingOperation/ResponseToCashReplenishment", Name = "Approval")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> Approval(AddCashReplenishmentApprovalCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Bank cash out at level of the branch
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("BankingOperation/BankCashOutCommand", Name = "BankCashOutCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> BankCashOutCommand(AddBankCashOutTransactionCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Bank cash IN at level of the branch
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("BankingOperation/AddBankCashInForTransactionCommand", Name = "AddBankCashInForTransactionCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddBankCashInForTransactionCommand(AddBankCashInForTransactionCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// GetBankTransactionQueryById By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BankingOperation/GetBankTransactionQueryById/{id}", Name = "GetBankTransactionQueryById")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetBankTransactionQueryById(string id)
        {
            var getBankTransactionQuery = new GetBankTransactionByIdQuery { Id = id };
            var result = await _mediator.Send(getBankTransactionQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// GetBankTransactionQueryById By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BankingOperation/GetBankTransactionQueryByReferenceId/{referenceId}", Name = "GetBankTransactionQueryByReferenceId")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetBankTransactionQueryByReferenceId(string referenceId)
        {
            var getBankTransactionQuery = new GetBankTransactionByReferenceIdQuery { ReferenceId = referenceId };
            var result = await _mediator.Send(getBankTransactionQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// GetTransactionReversalRequest By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BankingOperation/GetTransactionReversalRequestById/{id}", Name = "GetTransactionReversalRequestById")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetTransactionReversalRequestById(string id)
        {
            var getTransactionRequestReversalQuery = new GetTransactionRequestReversalQuery { Id = id };
            var result = await _mediator.Send(getTransactionRequestReversalQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// GetAllTransactionReversalRequest By Id
        /// </summary>

        /// <returns></returns>
        [HttpGet("BankingOperation/GetAllTransactionReversalRequests/{SearchOption}", Name = "GetAllTransactionReversalRequests")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetAllTransactionReversalRequests(string searchOption)
        {
            var getTransactionRequestReversalQuery = new GetAllTransactionRequestReversalQuery { SearchOption = searchOption };
            var result = await _mediator.Send(getTransactionRequestReversalQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// GetAllTransactionReversalRequest By ReferenceId
        /// </summary>

        /// <returns></returns>
        [HttpGet("BankingOperation/GetTransactionReversalRequestByReferenceId/{Id}", Name = "GetTransactionReversalRequestByReferenceId")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetTransactionReversalRequestByReferenceId(string Id)
        {
            var getTransactionRequestReversalQuery = new GetTransactionRequestReversalByReferenceIdQuery { ReferenceId = Id };
            var result = await _mediator.Send(getTransactionRequestReversalQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// GetAllTransactionReversalRequest By ReferenceId
        /// </summary>

        /// <returns></returns>
        [HttpGet("BankingOperation/CheckIfTransactionReversalRequestByReferenceIdExist/{Id}", Name = "CheckIfTransactionReversalRequestByReferenceIdExist")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> CheckIfTransactionReversalRequestByReferenceIdExist(string Id)
        {
            var getTransactionRequestReversalQuery = new CheckIfReferenceIdExistQuery { ReferenceId = Id };
            var result = await _mediator.Send(getTransactionRequestReversalQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Demand a specifique accounting entry reversal
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("BankingOperation/TransactionReversalRequest", Name = "TransactionReversalRequestCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> TransactionReversalRequestCommand(TransactionReversalRequestCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Approval of specifique accounting entry reversal Request
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("BankingOperation/TransactionReversalRequestApproval", Name = "TransactionReversalRequestApproval")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> TransactionReversalRequestApproval(TransactionReversalRequestApprovalCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// GetTransactionReversalRequest By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BankingOperation/GetTransactionReversalRequestDataById/{id}", Name = "GetTransactionReversalRequestDataById")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetTransactionReversalRequestDataById(string id)
        {
            var getTransactionRequestReversalQuery = new GetTransactionRequestReversalDataQuery { Id = id };
            var result = await _mediator.Send(getTransactionRequestReversalQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// GetAllTransactionReversalRequest By Id
        /// </summary>

        /// <returns></returns>
        [HttpGet("BankingOperation/GetAllTransactionReversalRequestDatas/{SearchOption}", Name = "GetAllTransactionReversalRequestDatas")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetAllTransactionReversalRequestDatas(string searchOption)
        {
            var getTransactionRequestReversalQuery = new GetAllTransactionRequestReversalDataQuery { SearchOption = searchOption };
            var result = await _mediator.Send(getTransactionRequestReversalQuery);
            return ReturnFormattedResponse(result);
        }
    }
}