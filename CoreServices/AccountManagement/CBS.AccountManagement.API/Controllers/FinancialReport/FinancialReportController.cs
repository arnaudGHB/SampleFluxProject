using CBS.AccountManagement.API.Model;
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
    public class FinancialReportController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// FinancialReport
        /// </summary>
        /// <param name="mediator"></param>
        public FinancialReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get DocumentReferenceCode By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("FinancialReport/GetDocumentReferenceCodeById/{id}", Name = "GetDocumentReferenceCodeById")]
        [Produces("application/json", "application/xml", Type = typeof(DocumentReferenceCodeDto))]
        public async Task<IActionResult> GetDocumentReferenceCode(string id)
        {
            var getStatementModelQuery = new GetDocumentReferenceCodeQuery { Id = id };
            var result = await _mediator.Send(getStatementModelQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get DocumentReferenceCode By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("FinancialReport/GetCorrespondingMappingByDocumentReferenceCodeIdQuery/{id}", Name = "GetCorrespondingMappingByDocumentReferenceCodeIdQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<CorrespondingMappingDto>))]
        public async Task<IActionResult> GetCorrespondingMappingByDocumentReferenceCodeIdQuery(string id)
        {
            var getStatementModelQuery = new GetCorrespondingMappingByDocumentReferenceCodeIdQuery { Id = id };
            var result = await _mediator.Send(getStatementModelQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get DocumentReferenceCode By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("FinancialReport/GetCorrespondingMappingExceptionsByDocumentReferenceCodeIdQuery/{id}", Name = "GetCorrespondingMappingExceptionsByDocumentReferenceCodeIdQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<CorrespondingMappingDto>))]
        public async Task<IActionResult> GetCorrespondingMappingExceptionsByDocumentReferenceCodeIdQuery(string id)
        {
            var getStatementModelQuery = new GetCorrespondingMappingExceptionsByDocumentReferenceCodeIdQuery { Id = id };
            var result = await _mediator.Send(getStatementModelQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All DocumentReferenceCode
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("FinancialReport/DocumentReferenceCode")]
        [Produces("application/json", "application/xml", Type = typeof(List<DocumentReferenceCodeDto>))]
        public async Task<IActionResult> GetAllDocumentReferenceCode()
        {
            var getAllStatementModelQuery = new GetAllDocumentReferenceCodeQuery { };
            var result = await _mediator.Send(getAllStatementModelQuery);
            return Ok(result);
        }

        /// <summary>
        /// Create a DocumentReferenceCode
        /// </summary>
        /// <param name="AddDocumentReferenceCodeCommand"></param>
        /// <returns></returns>
        [HttpPost("FinancialReport/DocumentReferenceCode/Create")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddDocumentReferenceCode(AddCommandDocumentReferenceCode addStatementModelCommand)
        {
            var result = await _mediator.Send(addStatementModelCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update DocumentReferenceCode By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateStatementModelCommand"></param>
        /// <returns></returns>
        [HttpPut("FinancialReport/DocumentReferenceCode/Update/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UpdateDocumentReferenceCode(string Id, UpdateCommandDocumentReferenceCode updateStatementModelCommand)
        {
            updateStatementModelCommand.Id = Id;
            var result = await _mediator.Send(updateStatementModelCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete DocumentReferenceCode By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("FinancialReport/DocumentReferenceCode/Delete/{Id}")]
        public async Task<IActionResult> DeleteDocumentReferenceCode(string Id)
        {
            var deleteStatementModelCommand = new DeleteDocumentReferenceCodeCommand { Id = Id };
            var result = await _mediator.Send(deleteStatementModelCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Upload BalanceSheetModel from excel file
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="uploadChartOfAccountQuery"></param>
        /// <returns></returns>
        [HttpPost("FinancialReport/FinancialReportConfiguration")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UploadFinancialReport(IFormFile form)
        {
            var query = new GetDocumentRefQuery();
             var queryresult = await _mediator.Send(query);
            var uploadModel = new AccountingFinancialParams().ReadingExcel(form, queryresult.Data);
            AddFinancialReportConfigurationCommand financialReportConfigurationsCommand = new AddFinancialReportConfigurationCommand { FinancialReportConfigurations = uploadModel };
            var result = await _mediator.Send(financialReportConfigurationsCommand);
            return Ok(result);
        }
    }
}