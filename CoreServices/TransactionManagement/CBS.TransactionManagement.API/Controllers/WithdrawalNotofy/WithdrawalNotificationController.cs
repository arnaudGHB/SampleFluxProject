using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Commands.WithdrawalNotificationP;
using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;
using CBS.TransactionManagement.Queries.WithdrawalNotificationP;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.WithdrawalNotofy
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class WithdrawalNotificationController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// WithdrawalNotification
        /// </summary>
        /// <param name="mediator"></param>
        public WithdrawalNotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get WithdrawalNotification By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("WithdrawalNotification/{id}", Name = "GetWithdrawalNotification")]
        [Produces("application/json", "application/xml", Type = typeof(WithdrawalNotificationDto))]
        public async Task<IActionResult> GetWithdrawalNotification(string id)
        {
            var getWithdrawalNotificationQuery = new GetWithdrawalNotificationQuery { Id = id };
            var result = await _mediator.Send(getWithdrawalNotificationQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get all WithdrawalNotification By customer id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("WithdrawalNotification/GetAllWithdrawalNotificationByCustomerId/{id}", Name = "GetAllWithdrawalNotificationByCustomerIdQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<WithdrawalNotificationDto>))]
        public async Task<IActionResult> GetAllWithdrawalNotificationByCustomerIdQuery(string id)
        {
            var getWithdrawalNotificationQuery = new GetAllWithdrawalNotificationByCustomerIdQuery { Id = id };
            var result = await _mediator.Send(getWithdrawalNotificationQuery);
            return ReturnFormattedResponse(result);
        }
        //GetAllWithdrawalNotificationByCustomerIdQuery
        /// <summary>
        /// Get All WithdrawalNotification
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("WithdrawalNotification")]
        [Produces("application/json", "application/xml", Type = typeof(List<WithdrawalNotificationDto>))]
        public async Task<IActionResult> GetWithdrawalNotifications()
        {
            var getAllWithdrawalNotificationQuery = new GetAllWithdrawalNotificationQuery { };
            var result = await _mediator.Send(getAllWithdrawalNotificationQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a WithdrawalNotification request
        /// </summary>
        /// <param name="addWithdrawalNotificationCommand"></param>
        /// <returns></returns>
        [HttpPost("WithdrawalNotification")]
        [Produces("application/json", "application/xml", Type = typeof(WithdrawalNotificationDto))]
        public async Task<IActionResult> AddWithdrawalNotification(AddWithdrawalNotificationRequestCommand addWithdrawalNotificationCommand)
        {
            var result = await _mediator.Send(addWithdrawalNotificationCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Get WithdrawalNotification By customer id & account number
        /// </summary>
        /// <param name="addWithdrawalNotificationCommand"></param>
        /// <returns></returns>
        [HttpPost("WithdrawalNotification/getNotification-mobileapp")]
        [Produces("application/json", "application/xml", Type = typeof(WithdrawalNotificationAndriodDto))]
        public async Task<IActionResult> GetWithdrawalNotificationMobileAppQuery(GetWithdrawalNotificationMobileAppQuery addWithdrawalNotificationCommand)
        {
            var result = await _mediator.Send(addWithdrawalNotificationCommand);
            return ReturnFormattedResponse(result);

        }
        
        /// <summary>
        /// Validate notification
        /// </summary>
        /// <param name="validationWithdrawalNotificationCommand"></param>
        /// <returns></returns>
        [HttpPut("WithdrawalNotification/ValidateNotification/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(WithdrawalNotificationDto))]
        public async Task<IActionResult> ValidationWithdrawalNotificationCommand(string Id, ValidationWithdrawalNotificationCommand validationWithdrawalNotificationCommand)
        {
            validationWithdrawalNotificationCommand.Id = Id;
            var result = await _mediator.Send(validationWithdrawalNotificationCommand);
            return ReturnFormattedResponse(result);


        }
        /// <summary>
        /// Notification payment to members.
        /// </summary>
        /// <param name="cashDeskWithdrawalNotificationCommand"></param>
        /// <returns></returns>
        [HttpPost("WithdrawalNotification/Payment")]
        [Produces("application/json", "application/xml", Type = typeof(WithdrawalNotificationDto))]
        public async Task<IActionResult> CashDeskWithdrawalNotificationCommand(CashDeskWithdrawalNotificationCommand cashDeskWithdrawalNotificationCommand)
        {
            var result = await _mediator.Send(cashDeskWithdrawalNotificationCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Update WithdrawalNotification By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateWithdrawalNotificationCommand"></param>
        /// <returns></returns>
        [HttpPut("WithdrawalNotification/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(WithdrawalNotificationDto))]
        public async Task<IActionResult> UpdateWithdrawalNotification(string Id, UpdateWithdrawalNotificationCommand updateWithdrawalNotificationCommand)
        {
            updateWithdrawalNotificationCommand.Id = Id;
            var result = await _mediator.Send(updateWithdrawalNotificationCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Pay in saving withdrawal notification by using notification id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="cashDeskWithdrawalNotificationCommand"></param>
        /// <returns></returns>
        [HttpPut("WithdrawalNotification/CashDeskWithdrawalNotificationCommand/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(WithdrawalNotificationDto))]
        public async Task<IActionResult> CashDeskWithdrawalNotificationCommand(string Id, CashDeskWithdrawalNotificationCommand cashDeskWithdrawalNotificationCommand)
        {
            cashDeskWithdrawalNotificationCommand.Id = Id;
            var result = await _mediator.Send(cashDeskWithdrawalNotificationCommand);
            return ReturnFormattedResponse(result);
        }
        //CashDeskWithdrawalNotificationCommand
        /// <summary>
        /// Delete WithdrawalNotification By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("WithdrawalNotification/{Id}")]
        public async Task<IActionResult> DeleteWithdrawalNotification(string Id)
        {
            var deleteWithdrawalNotificationCommand = new DeleteWithdrawalNotificationCommand { Id = Id };
            var result = await _mediator.Send(deleteWithdrawalNotificationCommand);
            return ReturnFormattedResponse(result);
        }
    }

}
