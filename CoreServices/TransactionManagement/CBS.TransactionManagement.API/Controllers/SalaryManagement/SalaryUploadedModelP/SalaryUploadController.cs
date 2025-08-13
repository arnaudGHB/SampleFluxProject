using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Data.SalaryFilesDto.SalaryFiles;
using CBS.TransactionManagement.FileUploadP.Queries;
using CBS.TransactionManagement.MediatR.FeeMediaR.FileUploadP.Commands;
using CBS.TransactionManagement.MediatR.FileUploadP.Queries;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Commands;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.SalaryManagement.SalaryUploadedModelP
{
    /// <summary>
    /// Handles Salary Upload operations including creation, retrieval, and deletion.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize] // Restrict access to authorized users only
    public class SalaryUploadController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryUploadController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for sending commands and queries.</param>
        public SalaryUploadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves Salary Upload records based on filters.
        /// </summary>
        /// <param name="date">Optional filter by date.</param>
        /// <param name="fileUploadId">Optional filter by AttachedFiles Upload ID.</param>
        /// <param name="salaryCode">Optional filter by Salary Code.</param>
        /// <returns>List of Salary Upload records.</returns>
        [HttpGet("all")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(List<SalaryUploadModelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSalaryUploads([FromQuery] DateTime? date, [FromQuery] string? fileUploadId, [FromQuery] string? salaryCode)
        {
            var query = new GetSalaryUploadModelQuery { Date = date, FileUploadId = fileUploadId, SalaryCode = salaryCode };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Deletes a Salary Upload record by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the Salary Upload to delete.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        [HttpDelete("{id}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteSalaryUpload(string id)
        {
            var command = new DeleteSalaryUploadModelCommand { FileUploadId = id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Creates a new Salary Upload record.
        /// </summary>
        /// <param name="command">The command to create a new Salary Upload.</param>
        /// <returns>Details of the created Salary Upload record.</returns>
        [HttpPost("create")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(SalaryUploadModelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSalaryUpload([FromBody] AddSalaryUploadModelCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves Salary Upload records based on filters.
        /// </summary>
        /// <param name="date">Optional filter by date.</param>
        /// <param name="fileUploadId">Optional filter by AttachedFiles Upload ID.</param>
        /// <param name="salaryCode">Optional filter by Salary Code.</param>
        /// <returns>List of Salary Upload records.</returns>
        [HttpGet("all/{fileCategory}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(List<FileUploadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllFileUploadQuery(string fileCategory)
        {
            var query = new GetAllFileUploadQuery { FileCategory=fileCategory };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves AttachedFiles Uploaded records based on file id.
        /// </summary>
        /// <param name="fileUploadId">Optional filter by AttachedFiles Upload ID.</param>
        /// <returns>List of Salary Upload records.</returns>
        [HttpGet("get-file-upload-byid/{fileid}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(List<FileUploadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFileUploadQuery(string fileid)
        {
            var query = new GetFileUploadQuery { Id=fileid };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Uploads a new Salary Upload file and processes it.
        /// </summary>
        /// <param name="uploadCommand"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        [Produces("application/json", "application/xml", Type = typeof(List<SalaryExtractDto>))]
        public async Task<IActionResult> UploadFile([FromForm] AddSalaryUploadModelCommand uploadCommand)
        {
            var result = await _mediator.Send(uploadCommand);
            return ReturnFormattedResponse(result);

        }

        /// <summary>
        /// Retrieves all salary file uploads filtered by their activation status.
        /// </summary>
        /// <param name="status">The activation status to filter salary files (true for activated, false for deactivated).</param>
        /// <param name="both">Whether to include both activated and deactivated files (true includes both).</param>
        /// <returns>A formatted response containing a list of salary file uploads with the specified status.</returns>
        [HttpGet("salary-file-uploaded")]
        [Produces("application/json", "application/xml", Type = typeof(List<SalaryExtractDto>))]
        public async Task<IActionResult> GetAllFileUploadSalaryFileActivatedQuery([FromQuery] bool Status, [FromQuery] bool Both)
        {
            var getAllFileUpload = new GetAllFileUploadSalaryFileActivatedQuery { Status = Status, Both = Both };
            var result = await _mediator.Send(getAllFileUpload);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Activates or deactivates a salary file by updating its status.
        /// </summary>
        /// <param name="fileCommand">The command containing the salary file ID and the activation status (true or false).</param>
        /// <returns>Returns a formatted response indicating the success or failure of the operation.</returns>
        [HttpPut("Activate-salary-file")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> ActivateSalaryFileCommand(ActivateSalaryFileCommand fileCommand)
        {
            var result = await _mediator.Send(fileCommand);
            return ReturnFormattedResponse(result);

        }

        /// <summary>
        /// Download Salary AttachedFiles by AttachedFiles Id
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("download-salay-file/{fileId}")]
        [Produces("application/json", "application/xml", Type = typeof(FileDownloadDto))]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            var query = new DownloadFileByIdQuery(fileId);
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
    }

}


