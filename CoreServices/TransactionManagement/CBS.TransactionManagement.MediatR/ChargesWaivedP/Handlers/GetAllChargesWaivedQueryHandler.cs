using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository.ChargesWaivedP;
using CBS.TransactionManagement.Queries.ChargesWaivedP;
using CBS.TransactionManagement.Data.Dto.ChargesWaivedP;

namespace CBS.TransactionManagement.Handlers.ChargesWaivedP
{
    /// <summary>
    /// Handles the retrieval of all ChargesWaived based on the GetAllChargesWaivedQuery.
    /// </summary>
    public class GetAllChargesWaivedQueryHandler : IRequestHandler<GetAllChargesWaivedQuery, ServiceResponse<List<ChargesWaivedDto>>>
    {
        private readonly IChargesWaivedRepository _ChargesWaivedRepository; // Repository for accessing ChargesWaiveds data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllChargesWaivedQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllChargesWaivedQueryHandler.
        /// </summary>
        /// <param name="ChargesWaivedRepository">Repository for ChargesWaiveds data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllChargesWaivedQueryHandler(
            IChargesWaivedRepository ChargesWaivedRepository,
            IMapper mapper, ILogger<GetAllChargesWaivedQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _ChargesWaivedRepository = ChargesWaivedRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllChargesWaivedQuery to retrieve all ChargesWaiveds.
        /// </summary>
        /// <param name="request">The GetAllChargesWaivedQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ChargesWaivedDto>>> Handle(GetAllChargesWaivedQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all ChargesWaiveds entities from the repository
                var entities = await _ChargesWaivedRepository.All.Where(x=>!x.IsDeleted).ToListAsync();
                return ServiceResponse<List<ChargesWaivedDto>>.ReturnResultWith200(_mapper.Map<List<ChargesWaivedDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all ChargesWaiveds: {e.Message}");
                return ServiceResponse<List<ChargesWaivedDto>>.Return500(e, "Failed to get all ChargesWaiveds");
            }
        }
    }
}
