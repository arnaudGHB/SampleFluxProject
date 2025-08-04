using AutoMapper;
using CBS.SystemConfiguration.Common;
using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Domain;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.MediatR.Commands;
using CBS.SystemConfiguration.Domain;
using CBS.SystemConfiguration.Repository;
using MediatR;
 
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
  
    /// <summary>
    /// Handles the command to add a new CountryType.
    /// </summary>
    public class AddCountryCommandHandler : IRequestHandler<AddCountryCommand, ServiceResponse<CountryDto>>
    {
        private readonly ICountryRepository _CountryRepository; // Repository for accessing OperationEventName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCountryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<SystemContext> _uow;
        private readonly UserInfoToken _userInfoToken;


        public AddCountryCommandHandler(
            ICountryRepository CountryTypeRepository,
            IMapper mapper,
            ILogger<AddCountryCommandHandler> logger,
            IUnitOfWork<SystemContext> uow, UserInfoToken userInfoToken)
        {
            _CountryRepository = CountryTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddCountryCommand to add a new Country.
        /// </summary>
        /// <param name="request">The AddOperationEventNameCommand containing OperationEventName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CountryDto>> Handle(AddCountryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Country with the same name already exists (case-insensitive)
                var existingCountryType = _CountryRepository.All.Where(c => c.Name == request.Name);
                if (existingCountryType.Any())
                {
                    var errorMessage = $"Country : {request.Name} has already been exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CountryDto>.Return409(errorMessage);
                }
                  Country modelExType = _mapper.Map<Country>(request);
              
                modelExType.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "DOC");
                _CountryRepository.Add(modelExType);
                await _uow.SaveAsync();

                var OperationEventNameDto = _mapper.Map<CountryDto>(modelExType);
                return ServiceResponse< CountryDto>.ReturnResultWith200(OperationEventNameDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Country: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse< CountryDto>.Return500(e);
            }
        }
    }
}