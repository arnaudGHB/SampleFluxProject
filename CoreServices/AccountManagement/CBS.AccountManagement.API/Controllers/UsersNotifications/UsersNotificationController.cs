using CBS.AccountingManagement.MediatR.Commands;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// UsersNotification
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class UsersNotificationController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// UsersNotification
        /// </summary>
        /// <param name="mediator"></param>
        public UsersNotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get UsersNotification By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("UsersNotification/{id}", Name = "GetUsersNotification")]
        [Produces("application/json", "application/xml", Type = typeof(UsersNotificationDto))]
        public async Task<IActionResult> GetUsersNotification(string id)
        {
            var getUsersNotificationQuery = new GetUsersNotificationQuery { Id = id };
            var result = await _mediator.Send(getUsersNotificationQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All UsersNotifications
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("UsersNotifications")]
        [Produces("application/json", "application/xml", Type = typeof(List<UsersNotificationDto>))]
        public async Task<IActionResult> GetUsersNotifications()
        {
            var getAllUsersNotificationQuery = new GetAllUsersNotificationQuery { };
            var result = await _mediator.Send(getAllUsersNotificationQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a UsersNotification
        /// </summary>
        /// <param name="addUsersNotificationCommand"></param>
        /// <returns></returns>
        [HttpPost("UsersNotification")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddUsersNotification(AddUserNotificationCommand addUsersNotificationCommand)
        {
            var result = await _mediator.Send(addUsersNotificationCommand);
            return ReturnFormattedResponseObject(result);
        }

        /// <summary>
        /// Update UsersNotification By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateUsersNotificationCommand"></param>
        /// <returns></returns>
        [HttpPut("UsersNotification/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(UsersNotificationDto))]
        public async Task<IActionResult> UpdateUsersNotification(string Id, UpdateUserNotificationCommand updateUsersNotificationCommand)
        {
            updateUsersNotificationCommand.Id = Id;
            var result = await _mediator.Send(updateUsersNotificationCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete UsersNotification By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("UsersNotification/{Id}")]
        public async Task<IActionResult> DeleteUsersNotification(string Id)
        {
            var deleteUsersNotificationCommand = new DeleteUserNotificationCommand { Id = Id };
            var result = await _mediator.Send(deleteUsersNotificationCommand);
            return ReturnFormattedResponse(result);
        }
    }
}