using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Account based on DeleteAccountCommand.
    /// </summary>
    public class DeleteAccountingRuleEntryCommandHandler : IRequestHandler<DeleteAccountingRuleEntryCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingRuleEntryRepository _AccountingRuleEntryRepository; // Repository for accessing Account data.
        private readonly ILogger<DeleteAccountingRuleEntryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteAccountingRuleEntryCommandHandler(
            IAccountingRuleEntryRepository AccountingRuleEntryRepository, IMapper mapper,
            ILogger<DeleteAccountingRuleEntryCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _AccountingRuleEntryRepository = AccountingRuleEntryRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteAccountCommand to delete a Account.
        /// </summary>
        /// <param name="request">The DeleteAccountCommand containing Account ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteAccountingRuleEntryCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Account entity with the specified ID exists
                var existingAccount = await _AccountingRuleEntryRepository.FindAsync(request.Id);
                if (existingAccount == null)
                {
                    errorMessage = $"Account with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingAccount.IsDeleted = true;

                _AccountingRuleEntryRepository.Update(existingAccount);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Account: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}