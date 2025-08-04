using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers.AccountType
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ProductAccountingBookController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// ProductAccountingBook
        /// </summary>
        /// <param name="mediator"></param>
        public ProductAccountingBookController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a AccountType for any banking product
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("ProductAccountingBook/CreateDefaultLoanConfiguration")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> CreateDefaultLoanConfiguration()
        {
            var Command = new CreateLoanProductDefaultCommand { };
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a AccountType for any banking product
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("ProductAccountingBook")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccountType(AddProductAccountingBookCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update product accounting books
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPut("ProductAccountingBook")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UpdateAccountingBook(UpdateProductAccountingBookCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get AccountType By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ProductAccountingBook/{id}", Name = "GetProductAccountingBook")]
        [Produces("application/json", "application/xml", Type = typeof(ProductAccountingBookDto))]
        public async Task<IActionResult> GetProductAccountingBook(string id)
        {
            var getAccountTypeQuery = new GetProductAccountingBookQuery { Id = id };
            var result = await _mediator.Send(getAccountTypeQuery);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Get Product Account configuration By name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ProductAccountingBook/GetProductAccountingConfigurations/{name}", Name = "GetAllAccountForProductQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountProduct>))]
        public async Task<IActionResult> GetAllAccountForProductQuery(string name)
        {
            var getAccountTypeQuery = new GetAllAccountForProductQuery { ProductName = name };
            var result = await _mediator.Send(getAccountTypeQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get Product Account configuration By productType
        /// </summary>
        /// <param name="id"></param>  "Physical_Teller "
        /// <returns></returns>
        [HttpGet("ProductAccountingBook/GetAllProductAccountingConfigurations/{productType}", Name = "GetAllAccountForASpecificProductTypeQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<ProductAccountingChart>))]
        public async Task<IActionResult> GetAllAccountForASpecificProductTypeQuery(string productType)
        {
            var getAccountTypeQuery = new GetAllAccountForASpecificProductTypeQuery { ProductType = productType };
            var result = await _mediator.Send(getAccountTypeQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get Account configuration By Id and type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ProductAccountingBook/{id}/{type}", Name = "GetProductAccountConfiguration")]
        [Produces("application/json", "application/xml", Type = typeof(List<ProductAccountConfiguration>))]
        public async Task<IActionResult> GetProductAccountConfiguration(string id,string type)
        {
            var getAccountTypeQuery = new GetProductConfigurationStatusQuery { Id = id,IdType=type };
            var result = await _mediator.Send(getAccountTypeQuery);
            return ReturnFormattedResponse(result);
        }
    }
}