using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.API.Controllers;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;
using CBS.TransactionManagement.MemberAccountConfiguration.Commands;
using CBS.TransactionManagement.MemberAccountConfiguration.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.MemberAccountSetting.Controllers
{
    /// <summary>
    /// Transaction
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class MemberAccountActivationController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// MemberAccountActivation 
        /// </summary>
        /// <param name="mediator"></param>
        public MemberAccountActivationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get MemberAccountActivation By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("MemberAccountActivation/{id}", Name = "GetMemberAccountActivation")]
        [Produces("application/json", "application/xml", Type = typeof(MemberAccountActivationDto))]
        public async Task<IActionResult> GetMemberAccountActivation(string id)
        {
            var getMemberAccountActivationQuery = new GetMemberAccountActivationQuery { Id = id };
            var result = await _mediator.Send(getMemberAccountActivationQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All MemberAccountActivation
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("MemberAccountActivation")]
        [Produces("application/json", "application/xml", Type = typeof(List<MemberAccountActivationDto>))]
        public async Task<IActionResult> GetMemberAccountActivations()
        {
            var getAllMemberAccountActivationQuery = new GetAllMemberAccountActivationQuery { };
            var result = await _mediator.Send(getAllMemberAccountActivationQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a MemberAccountActivation
        /// </summary>
        /// <param name="addMemberAccountActivationCommand"></param>
        /// <returns></returns>
        [HttpPost("MemberAccountActivation")]
        [Produces("application/json", "application/xml", Type = typeof(MemberAccountActivationDto))]
        public async Task<IActionResult> AddMemberAccountActivation(AddMemberAccountActivationCommand addMemberAccountActivationCommand)
        {
            var result = await _mediator.Send(addMemberAccountActivationCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update MemberAccountActivation By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateMemberAccountActivationCommand"></param>
        /// <returns></returns>
        [HttpPut("MemberAccountActivation/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UpdateMemberAccountActivation(string Id, UpdateAccountBalanceCommand updateMemberAccountActivationCommand)
        {
            updateMemberAccountActivationCommand.Id = Id;
            //updateMemberAccountActivationCommand.LoastOperation = "Subcription";
            //updateMemberAccountActivationCommand.IsDebit = true;
            var result = await _mediator.Send(updateMemberAccountActivationCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Update MemberAccountActivation By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateMemberAccountActivationCommand"></param>
        /// <returns></returns>
        [HttpGet("MemberAccountActivation/GetCustomerMemberAccountActivation/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(MemberAccountActivationDto))]
        public async Task<IActionResult> GetCustomerMemberAccountActivation(string Id)
        {
            
            var result = await _mediator.Send(new GetMemberAccountActivationByCustomerIdQuery{ CustomerId = Id });
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete MemberAccountActivation By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("MemberAccountActivation/{Id}")]
        public async Task<IActionResult> DeleteMemberAccountActivation(string Id)
        {
            var deleteMemberAccountActivationCommand = new DeleteMemberAccountActivationCommand { Id = Id };
            var result = await _mediator.Send(deleteMemberAccountActivationCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
