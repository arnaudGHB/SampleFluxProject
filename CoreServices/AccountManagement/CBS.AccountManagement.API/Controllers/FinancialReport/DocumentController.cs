using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// StatementModel
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class DocumentController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Document
        /// </summary>
        /// <param name="mediator"></param>
        public DocumentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Document By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Document/{id}", Name = "GetDocumentById")]
        [Produces("application/json", "application/xml", Type = typeof(DocumentDto))]
        public async Task<IActionResult> GetDocument(string id)
        {
            var getStatementModelQuery = new GetDocumentQuery { Id = id };
            var result = await _mediator.Send(getStatementModelQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All Document
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Documents")]
        [Produces("application/json", "application/xml", Type = typeof(List<DocumentDto>))]
        public async Task<IActionResult> GetAllDocument()
        {
            var getAllStatementModelQuery = new GetAllDocumentQuery { };
            var result = await _mediator.Send(getAllStatementModelQuery);
            return Ok(result);
        }

        /// <summary>
        /// Create a Document
        /// </summary>
        /// <param name="AddDocumentCommand"></param>
        /// <returns></returns>
        [HttpPost("Document")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddDocument(AddDocumentCommand addStatementModelCommand)
        {
            var result = await _mediator.Send(addStatementModelCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update Document By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateStatementModelCommand"></param>
        /// <returns></returns>
        [HttpPut("Document/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UpdateDocument(string Id, UpdateDocumentCommand updateStatementModelCommand)
        {
            updateStatementModelCommand.Id = Id;
            var result = await _mediator.Send(updateStatementModelCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete Document By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Document/{Id}")]
        public async Task<IActionResult> DeleteDocument(string Id)
        {
            var deleteStatementModelCommand = new DeleteDocumentCommand { Id = Id };
            var result = await _mediator.Send(deleteStatementModelCommand);
            return ReturnFormattedResponse(result);
        }
    }
}