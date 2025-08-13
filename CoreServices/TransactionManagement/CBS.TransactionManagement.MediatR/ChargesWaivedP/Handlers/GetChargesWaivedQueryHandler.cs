using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data.Dto.ChargesWaivedP;
using CBS.TransactionManagement.Queries.ChargesWaivedP;
using CBS.TransactionManagement.Repository.ChargesWaivedP;

namespace CBS.TransactionManagement.Handlers.ChargesWaivedP
{
    /// <summary>
    /// Handles the request to retrieve a specific ChargesWaived based on its unique identifier.
    /// </summary>
    public class GetChargesWaivedQueryHandler : IRequestHandler<GetChargesWaivedQuery, ServiceResponse<ChargesWaivedDto>>
    {
        private readonly IChargesWaivedRepository _ChargesWaivedRepository; // Repository for accessing ChargesWaived data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetChargesWaivedQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetChargesWaivedQueryHandler.
        /// </summary>
        /// <param name="ChargesWaivedRepository">Repository for ChargesWaived data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetChargesWaivedQueryHandler(
            IChargesWaivedRepository ChargesWaivedRepository,
            IMapper mapper,
            ILogger<GetChargesWaivedQueryHandler> logger)
        {
            _ChargesWaivedRepository = ChargesWaivedRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetChargesWaivedQuery to retrieve a specific ChargesWaived.
        /// </summary>
        /// <param name="request">The GetChargesWaivedQuery containing ChargesWaived ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ChargesWaivedDto>> Handle(GetChargesWaivedQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the ChargesWaived entity with the specified ID from the repository, include the product
                var entity = await _ChargesWaivedRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the ChargesWaived entity to ChargesWaivedDto and return it with a success response
                    var ChargesWaivedDto = _mapper.Map<ChargesWaivedDto>(entity);
                    return ServiceResponse<ChargesWaivedDto>.ReturnResultWith200(ChargesWaivedDto);
                }
                else
                {
                    // If the ChargesWaived entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("ChargesWaived not found.");
                    return ServiceResponse<ChargesWaivedDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting ChargesWaived: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<ChargesWaivedDto>.Return500(e);
            }
        }

    }

}
