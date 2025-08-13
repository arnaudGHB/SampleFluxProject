using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.SalaryFilesDto.SalaryFiles;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.FileUploadP.Queries;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Commands;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.SalaryManagement.SalaryAnalysisP
{
    /// <summary>
    /// Handles Salary Analysis operations including analyzing, retrieving, and deleting salary analysis results.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize] // Restrict access to authorized users
    public class SalaryAnalysisController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryAnalysisController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for sending commands and queries.</param>
        public SalaryAnalysisController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a salary analysis result by FileUploadId.
        /// </summary>
        /// <param name="fileUploadId">The unique identifier of the uploaded file.</param>
        /// <returns>The salary analysis result.</returns>
        [HttpGet("by-file-upload-id/{fileUploadId}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(ServiceResponse<SalaryAnalysisResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByFileUploadId(string fileUploadId)
        {
            var query = new GetSalaryAnalysisResultQueryByFileUploadId { FileUploadId = fileUploadId };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves a salary analysis result by Id.
        /// </summary>
        /// <param name="id">The unique identifier of the salary analysis result.</param>
        /// <returns>The salary analysis result.</returns>
        [HttpGet("{id}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(ServiceResponse<SalaryAnalysisResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(string id)
        {
            var query = new GetSalaryAnalysisResultQueryById { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Deletes a salary analysis result by Id.
        /// </summary>
        /// <param name="id">The unique identifier of the salary analysis result to delete.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        [HttpDelete("{id}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            var command = new DeleteSalaryAnalysisResultCommand { Id = id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Analyzes a salary file to compute loan repayments, standing order allocations, charges, and remaining salary for members.
        /// </summary>
        /// <param name="command">The command containing the file upload ID and related information for analysis.</param>
        /// <returns>The salary analysis result.</returns>
        [HttpPost("analyze")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(ServiceResponse<SalaryAnalysisResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Analyze([FromBody] SalaryAnalysisCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieves salary upload records for a specific branch based on the provided file ID.
        /// </summary>
        /// <param name="fileid">The ID of the file used to filter salary upload records.</param>
        /// <returns>A formatted response containing a list of salary upload records, or a 404 status if no records are found.</returns>
        [HttpGet("get-salary-model-for-branch-by-fileid/{fileid}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(List<SalaryUploadModelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFileUploadQuery(string fileid)
        {
            var query = new GetSalaryModelForAnalysisQueryByFileId { FileId = fileid };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }


    }

}


