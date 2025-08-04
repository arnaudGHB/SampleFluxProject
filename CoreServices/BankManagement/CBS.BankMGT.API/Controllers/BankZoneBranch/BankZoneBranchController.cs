using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.BankZoneBranchMGT.MediatR.Commands;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CBS.BankMGT.API.Controllers
{
    /// <summary>
    /// BankZoneBranch
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class BankZoneBranchController : BaseController
    {
        public IMediator _mediator { get; set; }
     

        /// <summary>
        /// BankZoneBranch
        /// </summary>
        /// <param name="mediator"></param>
        public BankZoneBranchController(IMediator mediator)
        {
            _mediator = mediator;
            
        }
        /// <summary>
        /// Get BankZoneBranch By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BankZoneBranch/{id}", Name = "GetBankZoneBranch")]
        [Produces("application/json", "application/xml", Type = typeof(BankZoneBranchDto))]
        public async Task<IActionResult> GetBankZoneBranch(string id)
        {
            var getBankZoneBranchQuery = new GetBankZoneBranchQuery { Id = id };
            var result = await _mediator.Send(getBankZoneBranchQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All BankZoneBranchs
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("BankZoneBranchs", Name = "GetBankZoneBranchs")]
        [Produces("application/json", "application/xml", Type = typeof(List<BankZoneBranchDto>))]
        public async Task<IActionResult> GetBankZoneBranchs()
        {
            var getAllBankZoneBranchQuery = new GetAllBankZoneBranchQuery { };
            var result = await _mediator.Send(getAllBankZoneBranchQuery);
            return Ok(result);
        }
        /// <summary>
        /// Get All BankZoneBranchs
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("BankZoneBranch/BankZoneBranchbyBranchId/{id}/{type}", Name = "GetBankingZonebyBankBranchQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<BankZoneBranchDto>))]
        public async Task<IActionResult> GetBankingZonebyBankBranchQuery(string id, string type)
        {
            var getAllBankZoneBranchQuery = new GetBankingZonebyBankBranchQuery {Id=id ,Type=type};
            var result = await _mediator.Send(getAllBankZoneBranchQuery);
            return Ok(result);
        }
        
           /// <summary>
           /// Create a BankZoneBranch
           /// </summary>
           /// <param name="addBankZoneBranchCommand"></param>
           /// <returns></returns>
           [HttpPost("BankZoneBranch", Name = "AddBankZoneBranch")]
        [Produces("application/json", "application/xml", Type = typeof(BankZoneBranchDto))]
        public async Task<IActionResult> AddBankZoneBranch(AddBankZoneBranchCommand addBankZoneBranchCommand)
        {
            var result = await _mediator.Send(addBankZoneBranchCommand);
            return ReturnFormattedResponse(result);
            
        }
 

        /// <summary>
        /// Delete BankZoneBranch By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("BankZoneBranch/{Id}", Name = "DeleteBankZoneBranch")]
        [Produces("application/json", "application/xml", Type = typeof(List<BankZoneBranchDto>))]
        public async Task<IActionResult> DeleteBankZoneBranch(string Id)
        {
            var deleteBankZoneBranchCommand = new DeleteBankZoneBranchCommand { Id = Id };
            var result = await _mediator.Send(deleteBankZoneBranchCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
