using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Account.
    /// </summary>
    public class AddRuleEntryCommandHandler : IRequestHandler<AddRuleEntryCommand, bool>
    {
        private readonly IAccountingRuleEntryRepository _AccountingRuleEntryRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountingRuleEntryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IOperationEventAttributeRepository _eventAttributeRepository;
        /// <summary>
        /// Constructor for initializing the AddAccountingRuleEntryCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddRuleEntryCommandHandler(
            IAccountingRuleEntryRepository AccountingRuleEntryRepository, IOperationEventAttributeRepository eventAttributeRepository,
            IMapper mapper,
            ILogger<AddAccountingRuleEntryCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _eventAttributeRepository = eventAttributeRepository;
            _AccountingRuleEntryRepository = AccountingRuleEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken= userInfoToken;
        }

        /// <summary>
        /// Handles the AddAccountingRuleEntryCommand to add a new Account.
        /// </summary>
        /// <param name="request">The AddAccountingRuleEntryCommand containing Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<bool> Handle(AddRuleEntryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                List<AccountingRuleEntry> AccountingRuleEntries = new List<AccountingRuleEntry>();
                foreach (var item in request.ruleEntries)
                {
                    var attributes = await _eventAttributeRepository.FindAsync(item.OperationEventAttributeId);
                 
                    var AccountingRuleEntryEntity = new AccountingRuleEntry
                    {

                        //OperationEventId = item.OperationEventId.Trim(),
                        AccountingRuleEntryName = $"{item.AccountingRuleEntryName} Accounting Entry Rule ",
                        OperationEventAttributeId = item.OperationEventAttributeId.Trim(),
                        IsPrimaryEntry = item.IsPrimaryEntry,
                        BalancingAccountId= "e1274f63-e1eb-4cf6-ab71-cf7c549c2204",
                        DeterminationAccountId= item.DeterminationAccountId,
                        EventCode ="",
                        CreatedBy = _userInfoToken.Id,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        ModifiedBy = _userInfoToken.Id,
                        IsDeleted = false,
                        BookingDirection = item.IsPrimaryEntry?"DEBIT":"CREDIT",
                      Id = BaseUtilities.GenerateUniqueNumber()
                };
                    AccountingRuleEntries.Add(AccountingRuleEntryEntity);
                }

                // Check if a Account with the same name already exists (case-insensitive)
                _AccountingRuleEntryRepository.AddRange(AccountingRuleEntries);

                // Map the AddAccountingRuleEntryCommand to a AccountingRuleEntry entity
                //await _uow.SaveAsync(); 
                return true;
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Account: {e.Message}";
                _logger.LogError(errorMessage);
                return false;
            }
        }

       
    }
}