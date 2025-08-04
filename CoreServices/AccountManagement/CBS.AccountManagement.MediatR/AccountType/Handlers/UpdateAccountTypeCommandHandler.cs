using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using Newtonsoft.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.Data.Enum;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a AccountType based on UpdateAccountTypeCommand.
    /// </summary>
    public class UpdateAccountTypeCommandHandler : IRequestHandler<UpdateAccountTypeCommand, ServiceResponse<AccountTypeDto>>
    {
        private readonly IAccountTypeRepository _AccountTypeRepository; // Repository for accessing AccountType data.
        private readonly ILogger<UpdateAccountTypeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly UserInfoToken _userInfoToken;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IProductAccountingBookRepository _productAccountingBookRepository; // Repository for accessing AccountType data.
        /// <summary>
        /// Constructor for initializing the UpdateAccountTypeCommandHandler.
        /// </summary>
        /// <param name="AccountTypeRepository">Repository for AccountType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateAccountTypeCommandHandler(
            UserInfoToken userInfoToken,
            IAccountTypeRepository AccountTypeRepository,
            ILogger<UpdateAccountTypeCommandHandler> logger,
            IMapper mapper,
             IProductAccountingBookRepository productAccountingBookRepository,
            IUnitOfWork<POSContext> uow = null)
        {
            _AccountTypeRepository = AccountTypeRepository;
            _productAccountingBookRepository = productAccountingBookRepository;
            _logger = logger;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateAccountTypeCommand to update a AccountType.
        /// </summary>
        /// <param name="request">The UpdateAccountTypeCommand containing updated AccountType data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountTypeDto>> Handle(UpdateAccountTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Account entity to be updated from the repository
                var existingAccount = await _AccountTypeRepository.FindAsync(request.Id);

                // Check if the Account entity exists
                if (existingAccount != null)
                {
                    // Update Account entity properties with values from the request
                    existingAccount = _mapper.Map(request, existingAccount);
                    // Use the repository to update the existing Account entity
                    _AccountTypeRepository.Update(existingAccount);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<AccountTypeDto>.ReturnResultWith200(_mapper.Map<AccountTypeDto>(existingAccount));
                    _logger.LogInformation($"AccountType {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Account entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<AccountTypeDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating AccountType: {e.Message}";
                LogAndAuditError(request, errorMessage, LogLevelInfo.Information, 500);
                return ServiceResponse<AccountTypeDto>.Return500(e);
            }

        }
        private bool GetAccountType(string operationAccountType)
        {
            return operationAccountType.ToLower() == AccountType_Product.Saving_Product.ToString().ToLower() || operationAccountType.ToLower() == AccountType_Product.Loan_Product.ToString().ToLower();
        }


        private void LogAndAuditError(UpdateAccountTypeCommand request, string errorMessage, LogLevelInfo logLevel, int statusCode)
        {
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateProductAccountingBookCommand",
                JsonConvert.SerializeObject(request), errorMessage, logLevel.ToString(), statusCode, _userInfoToken.Token).Wait();
        }

        private void LogAuditSuccess(UpdateAccountTypeCommand request)
        {
            string successMessage = "AccountType update successfully.";
            APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateProductAccountingBookCommand",
                JsonConvert.SerializeObject(request), successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token).Wait();
        }
    }
}