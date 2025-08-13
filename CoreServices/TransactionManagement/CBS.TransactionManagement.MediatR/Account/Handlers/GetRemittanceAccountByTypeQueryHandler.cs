using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of a remittance account based on AccountType and BranchId.
    /// </summary>
    public class GetRemittanceAccountByTypeQueryHandler : IRequestHandler<GetRemittanceAccountByTypeQuery, ServiceResponse<AccountDto>>
    {
        private readonly IAccountRepository _accountRepository; // Repository for accessing accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetRemittanceAccountByTypeQueryHandler> _logger; // Logger for logging actions and errors.

        /// <summary>
        /// Constructor for initializing the GetRemittanceAccountByTypeQueryHandler.
        /// </summary>
        /// <param name="accountRepository">Repository for accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        public GetRemittanceAccountByTypeQueryHandler(
            IAccountRepository accountRepository,
            IMapper mapper,
            ILogger<GetRemittanceAccountByTypeQueryHandler> logger)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetRemittanceAccountByTypeQuery to retrieve a remittance account based on AccountType and BranchId.
        /// </summary>
        /// <param name="request">The GetRemittanceAccountByTypeQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountDto>> Handle(GetRemittanceAccountByTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the account matching the AccountType and BranchId
                var account = await _accountRepository.All.FirstOrDefaultAsync(a => a.AccountType == request.AccountType && a.IsDeleted==false && a.BranchId == request.BranchId, cancellationToken);

                // If account is not found, return a 404 response
                if (account == null)
                {
                    _logger.LogWarning($"No account found for AccountType: {request.AccountType} and BranchId: {request.BranchId}");
                    return ServiceResponse<AccountDto>.Return404($"Account not found for AccountType: {request.AccountType} and BranchId: {request.BranchId}");
                }

                // Map the account to AccountDto and return
                return ServiceResponse<AccountDto>.ReturnResultWith200(_mapper.Map<AccountDto>(account));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to retrieve account for AccountType: {request.AccountType} and BranchId: {request.BranchId}. Error: {e.Message}");
                return ServiceResponse<AccountDto>.Return500(e, $"Failed to retrieve account for AccountType: {request.AccountType} and BranchId: {request.BranchId}");
            }
        }
    }
}
