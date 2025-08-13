using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data.Dto.VaultP;
using CBS.TransactionManagement.MediatR.VaultP;
using CBS.TransactionManagement.MediatR.VaultP.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.VaultP
{
    /// <summary>
    /// Handles Vault operations including cash movements and transfers.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize()] // Restrict access to users with the "VaultPolicy"
    public class VaultController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="VaultController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for sending commands.</param>
        public VaultController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Handles cash movement requests in the vault (CashIn or CashOut).
        /// </summary>
        /// <param name="vaultCashMovementCommand">Command object for cash movement request.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        [HttpPost("cash-movement")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CashMovement([FromBody] VaultCashMovementCommand vaultCashMovementCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(vaultCashMovementCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Handles vault-to-vault cash transfer requests.
        /// </summary>
        /// <param name="vaultTransferCommand">Command object for vault transfer.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        [HttpPost("transfer")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TransferVaultCash([FromBody] VaultTranferCommand vaultTransferCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(vaultTransferCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Handles the creation of a new vault.
        /// </summary>
        /// <param name="addVaultCommand">Command object for adding a new vault.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        [HttpPost("create")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateVault([FromBody] AddVaultCommand addVaultCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(addVaultCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Handles the update of an existing vault.
        /// </summary>
        /// <param name="updateVaultCommand">Command object for updating a vault.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        [HttpPut("update")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVault([FromBody] UpdateVaultCommand updateVaultCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(updateVaultCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieves a vault by its BranchId.
        /// </summary>
        /// <param name="branchId">BranchId of the vault to retrieve.</param>
        /// <returns>The vault details.</returns>
        [HttpGet("branch/{branchId}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(VaultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVaultByBranchId(string branchId)
        {
            var query = new GetVaultByBranchIdQuery { BranchId = branchId };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves all vaults.
        /// </summary>
        /// <returns>List of all vaults.</returns>
        [HttpGet("all")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(List<VaultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllVaults()
        {
            var query = new GetAllVaultsQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves a vault by its unique Id.
        /// </summary>
        /// <param name="id">Id of the vault to retrieve.</param>
        /// <returns>The vault details.</returns>
        [HttpGet("{id}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(VaultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVaultById(string id)
        {
            var query = new GetVaultByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Initializes a new vault with provided denominations and balances.
        /// </summary>
        /// <param name="vaultInitializationCommand">Command object for vault initialization.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        [HttpPost("initialize")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InitializeVault([FromBody] VaultInitializationCommand vaultInitializationCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(vaultInitializationCommand);
            return ReturnFormattedResponse(result);
        }
    }
}


