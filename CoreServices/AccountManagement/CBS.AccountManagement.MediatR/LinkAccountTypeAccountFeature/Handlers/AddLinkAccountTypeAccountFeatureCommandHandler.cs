using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new LinkAccountTypeAccountFeature.
    /// </summary>
    public class AddLinkAccountTypeAccountFeatureCommandHandler : IRequestHandler<AddLinkAccountTypeAccountFeatureCommand, ServiceResponse<LinkAccountTypeAccountFeatureDto>>
    {
        private readonly ILinkAccountTypeAccountFeatureRepository _LinkAccountTypeAccountFeatureRepository; // Repository for accessing LinkAccountTypeAccountFeature data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLinkAccountTypeAccountFeatureCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the AddLinkAccountTypeAccountFeatureCommandHandler.
        /// </summary>
        /// <param name="LinkAccountTypeAccountFeatureRepository">Repository for LinkAccountTypeAccountFeature data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLinkAccountTypeAccountFeatureCommandHandler(
            ILinkAccountTypeAccountFeatureRepository LinkAccountTypeAccountFeatureRepository,
            IMapper mapper,
            ILogger<AddLinkAccountTypeAccountFeatureCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _LinkAccountTypeAccountFeatureRepository = LinkAccountTypeAccountFeatureRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddLinkAccountTypeAccountFeatureCommand to add a new LinkAccountTypeAccountFeature.
        /// </summary>
        /// <param name="request">The AddLinkAccountTypeAccountFeatureCommand containing LinkAccountTypeAccountFeature data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LinkAccountTypeAccountFeatureDto>> Handle(AddLinkAccountTypeAccountFeatureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a LinkAccountTypeAccountFeature with the same name already exists (case-insensitive)
                var existingLinkAccountTypeAccountFeature = await _LinkAccountTypeAccountFeatureRepository.FindBy(c => c.LinkName == request.LinkName.ToUpper()).FirstOrDefaultAsync();

                // If a LinkAccountTypeAccountFeature with the same name already exists, return a conflict response
                if (existingLinkAccountTypeAccountFeature != null)
                {
                    var errorMessage = $"LinkAccountTypeAccountFeature {request.LinkName} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LinkAccountTypeAccountFeatureDto>.Return409(errorMessage);
                }

                // Map the AddLinkAccountTypeAccountFeatureCommand to a LinkAccountTypeAccountFeature entity
                var LinkAccountTypeAccountFeatureEntity = _mapper.Map<LinkAccountTypeAccountFeature>(request);

                LinkAccountTypeAccountFeatureEntity = LinkAccountTypeAccountFeature.SetAccountFeatureEntity(LinkAccountTypeAccountFeatureEntity, _userInfoToken);
                // Add the new LinkAccountTypeAccountFeature entity to the repository
                _LinkAccountTypeAccountFeatureRepository.Add(LinkAccountTypeAccountFeatureEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<LinkAccountTypeAccountFeatureDto>.Return500();
                }
                // Map the LinkAccountTypeAccountFeature entity to LinkAccountTypeAccountFeatureDto and return it with a success response
                var LinkAccountTypeAccountFeatureDto = _mapper.Map<LinkAccountTypeAccountFeatureDto>(LinkAccountTypeAccountFeatureEntity);
                return ServiceResponse<LinkAccountTypeAccountFeatureDto>.ReturnResultWith200(LinkAccountTypeAccountFeatureDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving LinkAccountTypeAccountFeature: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LinkAccountTypeAccountFeatureDto>.Return500(e);
            }
        }
    }
}