using CBS.CUSTOMER.API.Controllers.Base;
using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Command;
using CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.CUSTOMER.API.Controllers.CMoneyAPI
{
    /// <summary>
    /// Controller for managing Android App Versioning operations such as Add/Update Version, Delete Version, and Retrieve Version Information.
    /// </summary>
    [Route("api/v1/androidapp")]
    [ApiController]
    [APICaller.Helper.LoginModel.Authenthication.Authorize]
    public class AndroidAppVersionController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the AndroidAppVersionController.
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands.</param>
        public AndroidAppVersionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Adds or updates an Android app version.
        /// </summary>
        [HttpPost("versions/add-or-update")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<IActionResult> AddOrUpdateVersion([FromBody] AddOrUpdateAndroidVersionCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Deletes an Android app version by its ID.
        /// </summary>
        [HttpDelete("versions/{id}")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<IActionResult> DeleteVersionById(string id)
        {
            var command = new DeleteAndriodVersionByIdCommand { Id = id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves an Android app version by App Code Configuration.
        /// </summary>
        [HttpGet("versions/appcode/{appCodeConfiguration}")]
        [Produces("application/json", Type = typeof(AndriodVersionConfigurationDto))]
        public async Task<IActionResult> GetVersionByAppCode(string appCodeConfiguration)
        {
            var query = new GetAndriodVersionByAppCodeConfigurationQuery { AppCode = appCodeConfiguration };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves an Android app version by its ID.
        /// </summary>
        [HttpGet("versions/{id}")]
        [Produces("application/json", Type = typeof(AndriodVersionConfigurationDto))]
        public async Task<IActionResult> GetVersionById(string id)
        {
            var query = new GetAndriodVersionByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieves all Android app versions.
        /// </summary>
        [HttpGet("versions")]
        [Produces("application/json", Type = typeof(List<AndriodVersionConfigurationDto>))]
        public async Task<IActionResult> GetAllVersions()
        {
            var query = new GetAllAndroidVersionsQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
    }

}
