using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.BankMGT.MediatR.Validations;
using CBS.CUSTOMER.API.Controllers.Base;
using CBS.CUSTOMER.DATA;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Entity.ChangePhoneNumber;
using CBS.CUSTOMER.DATA.Resources;
using CBS.CUSTOMER.MEDIATR;
using CBS.CUSTOMER.MEDIATR.CMoney.ChangePhoneNumber.Command;
using CBS.CUSTOMER.MEDIATR.CMoney.ChangePhoneNumber.Queries;
using CBS.CUSTOMER.MEDIATR.Customer.Command;
using CBS.CUSTOMER.MEDIATR.Customer.Queries;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace CBS.CUSTOMER.API.Controllers.Customer
{
    /// <summary>
    /// Controller for managing customer phone number changes.
    /// This controller handles operations related to changing customer phone numbers,
    /// including retrieving change history, approving requests, and updating phone numbers.
    /// The controller is secured with authorization, and MediatR is used for handling queries and commands.
    /// Each method includes summaries for clarity.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ChangeCustomerPhoneController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for initializing the ChangeCustomerPhoneController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling requests and commands.</param>
        public ChangeCustomerPhoneController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves the phone number change history record by its unique identifier.
        /// </summary>
        /// <param name="PhoneNumberChangeHistoryId">The unique identifier of the phone number change history.</param>
        /// <returns>Returns the phone number change history record if found.</returns>
        [HttpGet("ChangeCustomerPhone/Get-History-byid/{PhoneNumberChangeHistoryId}", Name = "PhoneNumberChangeHistory")]
        [Produces("application/json", "application/xml", Type = typeof(PhoneNumberChangeHistory))]
        public async Task<IActionResult> GetCustomerAgeCategoryQuery(string PhoneNumberChangeHistoryId)
        {
            // Create a query request with the provided ID
            var getCustomerQuery = new GetPhoneNumberChangeHistoryRequestQuery { PhoneNumberChangeHistoryId = PhoneNumberChangeHistoryId };

            // Send the query request to MediatR for processing
            var result = await _mediator.Send(getCustomerQuery);

            // Return the formatted response to the client
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves a list of phone number change history records based on their status.
        /// </summary>
        /// <param name="Status">The status of the phone number change requests (e.g., Pending, Approved, Rejected).</param>
        /// <returns>A list of phone number change history records matching the given status.</returns>
        [HttpGet("ChangeCustomerPhone/History/{Status}")]
        [Produces("application/json", "application/xml", Type = typeof(List<PhoneNumberChangeHistory>))]
        public async Task<IActionResult> GetAllCustomerAgeCategoriesQuery(string Status)
        {
            // Create a query request to fetch change history records by status
            var getBranchPhoneNumberChangeHistoriesByStatusRequestQuery = new GetBranchPhoneNumberChangeHistoriesByStatusRequestQuery { Status = Status };

            // Send the query request to MediatR for processing
            var result = await _mediator.Send(getBranchPhoneNumberChangeHistoriesByStatusRequestQuery);

            // Return the result as an HTTP response
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Initiates a request to change the phone number of a C-MONEY member.
        /// </summary>
        /// <param name="command">The command containing details of the phone number change request.</param>
        /// <returns>Returns a response indicating whether the request was successful.</returns>
        [HttpPost("ChangeCustomerPhone/change-phone-number")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<IActionResult> ChangePhoneNumber([FromBody] ChangePhoneNumberRequestCommand command)
        {
            // Send the command request to MediatR for processing
            var result = await _mediator.Send(command);

            // Return the formatted response to the client
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Approves a pending phone number change request for a C-MONEY member.
        /// </summary>
        /// <param name="command">The command containing details for approving a phone number change request.</param>
        /// <returns>Returns a response indicating whether the approval was successful.</returns>
        [HttpPut("ChangeCustomerPhone/approve-phone-number")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<IActionResult> ApprovePhoneNumber([FromBody] ApprovePhoneNumberRequestCommand command)
        {
            // Send the approval command request to MediatR for processing
            var result = await _mediator.Send(command);

            // Return the formatted response to the client
            return ReturnFormattedResponse(result);
        }
    }

}


