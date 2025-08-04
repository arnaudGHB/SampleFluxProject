using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.CashOutThirdPartyP.Commands;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data.Dto.CashOutThirdPartyP;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Handlers;
using CBS.TransactionManagement.MediatR.Commands;
using CBS.TransactionManagement.MediatR.LoanDisbursementP.Commands;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Commands;
using CBS.TransactionManagement.MediatR.ThirtPartyPayment.Commands;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers
{
    /// <summary>
    /// Transaction
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class TransactionController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Transaction
        /// </summary>
        /// <param name="mediator"></param>
        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Transaction By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Transaction/{id}", Name = "GetTransaction")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> GetTransaction(string id)
        {
            var getTransactionQuery = new GetTransactionQuery { Id = id };
            var result = await _mediator.Send(getTransactionQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get Transaction By Account Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Transaction/Account/{id}", Name = "GetTransactionByAccount")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> GetTransactionByAccount(string id)
        {
            var getAllTransactionsByAccountQuery = new GetAllTransactionsByAccountQuery { AccountId = id };
            var result = await _mediator.Send(getAllTransactionsByAccountQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get Transaction By Account Number
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        [HttpGet("Transaction/AccountNumber/{accountNumber}", Name = "GetTransactionByAccountNumber")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> GetTransactionByAccountNumber(string accountNumber)
        {
            var getAllTransactionsByAccountNumberQuery = new GetAllTransactionsByAccountNumberQuery { AccountNumber = accountNumber };
            var result = await _mediator.Send(getAllTransactionsByAccountNumberQuery);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Get Transaction By Customer Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Transaction/Customer/{id}", Name = "GetTransactionByCustomer")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> GetTransactionByCustomer(string id)
        {
            var getAllTransactionsByCustomerQuery = new GetAllTransactionsByCustomerIdQuery { CustomerId = id };
            var result = await _mediator.Send(getAllTransactionsByCustomerQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieves transactions based on the provided query parameters such as date range, branch ID, and teller ID.
        /// The request can be filtered by accounting date or creation date depending on the query options.
        /// </summary>
        /// <param name="getAllTransactionsByDatesAndBranchQuery">
        /// A query object containing the necessary parameters for filtering transactions:
        /// DateFrom, DateTo, BranchID, TellerId, IsByDate, ByBranchId, ByTellerId, and UseAccountingDate.
        /// </param>
        /// <returns>
        /// Returns a formatted response containing a list of TransactionDto objects filtered by the provided query.
        /// The response is formatted based on the content type (JSON or XML).
        /// </returns>
        [HttpPost("Transaction/GetTransactionsByQueryParameters", Name = "GetAllTransactionsByDatesAndBranch")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> GetAllTransactionsByDatesAndBranchQuery(GetAllTransactionsByDatesAndBranchQuery getAllTransactionsByDatesAndBranchQuery)
        {
            // Send the query to the mediator to process and handle the logic for retrieving transactions
            var result = await _mediator.Send(getAllTransactionsByDatesAndBranchQuery);

            // Return the result in a formatted response (JSON/XML) with the appropriate status code
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get Transactions By dates and account number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Transaction/GetTransactionsByDatesAndAccountNumber", Name = "GetAllTransactionsByDatesAndAccountNumberQuery")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> GetAllTransactionsByDatesAndAccountNumberQueryHandler(GetAllTransactionsByDatesAndAccountNumberQuery getAllTransactionsByDatesAndAccountNumber)
        {
            var getAllTransactionsByCustomerQuery = new GetAllTransactionsByDatesAndAccountNumberQuery {DateFrom=getAllTransactionsByDatesAndAccountNumber.DateFrom, DateTo=getAllTransactionsByDatesAndAccountNumber.DateTo, AccountNumber=getAllTransactionsByDatesAndAccountNumber.AccountNumber };
            var result = await _mediator.Send(getAllTransactionsByCustomerQuery);
            return ReturnFormattedResponse(result);
        }
        //GetAllTransactionsByDatesAndAccountNumberQueryHandler
        /// <summary>
        /// Get Transactions between dates and by customer id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Transaction/GetAllTransactionsByDatesAndCustomerIDQuery", Name = "GetAllTransactionsByDatesAndCustomerIDQuery")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> GetAllTransactionsByDatesAndCustomerIDQuery(GetAllTransactionsByDatesAndCustomerIDQuery request)
        {
            var getAllTransactionsByCustomerQuery = new GetAllTransactionsByDatesAndCustomerIDQuery { DateFrom = request.DateFrom, DateTo = request.DateTo, CustomerID=request.CustomerID };
            var result = await _mediator.Send(getAllTransactionsByCustomerQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get Transactions between dates
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Transaction/GetAllTransactionsByDates", Name = "GetAllTransactionsByDatesQuery")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> GetAllTransactionsByDatesQuery(GetAllTransactionsByDatesQuery request)
        {
            var getAllTransactionsByCustomerQuery = new GetAllTransactionsByDatesQuery { DateFrom = request.DateFrom, DateTo = request.DateTo };
            var result = await _mediator.Send(getAllTransactionsByCustomerQuery);
            return ReturnFormattedResponse(result);
        }

      
        /// <summary>
        /// Create a Transaction
        /// </summary>
        /// <param name="addTransactionCommand"></param>
        /// <returns></returns>
        //[HttpPost("Transaction")]
        //[Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        //public async Task<IActionResult> AddTransaction(AddTransactionCommand addTransactionCommand)
        //{
        //        var result = await _mediator.Send(addTransactionCommand);
        //        return ReturnFormattedResponse(result);          
            
        //}

        /// <summary>
        /// Create Deposit Transaction
        /// </summary>
        /// <param name="depositTransactionCommand"></param>
        /// <returns></returns>
        [HttpPost("Transaction/Deposit")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> DepositTransaction(DepositTransactionCommand depositTransactionCommand)
        {
            var result = await _mediator.Send(depositTransactionCommand);
            return ReturnFormattedResponse(result);

        }


        /// <summary>
        /// Send cashout request
        /// </summary>
        /// <param name="addCashOutThirdPartyCommand"></param>
        /// <returns></returns>
        [HttpPost("Transaction/ThirdPartyCashoutRequest")]
        [Produces("application/json", "application/xml", Type = typeof(CashOutThirdPartyDto))]
        public async Task<IActionResult> ThirdPartyCashoutRequest(AddCashOutThirdPartyCommand addCashOutThirdPartyCommand)
        {
            var result = await _mediator.Send(addCashOutThirdPartyCommand);
            return ReturnFormattedResponse(result);

        }

        /// <summary>
        /// cashout request confirmation call 
        /// </summary>
        /// <param name="callBackCashOutThirdPartyCommand"></param>
        /// <returns></returns>
        [HttpPost("Transaction/CashoutConfirmation")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionThirdPartyDto))]
        public async Task<IActionResult> CashoutCallBack(CallBackCashOutThirdPartyCommand callBackCashOutThirdPartyCommand)
        {
            var result = await _mediator.Send(callBackCashOutThirdPartyCommand);
            return ReturnFormattedResponse(result);

        }

        /// <summary>
        /// Create a loan repayment transaction
        /// </summary>
        /// <param name="loanRepaymentCommand"></param>
        /// <returns></returns>
        [HttpPost("Transaction/LoanRepaymentCommand")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> LoanRepayment(LoanRepaymentCommand loanRepaymentCommand)
        {
            var result = await _mediator.Send(loanRepaymentCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Create loan dibursement
        /// </summary>
        /// <param name="loanDisbursmentCommand"></param>
        /// <returns></returns>
        [HttpPost("Transaction/LoanDisbursmentCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> LoanRepayment(LoanDisbursmentCommand loanDisbursmentCommand)
        {
            var result = await _mediator.Send(loanDisbursmentCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Block and Un block an amount of money from Account using status (Blocked or UnBlocked).
        /// </summary>
        /// <param name="blockAmountInAccountCommand"></param>
        /// <returns></returns>
        [HttpPost("Transaction/BlockAmountInAccountCommand")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> BlockedAmountInAccount(BlockAmountInAccountCommand blockAmountInAccountCommand)
        {
            var result = await _mediator.Send(blockAmountInAccountCommand);
            return ReturnFormattedResponse(result);

        }

      

        /// <summary>
        /// Withdrawal Transaction
        /// </summary>
        /// <param name="withdrawalTransactionCommand"></param>
        /// <returns></returns>
        [HttpPost("Transaction/Withdrawal")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> WithdrawalTransaction(AddBulkOperationWithdrawalCommand withdrawalTransactionCommand)
        {
            var result = await _mediator.Send(withdrawalTransactionCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Add bulk operation like multiple cash in or deposit.
        /// </summary>
        /// <param name="addBulkOperationCommand"></param>
        /// <returns></returns>
        [HttpPost("Transaction/BulkDeposit")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> BulkDeposit(AddBulkOperationDepositCommand addBulkOperationCommand)
        {
            var result = await _mediator.Send(addBulkOperationCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Cashin operation 3PP
        /// </summary>
        /// <param name="addThirdPartyCashInRequest"></param>
        /// <returns></returns>
        [HttpPost("Transaction/DepositThirtParty")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionThirdPartyDto))]
        public async Task<IActionResult> DepositThirtParty(AddThirdPartyCashInRequestCommand addThirdPartyCashInRequest)
        {
            var result = await _mediator.Send(addThirdPartyCashInRequest);
            return ReturnFormattedResponse(result);

        }

        /// <summary>
        /// Transfer Transaction
        /// </summary>
        /// <param name="transferTransactionCommand"></param>
        /// <returns></returns>
        [HttpPost("Transaction/Transfer")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> TransferTransaction(TransferTransactionCommand transferTransactionCommand)
        {
            var result = await _mediator.Send(transferTransactionCommand);
            return ReturnFormattedResponse(result);

        }

        /// <summary>
        /// Transfer third party
        /// </summary>
        /// <param name="transferThirdPartyCommand"></param>
        /// <returns></returns>
        [HttpPost("Transaction/TransferThirdPartyCommand")]
        [Produces("application/json", "application/xml", Type = typeof(TransferThirdPartyDto))]
        public async Task<IActionResult> TransferThirdPartyCommand(TransferThirdPartyLocalCommand transferThirdPartyCommand)
        {
            var result = await _mediator.Send(transferThirdPartyCommand);
            return ReturnFormattedResponse(result);

        }

        /// <summary>
        /// Transfer request
        /// </summary>
        /// <param name="transferRequestCommand"></param>
        /// <returns></returns>
        [HttpPost("Transaction/TransferRequestCommand")]
        [Produces("application/json", "application/xml", Type = typeof(TransferDto))]
        public async Task<IActionResult> TransferRequestCommand(TransferRequestCommand transferRequestCommand)
        {
            var result = await _mediator.Send(transferRequestCommand);
            return ReturnFormattedResponse(result);

        }

        /// <summary>
        /// Transfer request confirmation
        /// </summary>
        /// <param name="transferConfirmationCommand"></param>
        /// <returns></returns>
        [HttpPost("Transaction/TransferConfirmationCommand")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> TransferConfirmationCommand(TransferConfirmationCommand transferConfirmationCommand)
        {
            var result = await _mediator.Send(transferConfirmationCommand);
            return ReturnFormattedResponse(result);

        }


        ///// <summary>
        ///// Update Transaction By Id
        ///// </summary>
        ///// <param name="Id"></param>
        ///// <param name="updateTransactionCommand"></param>
        ///// <returns></returns>
        //[HttpPut("Transaction/{Id}")]
        //[Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        //public async Task<IActionResult> UpdateTransaction(string Id, UpdateLoanAccountBalanceCommand updateTransactionCommand)
        //{
        //    updateTransactionCommand.Id = Id;
        //    var result = await _mediator.Send(updateTransactionCommand);
        //    return ReturnFormattedResponse(result);
        //}
        /// <summary>
        /// Initial Deposit Transaction By SenderAccountNumber
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <param name="initialDepositCommand"></param>
        /// <returns></returns>
        [HttpPut("Transaction/Initiate/{SenderAccountNumber}")]
        [Produces("application/json", "application/xml", Type = typeof(TransactionDto))]
        public async Task<IActionResult> UpdateTransaction(string AccountNumber, InitialDepositCommand initialDepositCommand)
        {
            initialDepositCommand.AccountNumber = AccountNumber;
            var result = await _mediator.Send(initialDepositCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Transaction By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Transaction/{Id}")]
        public async Task<IActionResult> DeleteTransaction(string Id)
        {
            var deleteTransactionCommand = new DeleteTransactionCommand { Id = Id };
            var result = await _mediator.Send(deleteTransactionCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
