using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.Communication.API.Controllers;
using CBS.Communication.Helper.Helper;
using CBS.Communication.MediatR.Sms.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;



namespace CBS.Communication.API.Controllers
{
    /// <summary>
    /// Controller for Sending SMS 
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class SendSMSController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the SendSMSController
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public SendSMSController(IMediator mediator)
        {
            _mediator = mediator;
        }

   
        /// <summary>
        /// Send Single SMS
        /// </summary>
        [HttpPost("SendSingleSMS")]
        [Produces("application/json", "application/xml", Type = typeof(SendSingleSmsResponse))]
        public async Task<IActionResult> SendSingleSMS(SendSingleSmsHalfCommand sendSingleSmsCommand)
        {

            var result = await _mediator.Send(sendSingleSmsCommand);
            return ReturnFormattedResponse(result);
        } 
        
        /// <summary>
        /// Send Single SMS
        /// </summary>
        [HttpPost("SendSingleSMS/Other")]
        [Produces("application/json", "application/xml", Type = typeof(SendSingleSmsResponse))]
        public async Task<IActionResult> SendSingleSMS(SendSingleSmsCommand sendSingleSmsCommand)
        {

            var result = await _mediator.Send(sendSingleSmsCommand);
            return ReturnFormattedResponse(result);
        }  


        /// <summary>
        /// Send Multiple SMS
        /// </summary>
        [HttpPost("SendMultipleSMS")]
        [Produces("application/json", "application/xml", Type = typeof(List<SendSingleSmsResponse>))]
        public async Task<IActionResult> SendSingleSMS(SendMultipleSmsCommand sendMultipleSMSCommand)
        {

            var result = await _mediator.Send(sendMultipleSMSCommand);
            return ReturnFormattedResponse(result);
        }  
        
        
     
    }
}


