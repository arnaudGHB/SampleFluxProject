//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Commands;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.DocumentP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class DocumentPackController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// DocumentPack
        /// </summary>
        /// <param name="mediator"></param>
        public DocumentPackController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get DocumentPack By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DocumentPack/{DocumentPackId}", Name = "GetDocumentPack")]
        [Produces("application/json", "application/xml", Type = typeof(DocumentPackDto))]
        public async Task<IActionResult> GetDocumentPack(string DocumentPackId)
        {
            var getDocumentPackQuery = new GetDocumentPackQuery { Id = DocumentPackId };
            var result = await _mediator.Send(getDocumentPackQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All DocumentPacks
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("DocumentPacks")]
        [Produces("application/json", "application/xml", Type = typeof(List<DocumentPackDto>))]
        public async Task<IActionResult> GetDocumentPacks()
        {
            var getAllDocumentPackQuery = new GetAllDocumentPackQuery { };
            var result = await _mediator.Send(getAllDocumentPackQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a DocumentPack
        /// </summary>
        /// <param name="addDocumentPackCommand"></param>
        /// <returns></returns>
        [HttpPost("DocumentPack")]
        [Produces("application/json", "application/xml", Type = typeof(DocumentPackDto))]
        public async Task<IActionResult> AddDocumentPack(AddDocumentPackCommand addDocumentPackCommand)
        {
            var result = await _mediator.Send(addDocumentPackCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Create a DocumentPack
        /// </summary>
        /// <param name="addDocumenTotPackCommand"></param>
        /// <returns></returns>
        [HttpPost("DocumentPack/AddSingle")]
        [Produces("application/json", "application/xml", Type = typeof(DocumentJoinDto))]
        public async Task<IActionResult> AddDocumentToPack(AddDocumentToPackCommand addDocumenTotPackCommand)
        {
            var result = await _mediator.Send(addDocumenTotPackCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update DocumentPack By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateDocumentPackCommand"></param>
        /// <returns></returns>
        [HttpPut("DocumentPack/{DocumentPackId}")]
        [Produces("application/json", "application/xml", Type = typeof(DocumentPackDto))]
        public async Task<IActionResult> UpdateDocumentPack(string DocumentPackId, UpdateDocumentPackCommand updateDocumentPackCommand)
        {
            updateDocumentPackCommand.Id = DocumentPackId;
            var result = await _mediator.Send(updateDocumentPackCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete DocumentPack By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("DocumentPack/{DocumentPackId}")]
        public async Task<IActionResult> DeleteDocumentPack(string DocumentPackId)
        {
            var deleteDocumentPackCommand = new DeleteDocumentPackCommand { Id = DocumentPackId };
            var result = await _mediator.Send(deleteDocumentPackCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
