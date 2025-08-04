using CBS.AccountManagement.API.Model;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// ReportDownLoad
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ReportDownLoadController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// ReportDownLoad
        /// </summary>
        /// <param name="mediator"></param>
        public ReportDownLoadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get ReportDownLoad By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ReportDownLoad/{id}", Name = "GetReportDownLoad")]
        [Produces("application/json", "application/xml", Type = typeof(ReportDto))]
        public async Task<IActionResult> GetReportDownLoad(string id)
        {
            var getReportDownLoadQuery = new GetReportQuery { Id = id };
            var result = await _mediator.Send(getReportDownLoadQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get ReportDownLoad By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DownloadFileByIdQuery/{id}", Name = "DownloadFileByIdQuery")]
        [Produces("application/json", "application/xml", Type = typeof(ReportDto))]
        public async Task<IActionResult> DownloadFileByIdQuery(string id)
        {
            var getReportDownLoadQuery = new DownloadFileByIdQuery ( id );
            var result = await _mediator.Send(getReportDownLoadQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All ReportDownLoads
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("ReportDownLoads")]
        [Produces("application/json", "application/xml", Type = typeof(List<ReportDto>))]
        public async Task<IActionResult> GetReportDownLoads()
        {
            var getAllReportDownLoadQuery = new GetAllReportQuery { };
            var result = await _mediator.Send(getAllReportDownLoadQuery);
            return Ok(result);
        }
        /// <summary>
        /// Get All users downloaded report
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("DownloadFileByUserIdQuery/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<ReportDto>))]
        public async Task<IActionResult> DownloadFileByUserIdQuery( string Id)
        {
            var getAllReportDownLoadQuery = new DownloadFileByUserIdQuery { userId =Id };
            var result = await _mediator.Send(getAllReportDownLoadQuery);
            return Ok(result);
        }
 

        /// <summary>
        /// Create a ReportDownLoad
        /// </summary>
        /// <param name="addReportDownLoadCommand"></param>
        /// <returns></returns>
        [HttpPost("ReportDownLoad")]
        [Produces("application/json", "application/xml", Type = typeof(ReportDto))]
        public async Task<IActionResult> AddReportDownLoad(AddReportCommand addReportDownLoadCommand)
        {
            var result = await _mediator.Send(addReportDownLoadCommand);
            return ReturnFormattedResponse(result);
        }

       
        /// <summary>
        /// Update ReportDownLoad By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateReportDownLoadCommand"></param>
        /// <returns></returns>
        [HttpPut("ReportDownLoad/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(ReportDto))]
        public async Task<IActionResult> UpdateReportDownLoad(string Id, UpdateReportCommand updateReportDownLoadCommand)
        {
            updateReportDownLoadCommand.Id = Id;
            var result = await _mediator.Send(updateReportDownLoadCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete ReportDownLoad By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("ReportDownLoad/{Id}")]
        public async Task<IActionResult> DeleteReportDownLoad(string Id)
        {
            var deleteReportDownLoadCommand = new DeleteReportCommand { Id = Id };
            var result = await _mediator.Send(deleteReportDownLoadCommand);
            return ReturnFormattedResponse(result);
        }
    }
}