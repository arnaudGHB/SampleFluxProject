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
    public class GroupCustomerController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the GroupCustomerController
        /// This GroupCustomerController manages operations related to groups  customers.
        /// It includes methods for retrieving, creating, and updating  group customers.
        /// The controller is secured with authorization, and MediatR is used for handling queries and commands.
        /// Each method has appropriate summaries for clarity.
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public GroupCustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

      

        /// <summary>
        /// Get group customer by GroupCustomerId
        /// </summary>
        [HttpGet("GroupCustomer/{GroupCustomerId}", Name = "GetGroupCustomer")]
        [Produces("application/json", "application/xml", Type = typeof(DATA.Entity.GroupCustomer))]
        public async Task<IActionResult> GetGroupCustomer(string GroupCustomerId)
        {
            var getGroupQuery = new GetGroupCustomerQuery { GroupCustomerId = GroupCustomerId };
            var result = await _mediator.Send(getGroupQuery);
            return ReturnFormattedResponse(result);
        }

       

        /// <summary>
        /// Get all group customers 
        /// </summary>
        [HttpGet("GetGroupCustomers")]
        [Produces("application/json", "application/xml", Type = typeof(List<DATA.Entity.GroupCustomer>))]
        public async Task<IActionResult> GetGroupCustomers()
        {
            var getAllGroupQuery = new GetAllGroupCustomerQuery();
            var result = await _mediator.Send(getAllGroupQuery);
            return Ok(result);
        }
        
        
        /// <summary>
        /// Get all group customers by GroupId
        /// </summary>
        [HttpGet("GetGroupCustomersByGroupId/{GroupId}")]
        [Produces("application/json", "application/xml", Type = typeof(List<DATA.Entity.GroupCustomer>))]
        public async Task<IActionResult> GetGroupCustomers(string GroupId)
        {
            var getAllGroupCustomers = new GetAllGroupCustomerByGroupIdQuery() { GroupId=GroupId};
            var result = await _mediator.Send(getAllGroupCustomers);
            return Ok(result);
        }

       

        /// <summary>
        /// Create a customer group
        /// </summary>
        [HttpPost("GroupCustomer/Add")]
        [Produces("application/json", "application/xml", Type = typeof(CreateGroupCustomer))]
        public async Task<IActionResult> AddGroupCustomer(AddGroupCustomerCommand addCustomerGroupCommand)
        {
            var result = await _mediator.Send(addCustomerGroupCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update group customer by CustomerId
        /// </summary>
        [HttpPut("GroupCustomer/{GroupCustomerId}")]
        [Produces("application/json", "application/xml", Type = typeof(UpdateGroupCustomer))]
        public async Task<IActionResult> UpdateCustomer(string GroupCustomerId, UpdateGroupCustomerCommand updateGroupCustomerCommand)
        {
            updateGroupCustomerCommand.Id = GroupCustomerId;
            var result = await _mediator.Send(updateGroupCustomerCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete Group Customer by CustomerId
        /// </summary>
        [HttpDelete("GroupCustomer/{CustomerId}")]
        public async Task<IActionResult> DeleteGroupCustomer(string Id)
        {
            var deleteGroupCustomerCommand = new DeleteGroupCustomerCommand { Id = Id };
            var result = await _mediator.Send(deleteGroupCustomerCommand);
            return ReturnFormattedResponse(result);
        }

       
    }
}

