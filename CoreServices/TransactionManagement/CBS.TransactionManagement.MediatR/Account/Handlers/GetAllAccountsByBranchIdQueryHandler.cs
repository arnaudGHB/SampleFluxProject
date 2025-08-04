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
    /// Handles the retrieval of all Accounts based on the GetAllCustomerAccountsByBankIdAndBranchId.
    /// </summary>
    public class GetAllAccountsByBranchIdQueryHandler : IRequestHandler<GetAllAccountsByBranchIdQuery, ServiceResponse<List<AccountDto>>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountsByBranchIdQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllCustomerAccountsByBankIdAndBranchIdHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountsByBranchIdQueryHandler(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllAccountsByBranchIdQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _AccountRepository = AccountRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCustomerAccountsByBankIdAndBranchId to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetAllCustomerAccountsByBankIdAndBranchId containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountDto>>> Handle(GetAllAccountsByBranchIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Accounts entities from the repository
                var entities = _AccountRepository.FindBy(a => a.BranchId == request.BranchId && a.CustomerId != null).Include(p => p.Product);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Customer accounts returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<AccountDto>>.ReturnResultWith200(_mapper.Map<List<AccountDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all Accounts: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<AccountDto>>.Return500(e, "Failed to get all Accounts");
            }
        }
    }
}
