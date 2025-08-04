//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.Notifications;
using CBS.NLoan.MediatR.Notifications.Commands;
using CBS.NLoan.MediatR.Notifications.Queries;
using CBS.TransactionManagement.Data.Dto.OTP;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.Notifications
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class OTPNotificationController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// OTPNotification
        /// </summary>
        /// <param name="mediator"></param>
        public OTPNotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get OTPNotification By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("OTPNotification/{Id}", Name = "GetOTPNotification")]
        [Produces("application/json", "application/xml", Type = typeof(OTPNotificationDto))]
        public async Task<IActionResult> GetOTPNotification(string Id)
        {
            var getOTPNotificationQuery = new GetOTPNotificationQuery { Id = Id };
            var result = await _mediator.Send(getOTPNotificationQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All OTPNotifications
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("OTPNotifications")]
        [Produces("application/json", "application/xml", Type = typeof(List<OTPNotificationDto>))]
        public async Task<IActionResult> GetOTPNotifications()
        {
            var getAllOTPNotificationQuery = new GetAllOTPNotificationQuery { };
            var result = await _mediator.Send(getAllOTPNotificationQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a OTPNotification
        /// </summary>
        /// <param name="addOTPNotificationCommand"></param>
        /// <returns></returns>
        [HttpPost("OTPNotification")]
        [Produces("application/json", "application/xml", Type = typeof(OTPNotificationDto))]
        public async Task<IActionResult> AddOTPNotification(AddOTPNotificationCommand addOTPNotificationCommand)
        {
            var result = await _mediator.Send(addOTPNotificationCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update OTPNotification By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateOTPNotificationCommand"></param>
        /// <returns></returns>
        [HttpPut("OTPNotification/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(OTPNotificationDto))]
        public async Task<IActionResult> UpdateOTPNotification(string Id, UpdateOTPNotificationCommand updateOTPNotificationCommand)
        {
            updateOTPNotificationCommand.Id = Id;
            var result = await _mediator.Send(updateOTPNotificationCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete OTPNotification By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("OTPNotification/{Id}")]
        public async Task<IActionResult> DeleteOTPNotification(string Id)
        {
            var deleteOTPNotificationCommand = new DeleteOTPNotificationCommand { Id = Id };
            var result = await _mediator.Send(deleteOTPNotificationCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
