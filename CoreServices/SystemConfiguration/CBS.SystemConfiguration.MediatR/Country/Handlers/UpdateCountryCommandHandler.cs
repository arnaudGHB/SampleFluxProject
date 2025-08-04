using AutoMapper;
using CBS.SystemConfiguration.Common;
using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Domain;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.MediatR.Commands;
using CBS.SystemConfiguration.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.SystemConfiguration.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a Document based on UpdateDocumentCommand.
    /// </summary>
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, ServiceResponse<CountryDto>>
    {
        private readonly ICountryRepository _CountryRepository; // Repository for accessing Document data.
        private readonly ILogger<UpdateCountryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<SystemContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateDocumentCommandHandler.
        /// </summary>
        /// <param name="DocumentRepository">Repository for Document data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateCountryCommandHandler(
            ICountryRepository DocumentRepository,
            ILogger<UpdateCountryCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<SystemContext> uow = null)
        {
            _CountryRepository = DocumentRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateDocumentCommand to update a Document.
        /// </summary>
        /// <param name="request">The UpdateDocumentCommand containing updated Document data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CountryDto>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Document entity to be updated from the repository
                var existingDocument = await _CountryRepository.FindAsync(request.Id);

                // Check if the Document entity exists
                if (existingDocument != null)
                {
                    // Update Document entity properties with values from the request
                    existingDocument.Name= request.Name;
             
                    existingDocument = _mapper.Map(request, existingDocument);
                    // Use the repository to update the existing Document entity
                    _CountryRepository.Update(existingDocument);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<CountryDto>.ReturnResultWith200(_mapper.Map<CountryDto>(existingDocument));
                    _logger.LogInformation($"Document {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Document entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CountryDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Document: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CountryDto>.Return500(e);
            }
        }
    }
}