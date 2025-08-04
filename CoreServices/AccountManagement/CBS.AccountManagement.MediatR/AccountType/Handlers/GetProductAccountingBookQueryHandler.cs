using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountType based on its unique identifier.
    /// </summary>
    public class GetProductAccountingBookQueryHandler : IRequestHandler<GetProductAccountingBookQuery, ServiceResponse<List<ProductAccountingBookDto>>>
    {
        private readonly IProductAccountingBookRepository _ProductAccountingBookRepository; // Repository for accessing AccountType data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<GetProductAccountingBookQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAccountTypeQueryHandler.
        /// </summary>
        /// <param name="AccountTypeRepository">Repository for AccountType data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="userInfoToken">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetProductAccountingBookQueryHandler(
            IProductAccountingBookRepository AccountTypeRepository,
            IMapper mapper,
            ILogger<GetProductAccountingBookQueryHandler> logger,UserInfoToken userInfoToken)
        {
            _ProductAccountingBookRepository = AccountTypeRepository; 
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAccountTypeQuery to retrieve a specific AccountType.
        /// </summary>
        /// <param name="request">The GetAccountTypeQuery containing AccountType ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ProductAccountingBookDto>>> Handle(GetProductAccountingBookQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
            
                var entity =   GetExistingAccountType(request);
                if (entity != null)
                {
                  
                  
                        // Map the AccountType entity to AccountTypeDto and return it with a success response
                        var AccountTypeDto = _mapper.Map<List<ProductAccountingBookDto>>(entity);
                        string successMessage = $"Gotten ProductAccountingBookDto for productId {request.Id} successfully.";
                        LogAuditSuccess( request, successMessage);
                        
                        return ServiceResponse < List < ProductAccountingBookDto >> .ReturnResultWith200(AccountTypeDto);
                        
                     
                }
                else
                {
                    // If the AccountType entity was not found, log the error and return a 404 Not Found response
                    errorMessage= "ProductAccountingBookDto not found.";
                    _logger.LogError(errorMessage);
                    LogAndAuditError(request, errorMessage, LogLevelInfo.Error,  404);
                    return ServiceResponse<List<ProductAccountingBookDto>>.Return404(errorMessage);
                    
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountType: {e.Message}";
                LogAndAuditError(request, errorMessage, LogLevelInfo.Error,  500);
                _logger.LogError(errorMessage);
                return ServiceResponse<List<ProductAccountingBookDto>>.Return404(errorMessage);
 
            }
        }
        private  List<ProductAccountingBook> GetExistingAccountType(GetProductAccountingBookQuery request)
        {
            return  (_ProductAccountingBookRepository.FindBy(x => x.ProductAccountingBookId == request.Id)).ToList  ();
               
        }

        private void LogAndAuditError(GetProductAccountingBookQuery request, string errorMessage, LogLevelInfo logLevel, int statusCode)
        {
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                JsonConvert.SerializeObject(request), errorMessage, logLevel.ToString(), statusCode,_userInfoToken.Token).Wait();
        }

        private void LogAuditSuccess(GetProductAccountingBookQuery request, string successMessage)
        {
          
            APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                JsonConvert.SerializeObject(request), successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token).Wait();
        }
    }
}