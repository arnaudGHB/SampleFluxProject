using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Commands;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.MediatR.RemittanceP.Commands;
using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Repository.RemittanceP;

namespace CBS.TransactionManagement.MediatR.RemittanceP.Handlers
{

    /// <summary>
    /// Handles the command to update a Teller based on Remittance.
    /// </summary>
    public class ValidationRemittanceCommandHandler : IRequestHandler<ValidationOfRemittanceCommand, ServiceResponse<RemittanceDto>>
    {
        private readonly IMediator _mediator; // Mediator for sending notifications or commands.
        private readonly IRemittanceRepository _remittanceRepository; // Repository for accessing Remittance data.
        private readonly ILogger<ValidationRemittanceCommandHandler> _logger; // Logger for logging actions and errors.
        private readonly IMapper _mapper; // AutoMapper instance for mapping objects.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // Contains user information from the token.

        /// <summary>
        /// Constructor for initializing the ValidationRemittanceCommandHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access (optional, unused in this class).</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information extracted from the token.</param>
        /// <param name="mapper">AutoMapper for mapping objects between types.</param>
        /// <param name="uow">Unit of Work for transaction management.</param>
        /// <param name="RemittanceRepository">Repository for Remittance data access.</param>
        /// <param name="mediator">Mediator for command and event dispatching.</param>
        public ValidationRemittanceCommandHandler(
            ITellerRepository TellerRepository, // Unused parameter, can be removed for optimization.
            ILogger<ValidationRemittanceCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            IRemittanceRepository RemittanceRepository = null,
            IMediator mediator = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Ensure logger is not null.
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken)); // Ensure user info token is not null.
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); // Ensure mapper is not null.
            _uow = uow ?? throw new ArgumentNullException(nameof(uow)); // Ensure unit of work is not null.
            _remittanceRepository = RemittanceRepository ?? throw new ArgumentNullException(nameof(RemittanceRepository)); // Ensure repository is not null.
            _mediator = mediator; // Mediator is optional.
        }

        /// <summary>
        /// Handles the Remittance command to update a Teller's data.
        /// </summary>
        /// <param name="request">The Remittance command containing updated Teller data.</param>
        /// <param name="cancellationToken">A token to signal cancellation of the operation.</param>
        /// <returns>A service response indicating success or failure.</returns>
        public async Task<ServiceResponse<RemittanceDto>> Handle(ValidationOfRemittanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the remittance by ID from the repository.
                var remittance = await _remittanceRepository.FindAsync(request.Id);

                // Check if the remittance exists.
                if (remittance == null)
                {
                    string errorMessage = $"Remittance with ID {request.Id} was not found.";
                    _logger.LogError(errorMessage);

                    // Log and audit the not found error.
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.Remittance, LogLevelInfo.Warning);

                    // Return 404 response.
                    return ServiceResponse<RemittanceDto>.Return404(errorMessage);
                }

                // Check if the remittance is already approved.
                if (remittance.Status == Status.Approved.ToString())
                {
                    string errorMessage = $"Remittance {remittance.TransactionReference} is already validated.";
                    _logger.LogError(errorMessage);

                    // Log and audit the forbidden operation.
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.Remittance, LogLevelInfo.Warning);

                    // Return 403 response.
                    return ServiceResponse<RemittanceDto>.Return403(errorMessage);
                }

                // Map updated data from the command to the existing remittance entity.
                _mapper.Map(request, remittance);

                // Set the approver's name and approval date.
                remittance.ApprovedBy = !string.IsNullOrWhiteSpace(_userInfoToken.FullName) ? _userInfoToken.FullName : _userInfoToken.Email;
                remittance.ApprovalDate = BaseUtilities.UtcNowToDoualaTime();

                // Update the remittance in the repository.
                _remittanceRepository.Update(remittance);

                // Save changes using the Unit of Work.
                await _uow.SaveAsync();

                // Prepare a success message.
                string successMessage = $"Remittance was successfully validated by {_userInfoToken.FullName}";
                _logger.LogInformation(successMessage);

                // Log and audit the successful operation.
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.Remittance, LogLevelInfo.Information);

                // Return a successful response with the updated remittance DTO.
                return ServiceResponse<RemittanceDto>.ReturnResultWith200(_mapper.Map<RemittanceDto>(remittance), successMessage);
            }
            catch (Exception ex)
            {
                // Log the exception and return an internal server error response.
                string errorMessage = $"An error occurred while updating the remittance: {ex.Message}";
                _logger.LogError(errorMessage);

                // Log and audit the exception.
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.Remittance, LogLevelInfo.Error);

                // Return a 500 response with the exception details.
                return ServiceResponse<RemittanceDto>.Return500(ex);
            }
        }
    }
}
