using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.API.Controllers;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Commands;
using CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Queries;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.MemberAccountSetting.Controllers
{
    /// <summary>
    /// MemberAccountActivationPolicy
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class MemberAccountActivationPolicyController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// MemberAccountActivationPolicy 
        /// </summary>
        /// <param name="mediator"></param>
        public MemberAccountActivationPolicyController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get MemberAccountActivationPolicy  By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("MemberAccountActivationPolicy/{id}", Name = "GetMemberAccountActivationPolicy ")]
        [Produces("application/json", "application/xml", Type = typeof(MemberAccountActivationPolicyDto))]
        public async Task<IActionResult> GetMemberAccountActivationPolicy (string id)
        {
            var getMemberAccountActivationPolicyQuery = new GetMemberAccountActivationPolicyQuery { Id = id };
            var result = await _mediator.Send(getMemberAccountActivationPolicyQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All MemberAccountActivationPolicy 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("MemberAccountActivationPolicy")]
        [Produces("application/json", "application/xml", Type = typeof(List<MemberAccountActivationPolicyDto>))]
        public async Task<IActionResult> GetMemberAccountActivationPolicy()
        {
            var getAllMemberAccountActivationPolicyQuery = new GetAllMemberAccountActivationPolicyQuery { };
            var result = await _mediator.Send(getAllMemberAccountActivationPolicyQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a MemberAccountActivationPolicy 
        /// </summary>
        /// <param name="addMemberAccountActivationPolicy Command"></param>
        /// <returns></returns>
        [HttpPost("MemberAccountActivationPolicy")]
        [Produces("application/json", "application/xml", Type = typeof(MemberAccountActivationPolicyDto))]
        public async Task<IActionResult> AddMemberAccountActivationPolicy (AddMemberAccountActivationPolicyCommand addMemberAccountActivationPolicyCommand)
        {
                var result = await _mediator.Send(addMemberAccountActivationPolicyCommand);
                return ReturnFormattedResponse(result);               
        }
        /// <summary>
        /// Update MemberAccountActivationPolicy  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateMemberAccountActivationPolicy Command"></param>
        /// <returns></returns>
        [HttpPut("MemberAccountActivationPolicy/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(MemberAccountActivationPolicyDto))]
        public async Task<IActionResult> UpdateMemberAccountActivationPolicy (string Id, UpdateMemberAccountActivationPolicyCommand updateMemberAccountActivationPolicyCommand)
        {
            updateMemberAccountActivationPolicyCommand.Id = Id;
            var result = await _mediator.Send(updateMemberAccountActivationPolicyCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete MemberAccountActivationPolicy  By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("MemberAccountActivationPolicy/{Id}")]
        public async Task<IActionResult> DeleteMemberAccountActivationPolicy (string Id)
        {
            var deleteMemberAccountActivationPolicyCommand = new DeleteMemberAccountActivationPolicyCommand { Id = Id };
            var result = await _mediator.Send(deleteMemberAccountActivationPolicyCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
