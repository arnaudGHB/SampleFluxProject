using CBS.CUSTOMER.API.Controllers.Base;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CBS.CUSTOMER.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class CustomerCategoryController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the CustomerDashboardController
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public CustomerCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Create a CustomerCategory
        /// </summary>
        [HttpPost("CustomerCategory")]
        [Produces("application/json", "application/xml", Type = typeof(AddCustomerCategoryCommand))]
        public async Task<IActionResult> AddCustomerCategory(AddCustomerCategoryCommand addCustomerCategoryCommand)
        {

            var result = await _mediator.Send(addCustomerCategoryCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Update CustomerCategory by CustomerId
        /// </summary>
        [HttpPut("CustomerCategory/{CustomerId}")]
        [Produces("application/json", "application/xml", Type = typeof(UpdateCustomerCategoryCommand))]
        public async Task<IActionResult> UpdateCustomerCategory(string Id, UpdateCustomerCategoryCommand updateCustomerCategoryCommand)
        {

            updateCustomerCategoryCommand.Id = Id;
            var result = await _mediator.Send(updateCustomerCategoryCommand);
            return ReturnFormattedResponse(result);
        }



        /// <summary>
        /// Get all CustomerCategory 
        /// </summary>
        [HttpGet("CustomerCategory")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetAllCustomerCategoryQuery>))]
        public async Task<IActionResult> GetAllCustomerCategoryLeaves()
        {
            var getAllCustomerCategoryLeave = new GetAllCustomerCategoryQuery();
            var result = await _mediator.Send(getAllCustomerCategoryLeave);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get CustomerCategory by id
        /// </summary>
        [HttpGet("CustomerCategory/{CustomerId}")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetCustomerCategoryQuery>))]
        public async Task<IActionResult> GetAllCustomerCategory(string Id)
        {
            var getAllJobTitle = new GetCustomerCategoryQuery { Id=Id };
            var result = await _mediator.Send(getAllJobTitle);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete CustomerCategory by id
        /// </summary>
        [HttpDelete("CustomerCategory/{CustomerId}")]
        [Produces("application/json", "application/xml", Type = typeof(List<DeleteCustomerCommand>))]
        public async Task<IActionResult> DeleteCategory(string Id)
        {
            var getAllJobTitle = new DeleteCustomerCommand { Id = Id };
            var result = await _mediator.Send(getAllJobTitle);
            return ReturnFormattedResponse(result);
        }


    }
}
