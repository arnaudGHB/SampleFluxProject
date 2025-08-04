using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands.Global;
using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.REPOSITORY.OrganisationRepo;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Configuration;
using CBS.CUSTOMER.DATA.Entity.Document;
using CBS.CUSTOMER.REPOSITORY.DocumentRepo;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Entity;
using Microsoft.IdentityModel.Tokens;
using CBS.CUSTOMER.DATA.Entity.Groups;
using CBS.CUSTOMER.REPOSITORY.ConfigRepo;
using CBS.CUSTOMER.DATA.Entity.CMoney;
using CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Queries;
using CBS.CUSTOMER.DATA.Dto.CMoney;

namespace CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Handler
{
    /// <summary>
    /// Handles retrieval of Android version configuration by AppCode.
    /// Ensures structured logging, auditing, and error handling.
    /// </summary>
    public class GetAndriodVersionByAppCodeConfigurationHandler : IRequestHandler<GetAndriodVersionByAppCodeConfigurationQuery, ServiceResponse<AndriodVersionConfigurationDto>>
    {
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAndriodVersionConfigurationRepository _andriodVersionConfigurationRepository;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ILogger<GetAndriodVersionByAppCodeConfigurationHandler> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes the handler for retrieving Android version configuration by AppCode.
        /// </summary>
        public GetAndriodVersionByAppCodeConfigurationHandler(
            ILogger<GetAndriodVersionByAppCodeConfigurationHandler> logger,
            IAndriodVersionConfigurationRepository andriodVersionConfigurationRepository,
            IConfiguration configuration,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pathHelper = new PathHelper(configuration ?? throw new ArgumentNullException(nameof(configuration)));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
            _andriodVersionConfigurationRepository = andriodVersionConfigurationRepository ?? throw new ArgumentNullException(nameof(andriodVersionConfigurationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Handles retrieval of Android version configuration based on AppCode.
        /// </summary>
        public async Task<ServiceResponse<AndriodVersionConfigurationDto>> Handle(GetAndriodVersionByAppCodeConfigurationQuery request, CancellationToken cancellationToken)
        {
            string correlationId = Guid.NewGuid().ToString(); // Unique tracking ID for debugging

            try
            {
                // 🔍 Step 1: Validate Input
                if (string.IsNullOrWhiteSpace(request.AppCode))
                {
                    string validationError = $"[ERROR] AppCode is missing in request. CorrelationId: '{correlationId}'";
                    _logger.LogError(validationError);
                    await BaseUtilities.LogAndAuditAsync(validationError, request, HttpStatusCodeEnum.BadRequest, LogAction.Read, LogLevelInfo.Error,correlationId);
                    return ServiceResponse<AndriodVersionConfigurationDto>.Return404(validationError);
                }

                _logger.LogInformation($"[INFO] Retrieving Android version configuration for AppCode: '{request.AppCode}'. CorrelationId: '{correlationId}'");

                // 🔍 Step 2: Retrieve the Android version configuration
                var andriodVersionConfiguration = await _andriodVersionConfigurationRepository
                    .FindAsync(request.AppCode);

                if (andriodVersionConfiguration == null)
                {
                    string notFoundMessage = $"[ERROR] Android version configuration not found for AppCode: '{request.AppCode}'. CorrelationId: '{correlationId}'";
                    _logger.LogWarning(notFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.Read, LogLevelInfo.Warning,correlationId);
                    return ServiceResponse<AndriodVersionConfigurationDto>.Return404(notFoundMessage);
                }

                // 🔄 Step 3: Map entity to DTO
                var dto = _mapper.Map<AndriodVersionConfigurationDto>(andriodVersionConfiguration);

                // ✅ Step 4: Log and Audit Success
                string successMessage = $"[SUCCESS] Android version configuration retrieved successfully for AppCode: '{request.AppCode}'. CorrelationId: '{correlationId}'";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.Read, LogLevelInfo.Information,correlationId);

                return ServiceResponse<AndriodVersionConfigurationDto>.ReturnResultWith200(dto);
            }
            catch (Exception ex)
            {
                // ❌ Step 5: Handle Unexpected Errors
                string errorMessage = $"[ERROR] Failed to retrieve Android version configuration for AppCode: '{request.AppCode}'. Error: {ex.Message}. CorrelationId: '{correlationId}'";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.Read, LogLevelInfo.Error,correlationId);
                return ServiceResponse<AndriodVersionConfigurationDto>.Return500(errorMessage);
            }
        }
    }


}
