using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.BackGroundTasks;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Queries;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Commands;
using CBS.TransactionManagement.MediatR.UtilityServices;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.SalaryManagement.SalaryExecutionP
{
    /// <summary>
    /// Handles Salary Execution operations including upload, execution, and retrieval.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize] // Restrict access to authorized users only
    public class SalaryExecutionController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<SalaryExecutionController> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IAPIUtilityServicesRepository _aPIUtilityServicesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryExecutionController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for sending commands and queries.</param>
        public SalaryExecutionController(IMediator mediator, IBackgroundTaskQueue taskQueue, ILogger<SalaryExecutionController> logger, UserInfoToken userInfoToken, IServiceScopeFactory serviceScopeFactory, IAPIUtilityServicesRepository aPIUtilityServicesRepository)
        {
            _mediator = mediator;
            _taskQueue=taskQueue;
            _logger=logger;
            _userInfoToken=userInfoToken;
            _serviceScopeFactory=serviceScopeFactory;
            _aPIUtilityServicesRepository=aPIUtilityServicesRepository;
        }





        /// <summary>
        /// Uploads an analysed salary file for processing.
        /// </summary>
        /// <param name="uploadCommand">The command containing the analysed salary file.</param>
        /// <returns>A list of analysed salary extracts.</returns>
        [HttpPost("upload-analysed-file")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UploadAnalysedFile([FromForm] UploadAnalysedSalaryCommand uploadCommand)
        {
            var result = await _mediator.Send(uploadCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Executes salary processing for the given file upload ID.
        /// </summary>
        /// <param name="executeCommand">The command to execute salary processing.</param>
        /// <returns>Processed salary details.</returns>
        /// memberReferenceNumber
        //[HttpPost("execute-salary")]
        //[Produces("application/json", "application/xml", Type = typeof(SalaryProcessingDto))]
        //public async Task<IActionResult> ExecuteSalary([FromBody] ExecuteSalaryCommand executeCommand)
        //{
        //    var result = await _mediator.Send(executeCommand);
        //    return ReturnFormattedResponse(result);
        //}

        /// <summary>
        /// Initiates the salary processing request by enqueuing it as a background job.
        /// This allows the operation to run asynchronously, preventing UI blocking.
        /// Logs and audits each stage of the process for better traceability.
        /// </summary>
        /// <param name="executeCommand">The salary processing command containing salary file ID and processing details.</param>
        /// <returns>An HTTP response indicating that the request has been accepted and is processing in the background.</returns>

        [HttpPost("execute-salary")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> ExecuteSalary([FromBody] ExecuteSalaryCommand executeCommand)
        {
            // ✅ Generate a unique tracking ID for this execution
            string correlationId = Guid.NewGuid().ToString();
            executeCommand.UserInfoToken = _userInfoToken;


            // ✅ Step 3: Log and Audit Salary Processing Request
            string requestReceivedMessage = $"[INFO] Received salary processing request. CorrelationId: '{correlationId}', " +
                                            $"FileUploadId: '{executeCommand.FileUploadId}', RequestedBy: '{_userInfoToken.FullName}'.";
            _logger.LogInformation(requestReceivedMessage);
            await BaseUtilities.LogAndAuditAsync(requestReceivedMessage, executeCommand, HttpStatusCodeEnum.OK, LogAction.SalaryProcessing, LogLevelInfo.Information, _userInfoToken.FullName, _userInfoToken.Token, correlationId);

            // ✅ Step 4: Enqueue the Background Job
            _taskQueue.QueueBackgroundWorkItem(async token =>
            {
                using (var scope = _serviceScopeFactory.CreateScope()) // Create a new DI scope
                {
                    try
                    {
                        // Resolve Scoped Dependencies
                        var scopedMediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<SalaryExecutionController>>();
                        var scopedUserInfoToken = executeCommand.UserInfoToken; // Use the correct token

                        // 🔄 Log and Audit Salary Processing Start
                        string processingMessage = $"[INFO] Salary processing started. CorrelationId: '{correlationId}', " +
                                                   $"FileUploadId: '{executeCommand.FileUploadId}'. User: '{scopedUserInfoToken.FullName}'.";
                        scopedLogger.LogInformation(processingMessage);
                        await BaseUtilities.LogAndAuditAsync(processingMessage, executeCommand, HttpStatusCodeEnum.OK, LogAction.SalaryProcessing, LogLevelInfo.Information, scopedUserInfoToken.FullName, scopedUserInfoToken.Token, correlationId);

                        // 🚀 Execute Salary Processing
                        var result = await scopedMediator.Send(executeCommand);

                        if (result.StatusCode == 200)
                        {

                            


                            // ✅ Log and Audit Successful Salary Processing
                            string successMessage = $"[SUCCESS] Salary processing completed successfully. CorrelationId: '{correlationId}', " +
                                                    $"FileUploadId: '{executeCommand.FileUploadId}'. User: '{scopedUserInfoToken.FullName}'.";
                            scopedLogger.LogInformation(successMessage);
                            await BaseUtilities.LogAndAuditAsync(successMessage, executeCommand, HttpStatusCodeEnum.OK, LogAction.SalaryProcessing, LogLevelInfo.Information, scopedUserInfoToken.FullName, scopedUserInfoToken.Token, correlationId);
                        }
                        else
                        {
                            // ❌ Log and Audit Failed Salary Processing
                            string failureMessage = $"[ERROR] Salary processing failed. " +
                                                    $"Error Message: {result.Message}. CorrelationId: '{correlationId}', " +
                                                    $"FileUploadId: '{executeCommand.FileUploadId}'. User: '{scopedUserInfoToken.FullName}'.";
                            scopedLogger.LogError(failureMessage);
                            await BaseUtilities.LogAndAuditAsync(failureMessage, executeCommand, HttpStatusCodeEnum.Forbidden, LogAction.SalaryProcessing, LogLevelInfo.Warning, scopedUserInfoToken.FullName, scopedUserInfoToken.Token, correlationId);
                        }
                    }
                    catch (Exception ex)
                    {
                        // ❌ Log and Audit Unexpected Errors
                        var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<SalaryExecutionController>>();
                        string errorMessage = $"[ERROR] Salary processing failed due to an unexpected error. " +
                                              $"CorrelationId: '{correlationId}', FileUploadId: '{executeCommand.FileUploadId}'. " +
                                              $"Error: {ex.Message}";
                        scopedLogger.LogError(ex, errorMessage);
                        await BaseUtilities.LogAndAuditAsync(errorMessage, executeCommand, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryProcessing, LogLevelInfo.Error, "System", "N/A", correlationId);
                    }
                }
            });

            // ✅ Step 5: Construct a User-Friendly Response Message
            var responseMessage = "✅ Your salary processing request has been received and is being processed in the background. " +
                                  "🔄 The operation will run asynchronously, allowing you to continue using the system. " +
                                  "📢 You will be notified once the process is complete. " +
                                  $"🔍 Tracking ID: '{correlationId}'.";

            // ✅ Step 6: Return a Well-Structured API Response
            var response = ServiceResponse<bool>.ReturnResultWith200(true, responseMessage);
            return ReturnFormattedResponse(response);
        }





        /// <summary>
        /// Retrieves all salary extracts for a given file upload ID.
        /// </summary>
        /// <param name="fileUploadId">The file upload ID to filter salary extracts.</param>
        /// <returns>A list of salary extracts.</returns>
        [HttpGet("salary-extracts/{fileUploadId}")]
        [Produces("application/json", "application/xml", Type = typeof(List<SalaryExtractDto>))]
        public async Task<IActionResult> GetSalaryExtracts(string fileUploadId)
        {
            var query = new GetAllSalaryExtractQuery { FileUploadId = fileUploadId };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get all Salary Extracts by Salary File Upload Id with optional branch filter.
        /// </summary>
        /// <param name="id">The File Upload Id for the salary processing file</param>
        /// <param name="branchId">Optional filter for branch. If not provided, all branches are returned.</param>
        /// <returns>A list of Salary Extracts</returns>
        [HttpGet("salary-extracts")]
        [Produces("application/json", "application/xml", Type = typeof(List<SalaryExtractDto>))]
        public async Task<IActionResult> GetAllSalaryExtracts([FromQuery] string fileUploadId, [FromQuery] string branchId = null)
        {
            // Create the query object with the FileUploadId and optional BranchId
            var getFileDownloadInfoQuery = new GetAllSalaryExtractQuery
            {
                FileUploadId = fileUploadId,
                BranchId = branchId  // Optional, can be null
            };

            // Send the query to the mediator to retrieve the salary extracts
            var result = await _mediator.Send(getFileDownloadInfoQuery);

            // Return the response formatted based on the result
            return ReturnFormattedResponse(result);
        }

    }
}



