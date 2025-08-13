using CBS.TransactionManagement.CashOutThirdPartyP.Commands;
using CBS.TransactionManagement.Data.Dto.CashOutThirdPartyP;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CBS.TransactionManagement.API.Controllers;
using CBS.TransactionManagement.MediatR.ThirtPartyPayment.Commands;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Dto.Entity.ThirtPartyPayment;
using CBS.TransactionManagement.MediatR.GAV.Commands;
using Microsoft.AspNetCore.Authorization;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.GAV.Queries;
using CBS.TransactionManagement.MediatR.GAV.Query;
using CBS.TransactionManagement.ThirtPartyPayment.Queries;

namespace CBS.ThirdPartyOperationsManagement.API.Controllers
{
    /// <summary>
    /// Handles third-party operations including cashouts, transfers, and payments.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Policy = "ThirdPartyPolicy")] // Restrict access to users with the "ThirdPartyPolicy"
    public class ThirdPartyOperationsController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThirdPartyOperationsController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for sending commands.</param>
        public ThirdPartyOperationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Handles third-party cashout requests.
        /// </summary>
        /// <param name="addCashOutThirdPartyCommand">Command object for cashout request.</param>
        /// <returns>Cashout response object.</returns>
        [HttpPost("cashout-request")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(CashOutThirdPartyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ThirdPartyCashoutRequest([FromBody] AddCashOutThirdPartyCommand addCashOutThirdPartyCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(addCashOutThirdPartyCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Handles third-party GAV transfer requests.
        /// </summary>
        /// <param name="addGavTransactionCommand">Command object for GAV transfer.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        [HttpPost("transfer-gav")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TransferGAV([FromBody] AddGavTransactionCommand addGavTransactionCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(addGavTransactionCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Handles third-party cashout confirmation callbacks.
        /// </summary>
        /// <param name="callBackCashOutThirdPartyCommand">Command object for cashout confirmation callback.</param>
        /// <returns>Transfer response object.</returns>
        [HttpPost("cashout-confirmation")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(TransferThirdPartyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CashoutCallBack([FromBody] CallBackCashOutThirdPartyCommand callBackCashOutThirdPartyCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(callBackCashOutThirdPartyCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves all member accounts using a telephone number.
        /// </summary>
        /// <param name="telephonenumber">Telephone number of the member.</param>
        /// <returns>List of member account details.</returns>
        [HttpGet("accounts/{telephonenumber}", Name = "GetMemberAccountsQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<MemberAccountsThirdPartyDto>))]
        public async Task<IActionResult> GetMembersAccounts(string telephonenumber)
        {
            var getTellerQuery = new GetMemberAccountsQuery { TelephoneNumber = telephonenumber };
            var result = await _mediator.Send(getTellerQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves all accounts transactions by using account number.
        /// </summary>
        /// <param name="accountnumber">Account number of the member.</param>
        /// <returns>List of member account transactions.</returns>
        [HttpGet("transactions/{accountnumber}", Name = "GetTransactionThirdPartyQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<TransactionThirdPartyDto>))]
        public async Task<IActionResult> GetTransactionThirdPartyQuery(string accountnumber)
        {
            var getTellerQuery = new GetTransactionThirdPartyQuery { AccountNumber = accountnumber };
            var result = await _mediator.Send(getTellerQuery);
            return ReturnFormattedResponse(result);
        }
        //GetCustomerKYCByAccountNumberQuery
        ///// <summary>
        ///// Retrieves customer KYC details using a telephone number.
        ///// </summary>
        ///// <param name="telephonenumber">Telephone number of the customer.</param>
        ///// <returns>List of customer KYC details.</returns>
        //[HttpGet("kyc/{telephonenumber}", Name = "GetCustomerKYCByTelephoneQuery")]
        //[Produces("application/json", "application/xml", Type = typeof(List<MemberAccountsThirdPartyDto>))]
        //public async Task<IActionResult> GetCustomerKYCByTelephoneQuery(string telephonenumber)
        //{
        //    var getTellerQuery = new GetCustomerKYCByTelephoneQuery { TelephoneNumber = telephonenumber };
        //    var result = await _mediator.Send(getTellerQuery);
        //    return ReturnFormattedResponse(result);
        //}

        /// <summary>
        /// Retrieves customer KYC details using a accountnumber.
        /// </summary>
        /// <param name="accountnumber">Telephone number of the customer.</param>
        /// <returns>List of customer KYC details.</returns>
        [HttpGet("kyc/{accountnumber}", Name = "GetCustomerKYCByAccountNumberQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<MemberAccountsThirdPartyDto>))]
        public async Task<IActionResult> GetCustomerKYCByAccountNumberQuery(string accountnumber)
        {
            var getTellerQuery = new GetCustomerKYCByAccountNumberQuery { AccountNumber = accountnumber };
            var result = await _mediator.Send(getTellerQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves the balance of a member's account using an account number.
        /// </summary>
        /// <param name="accountnumber">Account number of the member.</param>
        /// <returns>Account balance details.</returns>
        [HttpGet("balance/{accountnumber}", Name = "GetMemberAccountBalanceTTPQuery")]
        [Produces("application/json", "application/xml", Type = typeof(AccountBalanceThirdPartyDto))]
        public async Task<IActionResult> GetMemberAccountBalance(string accountnumber)
        {
            var getTellerQuery = new GetMemberAccountBalanceTTPQuery { AccountNumber = accountnumber };
            var result = await _mediator.Send(getTellerQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieves the transfer charges for a specified account and amount.
        /// This endpoint calculates the service and total charges for a transfer based on the provided account number and transfer amount.
        /// The transfer charge is also influenced by the type of fee operation (e.g., CMoney, Gav, etc.).
        /// </summary>
        /// <param name="getTransferChargeQuery">The request body containing the details for calculating the transfer charge:</param>
        /// <param name="getTransferChargeQuery.SenderAccountNumber">Account number of the sender (required). Must be between 6 and 16 characters, numeric only.</param>
        /// <param name="getTransferChargeQuery.Amount">The amount for which the transfer charge is calculated (required). Must be greater than zero.</param>
        /// <param name="getTransferChargeQuery.ReceiverAccountNumber">Account number of the receiver (required). If provided, must be between 6 and 16 characters, numeric only.</param>
        /// <param name="getTransferChargeQuery.FeeOperationType">The type of fee operation for the transfer (required). It can be one of the following types:
        /// <list type="bullet">
        ///     <item><description>CMoney</description></item>
        ///     <item><description>Gav</description></item>
        ///     <item><description>MobileMoney</description></item>
        ///     <item><description>OrangeMoney</description></item>
        /// </list>
        /// </param>
        /// <returns>Returns transfer charge details, including service charges and total charges, in the response body.</returns>
        [HttpPost("transfer-charge", Name = "GetTransferChargeQuery")]
        [Produces("application/json", "application/xml", Type = typeof(TransferChargesDto))]
        public async Task<IActionResult> GetTransferCharge(GetTransferChargeQuery getTransferChargeQuery)
        {
            // Sending the query to the mediator to handle the logic of retrieving transfer charges
            var result = await _mediator.Send(getTransferChargeQuery);

            // Return the formatted response based on the result of the query execution
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Handles third-party deposit requests.
        /// </summary>
        /// <param name="addThirdPartyCashInRequest">Command object for third-party deposit request.</param>
        /// <returns>Transfer response object.</returns>
        [HttpPost("deposit-third-party")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(TransferThirdPartyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DepositThirdParty([FromBody] AddThirdPartyCashInRequestCommand addThirdPartyCashInRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(addThirdPartyCashInRequest);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Handles local transfer requests.
        /// </summary>
        /// <param name="thirdPartyLocalCommand">Command object for local transfer.</param>
        /// <returns>Transfer response object.</returns>
        [HttpPost("local-transfer")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(TransferThirdPartyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TransferLocal([FromBody] TransferThirdPartyLocalCommand thirdPartyLocalCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(thirdPartyLocalCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Handles inter-bank transfer requests.
        /// </summary>
        /// <param name="transferThirdPartyInterBankCommand">Command object for inter-bank transfer.</param>
        /// <returns>Transfer response object.</returns>
        [HttpPost("transfer-inter-bank")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(TransferThirdPartyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TransferInterBank([FromBody] TransferThirdPartyInterBankCommand transferThirdPartyInterBankCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(transferThirdPartyInterBankCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Handles third-party payment requests.
        /// </summary>
        /// <param name="gimacPaymentCommand">Command object for third-party payment.</param>
        /// <returns>Gimac payment response object.</returns>
        [HttpPost("third-party-payment")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(GimacPaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GimaPayment([FromBody] GimacPaymentCommand gimacPaymentCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(gimacPaymentCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
