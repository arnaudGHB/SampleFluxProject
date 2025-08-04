using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.Customer.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.API.Controllers.Base;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.Organization.MEDIATR;
using CBS.Organization.MEDIATR;
using MediatR;
using Microsoft.AspNetCore.Mvc;



namespace CBS.CUSTOMER.API.Controllers.Organization
{
    /// <summary>
    /// Controller for managing Organizations and related operations
    /// This OrganizationController manages operations related to Organizations and their customers.
    /// It includes methods for retrieving, creating, and updating Organizations and Organization customers.
    /// The controller is secured with authorization, and MediatR is used for handling queries and commands. 
    /// Each method has appropriate summaries for clarity.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class OrganizationController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the OrganizationController
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public OrganizationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Organization by OrganizationId
        /// </summary>
        [HttpGet("Organization/{OrganizationId}", Name = "GetOrganization")]
        [Produces("application/json", "application/xml", Type = typeof(DATA.Entity.Organization))]
        public async Task<IActionResult> GetOrganization(string OrganizationId)
        {
            var getOrganizationQuery = new GetOrganizationQuery { OrganizationId = OrganizationId };
            var result = await _mediator.Send(getOrganizationQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get Organization customer by OrganizationCustomerId
        /// </summary>
        [HttpGet("Organization/Customer/{OrganizationCustomerId}", Name = "GetOrganizationCustomer")]
        [Produces("application/json", "application/xml", Type = typeof(DATA.Entity.OrganizationCustomer))]
        public async Task<IActionResult> GetOrganizationCustomer(string OrganizationCustomerId)
        {
            var getOrganizationQuery = new GetOrganizationCustomerQuery { OrganizationCustomerId = OrganizationCustomerId };
            var result = await _mediator.Send(getOrganizationQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all Organizations
        /// </summary>
        [HttpGet("Organizations")]
        [Produces("application/json", "application/xml", Type = typeof(List<DATA.Entity.Organization>))]
        public async Task<IActionResult> GetOrganizations()
        {
            var getAllOrganizationQuery = new GetAllOrganizationQuery();
            var result = await _mediator.Send(getAllOrganizationQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get all Organization customers 
        /// </summary>
        [HttpGet("GetOrganizationCustomers")]
        [Produces("application/json", "application/xml", Type = typeof(List<DATA.Entity.OrganizationCustomer>))]
        public async Task<IActionResult> GetOrganizationCustomers()
        {
            var getAllOrganizationQuery = new GetAllOrganizationCustomerQuery();
            var result = await _mediator.Send(getAllOrganizationQuery);
            return Ok(result);
        }

        /// <summary>
        /// Create an Organization
        /// </summary>
        [HttpPost("Organization/Add")]
        [Produces("application/json", "application/xml", Type = typeof(CreateOrganization))]
        public async Task<IActionResult> AddOrganization(AddOrganizationCommand addOrganizationCommand)
        {
            var result = await _mediator.Send(addOrganizationCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a customer Organization
        /// </summary>
        [HttpPost("Organization/Customer/Add")]
        [Produces("application/json", "application/xml", Type = typeof(CreateOrganizationCustomer))]
        public async Task<IActionResult> AddOrganizationCustomer(AddOrganizationCustomerCommand addCustomerOrganizationCommand)
        {
            var result = await _mediator.Send(addCustomerOrganizationCommand);
            return ReturnFormattedResponse(result);
        }

      

        /// <summary>
        /// Update Organization by CustomerId
        /// </summary>
        [HttpPut("Organization/{CustomerId}")]
        [Produces("application/json", "application/xml", Type = typeof(UpdateOrganization))]
        public async Task<IActionResult> UpdateOrganization(string OrganizationId, UpdateOrganizationCommand updateOrganizationCommand)
        {
            updateOrganizationCommand.Id = OrganizationId;
            var result = await _mediator.Send(updateOrganizationCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update Organization customer by CustomerId
        /// </summary>
        [HttpPut("Organization/Customer/{OrganizationCustomerId}")]
        [Produces("application/json", "application/xml", Type = typeof(UpdateOrganizationCustomer))]
        public async Task<IActionResult> UpdateCustomer(string OrganizationCustomerId, UpdateOrganizationCustomerCommand updateOrganizationCustomerCommand)
        {
            updateOrganizationCustomerCommand.OrganizationCustomerId = OrganizationCustomerId;
            var result = await _mediator.Send(updateOrganizationCustomerCommand);
            return ReturnFormattedResponse(result);
        }
    }
}

