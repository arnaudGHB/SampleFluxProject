using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.BankMGT.MediatR.Validations;
using CBS.CUSTOMER.API.Controllers.Base;
using CBS.CUSTOMER.DATA;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Resources;
using CBS.CUSTOMER.MEDIATR;
using CBS.CUSTOMER.MEDIATR.Customer.Command;
using CBS.CUSTOMER.MEDIATR.Customer.Queries;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace CBS.CUSTOMER.API.Controllers.Customer
{
    /// <summary>
    /// Controller for managing customers age category and related operations
    /// This CustomerDashboardController manages operations related to customers age category and their dependencies. 
    /// It includes methods for retrieving, creating, updating, and deleting customers age category, customer age category dependents, and customer requirements. 
    /// The controller is secured with authorization, and MediatR is used for handling queries and commands. 
    /// Each method has appropriate summaries for clarity.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class CustomerAgeCategoryController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the CustomerDashboardController
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public CustomerAgeCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get customer age category by id
        /// </summary>
        [HttpGet("CustomerAgeCategory/{id}", Name = "CustomerAgeCategoryDto")]
        [Produces("application/json", "application/xml", Type = typeof(CustomerAgeCategoryDto))]
        public async Task<IActionResult> GetCustomerAgeCategoryQuery(string id)
        {
            var getCustomerQuery = new GetCustomerAgeCategoryQuery { Id = id };
            var result = await _mediator.Send(getCustomerQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all customer age categories
        /// </summary>
        [HttpGet("CustomerAgeCategories")]
        [Produces("application/json", "application/xml", Type = typeof(List<CustomerAgeCategoryDto>))]
        public async Task<IActionResult> GetAllCustomerAgeCategoriesQuery()
        {
            var getAllCustomerQuery = new GetAllCustomerAgeCategoriesQuery { };
            var result = await _mediator.Send(getAllCustomerQuery);
            return Ok(result);
        }
        /// <summary>
        /// Add a new customer age category
        /// </summary>
        [HttpPost("CustomerAgeCategory/add")]
        [Produces("application/json", "application/xml", Type = typeof(CustomerAgeCategoryDto))]
        public async Task<IActionResult> AddCustomerAgeCategory([FromBody] AddCustomerAgeCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update an existing customer age category
        /// </summary>
        [HttpPut("CustomerAgeCategory/update")]
        [Produces("application/json", "application/xml", Type = typeof(CustomerAgeCategoryDto))]
        public async Task<IActionResult> UpdateCustomerAgeCategory([FromBody] UpdateCustomerAgeCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete a customer age category
        /// </summary>
        [HttpDelete("CustomerAgeCategory/delete/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> DeleteCustomerAgeCategory(string id)
        {
            var command = new DeleteCustomerAgeCategoryCommand { Id = id };
            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

    }
}


