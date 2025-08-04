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
    /// Handles the command to add a new AccountFeature.
    /// </summary>
    public class AddAccountFeatureCommandHandler : IRequestHandler<AddAccountFeatureCommand, ServiceResponse<AccountFeatureDto>>
    {
        private readonly IAccountFeatureRepository _AccountFeatureRepository; // Repository for accessing AccountFeature data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountFeatureCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the AddAccountFeatureCommandHandler.
        /// </summary>
        /// <param name="AccountFeatureRepository">Repository for AccountFeature data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountFeatureCommandHandler(
            IAccountFeatureRepository AccountFeatureRepository,
            IMapper mapper,
            ILogger<AddAccountFeatureCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _AccountFeatureRepository = AccountFeatureRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddAccountFeatureCommand to add a new AccountFeature.
        /// </summary>
        /// <param name="request">The AddAccountFeatureCommand containing AccountFeature data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountFeatureDto>> Handle(AddAccountFeatureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a AccountFeature with the same name already exists (case-insensitive)
                var existingAccountFeature = await _AccountFeatureRepository.FindBy(c => c.Name == request.Name.ToUpper()).FirstOrDefaultAsync();

                // If a AccountFeature with the same name already exists, return a conflict response
                if (existingAccountFeature != null)
                {
                    var errorMessage = $"AccountFeature {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<AccountFeatureDto>.Return409(errorMessage);
                }

                // Map the AddAccountFeatureCommand to a AccountFeature entity
                var AccountFeatureEntity = _mapper.Map<AccountFeature>(request);

                AccountFeatureEntity = AccountFeature.SetAccountFeatureEntity(AccountFeatureEntity, _userInfoToken);
                // Add the new AccountFeature entity to the repository
                _AccountFeatureRepository.Add(AccountFeatureEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<AccountFeatureDto>.Return500();
                }
                // Map the AccountFeature entity to AccountFeatureDto and return it with a success response
                var AccountFeatureDto = _mapper.Map<AccountFeatureDto>(AccountFeatureEntity);
                return ServiceResponse<AccountFeatureDto>.ReturnResultWith200(AccountFeatureDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving AccountFeature: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountFeatureDto>.Return500(e);
            }
        }
    }
}