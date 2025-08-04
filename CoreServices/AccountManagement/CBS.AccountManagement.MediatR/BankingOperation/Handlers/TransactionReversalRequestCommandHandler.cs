using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.BankingOperation.Commands;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountCategory.
    /// </summary>
    public class TransactionReversalRequestCommandHandler : IRequestHandler<TransactionReversalRequestCommand, ServiceResponse<bool>>
    {
        private readonly ITransactionReversalRequestRepository _transactionReversalRequestRepository; // Repository for accessing AccountCategory data.
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<TransactionReversalRequestCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        /// <summary>
        /// Constructor for initializing the AddCashInfusionCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountClass data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public TransactionReversalRequestCommandHandler(ITransactionReversalRequestRepository transactionReversalRequestRepository, IAccountClassRepository AccountClassRepository,
            ILogger<TransactionReversalRequestCommandHandler> logger,
           UserInfoToken userInfoToken,
           IAccountingEntryRepository accountingEntryRepository,
            IMapper mapper, IUnitOfWork<POSContext> work)
        {
            _transactionReversalRequestRepository = transactionReversalRequestRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _uow = work;
        }

        /// <summary>
        /// Handles the AddAccountClassCommand to add a new AccountClass.
        /// </summary>
        /// <param name="request">The AddAccountClassCommand containing AccountClass data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(TransactionReversalRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (_accountingEntryRepository.FindBy(x => x.ReferenceID == request.ReferenceNumber ).Any())
                {
                    var Entity = TransactionReversalRequest.SetTransactionReversalRequest(request.ReferenceNumber, request.RequestMessage, _userInfoToken);
                    Entity.IssuedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                    Entity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "RVT");
                    _transactionReversalRequestRepository.Add(Entity);
                    await _uow.SaveAsync();


                    return ServiceResponse<bool>.ReturnResultWith200(true);
                }
                else
                {
                    return ServiceResponse<bool>.ReturnResultWith200(false);
                }
              
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while Adding CashInfusionCommands: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e + errorMessage);
            }
        }
    }
}