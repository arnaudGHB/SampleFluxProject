using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.Communication.API.Controllers;
using CBS.Communication.Helper.Helper;
using CBS.Communication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CBS.Communication.MediatR.Email.Commands;
using CBS.Communication.MediatR.Sms.Commands;

namespace CBS.Communication.API.Controllers
{
    /// <summary>
    /// Controller for Sending SMS 
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class SendPushNotificationController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the SendPushNotificationController
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public SendPushNotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Notify Client By Sending PushnNotification
        /// </summary>
        /// <param name="SendPushNotification">Data for client email notification.</param>
        /// <returns>Response with either true or false.</returns>
        [HttpPost("Send/PushNotification")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> NotifyClientByEmail(SendSinglePushNotificationCommand sendSinglePushNotification)
        {
            var result = await _mediator.Send(sendSinglePushNotification);
            return ReturnFormattedResponse(result);
        }



    }
}


