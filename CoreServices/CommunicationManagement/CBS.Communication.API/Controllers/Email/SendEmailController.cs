using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.Communication.API.Controllers;
using CBS.Communication.Helper.Helper;
using CBS.Communication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CBS.Communication.MediatR.Email.Commands;

namespace CBS.Communication.API.Controllers
{
    /// <summary>
    /// Controller for Sending SMS 
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class SendEmailController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the SendEmailController
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public SendEmailController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Notify Client By Email
        /// </summary>
        /// <param name="sendEmailSpecification">Data for client email notification.</param>
        /// <returns>Response with either true or false.</returns>
        [HttpPost("Send/Email")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> NotifyClientByEmail(SendEmailSpecificationCommand sendEmailSpecification)
        {
            var result = await _mediator.Send(sendEmailSpecification);
            return ReturnFormattedResponse(result);
        }



    }
}


