using AutoMapper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Data;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new EconomicActivity.
    /// </summary>
    public class AddEconomicActivityCommandHandler : IRequestHandler<AddEconomicActivityCommand, ServiceResponse<EconomicActivityDto>>
    {
        private readonly IEconomicActivityRepository _EconomicActivityRepository; // Repository for accessing EconomicActivity data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddEconomicActivityCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddEconomicActivityCommandHandler.
        /// </summary>
        /// <param name="EconomicActivityRepository">Repository for EconomicActivity data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddEconomicActivityCommandHandler(
            IEconomicActivityRepository EconomicActivityRepository,
            IMapper mapper,
            ILogger<AddEconomicActivityCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _EconomicActivityRepository = EconomicActivityRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddEconomicActivityCommand to add a new EconomicActivity.
        /// </summary>
        /// <param name="request">The AddEconomicActivityCommand containing EconomicActivity data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<EconomicActivityDto>> Handle(AddEconomicActivityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a EconomicActivity with the same name already exists (case-insensitive)
                var existingEconomicActivity = await _EconomicActivityRepository.FindBy(c => c.Name == request.Name && c.IsDeleted == false).FirstOrDefaultAsync();

                // If a EconomicActivity with the same name already exists, return a conflict response
                if (existingEconomicActivity!=null)
                {
                    var errorMessage = $"EconomicActivity {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<EconomicActivityDto>.Return409(errorMessage);
                }
               
                // Map the AddEconomicActivityCommand to a EconomicActivity entity
                var EconomicActivityEntity = _mapper.Map<EconomicActivity>(request);
                // Convert UTC to local time and set it in the entity
                EconomicActivityEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                EconomicActivityEntity.Id = BaseUtilities.GenerateUniqueNumber();
                // Add the new EconomicActivity entity to the repository
                _EconomicActivityRepository.Add(EconomicActivityEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<EconomicActivityDto>.Return500();
                }
                // Map the EconomicActivity entity to EconomicActivityDto and return it with a success response
                var EconomicActivityDto = _mapper.Map<EconomicActivityDto>(EconomicActivityEntity);
                return ServiceResponse<EconomicActivityDto>.ReturnResultWith200(EconomicActivityDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving EconomicActivity: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<EconomicActivityDto>.Return500(e);
            }
        }
    }

}
