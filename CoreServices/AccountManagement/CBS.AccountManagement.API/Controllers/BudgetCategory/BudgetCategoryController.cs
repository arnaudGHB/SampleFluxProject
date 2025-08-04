using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// BudgetCategory
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class BudgetCategoryController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// BudgetCategory
        /// </summary>
        /// <param name="mediator"></param>
        public BudgetCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get BudgetCategory By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BudgetCategory/{id}", Name = "GetBudgetCategory")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetCategoryDto))]
        public async Task<IActionResult> GetBudgetCategory(string id)
        {
            var getBudgetCategoryQuery = new GetBudgetCategoryQuery { Id = id };
            var result = await _mediator.Send(getBudgetCategoryQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All BudgetCategory
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("BudgetCategory")]
        [Produces("application/json", "application/xml", Type = typeof(List<BudgetCategoryDto>))]
        public async Task<IActionResult> GetBudgetCategory()
        {
            var getAllBudgetCategoryQuery = new GetAllBudgetCategoryQuery { };
            var result = await _mediator.Send(getAllBudgetCategoryQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Block BudgetCategory
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("BudgetCategory/BlockBudgetCategory/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<BudgetCategoryDto>))]
        public async Task<IActionResult> GetBlockBudgetCategory(string id)
        {
            var getAllBudgetCategoryQuery = new BlockBudgetCategoryCommand { Id = id };
            var result = await _mediator.Send(getAllBudgetCategoryQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Approved BudgetCategory
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("BudgetCategory/ActivateBudgetCategory/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<BudgetCategoryDto>))]
        public async Task<IActionResult> GetActivateBudgetCategory(string id)
        {
            var getAllBudgetCategoryQuery = new ActivateBudgetCategoryCommand { Id = id };
            var result = await _mediator.Send(getAllBudgetCategoryQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Upload all existing BudgetCategory
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        //[HttpPost("BudgetCategory/UploadFiles/{id}")]
        //[Produces("application/json", "application/xml", Type = typeof(List<BudgetCategoryDto>))]
        //public async Task<IActionResult> UploadBudgetCategory(IFormFile file)
        //{
        //    //var getAllBudgetCategoryQuery = new UploadBudgetCategoryQuery { BudgetCategory = new BudgetCategoryModel().Upload(file) };
        //    var result = await _mediator.Send(getAllBudgetCategoryQuery);
        //    return ReturnFormattedResponse(result);
        //}

        /// <summary>
        /// Create a BudgetCategory
        /// </summary>
        /// <param name="addBudgetCategoryCommand"></param>
        /// <returns></returns>
        [HttpPost("BudgetCategory")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetCategoryDto))]
        public async Task<IActionResult> AddBudgetCategory(AddBudgetCategoryCommand addBudgetCategoryCommand)
        {
            var result = await _mediator.Send(addBudgetCategoryCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update BudgetCategory By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateBudgetCategoryCommand"></param>
        /// <returns></returns>
        [HttpPut("BudgetCategory/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetCategoryDto))]
        public async Task<IActionResult> UpdateBudgetCategory(string Id, UpdateBudgetCategoryCommand updateBudgetCategoryCommand)
        {
            updateBudgetCategoryCommand.Id = Id;
            var result = await _mediator.Send(updateBudgetCategoryCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete BudgetCategory By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("BudgetCategory/{Id}")]
        public async Task<IActionResult> DeleteBudgetCategory(string Id)
        {
            var deleteBudgetCategoryCommand = new DeleteBudgetCategoryCommand { Id = Id };
            var result = await _mediator.Send(deleteBudgetCategoryCommand);
            return ReturnFormattedResponse(result);
        }
    }
}