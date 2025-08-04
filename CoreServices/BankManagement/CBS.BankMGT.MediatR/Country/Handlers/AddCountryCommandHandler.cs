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
    /// Handles the command to add a new Country.
    /// </summary>
    public class AddCountryCommandHandler : IRequestHandler<AddCountryCommand, ServiceResponse<CountryDto>>
    {
        private readonly ICountryRepository _CountryRepository; // Repository for accessing Country data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCountryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddCountryCommandHandler.
        /// </summary>
        /// <param name="CountryRepository">Repository for Country data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCountryCommandHandler(
            ICountryRepository CountryRepository,
            IMapper mapper,
            ILogger<AddCountryCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _CountryRepository = CountryRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddCountryCommand to add a new Country.
        /// </summary>
        /// <param name="request">The AddCountryCommand containing Country data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CountryDto>> Handle(AddCountryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Country with the same name already exists (case-insensitive)
                var existingCountry = await _CountryRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();

                // If a Country with the same name already exists, return a conflict response
                if (existingCountry!=null)
                {
                    var errorMessage = $"Country {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CountryDto>.Return409(errorMessage);
                }
                var existingCountryCode = await _CountryRepository.FindBy(c => c.Code == request.Code).FirstOrDefaultAsync();

                // If a Country with the same code already exists, return a conflict response
                if (existingCountryCode != null)
                {
                    var errorMessage = $"Country code {request.Code} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CountryDto>.Return409(errorMessage);
                }
                // Map the AddCountryCommand to a Country entity
                var CountryEntity = _mapper.Map<Country>(request);
                // Convert UTC to local time and set it in the entity
                CountryEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                CountryEntity.Id = BaseUtilities.GenerateUniqueNumber();
                // Add the new Country entity to the repository
                _CountryRepository.Add(CountryEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CountryDto>.Return500();
                }
                // Map the Country entity to CountryDto and return it with a success response
                var CountryDto = _mapper.Map<CountryDto>(CountryEntity);
                return ServiceResponse<CountryDto>.ReturnResultWith200(CountryDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Country: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CountryDto>.Return500(e);
            }
        }
    }

}
