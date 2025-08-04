using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.Communication.API.Controllers;
using CBS.Communication.Helper.Helper;
using CBS.Communication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CBS.Communication.MediatR.Email.Commands;
using CBS.Communication.MediatR.Sms.Commands;
using CBS.Communication.MediatR.Queries;
using CBS.Communication.Data.Dto;

namespace CBS.Communication.API.Controllers
{
    /// <summary>
    /// Controller for Sending SMS 
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class SendNotificationController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the SendNotificationController
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public SendNotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Get All Notification  By  NotificationType  And MemberReference
        /// </summary>
        /// <returns>Response with either true or false.</returns>
        [HttpGet("Notification/Get/{notificationType}/{memberReference}")]
        [Produces("application/json", "application/xml", Type = typeof(NotificationDto))]
        public async Task<IActionResult> GetAllNotificationsByCustomerReferenceQuery(string notificationType,string memberReference)
        {
            GetAllNotificationsByCustomerReferenceQuery query = new()
            {
                MemberReference=memberReference, NotificationType=notificationType,
            };
            
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All Notification  By  NotificationType 
        /// </summary>
        /// <returns>Response with either true or false.</returns>
        [HttpGet("Notification/Get/{notificationType}")]
        [Produces("application/json", "application/xml", Type = typeof(NotificationDto))]
        public async Task<IActionResult> GetAllNotificationsByNotificationTypeQuery(string notificationType)
        {
            GetAllNotificationsByNotificationTypeQuery query = new()
            {
                NotificationType = notificationType,
            };

            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }




    }
}


