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
    /// Handles the command to add a new AccountPolicyName.
    /// </summary>
    public class AddCashMovementTrackingConfigurationCommandHandler : IRequestHandler<AddCashMovementTrackingConfigurationCommand, ServiceResponse<CashMovementTrackingConfigurationDto>>
    {
        private readonly ICashMovementTrackingConfigurationRepository _cashMovementTrackerRepository; // Repository for accessing AccountPolicyName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCashMovementTrackerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the AddAccountPolicyNameCommandHandler.
        /// </summary>
        /// <param name="AccountPolicyNameRepository">Repository for AccountPolicyName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCashMovementTrackingConfigurationCommandHandler(
            ICashMovementTrackingConfigurationRepository AccountPolicyNameRepository,
            IMapper mapper,
            ILogger<AddCashMovementTrackerCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _cashMovementTrackerRepository = AccountPolicyNameRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken= userInfoToken;
        }

     
        public async Task<ServiceResponse<CashMovementTrackingConfigurationDto>> Handle(AddCashMovementTrackingConfigurationCommand request, CancellationToken cancellationToken)
        {
            try
            {
    
                var existingCashMovementTracker =   _cashMovementTrackerRepository.All.Where(c => c.From == request.From&&c.To== request.To).ToList();

           
                if (existingCashMovementTracker.Any())
                {
                    var errorMessage = $"Cash Movement Tracker configuration already exists on this account. Intially set by {existingCashMovementTracker.FirstOrDefault().CreatedBy} at {existingCashMovementTracker.FirstOrDefault().CreatedDate}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CashMovementTrackingConfigurationDto>.Return409(errorMessage);
                }
 
                var CashMovementTrackerEntity = _mapper.Map<CashMovementTrackingConfiguration>(request);

                CashMovementTrackerEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "CTRK");
                _cashMovementTrackerRepository.Add(CashMovementTrackerEntity);
                await _uow.SaveAsync();
              
                // Map the AccountPolicyName entity to AccountPolicyNameDto and return it with a success response
                var CashMovementTrackerDto = _mapper.Map<CashMovementTrackingConfigurationDto>(CashMovementTrackerEntity);
                return ServiceResponse<CashMovementTrackingConfigurationDto>.ReturnResultWith200(CashMovementTrackerDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving CashMovementTrackingConfiguration: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CashMovementTrackingConfigurationDto>.Return500(e);
            }
        }
    }
}