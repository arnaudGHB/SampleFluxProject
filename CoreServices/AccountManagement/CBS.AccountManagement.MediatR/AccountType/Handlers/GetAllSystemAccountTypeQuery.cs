using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountType based on its unique identifier.
    /// </summary>
    public class GetSystemAccountTypeQueryHandler : IRequestHandler<GetAllSystemAccountTypeQuery, ServiceResponse<List<AccountTypeDto>>>
    {
        private readonly IAccountTypeRepository _accountTypeRepository; // Repository for accessing AccountType data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<GetSystemAccountTypeQueryHandler> _logger; // Logger for logging handler actions and errors.
      /// <summary>
        /// Constructor for initializing the GetAccountTypeQueryHandler.
        /// </summary>
        /// <param name="AccountTypeRepository">Repository for AccountType data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="userInfoToken">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetSystemAccountTypeQueryHandler(
                    IAccountTypeRepository accountTypeRepository,
            IMapper mapper,
            ILogger<GetSystemAccountTypeQueryHandler> logger,UserInfoToken userInfoToken)
        {
             
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _accountTypeRepository = accountTypeRepository;
        }

        /// <summary>
        /// Handles the GetAllSystemAccountTypeQuery to retrieve a specific AccountType.
        /// </summary>
        /// <param name="request">The GetAllSystemAccountTypeQuery containing AccountType ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountTypeDto>>> Handle(GetAllSystemAccountTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all AccountTypes entities from the repository
                var entities =  _accountTypeRepository.All.Where(x => x.IsDeleted.Equals(false)&& x.OperationAccountType.ToUpper()!= AccountType_Product.Saving_Product.ToString().ToUpper()&& x.OperationAccountType.ToUpper() != AccountType_Product.Loan_Product.ToString().ToUpper()).ToList();
                var errorMessag = $" GetAllAccountTypeQuery executed Successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllAccountTypeQuery",
                    JsonConvert.SerializeObject(request), errorMessag, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                return ServiceResponse<List<AccountTypeDto>>.ReturnResultWith200(_mapper.Map<List<AccountTypeDto>>(entities));
            }
            catch (Exception e)
            {

                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Failed to get all AccountTypes: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllAccountTypeQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<AccountTypeDto>>.Return500(e, errorMessage);
            }
        }

        private void LogAndAuditError(GetOperationEventAttributeByOperationAccountTypeIdQuery request, string errorMessage, LogLevelInfo logLevel, int statusCode)
        {
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, "GetSystemAccountTypeQuery",
                JsonConvert.SerializeObject(request), errorMessage, logLevel.ToString(), statusCode,_userInfoToken.Token).Wait();
        }

        private void LogAuditSuccess(GetOperationEventAttributeByOperationAccountTypeIdQuery request, string successMessage)
        {
          
            APICallHelper.AuditLogger(_userInfoToken.Email, "GetSystemAccountTypeQuery",
                JsonConvert.SerializeObject(request), successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token).Wait();
        }
    }
}