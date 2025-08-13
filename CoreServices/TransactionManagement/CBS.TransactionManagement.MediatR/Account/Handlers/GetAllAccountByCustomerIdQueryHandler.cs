using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using DocumentFormat.OpenXml.Spreadsheet;
using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Repository.WithdrawalNotificationP;
using Newtonsoft.Json;

namespace CBS.TransactionManagement.GAV.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetAllAccountByCustomerIdQuery.
    /// </summary>
    public class GetAllAccountByCustomerIdQueryHandler : IRequestHandler<GetAllAccountByCustomerIdQuery, ServiceResponse<List<AccountDto>>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountByCustomerIdQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly ISavingProductRepository _savingProductRepository; // Repository for accessing Accounts data.
        private readonly IWithdrawalNotificationRepository _withdrawalNotificationRepository; // Repository for accessing Accounts data.
        private readonly IWithdrawalLimitsRepository _withdrawalLimitsRepository; // Repository for accessing Accounts data.

        /// <summary>
        /// Constructor for initializing the GetAllAccountByCustomerIdQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountByCustomerIdQueryHandler(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllAccountByCustomerIdQueryHandler> logger, ISavingProductRepository savingProductRepository, IWithdrawalNotificationRepository withdrawalNotificationRepository, IWithdrawalLimitsRepository withdrawalLimitsRepository = null)
        {
            // Assign provided dependencies to local variables.
            _AccountRepository = AccountRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
            _savingProductRepository = savingProductRepository;
            _withdrawalNotificationRepository = withdrawalNotificationRepository;
            _withdrawalLimitsRepository = withdrawalLimitsRepository;
        }

        /// <summary>
        /// Handles the GetAllAccountByCustomerIdQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetAllAccountByCustomerIdQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountDto>>> Handle(GetAllAccountByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Fetch accounts without navigation properties
                var accounts = await _AccountRepository
                    .FindBy(a => a.CustomerId == request.CustomerId && a.IsDeleted == false)
                    .AsNoTracking()
                    .ToListAsync();

                // Step 2: Use AutoMapper to map the list
                var accountDtos = _mapper.Map<List<AccountDto>>(accounts);

                // Step 3: Attach virtual properties after mapping
                foreach (var accountDto in accountDtos)
                {
                    // Fetch the corresponding account to get IDs for related entities
                    var account = accounts.FirstOrDefault(a => a.Id == accountDto.Id);

                    if (account != null)
                    {
                        // Load the related Product entity and attach it to the DTO
                        var product = await _savingProductRepository
                            .FindBy(p => p.Id == account.ProductId)
                            .AsNoTracking()
                            .FirstOrDefaultAsync();

                        if (product != null)
                        {
                            accountDto.Product = product;
                        }

                        // Load related WithdrawalNotifications and attach them to the DTO
                        var withdrawalNotifications = await _withdrawalNotificationRepository
                            .FindBy(wn => wn.AccountId == account.Id && !wn.IsDeleted && !wn.IsWithdrawalDone)
                            .AsNoTracking()
                            .ToListAsync();

                        if (withdrawalNotifications.Any())
                        {
                            accountDto.WithdrawalNotifications = withdrawalNotifications;
                        }
                        // Load related WithdrawalNotifications and attach them to the DTO
                        var withdrawalParameters = await _withdrawalLimitsRepository
                            .FindBy(wn => wn.ProductId == account.ProductId && !wn.IsDeleted)
                            .AsNoTracking()
                            .ToListAsync();

                        if (withdrawalParameters.Any())
                        {
                            accountDto.Product.WithdrawalParameters = withdrawalParameters;
                        }
                    }
                }
                //_AccountRepository.UpdateAccountNumber(accounts);
                // Log the successful operation
                await BaseUtilities.LogAndAuditAsync(
                    $"Customer: [{request.CustomerId}] accounts returned successfully. Number of accounts: {accounts.Count}. Accounts data: {JsonConvert.SerializeObject(accounts)}",
                    accounts,
                    HttpStatusCodeEnum.OK,
                    LogAction.Read,
                    LogLevelInfo.Information, request.CustomerId);
                // Return the mapped result
                return ServiceResponse<List<AccountDto>>.ReturnResultWith200(accountDtos);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {e.Message}");
                await BaseUtilities.LogAndAuditAsync($"Failed to get all Accounts: {e.Message}", request, HttpStatusCodeEnum.InternalServerError, LogAction.Read, LogLevelInfo.Error,request.CustomerId);
                return ServiceResponse<List<AccountDto>>.Return500(e, "Failed to get all Accounts");
            }
        }

        public async Task<ServiceResponse<List<AccountDto>>> Handlexxx(GetAllAccountByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                ////Retrieve all Accounts entities from the repository

                //var entities = _AccountRepository
                //    .FindBy(a => a.CustomerId == request.CustomerId && a.IsDeleted == false)
                //    //.Include(a => a.Product).ThenInclude(a => a.WithdrawalParameters).AsNoTracking()
                //    .Select(a => new
                //    {
                //        Account = a,
                //        FilteredWithdrawalNotifications = a.WithdrawalNotifications.Where(wn => !wn.IsDeleted && !wn.IsWithdrawalDone).ToList()
                //    })
                //    .AsEnumerable() // Execute the query and bring the data to the client side
                //    .Select(a =>
                //    {
                //        a.Account.WithdrawalNotifications = a.FilteredWithdrawalNotifications;
                //        return a.Account;
                //    }).ToList();
                var entities = await _AccountRepository.FindBy(a => a.CustomerId == request.CustomerId && a.IsDeleted == false).Include(x=>x.WithdrawalNotifications).Include(x=>x.Product).ThenInclude(a => a.WithdrawalParameters).AsNoTracking().ToListAsync();
                await BaseUtilities.LogAndAuditAsync($"Customer: [{request.CustomerId}] accounts returned successfully", entities,HttpStatusCodeEnum.OK,LogAction.Read,LogLevelInfo.Information, request.CustomerId);
                return ServiceResponse<List<AccountDto>>.ReturnResultWith200(_mapper.Map<List<AccountDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {e.Message}");
                await BaseUtilities.LogAndAuditAsync($"Failed to get all Accounts: {e.Message}", request, HttpStatusCodeEnum.InternalServerError, LogAction.Read, LogLevelInfo.Error, request.CustomerId);
                return ServiceResponse<List<AccountDto>>.Return500(e, "Failed to get all Accounts");
            }
        }
    }
}
