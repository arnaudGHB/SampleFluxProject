//using CBS.APICaller.Helper;
//using CBS.LoanApplication.Dto;
//using CBS.LoanApplication.MediatR.Commands;
//using CBS.LoanApplication.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Commands;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Commands;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.LoanApplicationP
{
    /// <summary>
    /// LoanApplication
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class LoanApplicationController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanApplication
        /// </summary>
        /// <param name="mediator"></param>
        public LoanApplicationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanApplication By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanApplication/{LoanApplicationId}", Name = "GetLoanApplication")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationDto))]
        public async Task<IActionResult> GetLoanApplication(string LoanApplicationId)
        {
            var getLoanApplicationQuery = new GetLoanApplicationQuery { Id = LoanApplicationId };
            var result = await _mediator.Send(getLoanApplicationQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get LoanApplication By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanApplication/GetAllLoanApplicationByCustomerId/{id}", Name = "GetAllLoanApplicationByCustomerIdQuery")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationDto))]
        public async Task<IActionResult> GetAllLoanApplicationByCustomerIdQuery(string id)
        {
            var getLoanApplicationQuery = new GetAllLoanApplicationByCustomerIdQuery { CustomerId = id };
            var result = await _mediator.Send(getLoanApplicationQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get LoanApplication By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanApplication/Installment/{LoanApplicationId}", Name = "GetLoanApplicationInstallement")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationDto))]
        public async Task<IActionResult> GetLoanApplicationInstallment(string LoanApplicationId)
        {
            var getLoanApplicationQuery = new GetLoanInstallmentQuery { Id = LoanApplicationId };
            var result = await _mediator.Send(getLoanApplicationQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get paginated loan applications using DataTable
        /// </summary>
        /// <param name="query">Query containing DataTable options and filters</param>
        /// <returns>A paginated list of loan applications</returns>
        [HttpPost("LoanApplication/Paggination-DataTable", Name = "GetLoanApplicationsDataTable")]
        [Produces("application/json", "application/xml", Type = typeof(CustomDataTable))]
        public async Task<IActionResult> GetLoanApplicationsDataTable([FromBody] GetLoanApplicationsDataTableQuery query)
        {
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
        [HttpGet("LoanApplication/Paginated/{pageNumber}/{pageSize}", Name = "GetPaginatedLoanApplications")]
        [Produces("application/json", "application/xml", Type = typeof(PaginatedResult<LoanApplicationDto>))]
        public async Task<IActionResult> GetPaginatedLoanApplications(int pageNumber,int pageSize,[FromQuery] string paramMeter = "all",[FromQuery] string branchId = null,[FromQuery] string memberId = null,[FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var query = new GetPaginatedLoanApplicationsQuery(paramMeter, branchId, memberId, startDate, endDate, pageNumber, pageSize);
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All LoanApplications
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanApplications/Query/{param}")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanApplicationDto>))]
        public async Task<IActionResult> GetLoanApplications(string param)
        {
            var getAllLoanApplicationQuery = new GetAllLoanApplicationQuery {ParamMeter= param};
            var result = await _mediator.Send(getAllLoanApplicationQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanApplication
        /// </summary>
        /// <param name="addLoanApplicationCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanApplication")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationDto))]
        public async Task<IActionResult> AddLoanApplication(AddLoanApplicationCommand addLoanApplicationCommand)
        {
            var result = await _mediator.Send(addLoanApplicationCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Create a LoanApplication
        /// </summary>
        /// <param name="simulatioLoanCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanApplication/Simulation")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationDto))]
        public async Task<IActionResult> SimulateLoanApplication(SimulateLoanInstallementQuery simulatioLoanCommand)
        {
            var result = await _mediator.Send(simulatioLoanCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update LoanApplication By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateLoanApplicationCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanApplication/{LoanApplicationId}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationDto))]
        public async Task<IActionResult> UpdateLoanApplication(string LoanApplicationId, UpdateLoanApplicationCommand updateLoanApplicationCommand)
        {
            updateLoanApplicationCommand.Id = LoanApplicationId;
            var result = await _mediator.Send(updateLoanApplicationCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Update LoanApplication By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateLoanApplicationCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanApplication/ValidateLoan")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationDto))]
        public async Task<IActionResult> ValidateLoanApplication(ValidateLoanCommand validateLoanCommand)
        {
            var result = await _mediator.Send(validateLoanCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Update LoanApplication By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateLoanApplicationCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanApplication/ChangeStatus/{LoanApplicationId}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationDto))]
        public async Task<IActionResult> UpdateLoanApplicationStatus(string LoanApplicationId, AddLoanApprovalCommand updateLoanApplicationStatusCommand)
        {
            updateLoanApplicationStatusCommand.Id = LoanApplicationId;
            var result = await _mediator.Send(updateLoanApplicationStatusCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanApplication By Id
        /// </summary>
        /// <param name="LoanApplicationId"></param>
        /// <returns></returns>
        [HttpDelete("LoanApplication/{LoanApplicationId}")]
        public async Task<IActionResult> DeleteLoanApplication(string LoanApplicationId)
        {
            var deleteLoanApplicationCommand = new DeleteLoanApplicationCommand { Id = LoanApplicationId };
            var result = await _mediator.Send(deleteLoanApplicationCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
