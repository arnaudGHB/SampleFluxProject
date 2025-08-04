using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
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
    /// Branch
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    //[Authorize]
    public class BranchController : BaseController
    {
        public IMediator _mediator { get; set; }
        private IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Branch
        /// </summary>
        /// <param name="mediator"></param>
        public BranchController(IMediator mediator, IWebHostEnvironment webHostEnvironment = null)
        {
            _mediator = mediator;
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// Get Branch By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Branch/{id}", Name = "GetBranch")]
        [Produces("application/json", "application/xml", Type = typeof(BranchDto))]
        public async Task<IActionResult> GetBranch(string id)
        {
            var getBranchQuery = new GetBranchQuery { Id = id };
            var result = await _mediator.Send(getBranchQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Branchs
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Branchs")]
        [Produces("application/json", "application/xml", Type = typeof(List<BranchDto>))]
        public async Task<IActionResult> GetBranchs()
        {
            var getAllBranchQuery = new GetAllBranchQuery { };
            var result = await _mediator.Send(getAllBranchQuery);
            return Ok(result);
        }
        /// <summary>
        /// Get All Branchs by Bank ID
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns List of branches by bankid</response>
        [HttpGet("Branch/GetBranchsByBank/{bankid}", Name = "GetBranchsByBank")]
        [Produces("application/json", "application/xml", Type = typeof(BranchDto))]
        public async Task<IActionResult> GetBranchsByBank(string bankid)
        {
            var getAllBranchQuery = new GetBranchesByBankIDQuery { BankID=bankid};
            var result = await _mediator.Send(getAllBranchQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Branch
        /// </summary>
        /// <param name="addBranchCommand"></param>
        /// <returns></returns>
        [HttpPost("Branch")]
        [Produces("application/json", "application/xml", Type = typeof(BranchDto))]
        public async Task<IActionResult> AddBranch(AddBranchCommand addBranchCommand)
        {
            var result = await _mediator.Send(addBranchCommand);
            return ReturnFormattedResponse(result);
            
        }

        [HttpPost("UploadBranchLogo"), DisableRequestSizeLimit]
        [Produces("application/json", "application/xml", Type = typeof(BranchDto))]
        [ProducesResponseType(typeof(BranchDto), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> UploadBranchLogo()
        {
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var form = await Request.ReadFormAsync();
            var BranchID = form["BranchID"];
            var attachedFiles = request.Form.Files;
            var command = new UpdateBranchLogoCommand
            {
                FormFile = Request.Form.Files,
                RootPath = _webHostEnvironment.WebRootPath,
                BaseURL = baseUrl,
                BranchID = BranchID,
            };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Update Branch By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateBranchCommand"></param>
        /// <returns></returns>
        [HttpPut("Branch/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(BranchDto))]
        public async Task<IActionResult> UpdateBranch(string Id, UpdateBranchCommand updateBranchCommand)
        {
            updateBranchCommand.Id = Id;
            var result = await _mediator.Send(updateBranchCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Branch By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Branch/{Id}")]
        public async Task<IActionResult> DeleteBranch(string Id)
        {
            var deleteBranchCommand = new DeleteBranchCommand { Id = Id };
            var result = await _mediator.Send(deleteBranchCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
