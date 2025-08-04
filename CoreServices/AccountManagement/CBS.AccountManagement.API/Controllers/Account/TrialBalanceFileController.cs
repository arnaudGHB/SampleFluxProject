using CBS.AccountManagement.API.Model;
using CBS.AccountManagement.Data;

using CBS.AccountManagement.MediatR.Account.Queries;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.ChartOfAccount.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CBS.AccountManagement.Helper;
using Microsoft.AspNetCore.Http;
using CBS.AccountManagement.MediatR.Queries;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// TrialBalanceFile
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    //[Authorize]
    public class TrialBalanceFileController : BaseController
    {
        public IMediator _mediator { get; set; }
        public UserInfoToken _userInfoToken { get; set; }
        public PathHelper _pathHelper { get; set; }
        /// <summary>
        /// TrialBalanceFile 
        /// </summary>
        /// <param name="mediator"></param>
        public TrialBalanceFileController(IMediator mediator, UserInfoToken userInfoToken, PathHelper pathHelper)
        {
            _mediator = mediator;
            _userInfoToken = userInfoToken;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Get TrialBalanceFile By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("TrialBalanceFile/{id}", Name = "GetTrialBalanceFile")]
        [Produces("application/json", "application/xml", Type = typeof(TrialBalanceFileDto))]
        public async Task<IActionResult> GetTrialBalanceFile(string id)
        {
            var getTrialBalanceFileQuery = new GetTrialBalanceFileQuery { Id = id };
            var result = await _mediator.Send(getTrialBalanceFileQuery);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Get All TrialBalanceFiles
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("TrialBalanceFiles")]
        [Produces("application/json", "application/xml", Type = typeof(List<TrialBalanceFileDto>))]
        public async Task<IActionResult> GetTrialBalanceFiles()
        {
            var getAllTrialBalanceFileQuery = new GetAllTrialBalanceFileQuery { };
            var result = await _mediator.Send(getAllTrialBalanceFileQuery);
            return ReturnFormattedResponse(result);
        }


    }
}
