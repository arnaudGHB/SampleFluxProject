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

namespace CBS.AccountManagement.MediatR.Handlers
{

    public class UploadAccountFeatureQueryHandler : IRequestHandler<UploadAccountFeatureQuery, ServiceResponse<List<AccountFeatureDto>>>
    {
        // Dependencies
        private readonly IAccountFeatureRepository _AccountFeatureRepository;
        private readonly ILogger<UploadAccountCartegoriesQueryHandler> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper;

        // Constructor to inject dependencies
        public UploadAccountFeatureQueryHandler(IAccountFeatureRepository AccountFeatureRepository,
            ILogger<UploadAccountCartegoriesQueryHandler> logger,
           UserInfoToken userInfoToken,
            IMapper mapper)
        {
            _AccountFeatureRepository = AccountFeatureRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
        }

        // Handle method implementation
        public async Task<ServiceResponse<List<AccountFeatureDto>>> Handle(UploadAccountFeatureQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if there are existing AccountFeatures in the repository
                var entityExists = _AccountFeatureRepository.All.Count() == 0;

                if (entityExists)
                {
                    // Map DTOs to entity models
                    var listAccountFeatures = _mapper.Map<List<AccountFeature>>(request.AccountFeature);

                    listAccountFeatures= AccountFeature.AddWithEntity(listAccountFeatures,_userInfoToken);
                    // Add the new AccountFeatures to the repository
                    _AccountFeatureRepository.AddRange(listAccountFeatures);

                    // Map the added AccountFeatures back to DTOs
                    var AccountFeaturesDto = _mapper.Map<List<AccountFeatureDto>>(request.AccountFeature);

                    // Return a successful response with the added AccountFeatures
                    return ServiceResponse<List<AccountFeatureDto>>.ReturnResultWith200(AccountFeaturesDto);
                }
                else
                {
                    var messageError = "AccountFeature has already been configured.";
                    // Log an error if AccountFeature list is empty
                    _logger.LogError(messageError);

                    // Return a not found response
                    return ServiceResponse<List<AccountFeatureDto>>.Return404(messageError);
                }
            }
            catch (Exception e)
            {
                // Log an error if an exception occurs during processing
                var errorMessage = $"Error occurred while reading the AccountFeature configurations: {e.Message}";
                _logger.LogError(errorMessage);

                // Return a server error response
                return ServiceResponse<List<AccountFeatureDto>>.Return500(errorMessage);
            }
        }
    }
}
