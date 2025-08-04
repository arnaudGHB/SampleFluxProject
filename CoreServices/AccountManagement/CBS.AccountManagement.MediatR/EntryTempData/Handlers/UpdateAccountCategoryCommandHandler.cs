using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a AccountFeature based on UpdateAccountFeatureCommand.
    /// </summary>
    public class UpdateEntryTempDataCommandHandler : IRequestHandler<UpdateEntryTempDataCommand, ServiceResponse<EntryTempDataDto>>
    {
        private readonly IEntryTempDataRepository _entryTempDataRepository; // Repository for accessing AccountFeature data.
        private readonly ILogger<UpdateEntryTempDataCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the UpdateEntryTempDataCommandHandler.
        /// </summary>
        /// <param name="AccountFeatureRepository">Repository for AccountFeature data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateEntryTempDataCommandHandler(
              IEntryTempDataRepository AccountFeatureRepository,
              IMapper mapper,
              ILogger<UpdateEntryTempDataCommandHandler> logger,
              IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _entryTempDataRepository = AccountFeatureRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the UpdateAccountFeatureCommand to update a AccountFeature.
        /// </summary>
        /// <param name="request">The UpdateAccountFeatureCommand containing updated AccountFeature data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<EntryTempDataDto>> Handle(UpdateEntryTempDataCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the AccountFeature entity to be updated from the repository
                var existing = await _entryTempDataRepository.FindAsync(request.Id);

                // Check if the AccountFeature entity exists
                if (existing != null)
                {
                    // Update AccountFeature entity properties with values from the request

                    var entry = _mapper.Map(request, existing);
                    // Use the repository to update the existing AccountFeature entity
                    _entryTempDataRepository.Update(entry);

                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<EntryTempDataDto>.ReturnResultWith200(_mapper.Map<EntryTempDataDto>(existing));
                    _logger.LogInformation($"EntryTempData {request.Id} was successfully updated.");
                    string errorMessage = $"EntryTempData {request.Id} was successfully updated.";
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateEntryTempDataCommand",
                request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return response;
                }
                else
                {
                    // If the AccountFeature entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateEntryTempDataCommand",
     request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<EntryTempDataDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating EntryTempData: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateEntryTempDataCommand",
   request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<EntryTempDataDto>.Return500(e);
            }
        }
    }
}