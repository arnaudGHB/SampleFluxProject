using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Dto.Resources;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanMediaR.Commands;
using CBS.NLoan.MediatR.LoanMediaR.Queries;
using CBS.NLoan.MediatR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.LoanP
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class LoanController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Loan
        /// </summary>
        /// <param name="mediator"></param>
        public LoanController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Loan By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Loan/{id}", Name = "GetLoan")]
        [Produces("application/json", "application/xml", Type = typeof(LoanDto))]
        public async Task<IActionResult> GetLoan(string id)
        {
            var getLoanQuery = new GetLoanQuery { Id = id };
            var result = await _mediator.Send(getLoanQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves a lighter version of all open loans for a specified branch.
        /// </summary>
        /// <param name="id">The unique identifier of the branch.</param>
        /// <returns>A formatted response containing the list of open loans.</returns>

        [HttpGet("Loan/all-opened-loans-by-branch/lighter-version/{id}", Name = "GetLighterLoanByBranch")]
        [Produces("application/json", "application/xml", Type = typeof(LightLoanDto))]
        public async Task<IActionResult> GetLighterLoanByBranch(string id)
        {
            var getOpenLoansByBranchQuery = new GetOpenLoansByBranchQuery { BranchId = id };
            var result = await _mediator.Send(getOpenLoansByBranchQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Disburse loan using loan id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Loan/Disbursed", Name = "LoanDisbursed")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> LoanDisbursed(AddLoanDisbumentCommand addLoanDisbumentCommand)
        {
            var result = await _mediator.Send(addLoanDisbumentCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get paginated loans using DataTable
        /// </summary>
        /// <param name="query">Query containing DataTable options and filters</param>
        /// <returns>A paginated list of loans</returns>
        [HttpPost("Loan/Paggination-DataTable", Name = "GetLoansDataTable")]
        [Produces("application/json", "application/xml", Type = typeof(CustomDataTable))]
        public async Task<IActionResult> GetLoansDataTable([FromBody] GetLoansDataTableQuery query)
        {
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Endpoint to search and retrieve paginated loans based on search criteria.
        /// </summary>
        /// <param name="query">The search query containing search criteria and pagination details.</param>
        /// <returns>A paginated list of customers.</returns>
        [HttpGet("Loan/SearchByAnyCriterialQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanList>))]
        public async Task<IActionResult> SearchByAnyCriterialQuery([FromQuery] LoanResource loanResource)
        {
            var searchByAnyCriterialQuery = new SearchByAnyCriterialQuery { LoanResource = loanResource };
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
        /// Retrieves loans for multiple customers based on specified criteria.
        /// Supports filtering by loan status or other attributes through the QueryParameter.
        /// </summary>
        /// <param name="getLoansByMultipleCustomersQuery">
        /// Query object containing the list of CustomerIds and an optional QueryParameter for filtering loans.
        /// </param>
        /// <returns>
        /// A formatted response containing a list of loans matching the specified criteria.
        /// The response format can be JSON or XML, as specified by the client.
        /// </returns>
        [HttpGet("Loan/GetLoansByMultipleCustomersQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanList>))]
        public async Task<IActionResult> SearchByAnyCriterialQuery([FromQuery] GetLoansByMultipleCustomersQuery getLoansByMultipleCustomersQuery)
        {
            // Send the query to the mediator for processing
            var result = await _mediator.Send(getLoansByMultipleCustomersQuery);

            // Format and return the response based on the result
            return ReturnFormattedResponse(result);
        }

        //GetLoansByMultipleCustomersQuery
        /// <summary>
        /// Get Loan By customer id and Query parameter [Closed, Open, Pending, Disbursed]
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("Loan/GetAllLoanByCustomerId", Name = "GetAllLoanByCustomerIdQuery")]
        [Produces("application/json", "application/xml", Type = typeof(LoanDto))]
        public async Task<IActionResult> GetAllLoanByCustomerIdQuery(GetAllLoanByCustomerIdQuery byCustomerIdQuery)
        {
            var result = await _mediator.Send(byCustomerIdQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get Loans By branch id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Loan/GetAllLoanByBranchIdQuery/{id}", Name = "GetAllLoanByBranchIdQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanDto>))]
        public async Task<IActionResult> GetAllLoanByBranchIdQuery(string id)
        {
            var getLoanQuery = new GetAllLoanByBranchIdQuery { BranchId = id };
            var result = await _mediator.Send(getLoanQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get customer current loan by customer id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Loan/GetLoanByCustomerOpenLoanLightQuery/{id}", Name = "GetLoanByCustomerOpenLoanLightQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanDto>))]
        public async Task<IActionResult> GetLoanByCustomerOpenLoanLightQuery(string id)
        {
            var getLoanQuery = new GetLoanByCustomerOpenLoanLightQuery { CustomerId = id };
            var result = await _mediator.Send(getLoanQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Loans
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("Loans/GetAllLoans")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanDto>))]
        public async Task<IActionResult> GetLoans(GetAllLoanQuery getAllLoanQuery)
        {
            var result = await _mediator.Send(getAllLoanQuery);
            return Ok(result);
        }
       
       
    }
}
