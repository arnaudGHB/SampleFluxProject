using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.Customer.MEDIATR;
using CBS.Customer.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.API.Controllers.Base;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.Groups;
using CBS.CUSTOMER.MEDIATR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace CBS.CUSTOMER.API.Controllers.Group
{
    /// <summary>
    /// Controller for managing groups and related operations
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class GroupTypeController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the GroupTypeController
        /// This GroupTypeController manages operations related to group types.
        /// It includes methods for retrieving, creating, and updating groups and group types.
        /// The controller is secured with authorization, and MediatR is used for handling queries and commands.
        /// Each method has appropriate summaries for clarity.
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public GroupTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Create a customer group type
        /// </summary>
        [HttpPost("AddGroupType")]
        [Produces("application/json", "application/xml", Type = typeof(CreateGroupTypeDto))]
        public async Task<IActionResult> AddGroupCustomer(AddGroupTypeCommand addGroupTypeCommand)
        {
            var result = await _mediator.Send(addGroupTypeCommand);
            return ReturnFormattedResponse(result);
        }
        
        
        /// <summary>
        /// get all customer groupTypes
        /// </summary>
        [HttpGet("GroupTypes")]
        [Produces("application/json", "application/xml", Type = typeof(List<DATA.Entity.GroupType>))]
        public async Task<IActionResult> AddGroupCustomer()
        {
            var getAllGroupType = new GetAllGroupTypeQuery();
            var result = await _mediator.Send(getAllGroupType);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update group type by CustomerId
        /// </summary>
        [HttpPut("GroupType/{GroupTypeId}")]
        [Produces("application/json", "application/xml", Type = typeof(UpdateGroupTypeDto))]
        public async Task<IActionResult> UpdateGroup(string GroupTypeId, UpdateGroupTypeCommand updateGroupTypeCommand)
        {
            updateGroupTypeCommand.Id = GroupTypeId;
            var result = await _mediator.Send(updateGroupTypeCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete Group Type  by CustomerId
        /// </summary>
        [HttpDelete("GroupType/{CustomerId}")]
        public async Task<IActionResult> DeleteGroupType(string Id)
        {
            var deleteGroupTypeCommand = new DeleteGroupTypeCommand { Id = Id };
            var result = await _mediator.Send(deleteGroupTypeCommand);
            return ReturnFormattedResponse(result);
        }

    }
}

