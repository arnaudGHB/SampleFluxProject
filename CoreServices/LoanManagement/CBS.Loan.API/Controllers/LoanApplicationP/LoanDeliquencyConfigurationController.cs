//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Commands;
using CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.LoanApplicationP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class LoanDeliquencyConfigurationController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanDeliquencyConfiguration
        /// </summary>
        /// <param name="mediator"></param>
        public LoanDeliquencyConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanDeliquencyConfiguration By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanDeliquencyConfiguration/{Id}", Name = "GetLoanDeliquencyConfiguration")]
        [Produces("application/json", "application/xml", Type = typeof(LoanDeliquencyConfigurationDto))]
        public async Task<IActionResult> GetLoanDeliquencyConfiguration(string Id)
        {
            var getLoanDeliquencyConfigurationQuery = new GetLoanDeliquencyConfigurationQuery { Id = Id };
            var result = await _mediator.Send(getLoanDeliquencyConfigurationQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All LoanDeliquencyConfigurations
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanDeliquencyConfigurations")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanDeliquencyConfigurationDto>))]
        public async Task<IActionResult> GetLoanDeliquencyConfigurations()
        {
            var getAllLoanDeliquencyConfigurationQuery = new GetAllLoanDeliquencyConfigurationQuery { };
            var result = await _mediator.Send(getAllLoanDeliquencyConfigurationQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanDeliquencyConfiguration
        /// </summary>
        /// <param name="addLoanDeliquencyConfigurationCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanDeliquencyConfiguration")]
        [Produces("application/json", "application/xml", Type = typeof(LoanDeliquencyConfigurationDto))]
        public async Task<IActionResult> AddLoanDeliquencyConfiguration(AddLoanDeliquencyConfigurationCommand addLoanDeliquencyConfigurationCommand)
        {
            var result = await _mediator.Send(addLoanDeliquencyConfigurationCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update LoanDeliquencyConfiguration By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateLoanDeliquencyConfigurationCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanDeliquencyConfiguration/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanDeliquencyConfigurationDto))]
        public async Task<IActionResult> UpdateLoanDeliquencyConfiguration(string Id, UpdateLoanDeliquencyConfigurationCommand updateLoanDeliquencyConfigurationCommand)
        {
            updateLoanDeliquencyConfigurationCommand.Id = Id;
            var result = await _mediator.Send(updateLoanDeliquencyConfigurationCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanDeliquencyConfiguration By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("LoanDeliquencyConfiguration/{Id}")]
        public async Task<IActionResult> DeleteLoanDeliquencyConfiguration(string Id)
        {
            var deleteLoanDeliquencyConfigurationCommand = new DeleteLoanDeliquencyConfigurationCommand { Id = Id };
            var result = await _mediator.Send(deleteLoanDeliquencyConfigurationCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
