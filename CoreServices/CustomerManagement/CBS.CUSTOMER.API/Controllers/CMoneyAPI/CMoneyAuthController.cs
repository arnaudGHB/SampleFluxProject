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
    public class CMoneyAuthController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the CMoneyController.
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands.</param>
        public CMoneyAuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

     
        /// <summary>
        /// Handles the login process for a C-MONEY member.
        /// </summary>
        [HttpPost("members/app-login")]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(LoginResponseDto))]
        public async Task<IActionResult> LoginMember([FromBody] LoginCMoneyMemberCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

    }

}
