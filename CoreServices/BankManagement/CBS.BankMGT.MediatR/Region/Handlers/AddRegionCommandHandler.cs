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
    /// Handles the command to add a new Region.
    /// </summary>
    public class AddRegionCommandHandler : IRequestHandler<AddRegionCommand, ServiceResponse<RegionDto>>
    {
        private readonly IRegionRepository _RegionRepository; // Repository for accessing Region data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddRegionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ICountryRepository _countryRepository; // Repository for accessing Region data.

        /// <summary>
        /// Constructor for initializing the AddRegionCommandHandler.
        /// </summary>
        /// <param name="RegionRepository">Repository for Region data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddRegionCommandHandler(
            IRegionRepository RegionRepository,
            IMapper mapper,
            ILogger<AddRegionCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            ICountryRepository countryRepository)
        {
            _RegionRepository = RegionRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _countryRepository = countryRepository;
        }

        /// <summary>
        /// Handles the AddRegionCommand to add a new Region.
        /// </summary>
        /// <param name="request">The AddRegionCommand containing Region data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<RegionDto>> Handle(AddRegionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Region with the same name already exists (case-insensitive)
                var existingRegion = await _RegionRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();

                // If a Region with the same name already exists, return a conflict response
                if (existingRegion!=null)
                {
                    var errorMessage = $"Region {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RegionDto>.Return409(errorMessage);
                }
                var existingCountryCode = await _countryRepository.FindBy(c => c.Id == request.CountryId).FirstOrDefaultAsync();
                // If a Region with the same code already exists, return a conflict response
                if (existingCountryCode == null)
                {
                    var errorMessage = $"Country code {request.CountryId} does not exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RegionDto>.Return404(errorMessage);
                }
                // Map the AddRegionCommand to a Region entity
                var RegionEntity = _mapper.Map<Region>(request);
                // Convert UTC to local time and set it in the entity
                RegionEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                RegionEntity.Id = BaseUtilities.GenerateUniqueNumber();
                // Add the new Region entity to the repository
                _RegionRepository.Add(RegionEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<RegionDto>.Return500();
                }
                // Map the Region entity to RegionDto and return it with a success response
                var RegionDto = _mapper.Map<RegionDto>(RegionEntity);
                return ServiceResponse<RegionDto>.ReturnResultWith200(RegionDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Region: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<RegionDto>.Return500(e);
            }
        }
    }

}
