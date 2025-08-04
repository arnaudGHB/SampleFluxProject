using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Country based on DeleteCountryCommand.
    /// </summary>
    public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, ServiceResponse<bool>>
    {
        private readonly ICountryRepository _CountryRepository; // Repository for accessing Country data.
        private readonly ILogger<DeleteCountryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteCountryCommandHandler.
        /// </summary>
        /// <param name="CountryRepository">Repository for Country data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteCountryCommandHandler(
            ICountryRepository CountryRepository, IMapper mapper,
            ILogger<DeleteCountryCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _CountryRepository = CountryRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteCountryCommand to delete a Country.
        /// </summary>
        /// <param name="request">The DeleteCountryCommand containing Country ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Country entity with the specified ID exists
                var existingCountry = await _CountryRepository.FindAsync(request.Id);
                if (existingCountry == null)
                {
                    errorMessage = $"Country with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                existingCountry.IsDeleted = true;
                _CountryRepository.Update(existingCountry);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Country: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
