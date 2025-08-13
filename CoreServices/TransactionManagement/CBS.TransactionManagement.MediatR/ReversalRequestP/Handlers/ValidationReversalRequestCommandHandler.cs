using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data.Dto.ReversalRequestP;
using CBS.TransactionManagement.Data.Entity.ReversalRequestP;
using CBS.TransactionManagement.MediatR.Commands.ReversalRequestP;
using CBS.TransactionManagement.Repository.AccountingDayOpening;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Handlers.ReversalRequestP
{

    /// <summary>
    /// Handles the command to validate a Reversal Request.
    /// </summary>
    public class ValidationReversalRequestCommandHandler : IRequestHandler<ValidationReversalRequestCommand, ServiceResponse<ReversalRequestDto>>
    {
        private readonly IReversalRequestRepository _reversalRequestRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ValidationReversalRequestCommandHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountingDayRepository _accountingDayRepository;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Initializes the ValidationReversalRequestCommandHandler.
        /// </summary>
        public ValidationReversalRequestCommandHandler(
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<ValidationReversalRequestCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            IReversalRequestRepository reversalRequestRepository = null,
            IAccountingDayRepository accountingDayRepository = null)
        {
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _reversalRequestRepository = reversalRequestRepository;
            _accountingDayRepository = accountingDayRepository;
        }

        /// <summary>
        /// Handles the validation of a Reversal Request.
        /// </summary>
        /// <param name="request">The command containing validation details.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ReversalRequestDto>> Handle(ValidationReversalRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the accounting date for the branch
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Step 2: Find the reversal request by ID
                var reversalRequest = await _reversalRequestRepository.FindAsync(request.Id);
                if (reversalRequest == null)
                {
                    string errorMessage = $"Reversal request with ID {request.Id} was not found.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.ReversalValidation, LogLevelInfo.Error);
                    return ServiceResponse<ReversalRequestDto>.Return404(errorMessage);
                }

                // Step 3: Check if the request has already been validated
                if (reversalRequest.IsValidated)
                {
                    string errorMessage = $"Reversal request ID {request.Id} has already been validated.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Conflict, LogAction.ReversalValidation, LogLevelInfo.Warning);
                    return ServiceResponse<ReversalRequestDto>.Return409(errorMessage);
                }

                // Step 4: Mark the request as validated
                if (request.Status == Status.Validated.ToString())
                {
                    reversalRequest.IsValidated = true;
                }
                reversalRequest.Status = request.Status;
                reversalRequest.ValidationComment = request.ValidationComment;
                reversalRequest.ValidatedBy = _userInfoToken.FullName;
                reversalRequest.ValidationDate = BaseUtilities.UtcNowToDoualaTime();

                // Step 5: Update the request and save changes
                _reversalRequestRepository.Update(reversalRequest);
                await _uow.SaveAsync();

                // Step 6: Log and audit successful validation
                string successMessage = $"Reversal request ID {request.Id} was successfully validated by {_userInfoToken.FullName}.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.ReversalValidation, LogLevelInfo.Information);

                // Step 7: Map the entity to a DTO and return success response
                var reversalRequestDto = _mapper.Map<ReversalRequestDto>(reversalRequest);
                return ServiceResponse<ReversalRequestDto>.ReturnResultWith200(reversalRequestDto, successMessage);
            }
            catch (Exception e)
            {
                // Step 8: Log and audit error if any exception occurs
                string errorMessage = $"An error occurred while validating the reversal request: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.ReversalValidation, LogLevelInfo.Error);
                return ServiceResponse<ReversalRequestDto>.Return500(e, errorMessage);
            }
        }
    }

}
