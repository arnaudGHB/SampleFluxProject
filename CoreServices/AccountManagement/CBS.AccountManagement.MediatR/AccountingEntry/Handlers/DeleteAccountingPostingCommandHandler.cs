using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Account based on DeleteAccountCommand.
    /// </summary>
    public class DeleteAccountingPostingCommandHandler : IRequestHandler<DeleteAccountPostingCommand, ServiceResponse<List<AccountingEntryDto>>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository; // Repository for accessing Account data.
        private readonly ILogger<DeleteAccountingPostingCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteAccountingPostingCommandHandler(
            IAccountingEntryRepository accountingEntryRepository, IMapper mapper,
            ILogger<DeleteAccountingPostingCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _accountingEntryRepository = accountingEntryRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteAccountCommand to delete a Account.
        /// </summary>
        /// <param name="request">The DeleteAccountCommand containing Account ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List< AccountingEntryDto>>> Handle(DeleteAccountPostingCommand request, CancellationToken cancellationToken)
        {
            List<AccountingEntryDto> results = new();
            string errorMessage = null;
            try
            {
                // Check if the Account entity with the specified ID exists
                var existingEntries =   _accountingEntryRepository.FindBy(x=>x.ReferenceID==(request.referenceId)  );
                if (existingEntries == null)
                {
                    errorMessage = $"Account with ID {request.referenceId} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<List<AccountingEntryDto>>.Return404(errorMessage);
                }

                foreach (var existingEntry in existingEntries)
                {
                    existingEntry.IsDeleted = true;
                    _accountingEntryRepository.Update(existingEntry);
                    var model = _mapper.Map<AccountingEntryDto>(existingEntry);
                    
                    results.Add(model);
                }
               

             
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<List<AccountingEntryDto>>.Return500();
                }
                return ServiceResponse<List<AccountingEntryDto>>.ReturnResultWith200(results);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Account: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return    ServiceResponse<List<AccountingEntryDto>>.Return500(e);
            }
        }
    }
}