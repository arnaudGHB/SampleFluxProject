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

    public class UploadAccountCartegoriesQueryHandler : IRequestHandler<UploadAccountCategoriesQuery, ServiceResponse<List<AccountCartegoryDto>>>
    {
        // Dependencies
        private readonly IAccountCategoryRepository _AccountCategoryRepository;
        private readonly ILogger<UploadAccountCartegoriesQueryHandler> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper;

        // Constructor to inject dependencies
        public UploadAccountCartegoriesQueryHandler(IAccountCategoryRepository AccountCategoryRepository, 
            ILogger<UploadAccountCartegoriesQueryHandler> logger,
           UserInfoToken userInfoToken,
            IMapper mapper)
        {
            _AccountCategoryRepository = AccountCategoryRepository;
             _userInfoToken = userInfoToken;
        _logger = logger;
            _mapper = mapper;
        }

        // Handle method implementation
        public async Task<ServiceResponse<List<AccountCartegoryDto>>> Handle(UploadAccountCategoriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if there are existing AccountCategorys in the repository
                var entityExists = _AccountCategoryRepository.All.Count() == 0;

                if (entityExists)
                {
                    // Map DTOs to entity models
                    var listAccountCategorys = _mapper.Map<List<AccountCategory>>(request.AccountCategories);


                    listAccountCategorys = null;// AccountCategory.SetAccountCategoriesEntities(listAccountCategorys, _userInfoToken);
                    // Add the new AccountCategorys to the repository
                    _AccountCategoryRepository.AddRange(listAccountCategorys);

                    // Map the added AccountCategorys back to DTOs
                    var AccountCategorysDto = _mapper.Map<List<AccountCartegoryDto>>(request.AccountCategories);

                    // Return a successful response with the added AccountCategorys
                    return ServiceResponse<List<AccountCartegoryDto>>.ReturnResultWith200(AccountCategorysDto);
                }
                else
                {
                    var messageError = "AccountCategory has already been configured.";
                    // Log an error if AccountCategory list is empty
                    _logger.LogError(messageError);

                    // Return a not found response
                    return ServiceResponse<List<AccountCartegoryDto>>.Return404(messageError);
                }
            }
            catch (Exception e)
            {
                // Log an error if an exception occurs during processing
                var errorMessage = $"Error occurred while reading the AccountCategory configurations: {e.Message}";
                _logger.LogError(errorMessage);

                // Return a server error response
                return ServiceResponse<List<AccountCartegoryDto>>.Return500(errorMessage);
            }
        }
    }
}
