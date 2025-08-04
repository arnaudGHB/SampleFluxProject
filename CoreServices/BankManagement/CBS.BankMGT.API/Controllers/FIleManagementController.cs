using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using CBS.BankMGT.MediatR.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace CBS.BankMGT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FIleManagementController : BaseController
    {
        public IMediator _mediator { get; set; }
        private IWebHostEnvironment _webHostEnvironment;
        /// <summary>
        /// User
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="userInfo"></param>
        /// <param name="webHostEnvironment"></param>
        public FIleManagementController(IMediator mediator, IWebHostEnvironment webHostEnvironment)
        {
            _mediator = mediator;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Handles HTTP POST requests for document uploads.
        /// </summary>
        /// <returns>An asynchronous task returning IActionResult.</returns>
        [HttpPost("Upload"), DisableRequestSizeLimit]
        [Produces("application/json", "application/xml", Type = typeof(DocumentUploadedDto))]
        [ProducesResponseType(typeof(DocumentUploadedDto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                var form = await Request.ReadFormAsync();
                if (Request.Form.Files.Count == 0)
                {
                    var documentUploadedDto = new DocumentUploadedDto
                    {
                        BaseUrl = baseUrl,
                        DocumentName = null,
                        DocumentType = form["documentType"],
                        Extension = "",
                        FullPath = "",
                        Id = "",
                        OperationId = form["Id"],
                        ServiceType = form["serviceType"],
                        UrlPath = ""
                    };
                    var response = ServiceResponse<DocumentUploadedDto>.Return400(documentUploadedDto, "No file was attached.");
                    return ReturnFormattedResponse(response);
                }
                var command = new AddDocumentUploadedCommand
                {
                    FormFiles = Request.Form.Files,
                    RootPath = _webHostEnvironment.WebRootPath,
                    BaseURL = baseUrl,
                    OperationID = form["Id"],
                    DocumentType = form["documentType"],
                    ServiceType = form["serviceType"],
                };

                var result = await _mediator.Send(command);

                return ReturnFormattedResponse(result);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while reading: {e.Message}";
                return ReturnFormattedResponse(ServiceResponse<DocumentUploadedDto>.Return500(e));
            }
        }
    }
}
