using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// Account
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ChartOfAccountManagementPositionController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Account
        /// </summary>
        /// <param name="mediator"></param>
        public ChartOfAccountManagementPositionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get ChartOfAccountManagementPosition By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("ChartOfAccountManagementPosition/{Id}", Name = "GetChartOfAccountManagementPosition")]
        [Produces("application/json", "application/xml", Type = typeof(ChartOfAccountManagementPositionDto))]
        public async Task<IActionResult> GetChartOfAccountManagementPositionManagementPosition(string Id)
        {
            var getAccountQuery = new GetChartOfAccountManagementPositionQuery { Id = Id };
            var result = await _mediator.Send(getAccountQuery);
            return ReturnFormattedResponse(result);
        }

        //
        /// <summary>
        /// Get All ChartOfAccountManagementPosition
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("ChartOfAccountManagementPositions")]
        [Produces("application/json", "application/xml", Type = typeof(List<ChartOfAccountManagementPositionDto>))]
        public async Task<IActionResult> GetAllChartOfAccountManagementPositionQuery()
        {
            var getAllAccountQuery = new GetAllChartOfAccountManagementPositionQuery { };
            var result = await _mediator.Send(getAllAccountQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get All ChartOfAccountManagementPosition
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("ChartOfAccountManagementPositions/DownloadQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<ChartOfAccountManagementPositionDto>))]
        public async Task<IActionResult> DownloadQuery()
        {
            var getAllAccountQuery = new DownloadQuery { };
            var result = await _mediator.Send(getAllAccountQuery);
            return Ok(result);
        }

        /// <summary>
        /// Create a ChartOfAccountManagementPosition
        /// </summary>
        /// <param name="addChartOfAccountManagementPositionCommand"></param>
        /// <returns></returns>
        [HttpPost("ChartOfAccountManagementPosition")]
        [Produces("application/json", "application/xml", Type = typeof(ChartOfAccountManagementPositionDto))]
        public async Task<IActionResult> AddAccount(AddChartOfAccountManagementPositionCommand addChartOfAccountManagementPositionCommand)
        {
            var result = await _mediator.Send(addChartOfAccountManagementPositionCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// upload account from file
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        [HttpPost("ChartOfAccountManagementPosition/UploadAccountForm")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UploadMOdelAccount(IFormFile formData)
        {
            UploadChartOfAccountManagementPositionCommand UploadAccountCommand = new UploadChartOfAccountManagementPositionCommand();
            UploadAccountCommand.ChartOfAccountManagementPositionFile = UploadAccountCommand.UploadAccountQueryModel(formData);

            var result = await _mediator.Send(UploadAccountCommand);
            return ReturnFormattedResponseObject(result);
        }


        /// <summary>
        /// upload account from file
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        [HttpPost("ChartOfAccountManagementPosition/UploadChartOfAccountMFI")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UploadChartOfAccount(IFormFile formData)
        {
            UploadChartOfAccountMFICommand UploadAccountCommand = new UploadChartOfAccountMFICommand();
            UploadAccountCommand.ChartOfAccountManagementPositionFile = UploadAccountCommand.UploadAccountQueryModel(formData);

            var result = await _mediator.Send(UploadAccountCommand);
            return ReturnFormattedResponseObject(result);
        }

        /// <summary>
        /// Update ChartOfAccountManagementPosition By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateAccountCommand"></param>
        /// <returns></returns>
        [HttpPut("ChartOfAccountManagementPosition/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(ChartOfAccountManagementPositionDto))]
        public async Task<IActionResult> UpdateChartOfAccountManagementPosition(string Id, UpdateChartOfAccountManagementPositionCommand updateAccountCommand)
        {
            updateAccountCommand.Id = Id;
            var result = await _mediator.Send(updateAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// <summary>
        /// Get All ChartOfAccountManagementPosition
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("ChartOfAccountManagementPosition/GetAllChartOfAccounts")]
        [Produces("application/json", "application/xml", Type = typeof(List<ChartOfAccountManagementPositionDto>))]
        public async Task<IActionResult> GetAllChartOfAccountsQueryHandler()
        {
            var getAllAccountQuery = new GetAllChartOfAccountMPsQuery { };
            var result = await _mediator.Send(getAllAccountQuery);
            return Ok(result);
        }
        /// <summary>
        /// <summary>
        /// Get All ChartOfAccountManagementPosition used by a branch
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("ChartOfAccountManagementPosition/GetAllChartOfAccountByBranchId/{branchId}")]
        [Produces("application/json", "application/xml", Type = typeof(List<ChartOfAccountManagementPositionDto>))]
        public async Task<IActionResult> GetAllChartOfAccountManagementPositionUsedByBranchQueryHandler(string branchId)
        {
            var getAllAccountQuery = new GetAllChartOfAccountMPUsedByBranchQuery { BranchId= branchId };
            var result = await _mediator.Send(getAllAccountQuery);
            return Ok(result);
        }

        /// <summary>
        /// Delete ChartOfAccountManagementPosition By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("ChartOfAccountManagementPosition/{Id}")]
        public async Task<IActionResult> DeleteAccount(string Id)
        {
            var deleteAccountCommand = new DeleteChartOfAccountManagementPositionCommand { Id = Id };
            var result = await _mediator.Send(deleteAccountCommand);
            return ReturnFormattedResponse(result);
        }
    }
}