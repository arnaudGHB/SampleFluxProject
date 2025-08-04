using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Commands;
using CBS.TransactionManagement.MediatR.SubTellerProvissioningP.Queries;
using CBS.TransactionManagement.MediatR.TellerP.Commands;
using CBS.TransactionManagement.MediatR.TellerP.Queries;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers
{
    /// <summary>
    /// Transaction
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class TellerController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Teller 
        /// </summary>
        /// <param name="mediator"></param>
        public TellerController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Teller By Id
        /// </summary>
        /// <param id="id"></param>
        /// <returns></returns>
        [HttpGet("Teller/{id}", Name = "GetTellerById")]
        [Produces("application/json", "application/xml", Type = typeof(TellerDto))]
        public async Task<IActionResult> GetTeller (string id)
        {
            var getTellerQuery = new GetTellerQuery { Id = id };
            var result = await _mediator.Send(getTellerQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get GetSubTellerProvioningHistoryQuery By user incharge
        /// </summary>
        /// <param UserInchargeId="UserInchargeId"></param>
        /// <returns></returns>
        [HttpGet("Teller/GetSubTellerProvisioningHistoryByUserIncharge/{id}", Name = "GetSubTellerProvisioningHistoryByUserIncharge")]
        [Produces("application/json", "application/xml", Type = typeof(SubTellerProvioningHistoryDto))]
        public async Task<IActionResult> GetSubTellerProvisioningHistoryByUserIncharge(string id)
        {
            var getSubTellerProvioning = new GetSubTellerProvioningHistoryByUserIDQuery { UserInchargeId = id };
            var result = await _mediator.Send(getSubTellerProvioning);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get subteller provision history by Id
        /// </summary>
        /// <param Id="Id"></param>
        /// <returns></returns>
        [HttpGet("Teller/GetSubTellerProvioningHistoryQuery/{id}", Name = "GetSubTellerProvioningHistoryQuery")]
        [Produces("application/json", "application/xml", Type = typeof(SubTellerProvioningHistoryDto))]
        public async Task<IActionResult> GetSubTellerProvioningHistoryQuery(string id)
        {
            var getSubTellerProvioning = new GetSubTellerProvioningHistoryQuery { Id = id };
            var result = await _mediator.Send(getSubTellerProvioning);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get subteller provision history by Id
        /// </summary>
        /// <param Id="Id"></param>
        /// <returns></returns>
        [HttpGet("Teller/GetSubTellerProvioningHistoryByPrimaryTellerUserIDQuery/{id}", Name = "GetSubTellerProvioningHistoryByPrimaryTellerUserIDQuery")]
        [Produces("application/json", "application/xml", Type = typeof(SubTellerProvioningHistoryDto))]
        public async Task<IActionResult> GetSubTellerProvioningHistoryByPrimaryTellerUserIDQuery(string id)
        {
            var getSubTellerProvioning = new GetSubTellerProvioningHistoryByPrimaryTellerUserIDQuery { PrimaryTellerUSerID = id };
            var result = await _mediator.Send(getSubTellerProvioning);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get primary teller provisioning history By user incharge
        /// </summary>
        /// <param UserInchargeId="UserInchargeId"></param>
        /// <returns></returns>
        [HttpGet("Teller/GetPrimaryTellerProvisioningHistoryByUserIncharge/{id}", Name = "GetPrimaryTellerProvisioningHistoryByUserIncharge")]
        [Produces("application/json", "application/xml", Type = typeof(PrimaryTellerProvisioningHistoryDto))]
        public async Task<IActionResult> GetPrimaryTellerProvisioningHistoryByUserIncharge(string id)
        {
            var getPrimaryTellerProvisioning = new GetPrimaryTellerProvisioningHistoryByUserIDQuery { UserInchargeId = id };
            var result = await _mediator.Send(getPrimaryTellerProvisioning);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get primary teller provisioning history By id
        /// </summary>
        /// <param Id="Id"></param>
        /// <returns></returns>
        [HttpGet("Teller/GetPrimaryTellerProvisioningHistoryQuery/{id}", Name = "GetPrimaryTellerProvisioningHistoryQuery")]
        [Produces("application/json", "application/xml", Type = typeof(PrimaryTellerProvisioningHistoryDto))]
        public async Task<IActionResult> GetPrimaryTellerProvisioningHistoryQuery(string id)
        {
            var getPrimaryTellerProvisioning = new GetPrimaryTellerProvisioningHistoryQuery { Id = id };
            var result = await _mediator.Send(getPrimaryTellerProvisioning);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get primary teller provisioning history By id
        /// </summary>
        /// <param Id="Id"></param>
        /// <returns></returns>
        [HttpGet("Teller/GetPrimaryTellerProvisioningHistoryByBranchIDQuery/{id}", Name = "GetPrimaryTellerProvisioningHistoryByBranchIDQuery")]
        [Produces("application/json", "application/xml", Type = typeof(PrimaryTellerProvisioningHistoryDto))]
        public async Task<IActionResult> GetPrimaryTellerProvisioningHistoryByBranchIDQuery(string id)
        {
            var getPrimaryTellerProvisioning = new GetPrimaryTellerProvisioningHistoryByBranchIDQuery { BranchId = id };
            var result = await _mediator.Send(getPrimaryTellerProvisioning);
            return ReturnFormattedResponse(result);
        }
        //
        /// <summary>
        /// Get all sub tellers provisions history
        /// </summary>
        /// <returns></returns>
        [HttpGet("Teller/GetAllSubTellerProvioningHistoryQuery", Name = "GetAllSubTellerProvioningHistoryQuery")]
        [Produces("application/json", "application/xml", Type = typeof(SubTellerProvioningHistoryDto))]
        public async Task<IActionResult> GetAllSubTellerProvioningHistoryQuery()
        {
            var getPrimaryTellerProvisioning = new GetAllSubTellerProvioningHistoryQuery { };
            var result = await _mediator.Send(getPrimaryTellerProvisioning);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get all primary tellers provisions history between dates
        /// </summary>
        /// <returns></returns>
        [HttpGet("Teller/GetAllPrimaryTellerProvisioningHistoryBetweenDatesQuery", Name = "GetAllPrimaryTellerProvisioningHistoryBetweenDatesQuery")]
        [Produces("application/json", "application/xml", Type = typeof(PrimaryTellerProvisioningHistoryDto))]
        public async Task<IActionResult> GetAllPrimaryTellerProvisioningHistoryBetweenDatesQuery()
        {
            var getPrimaryTellerProvisioning = new GetAllPrimaryTellerProvisioningHistoryBetweenDatesQuery { };
            var result = await _mediator.Send(getPrimaryTellerProvisioning);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// EOD by accountant
        /// </summary>
        /// <param name="addTellerCommand"></param>
        /// <returns></returns>
        [HttpPost("Teller/EndOfDayAccountant", Name = "EndOfDayAccountantCommand")]
        [Produces("application/json", "application/xml", Type = typeof(TellerDto))]
        public async Task<IActionResult> EndOfDayAccountant(EndOfDayAccountantCommand addTellerCommand)
        {
            var result = await _mediator.Send(addTellerCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Endpoint to retrieve the opening and closing information of tellers based on the specified criteria.
        /// This method is primarily used by accountants to get the End of Day (EOD) report for tellers,
        /// including details on cash replenishments, opening and closing balances, and other related data.
        /// The data can be filtered by branch and/or date range.
        /// </summary>
        /// <param name="getTellerOpenningAndClossing">
        /// An instance of the <see cref="GetTellerOpenningAndClossingQuery"/> class, containing the 
        /// filtering criteria such as branch, date range, and whether to filter by branch.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a JSON or XML formatted response. 
        /// The response includes a list of <see cref="OpenningAnclClossingTillDto"/> objects, representing the teller 
        /// opening and closing details based on the criteria provided.
        /// </returns>
        [HttpPost("Teller/TellerOpenningAndClossingQuery", Name = "GetTellerOpenningAndClossingQuery")]
        [Produces("application/json", "application/xml", Type = typeof(OpenningAnclClossingTillDto))]
        public async Task<IActionResult> GetTellerOpenningAndClossingQuery(GetTellerOpenningAndClossingQuery getTellerOpenningAndClossing)
        {
            // Send the query to the mediator to process the request and get the result
            var result = await _mediator.Send(getTellerOpenningAndClossing);

            // Return the result in the appropriate format (JSON/XML) based on the client's request
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves the opening and closing information for tills based on the specified criteria.
        /// This endpoint provides the End of Day (EOD) report for tills, including details on cash replenishments, 
        /// opening and closing balances, and related data. It supports filtering by branch and/or date range.
        /// </summary>
        /// <param name="getTillStatusQuery">
        /// An instance of the <see cref="GetTillStatusQuery"/> class, which contains filtering criteria such as 
        /// branch identifier, date range, and whether to apply branch-based filtering.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> with a JSON or XML response. The response contains a list of
        /// <see cref="TillOpenAndCloseOfDayDto"/> objects, representing the detailed opening and closing information 
        /// for each till based on the provided query criteria.
        /// </returns>
        [HttpPost("Teller/TillStatus", Name = "GetTillStatusQuery")]
        [Produces("application/json", "application/xml", Type = typeof(TillOpenAndCloseOfDayDto))]
        public async Task<IActionResult> GetTillStatusQuery(GetTillStatusQuery getTillStatusQuery)
        {
            // Send the query to the mediator to process and retrieve the results
            var result = await _mediator.Send(getTillStatusQuery);
            // Return the result formatted as JSON or XML based on the client's request
            return ReturnFormattedResponse(result);
        }

        //EndOfDayAccountantCommand
        /// <summary>
        /// Get all sub tellers provisions history between dates
        /// </summary>
        /// <returns></returns>
        [HttpGet("Teller/GetAllSubTellerProvioningHistoryBetweenDatesQuery", Name = "GetAllSubTellerProvioningHistoryBetweenDatesQuery")]
        [Produces("application/json", "application/xml", Type = typeof(SubTellerProvioningHistoryDto))]
        public async Task<IActionResult> GetAllSubTellerProvioningHistoryBetweenDatesQuery()
        {
            var getPrimaryTellerProvisioning = new GetAllSubTellerProvioningHistoryBetweenDatesQuery { };
            var result = await _mediator.Send(getPrimaryTellerProvisioning);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get all primary tellers provisions history
        /// </summary>
        /// <returns></returns>
        [HttpGet("Teller/GetAllPrimaryTellerProvisioningHistoryQuery", Name = "GetAllPrimaryTellerProvisioningHistoryQuery")]
        [Produces("application/json", "application/xml", Type = typeof(PrimaryTellerProvisioningHistoryDto))]
        public async Task<IActionResult> GetAllPrimaryTellerProvisioningHistoryQuery()
        {
            var getPrimaryTellerProvisioning = new GetAllPrimaryTellerProvisioningHistoryQuery { };
            var result = await _mediator.Send(getPrimaryTellerProvisioning);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get the current daily operation account for the day
        /// </summary>
        /// <returns></returns>
        [HttpGet("Teller/CurrentOpenOfDayHistory", Name = "CurrentOpenOfDayHistory")]
        [Produces("application/json", "application/xml", Type = typeof(CurrentProvisionPrimaryTellerDto))]
        public async Task<IActionResult> CurrentOpenOfDayHistory()
        {
            var getTellerQuery = new GetCurrentProvisionPrimaryTellerQuery { };
            var result = await _mediator.Send(getTellerQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Tellers 
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Teller")]
        [Produces("application/json", "application/xml", Type = typeof(List<TellerDto>))]
        public async Task<IActionResult> GetTeller()
        {
            var getAllTellerQuery = new GetAllTellerQuery { };
            var result = await _mediator.Send(getAllTellerQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Teller 
        /// </summary>
        /// <param name="addTellerCommand"></param>
        /// <returns></returns>
        [HttpPost("Teller")]
        [Produces("application/json", "application/xml", Type = typeof(TellerDto))]
        public async Task<IActionResult> AddTeller (AddTellerCommand addTellerCommand)
        {
                var result = await _mediator.Send(addTellerCommand);
                return ReturnFormattedResponse(result);               
        }
        /// <summary>
        /// Update sub teller provision by primary teller.
        /// </summary>
        /// <param name="addTellerCommand"></param>
        /// <returns></returns>
        [HttpPost("Teller/EndOfDaySubTellerBYPrimaryTeller")]
        [Produces("application/json", "application/xml", Type = typeof(SubTellerProvioningHistoryDto))]
        public async Task<IActionResult> EndOfDaySubTellerBYPrimaryTeller(EndOfDayBySubTellerIDCommand addTellerCommand)
        {
            var result = await _mediator.Send(addTellerCommand);
            return ReturnFormattedResponse(result);
        }
        //EndOfDayBySubTellerIDCommand
        /// <summary>
        /// Update Teller By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateTellerCommand"></param>
        /// <returns></returns>
        [HttpPut("Teller/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(TellerDto))]
        public async Task<IActionResult> UpdateTeller (string Id,UpdateTellerCommand updateTellerCommand)
        {
            updateTellerCommand.Id = Id;
            var result = await _mediator.Send(updateTellerCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Update MobileMoney Teller Configuration
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="mobileMoneyTellerConfigurationCommand"></param>
        /// <returns></returns>
        [HttpPut("Teller/MobileMoney/Configuration/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(TellerDto))]
        public async Task<IActionResult> MobileMoneyTellerConfigurationCommand(string Id, MobileMoneyTellerConfigurationCommand mobileMoneyTellerConfigurationCommand)
        {
            mobileMoneyTellerConfigurationCommand.Id = Id;
            var result = await _mediator.Send(mobileMoneyTellerConfigurationCommand);
            return ReturnFormattedResponse(result);
        }
        [HttpPost("Teller/Dinomination/Provisioning/PrimaryTeller")]
        [Produces("application/json", "application/xml", Type = typeof(TellerDto))]
        public async Task<IActionResult> ProvisionPrimaryTeller(ProvisionPrimaryTellerCommand provisionPrimaryTellerCommand)
        {
            var result = await _mediator.Send(provisionPrimaryTellerCommand);
            return ReturnFormattedResponse(result);
        }

        [HttpPost("Teller/SubTeller/EndOfDay")]
        [Produces("application/json", "application/xml", Type = typeof(SubTellerProvioningHistoryDto))]
        public async Task<IActionResult> SubTellerEndOfDay(EndOfDaySubTellerCommand endTellerDayCommand)
        {
            var result = await _mediator.Send(endTellerDayCommand);
            return ReturnFormattedResponse(result);
        }
        [HttpPost("Teller/PrimaryTeller/EndOfDay")]
        [Produces("application/json", "application/xml", Type = typeof(PrimaryTellerProvisioningHistoryDto))]
        public async Task<IActionResult> PrimaryTellerEndOfDay(EndOfDayPrimaryTellerCommand endTellerDayCommand)
        {
            var result = await _mediator.Send(endTellerDayCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Openning of operation day by the sub teller.
        /// </summary>
        /// <param name="setInitialAmountCommand"></param>
        /// <returns></returns>
        [HttpPost("Teller/SubTeller/OpenningOfTheDay")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> ProvisionSubTeller(OpenningOfDaySubTellerCommand setInitialAmountCommand)
        {
            var result = await _mediator.Send(setInitialAmountCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Openning of operation day by the primary teller.
        /// </summary>
        /// <param name="openningOfDayPrimary"></param>
        /// <returns></returns>
        [HttpPost("Teller/Primary/OpenningOfTheDay")]
        [Produces("application/json", "application/xml", Type = typeof(TellerDto))]
        public async Task<IActionResult> OpenningOfDayPrimaryTellerCommand(OpenningOfDayPrimaryTellerCommand openningOfDayPrimary)
        {
            var result = await _mediator.Send(openningOfDayPrimary);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Sub-teller cash requisition validation.
        /// </summary>
        /// <param name="validateCashReplenishment"></param>
        /// <returns></returns>
        [HttpPost("Teller/SubTeller/ValidateCashReplenishment")]
        [Produces("application/json", "application/xml", Type = typeof(SubTellerCashReplenishmentDto))]
        public async Task<IActionResult> ValidateCashReplenishmentSubTellerCommand(ValidateCashReplenishmentSubTellerCommand validateCashReplenishment)
        {
            var result = await _mediator.Send(validateCashReplenishment);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Teller  By Id
        /// </summary>
        /// <param id="id"></param>
        /// <returns></returns>
        [HttpDelete("Teller/{id}")]
        public async Task<IActionResult> DeleteTeller (string id)
        {
            var deleteTellerCommand = new DeleteTellerCommand { Id = id };
            var result = await _mediator.Send(deleteTellerCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
