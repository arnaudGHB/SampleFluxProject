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
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using CBS.TransactionManagement.Data.Dto.DailyStatisticBoard;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace CBS.CUSTOMER.API.Controllers.Dashboard
{
    /// <summary>
    /// Controller responsible for managing customer statistics and related dashboard operations.
    /// The CustomerDashboardController provides methods for retrieving customer statistics data, 
    /// specifically designed for dashboard insights. It uses MediatR for handling the GetAllCustomerStatisticsQuery 
    /// to ensure a clean separation of concerns. 
    /// The controller is secured with authorization to ensure only authorized users can access the data.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class CustomerDashboardController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the CustomerDashboardController with the specified mediator.
        /// </summary>
        /// <param name="mediator">The mediator used for sending queries and commands to their respective handlers.</param>
        public CustomerDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves customer statistics data for all members or filtered by branch, based on the provided parameters.
        /// </summary>
        /// <param name="getAllGeneralDailyDashboardQuery">Contains parameters for querying customer statistics, 
        /// including options for date range and branch filtering.</param>
        /// <returns>A formatted response containing customer statistics data, organized by branch if specified.</returns>
        [HttpPost("Dashboard/GetAllMembersDashboard", Name = "GetAllCustomerStatisticsQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<CustomerStatisticsDto>))]
        public async Task<IActionResult> GetAllGeneralDailyDashboard([FromBody] GetAllCustomerStatisticsQuery getAllGeneralDailyDashboardQuery)
        {
            var result = await _mediator.Send(getAllGeneralDailyDashboardQuery);
            return ReturnFormattedResponse(result);
        }
    }
}


