using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountClass based on its unique identifier.
    /// </summary>
    public class GetAccountCategoryByAccountNumberQueryHandler : IRequestHandler<GetAccountCategoryByAccountNumberQuery, ServiceResponse<List<AccountClassCategoryDto>>>
    {
        private readonly IAccountCategoryRepository _accountCategoryRepository;
        private readonly IAccountClassRepository _AccountClassRepository; // Repository for accessing AccountClass data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountCategoryByAccountNumberQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAccountClassQueryHandler.
        /// </summary>
        /// <param name="AccountClassRepository">Repository for AccountClass data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountCategoryByAccountNumberQueryHandler(
            IAccountClassRepository AccountClassRepository,
               IAccountCategoryRepository AccountCategoryRepository,
            IMapper mapper,
            ILogger<GetAccountCategoryByAccountNumberQueryHandler> logger)
        {
            _AccountClassRepository = AccountClassRepository;
            _accountCategoryRepository = AccountCategoryRepository;
            _mapper = mapper;
            _logger = logger;
        }
         
        /// <summary>
        /// Handles the GetAccountClassQuery to retrieve a specific AccountClass.
        /// </summary>
        /// <param name="request">The GetAccountClassQuery containing AccountClass ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountClassCategoryDto>>> Handle(GetAccountCategoryByAccountNumberQuery request, CancellationToken cancellationToken)
        {
            string accountNumber = "";
            string errorMessage = "";
            List < AccountClassCategoryDto > models = new List<AccountClassCategoryDto>();
         
            
            try
            {
                accountNumber = accountNumber.Length == 1 ? request.AccountNumber : request.AccountNumber.Substring(0, 1);


                    var listAccountCategorys =  _AccountClassRepository.FindBy(c => c.AccountNumber == accountNumber && c.IsDeleted == false).ToList();
                    foreach (var item in listAccountCategorys)
                    {
                        AccountClassCategoryDto accountClass= new AccountClassCategoryDto();
                      
                        accountClass.Id=item.AccountCategoryId;
                       var modelName = await _accountCategoryRepository.FindAsync(item.AccountCategoryId);
                        accountClass.Name = modelName.Name;
                       models.Add(accountClass);
                    }
                    return ServiceResponse<List<AccountClassCategoryDto>>.ReturnResultWith200(models);
              

    
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountClass: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<AccountClassCategoryDto>>.Return500(e);
            }
        }
    }
}