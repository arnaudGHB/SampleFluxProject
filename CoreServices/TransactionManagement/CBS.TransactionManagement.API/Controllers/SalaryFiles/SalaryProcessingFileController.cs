using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Commands;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Queries;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Commands;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.SalaryFiles
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class SalaryProcessingFileController : BaseController
    {
        public IMediator _mediator { get; set; }
        private readonly ILogger<SalaryProcessingFileController> _logger;
        /// <summary>
        /// FileDownloadInfo
        /// </summary>
        /// <param name="mediator"></param>
        public SalaryProcessingFileController(IMediator mediator, ILogger<SalaryProcessingFileController> logger = null)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Upload a file
        /// </summary>
        /// <param name="addSalaryExecution"></param>
        /// <returns></returns>
        [HttpPost("SalaryProcessingFile/Salary/Execute")]
        [Produces("application/json", "application/xml", Type = typeof(List<SalaryProcessingDto>))]
        public async Task<IActionResult> AddSalaryExecutionCommand([FromForm] ExecuteSalaryCommand addSalaryExecution)
        {
            try
            {
                var result = await _mediator.Send(addSalaryExecution);
                return ReturnFormattedResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to upload file: {ex.Message}");
                return BadRequest(ServiceResponse<bool>.Return500($"Failed to upload file: {ex.Message}"));
            }
        }

        /// <summary>
        /// Upload a file
        /// </summary>
        /// <param name="uploadCommand"></param>
        /// <returns></returns>
        [HttpPost("SalaryProcessingFile/Upload")]
        [Produces("application/json", "application/xml", Type = typeof(List<SalaryExtractDto>))]
        public async Task<IActionResult> UploadFile([FromForm] UploadAnalysedSalaryCommand uploadCommand)
        {
            try
            {
                _logger.LogDebug("Attempting to upload file");
                _logger.LogInformation($"Failed to upload file");
                var result = await _mediator.Send(uploadCommand);
                return ReturnFormattedResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to upload file: {ex.Message}");
                return BadRequest(ServiceResponse<bool>.Return500($"Failed to upload file: {ex.Message}"));
            }
        }
      

    }

}
