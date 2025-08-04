using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.CUSTOMER.API.Controllers.Base;
using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.DATA.Dto.OTP;
using CBS.CUSTOMER.DATA.Resources;
using CBS.CUSTOMER.MEDIATR.CMoney.ChangePhoneNumber.Command;
using CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Command;
using CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Queries;
using CBS.CustomerSmsConfigurations.MEDIAT.CMoney.MembersActivation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CBS.CUSTOMER.API.Controllers.CMoneyAPI
{
    /// <summary>
    /// Controller for managing C-MONEY operations such as Member Activation, Change PIN, Reset PIN, Update Activation, Deactivation, Reactivation, Query Operations, Secret Question Management, and OTP Generation.
    /// </summary>
    [Route("api/v1/cmoney")]
    [ApiController]
    [APICaller.Helper.LoginModel.Authenthication.Authorize]
    public class CMoneyController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the CMoneyController.
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands.</param>
        public CMoneyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Activates a C-MONEY member.
        /// </summary>
        [HttpPost("members/activate")]
        [Produces("application/json", Type = typeof(CMoneyMembersActivationAccountDto))]
        public async Task<IActionResult> ActivateMember([FromBody] AddCMoneyMemberActivationCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Changes the PIN for a C-MONEY member.
        /// </summary>
        [HttpPut("members/change-pin")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<IActionResult> ChangePin([FromBody] ChangeCMoneyMemberPinCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Resets the PIN for a C-MONEY member to the default value.
        /// </summary>
        [HttpPut("members/reset-pin")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<IActionResult> ResetPin([FromBody] ResetCMoneyMemberPinCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Resets the PIN for a C-MONEY member using a secret question and answer.
        /// </summary>
        [HttpPut("members/reset-pin-with-security")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<IActionResult> ResetPinWithSecurity([FromBody] ResetPinWithSecurityCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Updates the activation details for a C-MONEY member.
        /// </summary>
        [HttpPut("members/update-activation")]
        [Produces("application/json", Type = typeof(CMoneyMembersActivationAccountDto))]
        public async Task<IActionResult> UpdateActivation([FromBody] UpdateCMoneyMemberActivationCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Deactivates a C-MONEY member.
        /// </summary>
        [HttpPut("members/deactivate")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<IActionResult> DeactivateMember([FromBody] DeactivateCMoneyMemberCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Reactivates a C-MONEY member.
        /// </summary>
        [HttpPut("members/reactivate")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<IActionResult> ReactivateMember([FromBody] ReactivateCMoneyMemberCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        

        /// <summary>
        /// Retrieves a C-MONEY member activation by ID.
        /// </summary>
        [HttpGet("members/{id}")]
        [Produces("application/json", Type = typeof(CMoneyMembersActivationAccountDto))]
        public async Task<IActionResult> GetMemberById(string id)
        {
            var query = new GetCMoneyMemberActivationByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves a C-MONEY member activation by CustomerReference.
        /// </summary>
        [HttpGet("members/CustomerReference/{CustomerReference}")]
        [Produces("application/json", Type = typeof(CMoneyMembersActivationAccountDto))]
        public async Task<IActionResult> GetCMoneyMemberActivationByCustomerReference(string CustomerReference)
        {
            var query = new GetCMoneyMemberActivationByCustomerReferenceQuery { CustomerReference = CustomerReference };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves C-MONEY member activations based on various filters.
        /// </summary>
        [HttpGet("members")]
        [Produces("application/json", Type = typeof(List<CMoneyMembersActivationAccountDto>))]
        public async Task<IActionResult> GetMembers([FromQuery] GetCMoneyMemberActivationsQuery query)
        {
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves paginated C-MONEY member activations based on query parameters.
        /// </summary>
        [HttpGet("Customers/CMoneyMembersPagginationQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<CMoneyMembersActivationAccountDto>))]
        public async Task<IActionResult> SearchByAnyCriterialQuery([FromQuery] CustomerResource customerResource)
        {
            var searchByAnyCriterialQuery = new CMoneyMembersPagginationQuery { CustomerResource = customerResource };
            var result = await _mediator.Send(searchByAnyCriterialQuery);

            if (result.Data != null)
            {
                var paginationMetadata = new
                {
                    totalCount = result.Data.TotalCount,
                    pageSize = result.Data.PageSize,
                    skip = result.Data.Skip,
                    totalPages = result.Data.TotalPages
                };
                Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
            }

            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Handles the login process for a C-MONEY member.
        /// </summary>
        [HttpPost("members/login")]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(LoginResponseDto))]
        public async Task<IActionResult> LoginMember([FromBody] LoginCMoneyMemberCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Manages secret question and answer for a C-MONEY member.
        /// </summary>
        [HttpPut("members/secret-question")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<IActionResult> ManageSecretQuestion([FromBody] ManageSecretQuestionCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Generates an OTP for a C-MONEY member activation.
        /// </summary>
        [HttpPost("members/generate-otp")]
        [Produces("application/json", Type = typeof(TempOTPDto))]
        public async Task<IActionResult> GenerateOTP([FromBody] GenerateMemberActivationOTPCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }

}
