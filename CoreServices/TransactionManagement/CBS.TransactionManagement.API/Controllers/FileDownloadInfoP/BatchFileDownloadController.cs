



using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.FileDownloadInfoP.Queries;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.FileDownloadInfoP.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.FileDownloadInfoP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class BatchFileDownloadController : BaseController
    {
        public IMediator _mediator { get; set; }
        private readonly ILogger<BatchFileDownloadController> _logger;
        /// <summary>
        /// FileDownloadInfo
        /// </summary>
        /// <param name="mediator"></param>
        public BatchFileDownloadController(IMediator mediator, ILogger<BatchFileDownloadController> logger = null)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get FileDownloadInfo By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("FileDownloadInfo/{Id}", Name = "GetFileDownloadInfo")]
        [Produces("application/json", "application/xml", Type = typeof(FileDownloadInfoDto))]
        public async Task<IActionResult> GetFileDownloadInfo(string Id)
        {
            var getFileDownloadInfoQuery = new GetFileDownloadInfoQuery { Id = Id };
            var result = await _mediator.Send(getFileDownloadInfoQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All FileDownloadInfos
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("FileDownloadInfos")]
        [Produces("application/json", "application/xml", Type = typeof(List<FileDownloadInfoDto>))]
        public async Task<IActionResult> GetFileDownloadInfos()
        {
            var getAllFileDownloadInfoQuery = new GetAllFileDownloadInfoQuery { };
            var result = await _mediator.Send(getAllFileDownloadInfoQuery);
            return Ok(result);
        }
        /// <summary>
        /// Initiate bulk download.
        /// </summary>
        /// <param name="initiateDownloadInfoCommand"></param>
        /// <returns></returns>
        [HttpPost("FileDownloadInfo/InitiateBulkDownloadIndividualAccountBalances")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> InitiateBulkDownload(InitiateDownloadInfoCommand initiateDownloadInfoCommand)
        {
            try
            {
                var result = await _mediator.Send(initiateDownloadInfoCommand);
                return ReturnFormattedResponse(result);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError($"Failed to initiate bulk download process: {ex.Message}");
                // Respond with false and an error message
                return BadRequest(ServiceResponse<bool>.Return500($"Failed to initiate bulk download process: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete FileDownloadInfo By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("FileDownloadInfo/{Id}")]
        public async Task<IActionResult> DeleteFileDownloadInfo(string Id)
        {
            var deleteFileDownloadInfoCommand = new DeleteFileDownloadInfoCommand { Id = Id };
            var result = await _mediator.Send(deleteFileDownloadInfoCommand);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Get All FileDownloadInfos
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("FileDownloadInfos/GetAllFileDownloadInfoByUserIdQuery/{userid}")]
        [Produces("application/json", "application/xml", Type = typeof(List<FileDownloadInfoDto>))]
        public async Task<IActionResult> GetAllFileDownloadInfoByUserIdQuery(string userid)
        {
            var getAllFileDownloadInfoQuery = new GetAllFileDownloadInfoByUserIdQuery { UserId = userid };
            var result = await _mediator.Send(getAllFileDownloadInfoQuery);
            return Ok(result);
        }
        /// <summary>
        /// Download AttachedFiles by AttachedFiles Id
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("FileDownloadInfos/download/{fileId}")]
        [Produces("application/json", "application/xml", Type = typeof(FileDownloadDto))]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            var query = new DownloadFileByIdQuery(fileId);
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

       
    }

}
