using AutoMapper;
using CBS.SystemConfiguration.Common;
using CBS.SystemConfiguration.Domain;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.MediatR.Commands;
using CBS.SystemConfiguration.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.SystemConfiguration.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a CountryName based on DeleteCountryNameCommand.
    /// </summary>
    public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, ServiceResponse<bool>>
    {
        private readonly ICountryRepository _CountryRepository; // Repository for accessing CountryName data.
        private readonly ILogger<DeleteCountryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<SystemContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteCountryNameCommandHandler.
        /// </summary>
        /// <param name="CountryNameRepository">Repository for CountryName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteCountryCommandHandler(
            ICountryRepository CountryNameRepository, IMapper mapper,
            ILogger<DeleteCountryCommandHandler> logger
, IUnitOfWork<SystemContext> uow)
        {
            _CountryRepository = CountryNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteCountryNameCommand to delete a CountryName.
        /// </summary>
        /// <param name="request">The DeleteCountryNameCommand containing CountryName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the CountryName entity with the specified ID exists
                var existingCountryName = await _CountryRepository.FindAsync(request.Id);
                if (existingCountryName == null)
                {
                    errorMessage = $"CountryName with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingCountryName.IsDeleted = true;

                _CountryRepository.Update(existingCountryName);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting CountryName: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}