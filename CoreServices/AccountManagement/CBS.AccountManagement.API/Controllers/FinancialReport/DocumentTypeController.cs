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
    public class DocumentTypeController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// DocumentType
        /// </summary>
        /// <param name="mediator"></param>
        public DocumentTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get DocumentType By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DocumentType/{id}", Name = "GetDocumentTypeById")]
        [Produces("application/json", "application/xml", Type = typeof(DocumentTypeDto))]
        public async Task<IActionResult> GetDocumentType(string id)
        {
            var getStatementModelQuery = new GetDocumentTypeQuery { Id = id };
            var result = await _mediator.Send(getStatementModelQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All DocumentType
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("DocumentType")]
        [Produces("application/json", "application/xml", Type = typeof(List<DocumentTypeDto>))]
        public async Task<IActionResult> GetAllDocumentType()
        {
            var getAllStatementModelQuery = new GetAllDocumentTypeQuery { };
            var result = await _mediator.Send(getAllStatementModelQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get All DocumentType
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("DocumentTypeByDocumentReference/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<DocumentTypeDto>))]
        public async Task<IActionResult> GetDocumentTypeByDocumentReference(string Id)
        {
            var getAllStatementModelQuery = new GetAllDocumentTypeByDocumentIdQuery { Id = Id };
            var result = await _mediator.Send(getAllStatementModelQuery);

            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a DocumentType
        /// </summary>
        /// <param name="AddDocumentTypeCommand"></param>
        /// <returns></returns>
        [HttpPost("DocumentType")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddDocumentType(AddDocumentTypeCommand addStatementModelCommand)
        {
            var result = await _mediator.Send(addStatementModelCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update DocumentType By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateStatementModelCommand"></param>
        /// <returns></returns>
        [HttpPut("DocumentType/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UpdateDocumentType(string Id, UpdateDocumentTypeCommand updateStatementModelCommand)
        {
            updateStatementModelCommand.Id = Id;
            var result = await _mediator.Send(updateStatementModelCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete DocumentType By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("DocumentType/{Id}")]
        public async Task<IActionResult> DeleteDocumentType(string Id)
        {
            var deleteStatementModelCommand = new DeleteDocumentTypeCommand { Id = Id };
            var result = await _mediator.Send(deleteStatementModelCommand);
            return ReturnFormattedResponse(result);
        }
    }
}