using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR 
{

    public class UploadAccountClassesQueryHandler : IRequestHandler<UploadAccountClassesQuery, ServiceResponse<List<AccountClassDto>>>
    {
        // Dependencies
        private readonly IAccountClassRepository _AccountClassRepository;
        private readonly ILogger<UploadAccountClassesQueryHandler> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper;

        // Constructor to inject dependencies
        public UploadAccountClassesQueryHandler(IAccountClassRepository AccountClassRepository,
            IAccountCategoryRepository AccountCategoryRepository,
            ILogger<UploadAccountClassesQueryHandler> logger,
           UserInfoToken userInfoToken,
            IMapper mapper)
        {
            _AccountClassRepository = AccountClassRepository;
 
             _userInfoToken = userInfoToken;
        _logger = logger;
            _mapper = mapper;
        }

        // Handle method implementation
        public async Task<ServiceResponse<List<AccountClassDto>>> Handle(UploadAccountClassesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if there are existing AccountCategorys in the repository
                var entityExists = _AccountClassRepository.All.Count() == 0;

                if (entityExists)
                {
                    // Map DTOs to entity models
                    var listAccountCategorys = _mapper.Map<List<AccountClass>>(request.AccountClasses);

                   
                    listAccountCategorys = AccountClass.SetAccountCategoriesEntities(listAccountCategorys, _userInfoToken);
                    // Add the new AccountCategorys to the repository
                    _AccountClassRepository.AddRange(listAccountCategorys);

                    // Map the added AccountCategorys back to DTOs
                    var AccountClasses = _mapper.Map<List<AccountClassDto>>(request.AccountClasses);

                    // Return a successful response with the added AccountCategorys
                    return ServiceResponse<List<AccountClassDto>>.ReturnResultWith200(request.AccountClasses);
                }
                else
                {
                    var messageError = "AccountClass has already been configured.";
                    // Log an error if AccountCategory list is empty
                    _logger.LogError(messageError);

                    // Return a not found response
                    return ServiceResponse<List<AccountClassDto>>.Return404(messageError);
                }
            }
            catch (Exception e)
            {
                // Log an error if an exception occurs during processing
                var errorMessage = $"Error occurred while reading the AccountCategory configurations: {e.Message}";
                _logger.LogError(errorMessage);

                // Return a server error response
                return ServiceResponse<List<AccountClassDto>>.Return500(errorMessage);
            }
        }

      
    }
}
