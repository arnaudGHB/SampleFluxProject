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

    public class UploadLinkAccountTypeAccountFeatureQueryHandler : IRequestHandler<UploadLinkAccountTypeAccountFeatureQuery, ServiceResponse<List<LinkAccountTypeAccountFeatureDto>>>
    {
        // Dependencies
        private readonly ILinkAccountTypeAccountFeatureRepository _LinkAccountTypeAccountFeatureRepository;
        private readonly ILogger<UploadAccountCartegoriesQueryHandler> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper;

        // Constructor to inject dependencies
        public UploadLinkAccountTypeAccountFeatureQueryHandler(ILinkAccountTypeAccountFeatureRepository LinkAccountTypeAccountFeatureRepository,
            ILogger<UploadAccountCartegoriesQueryHandler> logger,
           UserInfoToken userInfoToken,
            IMapper mapper)
        {
            _LinkAccountTypeAccountFeatureRepository = LinkAccountTypeAccountFeatureRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
        }

        // Handle method implementation
        public async Task<ServiceResponse<List<LinkAccountTypeAccountFeatureDto>>> Handle(UploadLinkAccountTypeAccountFeatureQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if there are existing LinkAccountTypeAccountFeatures in the repository
                var entityExists = _LinkAccountTypeAccountFeatureRepository.All.Count() == 0;

                if (entityExists)
                {
                    // Map DTOs to entity models
                    var listLinkAccountTypeAccountFeatures = _mapper.Map<List<LinkAccountTypeAccountFeature>>(request.LinkAccountTypeAccountFeature);

                    listLinkAccountTypeAccountFeatures= LinkAccountTypeAccountFeature.AddWithEntity(listLinkAccountTypeAccountFeatures,_userInfoToken);
                    // Add the new LinkAccountTypeAccountFeatures to the repository
                    _LinkAccountTypeAccountFeatureRepository.AddRange(listLinkAccountTypeAccountFeatures);

                    // Map the added LinkAccountTypeAccountFeatures back to DTOs
                    var LinkAccountTypeAccountFeaturesDto = _mapper.Map<List<LinkAccountTypeAccountFeatureDto>>(request.LinkAccountTypeAccountFeature);

                    // Return a successful response with the added LinkAccountTypeAccountFeatures
                    return ServiceResponse<List<LinkAccountTypeAccountFeatureDto>>.ReturnResultWith200(LinkAccountTypeAccountFeaturesDto);
                }
                else
                {
                    var messageError = "LinkAccountTypeAccountFeature has already been configured.";
                    // Log an error if LinkAccountTypeAccountFeature list is empty
                    _logger.LogError(messageError);

                    // Return a not found response
                    return ServiceResponse<List<LinkAccountTypeAccountFeatureDto>>.Return404(messageError);
                }
            }
            catch (Exception e)
            {
                // Log an error if an exception occurs during processing
                var errorMessage = $"Error occurred while reading the LinkAccountTypeAccountFeature configurations: {e.Message}";
                _logger.LogError(errorMessage);

                // Return a server error response
                return ServiceResponse<List<LinkAccountTypeAccountFeatureDto>>.Return500(errorMessage);
            }
        }
    }
}
