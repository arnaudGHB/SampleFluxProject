using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Handlers
{

    /// <summary>
    /// Handles the command to update a Config based on AddConfigCommand.
    /// </summary>
    public class AddConfigCommandHandler : IRequestHandler<AddConfigCommand, ServiceResponse<ConfigDto>>
    {
        private readonly IConfigRepository _configRepository; // Repository for accessing Config data.
        private readonly ILogger<AddConfigCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        // Constructor for initializing the AddConfigCommandHandler.
        public AddConfigCommandHandler(
            IConfigRepository configRepository,
            ILogger<AddConfigCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _configRepository = configRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        // Handles the AddConfigCommand to add a new configuration.
        public async Task<ServiceResponse<ConfigDto>> Handle(AddConfigCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a configuration with the same name already exists.
                var existingConfigName = await _configRepository.FindBy(x => x.Name == request.Name).FirstOrDefaultAsync();
                string message = $"{request.Value} Created with success.";

                if (existingConfigName != null)
                {
                    message = $"{request.Name} already exists.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<ConfigDto>.Return409(message);
                }

                // Check if a configuration with the same value already exists.
                var existingConfigValue = await _configRepository.FindBy(x => x.Value == request.Value).FirstOrDefaultAsync();

                if (existingConfigValue != null)
                {
                    message = $"{request.Name} already exists.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<ConfigDto>.Return409(message);
                }

                // Check if there's already an active year running.
                var configsYearOpen = await _configRepository.All.Where(x => x.IsYearOpen).ToListAsync();

                if (configsYearOpen.Any())
                {
                    message = $"Only one active year can be running at a time.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 403, _userInfoToken.Token);
                    return ServiceResponse<ConfigDto>.Return409(message);
                }

                // Map the request to a configuration entity.
                var config = _mapper.Map<Config>(request);
                config.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new configuration to the repository.
                _configRepository.Add(config);
                await _uow.SaveAsync();

                // Log successful creation of the configuration.
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the configuration entity back to a DTO for response.
                var configDto = _mapper.Map<ConfigDto>(config);
                return ServiceResponse<ConfigDto>.ReturnResultWith200(configDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                string msg = $"Error occurred while adding configuration: {e.Message}";
                _logger.LogError(msg);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<ConfigDto>.Return500(msg);
            }
        }
    }

}
