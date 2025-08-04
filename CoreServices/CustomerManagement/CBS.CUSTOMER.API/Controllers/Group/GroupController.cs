using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.Customer.MEDIATR;
using CBS.Customer.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.API.Controllers.Base;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.Groups;
using CBS.CUSTOMER.DATA.Resources;
using CBS.CUSTOMER.MEDIATR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
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
    public class GroupController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the GroupController
        /// This GroupController manages operations related to groups .
        /// It includes methods for retrieving, creating, and updating groups.
        /// The controller is secured with authorization, and MediatR is used for handling queries and commands.
        /// Each method has appropriate summaries for clarity.
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public GroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get group by GroupId
        /// </summary>
        [HttpGet("Group/{GroupId}", Name = "GetGroup")]
        [Produces("application/json", "application/xml", Type = typeof(DATA.Entity.Group))]
        public async Task<IActionResult> GetGroup(string GroupId)
        {
            var getGroupQuery = new GetGroupQuery { GroupId = GroupId };
            var result = await _mediator.Send(getGroupQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Endpoint to search and retrieve paginated customers based on search criteria.
        /// </summary>
        /// <param name="query">The search query containing search criteria and pagination details.</param>
        /// <returns>A paginated list of customers.</returns>
        [HttpGet("Group/SearchByAnyCriterialGroupQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<GroupsList>))]
        public async Task<IActionResult> SearchByAnyCriterialQuery([FromQuery] GroupResource customerResource)
        {
            var searchByAnyCriterialQuery = new SearchByAnyCriterialGroupQuery { ResourceParameter = customerResource };
            var result = await _mediator.Send(searchByAnyCriterialQuery);
            if (result.Data != null)
            {
                var paginationMetadata = new
                {
                    totalCount = result.Data.TotalCount,
                    pageSize = result.Data.PageSize,
                    skip = result.Data.Skip,
                    totalPages = result.Data.TotalPages
                };
                Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
            }
            // Assign pagination metadata to the PaginationMetadata property
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all groups
        /// </summary>
        [HttpGet("Groups")]
        [Produces("application/json", "application/xml", Type = typeof(List<DATA.Entity.Group>))]
        public async Task<IActionResult> GetGroups()
        {
            var getAllGroupQuery = new GetAllGroupQuery();
            var result = await _mediator.Send(getAllGroupQuery);
            return Ok(result);
        }

     

        /// <summary>
        /// Create a group
        /// </summary>
        [HttpPost("Group/Add")]
        [Produces("application/json", "application/xml", Type = typeof(CreateGroup))]
        public async Task<IActionResult> AddGroup(AddGroupCommand addGroupCommand)
        {
            var result = await _mediator.Send(addGroupCommand);
            return ReturnFormattedResponse(result);
        }

      
        
        /// <summary>
        /// Update group by CustomerId
        /// </summary>
        [HttpPut("Group/{GroupId}")]
        [Produces("application/json", "application/xml", Type = typeof(UpdateGroup))]
        public async Task<IActionResult> UpdateGroup(string GroupId, UpdateGroupCommand updateGroupCommand)
        {
            updateGroupCommand.GroupId = GroupId;
            var result = await _mediator.Send(updateGroupCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete Group by CustomerId
        /// </summary>
        [HttpDelete("Group/{CustomerId}")]
        public async Task<IActionResult> DeleteGroup(string Id)
        {
            var deleteGroupCommand = new DeleteGroupCommand { Id = Id };
            var result = await _mediator.Send(deleteGroupCommand);
            return ReturnFormattedResponse(result);
        }

    }
}

