using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.Customer.MEDIATR;
using CBS.Customer.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.API.Controllers.Base;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CustomerSmsConfigurations.MEDIAT;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace CBS.CUSTOMER.API.Controllers.Sms
{
    /// <summary>
    /// Controller for managing CustomerSmsConfigurationss and related operations
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class CustomerSmsConfigurationsController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the CustomerSmsConfigurationsController
        /// This CustomerSmsConfigurationsController manages operations related to CustomerSmsConfigurationss and their customers.
        /// It includes methods for retrieving, creating, and updating CustomerSmsConfigurationss and CustomerSmsConfigurations customers.
        /// The controller is secured with authorization, and MediatR is used for handling queries and commands.
        /// Each method has appropriate summaries for clarity.
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public CustomerSmsConfigurationsController(IMediator mediator)
        {
            _mediator = mediator;
        }





        /// <summary>
        /// Get all CustomerSmsConfigurationss
        /// </summary>
        [HttpGet("CustomerSmsConfigurationss")]
        [Produces("application/json", "application/xml", Type = typeof(List<CreateCustomerSmsConfigurations>))]
        public async Task<IActionResult> GetCustomerSmsConfigurationss()
        {
            var getAllCustomerSmsConfigurationsQuery = new GetAllCustomerSmsConfigurationsQuery();
            var result = await _mediator.Send(getAllCustomerSmsConfigurationsQuery);
            return Ok(result);
        }



        /// <summary>
        /// Create a CustomerSmsConfigurations
        /// </summary>
        [HttpPost("CustomerSmsConfigurations/AddOrUpdate")]
        [Produces("application/json", "application/xml", Type = typeof(CreateCustomerSmsConfigurations))]
        public async Task<IActionResult> AddCustomerSmsConfigurations(AddCustomerSmsConfigurationsCommand addCustomerSmsConfigurations)
        {
            var result = await _mediator.Send(addCustomerSmsConfigurations);
            return ReturnFormattedResponse(result);
        }

        
    }
}

