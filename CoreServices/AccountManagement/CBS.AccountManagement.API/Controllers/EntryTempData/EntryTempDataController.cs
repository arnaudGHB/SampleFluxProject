using CBS.AccountManagement.API;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.EntryTempDataManagement.API
{
    /// <summary>
    /// EntryTempData
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class EntryTempDataController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// EntryTempData
        /// </summary>
        /// <param name="mediator"></param>
        public EntryTempDataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get EntryTempData By entry Reference
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("EntryTempData/GetEntryTempDataQuery/{id}", Name = "GetEntryTempDataQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<EntryTempDataDto>))]
        public async Task<IActionResult> GetEntryTempDataByReference(string id)
        {
            var getEntryTempDataQuery = new GetEntryTempDataQuery() { Id = id };
            var result = await _mediator.Send(getEntryTempDataQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get EntryTempData By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("EntryTempData/{id}", Name = "GetEntryTempData")]
        [Produces("application/json", "application/xml", Type = typeof(EntryTempDataDto))]
        public async Task<IActionResult> GetEntryTempData(string id)
        {
            var getEntryTempDataQuery = new GetEntryTempDataQuery { Id = id };
            var result = await _mediator.Send(getEntryTempDataQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All EntryTempDatas
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("EntryTempDatas/{ReferenceId}", Name = "GetEntryTempDatas")]
        [Produces("application/json", "application/xml", Type = typeof(List<EntryTempDataDto>))]
        public async Task<IActionResult> GetEntryTempDatas(string ReferenceId)
        {
            var getAllEntryTempDataQuery = new GetAllEntryTempDataQuery { ReferenceId = ReferenceId };
            var result = await _mediator.Send(getAllEntryTempDataQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get All EntryTempDatas
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("EntryTempDatas/PostedEntry/{ReferenceId}", Name = "GetPostedEntry")]
        [Produces("application/json", "application/xml", Type = typeof(PostedEntry))]
        public async Task<IActionResult> GetPostedEntry(string ReferenceId)
        {
            var getAllEntryTempDataQuery = new GetPostedEntriesQuery { ReferenceId = ReferenceId };
            var result = await _mediator.Send(getAllEntryTempDataQuery);
            return Ok(result);
        }


        /// <summary>
        /// Get All GetAllPostedEntry
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("EntryTempDatas/PostedEntry", Name = "GetAllPostedEntry")]
        [Produces("application/json", "application/xml", Type = typeof(List<PostedEntry>))]
        public async Task<IActionResult> GetAllPostedEntry()
        {
            var getAllEntryTempDataQuery = new GetAllPostedEntriesQuery { };
            var result = await _mediator.Send(getAllEntryTempDataQuery);
            return Ok(result);
        }



        /// <summary>
        /// Create a EntryTempData
        /// </summary>
        /// <param name="AddTempDataEntriesCommand"></param>
        /// <returns></returns>
        [HttpPost("EntryTempData/PostManualEntries")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddEntryTempData(AddEntryTempDataListCommand AddTempDataEntriesCommand)
        {
            //AddTempDataEntriesCommand.IsSystem = false;
            var result = await _mediator.Send(AddTempDataEntriesCommand);
            return ReturnFormattedResponseObject(result);
        }

        /// <summary>/api/v1/EntryTempData/ManaulEntryApproval
        /// Manaul Entry Approval
        /// </summary>
        /// <param name="ConfirmPostedEntriesCommand"></param>
        /// <returns></returns>
        [HttpPost("EntryTempData/ManaulEntryApproval")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddEntryTempData(ConfirmPostedEntriesCommand ConfirmPostedEntriesCommand)
        {
            var result = await _mediator.Send(ConfirmPostedEntriesCommand);
            return ReturnFormattedResponseObject(result);
        }
 
        /// <summary>
        /// Update EntryTempData By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateEntryTempDataCommand"></param>
        /// <returns></returns>
        [HttpPut("EntryTempData/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(EntryTempDataDto))]
        public async Task<IActionResult> UpdateEntryTempData(string Id, UpdateEntryTempDataCommand updateEntryTempDataCommand)
        {
            updateEntryTempDataCommand.Id = Id;
            var result = await _mediator.Send(updateEntryTempDataCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete EntryTempData By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("EntryTempData/{Id}")]
        public async Task<IActionResult> DeleteEntryTempData(string Id)
        {
            var deleteEntryTempDataCommand = new DeleteEntryTempDataCommand { Id = Id };
            var result = await _mediator.Send(deleteEntryTempDataCommand);
            return ReturnFormattedResponse(result);
        }
    }
}