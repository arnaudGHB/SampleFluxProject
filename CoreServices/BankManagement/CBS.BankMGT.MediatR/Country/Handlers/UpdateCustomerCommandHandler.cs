using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;

using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{

    /// <summary>
    /// Handles the command to update a Country based on UpdateCountryCommand.
    /// </summary>
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, ServiceResponse<CountryDto>>
    {
        private readonly ICountryRepository _CountryRepository; // Repository for accessing Country data.
        private readonly ILogger<UpdateCountryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateCountryCommandHandler.
        /// </summary>
        /// <param name="CountryRepository">Repository for Country data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateCountryCommandHandler(
            ICountryRepository CountryRepository,
            ILogger<UpdateCountryCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _CountryRepository = CountryRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateCountryCommand to update a Country.
        /// </summary>
        /// <param name="request">The UpdateCountryCommand containing updated Country data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CountryDto>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Country entity to be updated from the repository
                var existingCountry = await _CountryRepository.FindAsync(request.Id);

                // Check if the Country entity exists
                if (existingCountry != null)
                {
                    // Update Country entity properties with values from the request
                    existingCountry.Name = request.Name;
                    existingCountry.Code = request.Code;
                    // Use the repository to update the existing Country entity
                    _CountryRepository.Update(existingCountry);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<CountryDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<CountryDto>.ReturnResultWith200(_mapper.Map<CountryDto>(existingCountry));
                    _logger.LogInformation($"Country {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Country entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CountryDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Country: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CountryDto>.Return500(e);
            }
        }
    }

}
