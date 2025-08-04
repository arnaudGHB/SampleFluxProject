using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Budget.Commands;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// Budget
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class BudgetController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Budget
        /// </summary>
        /// <param name="mediator"></param>
        public BudgetController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Budget By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Budget/{id}", Name = "GetBudget")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetDto))]
        public async Task<IActionResult> GetBudget(string id)
        {
            var getBudgetQuery = new GetBudgetQuery { Id = id };
            var result = await _mediator.Send(getBudgetQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All Budget
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Budget")]
        [Produces("application/json", "application/xml", Type = typeof(List<BudgetDto>))]
        public async Task<IActionResult> GetBudget()
        {
            var getAllBudgetQuery = new GetAllBudgetQuery { };
            var result = await _mediator.Send(getAllBudgetQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Lock Budget
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Budget/LockBudget/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<BudgetDto>))]
        public async Task<IActionResult> GetBudgetLockBudget(string Id)
        {
            var getAllBudgetQuery = new LockBudgetCommand { Id = Id };
            var result = await _mediator.Send(getAllBudgetQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Approved Budget
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Budget/ApprovedBudget/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<BudgetDto>))]
        public async Task<IActionResult> GetBudgetApprovedBudget(string Id)
        {
            var getAllBudgetQuery = new AddBudgetApprovalCommand { Id = Id };
            var result = await _mediator.Send(getAllBudgetQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Upload all existing Budget
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        //[HttpPost("Budget/UploadFiles/{id}")]
        //[Produces("application/json", "application/xml", Type = typeof(List<BudgetDto>))]
        //public async Task<IActionResult> UploadBudget(IFormFile file)
        //{
        //    //var getAllBudgetQuery = new UploadBudgetQuery { Budget = new BudgetModel().Upload(file) };
        //    var result = await _mediator.Send(getAllBudgetQuery);
        //    return ReturnFormattedResponse(result);
        //}

        /// <summary>
        /// Create a Budget
        /// </summary>
        /// <param name="addBudgetCommand"></param>
        /// <returns></returns>
        [HttpPost("Budget")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetDto))]
        public async Task<IActionResult> AddBudget(AddBudgetCommand addBudgetCommand)
        {
            var result = await _mediator.Send(addBudgetCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update Budget By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateBudgetCommand"></param>
        /// <returns></returns>
        [HttpPut("Budget/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetDto))]
        public async Task<IActionResult> UpdateBudget(string Id, UpdateBudgetCommand updateBudgetCommand)
        {
            updateBudgetCommand.Id = Id;
            var result = await _mediator.Send(updateBudgetCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete Budget By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Budget/{Id}")]
        public async Task<IActionResult> DeleteBudget(string Id)
        {
            var deleteBudgetCommand = new DeleteBudgetCommand { Id = Id };
            var result = await _mediator.Send(deleteBudgetCommand);
            return ReturnFormattedResponse(result);
        }
    }
}