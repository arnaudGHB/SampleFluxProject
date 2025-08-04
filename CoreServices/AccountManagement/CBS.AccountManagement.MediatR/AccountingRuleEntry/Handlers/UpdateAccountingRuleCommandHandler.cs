using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a Account based on UpdateAccountCommand.
    /// </summary>
    public class UpdateAccountingRuleEntryCommandHandler : IRequestHandler<UpdateAccountingRuleEntryCommand, ServiceResponse<AccountingRuleEntryDto>>
    {
        private readonly IAccountingRuleEntryRepository _AccountingRuleEntryRepository; // Repository for accessing Account data.
        private readonly ILogger<UpdateAccountingRuleEntryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IOperationEventRepository _operationEventRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        /// <summary>
        /// Constructor for initializing the UpdateAccountCommandHandler.
        /// </summary>
        /// <param name="AccountingRuleEntryRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateAccountingRuleEntryCommandHandler(
            IAccountingRuleEntryRepository AccountingRuleEntryRepository,
            ILogger<UpdateAccountingRuleEntryCommandHandler> logger,
            IMapper mapper,
            IOperationEventRepository operationEventRepository,
            IOperationEventAttributeRepository operationEventAttributeRepository,
            IUnitOfWork<POSContext> uow = null)
        {
            _AccountingRuleEntryRepository = AccountingRuleEntryRepository;
            _operationEventRepository = operationEventRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateAccountingRuleEntry Command to update a AccountingRuleEntry.
        /// </summary>
        /// <param name="request">The UpdateAccountingRuleEntryCommand containing updated AccountingRuleEntry data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountingRuleEntryDto>> Handle(UpdateAccountingRuleEntryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Account entity to be updated from the repository
                var existingAccount = await _AccountingRuleEntryRepository.FindAsync(request.Id);

                // Check if the Account entity exists
                if (existingAccount != null)
                {
                    // Update Account entity properties with values from the request
                    var modelOA = await _operationEventAttributeRepository.FindAsync(existingAccount.OperationEventAttributeId);
                    if (modelOA == null) {
                        string errorMessage = $"There no OperationEventAttribut:{existingAccount.OperationEventAttributeId} was not found to be updated.";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<AccountingRuleEntryDto>.Return404(errorMessage);
                    }
                    var modelO = await _operationEventRepository.FindAsync(modelOA.OperationEventId);
                    if (modelO == null)
                    {
                        string errorMessage = $"There no OperationEvent:{existingAccount.OperationEventAttributeId} was not found to be updated.";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<AccountingRuleEntryDto>.Return404(errorMessage);
                    }
                    existingAccount = _mapper.Map(request, existingAccount);
                    existingAccount.BookingDirection = request.BookingDirection.ToUpper();
                    existingAccount.EventCode = modelO.EventCode;
                    // Use the repository to update the existing Account entity
                    _AccountingRuleEntryRepository.Update(existingAccount);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<AccountingRuleEntryDto>.ReturnResultWith200(_mapper.Map<AccountingRuleEntryDto>(existingAccount));
                    _logger.LogInformation($"Account {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Account entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<AccountingRuleEntryDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Account: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountingRuleEntryDto>.Return500(e);
            }
        }
    }
}