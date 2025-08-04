using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
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
using CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Command;
using CBS.CUSTOMER.DATA.Dto.CMoney;

namespace CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Handler
{
    /// <summary>
    /// Handles the addition or update of Android version configuration.
    /// Ensures structured logging, auditing, and data integrity.
    /// </summary>
    public class AddOrUpdateAndroidVersionHandler : IRequestHandler<AddOrUpdateAndroidVersionCommand, ServiceResponse<AndriodVersionConfigurationDto>>
    {
        private readonly IMapper _mapper;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAndriodVersionConfigurationRepository _andriodVersionConfigurationRepository;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ILogger<AddOrUpdateAndroidVersionHandler> _logger;

        /// <summary>
        /// Initializes the handler for adding or updating Android version configurations.
        /// </summary>
        public AddOrUpdateAndroidVersionHandler(
            IMapper mapper,
            ILogger<AddOrUpdateAndroidVersionHandler> logger,
            IAndriodVersionConfigurationRepository andriodVersionConfigurationRepository,
            IConfiguration configuration,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken
        )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pathHelper = new PathHelper(configuration ?? throw new ArgumentNullException(nameof(configuration)));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
            _andriodVersionConfigurationRepository = andriodVersionConfigurationRepository ?? throw new ArgumentNullException(nameof(andriodVersionConfigurationRepository));
        }

        /// <summary>
        /// Handles the command to add or update Android version configuration.
        /// </summary>
        public async Task<ServiceResponse<AndriodVersionConfigurationDto>> Handle(AddOrUpdateAndroidVersionCommand request, CancellationToken cancellationToken)
        {
            string correlationId = Guid.NewGuid().ToString(); // Unique tracking ID for logging

            try
            {
                // 🔍 Step 1: Validate Request Data
                if (string.IsNullOrEmpty(request.ApkUrl) || string.IsNullOrEmpty(request.Version))
                {
                    string errorMessage = $"[ERROR] ApkUrl or Version is missing in the request. CorrelationId: '{correlationId}'";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.Update, LogLevelInfo.Error,  correlationId);
                    return ServiceResponse<AndriodVersionConfigurationDto>.Return404(errorMessage);
                }

                // 🔍 Step 2: Check if an existing record needs updating
                var andriodVersionConfiguration = await _andriodVersionConfigurationRepository.FindAsync(request.Id);
                bool isNewRecord = andriodVersionConfiguration == null;

                if (isNewRecord)
                {
                    // 🚀 Step 3A: Insert New Record
                    andriodVersionConfiguration = _mapper.Map<AndriodVersionConfiguration>(request);
                    andriodVersionConfiguration.Id=BaseUtilities.GenerateUniqueNumber();
                    _andriodVersionConfigurationRepository.Add(andriodVersionConfiguration);
                    _logger.LogInformation($"[INFO] New Android version added. Version: '{request.Version}', ApkUrl: '{request.ApkUrl}'. CorrelationId: '{correlationId}'");
                }
                else
                {
                    // 🔄 Step 3B: Update Existing Record
                    _mapper.Map(request, andriodVersionConfiguration);
                    _andriodVersionConfigurationRepository.Update(andriodVersionConfiguration);
                    _logger.LogInformation($"[INFO] Android version updated. Version: '{request.Version}', ApkUrl: '{request.ApkUrl}'. CorrelationId: '{correlationId}'");
                }

                // 💾 Step 4: Save Changes
                await _uow.SaveAsync();

                // ✅ Step 5: Log and Audit Success
                var result = _mapper.Map<AndriodVersionConfigurationDto>(andriodVersionConfiguration);
                string successMessage = $"[SUCCESS] Android version configuration processed successfully. Version: '{request.Version}', ApkUrl: '{request.ApkUrl}'. CorrelationId: '{correlationId}'";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.Update, LogLevelInfo.Information, correlationId);

                return ServiceResponse<AndriodVersionConfigurationDto>.ReturnResultWith200(result, successMessage);
            }
            catch (Exception ex)
            {
                // ❌ Step 6: Handle Unexpected Errors
                string errorMessage = $"[ERROR] Failed to process Android version configuration. Error: {ex.Message}. CorrelationId: '{correlationId}'";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.Update, LogLevelInfo.Error, correlationId);
                return ServiceResponse<AndriodVersionConfigurationDto>.Return500(errorMessage);
            }
        }
    }


}
