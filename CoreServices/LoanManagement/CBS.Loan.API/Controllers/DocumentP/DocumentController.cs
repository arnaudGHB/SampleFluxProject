//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Commands;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Queries;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Commands;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.DocumentP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class DocumentController : BaseController
    {
        public IMediator _mediator { get; set; }
        private IWebHostEnvironment _webHostEnvironment;
        /// <summary>
        /// Document
        /// </summary>
        /// <param name="mediator"></param>
        public DocumentController(IMediator mediator, IWebHostEnvironment webHostEnvironment)
        {
            _mediator = mediator;
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// Get Document By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Document/{Id}", Name = "GetDocument")]
        [Produces("application/json", "application/xml", Type = typeof(DocumentDto))]
        public async Task<IActionResult> GetDocument(string Id)
        {
            var getDocumentQuery = new GetDocumentQuery { Id = Id };
            var result = await _mediator.Send(getDocumentQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Documents
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Documents")]
        [Produces("application/json", "application/xml", Type = typeof(List<DocumentDto>))]
        public async Task<IActionResult> GetDocuments()
        {
            var getAllDocumentQuery = new GetAllDocumentQuery { };
            var result = await _mediator.Send(getAllDocumentQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Document
        /// </summary>
        /// <param name="addDocumentCommand"></param>
        /// <returns></returns>
        [HttpPost("Document")]
        [Produces("application/json", "application/xml", Type = typeof(DocumentDto))]
        public async Task<IActionResult> AddDocument(AddDocumentCommand addDocumentCommand)
        {
            var result = await _mediator.Send(addDocumentCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update Document By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateDocumentCommand"></param>
        /// <returns></returns>
        [HttpPut("Document/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(DocumentDto))]
        public async Task<IActionResult> UpdateDocument(string Id, UpdateDocumentCommand updateDocumentCommand)
        {
            updateDocumentCommand.Id = Id;
            var result = await _mediator.Send(updateDocumentCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Document By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("Document/{Id}")]
        public async Task<IActionResult> DeleteDocument(string Id)
        {
            var deleteDocumentCommand = new DeleteDocumentCommand { Id = Id };
            var result = await _mediator.Send(deleteDocumentCommand);
            return ReturnFormattedResponse(result);
        }

        [HttpPost("DocumentAttachedToLoan"), DisableRequestSizeLimit]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(DocumentAttachedToLoanDto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> AttachedLoanDocument()
        {
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var form = await Request.ReadFormAsync();
            var loanApplicationId = form["loanApplicationId"];
            var documentId = form["documentId"];
            var attachedFiles = request.Form.Files;
            var command = new DocumentAttachedToLoanCommand
            {
                AttachedFiles = attachedFiles,
                RootPath = _webHostEnvironment.WebRootPath,
                BaseURL = baseUrl, DocumentId= documentId,
                LoanApplicationId = loanApplicationId,
            };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);

        }
        
        /// <summary>
        /// Document Attached to loan callback
        /// </summary>
        /// <param name="addLoanAttachedDocumentCallBackCommand"></param>
        /// <returns></returns>
        [HttpPost("DocumentAttachedToLoan/AddLoanAttachedDocumentCallBackCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> DocumentCallBack(AddLoanAttachedDocumentCallBackCommand addLoanAttachedDocumentCallBackCommand)
        {
            var result = await _mediator.Send(addLoanAttachedDocumentCallBackCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Get DocumentAttachedToLoan By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DocumentAttachedToLoan/{Id}", Name = "GetDocumentAttachedToLoan")]
        [Produces("application/json", "application/xml", Type = typeof(DocumentAttachedToLoanDto))]
        public async Task<IActionResult> GetDocumentAttachedToLoan(string Id)
        {
            var getDocumentAttachedToLoanQuery = new GetDocumentAttachedToLoanQuery { Id = Id };
            var result = await _mediator.Send(getDocumentAttachedToLoanQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All DocumentAttachedToLoans
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("DocumentAttachedToLoans")]
        [Produces("application/json", "application/xml", Type = typeof(List<DocumentAttachedToLoanDto>))]
        public async Task<IActionResult> GetDocumentAttachedToLoans()
        {
            var getAllDocumentAttachedToLoanQuery = new GetAllDocumentAttachedToLoanQuery { };
            var result = await _mediator.Send(getAllDocumentAttachedToLoanQuery);
            return Ok(result);
        }
        /// <summary>
        /// Update DocumentAttachedToLoan By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateDocumentAttachedToLoanCommand"></param>
        /// <returns></returns>
        [HttpPut("DocumentAttachedToLoan/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(DocumentAttachedToLoanDto))]
        public async Task<IActionResult> UpdateDocumentAttachedToLoan(string Id, UpdateDocumentAttachedToLoanCommand updateDocumentAttachedToLoanCommand)
        {
            updateDocumentAttachedToLoanCommand.Id = Id;
            var result = await _mediator.Send(updateDocumentAttachedToLoanCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete DocumentAttachedToLoan By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("DocumentAttachedToLoan/{Id}")]
        public async Task<IActionResult> DeleteDocumentAttachedToLoan(string Id)
        {
            var deleteDocumentAttachedToLoanCommand = new DeleteDocumentAttachedToLoanCommand { Id = Id };
            var result = await _mediator.Send(deleteDocumentAttachedToLoanCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
