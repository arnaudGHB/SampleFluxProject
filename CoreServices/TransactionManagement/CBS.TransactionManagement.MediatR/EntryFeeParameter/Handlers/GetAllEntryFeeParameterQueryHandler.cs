using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using System.Linq;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all EntryFeeParameter based on the GetAllEntryFeeParameterQuery.
    /// </summary>
    public class GetAllEntryFeeParameterQueryHandler : IRequestHandler<GetAllEntryFeeParameterQuery, ServiceResponse<List<EntryFeeParameterDto>>>
    {
        private readonly IEntryFeeParameterRepository _EntryFeeParameterRepository; // Repository for accessing EntryFeeParameters data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllEntryFeeParameterQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllEntryFeeParameterQueryHandler.
        /// </summary>
        /// <param name="EntryFeeParameterRepository">Repository for EntryFeeParameters data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllEntryFeeParameterQueryHandler(
            IEntryFeeParameterRepository EntryFeeParameterRepository,
            IMapper mapper, ILogger<GetAllEntryFeeParameterQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _EntryFeeParameterRepository = EntryFeeParameterRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllEntryFeeParameterQuery to retrieve all EntryFeeParameters.
        /// </summary>
        /// <param name="request">The GetAllEntryFeeParameterQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<EntryFeeParameterDto>>> Handle(GetAllEntryFeeParameterQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all EntryFeeParameters entities from the repository
                var entities = await _EntryFeeParameterRepository.All.Include(a => a.Product).ToListAsync();
                return ServiceResponse<List<EntryFeeParameterDto>>.ReturnResultWith200(_mapper.Map<List<EntryFeeParameterDto>>(entities));
            }
            catch (Exception e)
            {

                Console.WriteLine("error");
                Console.WriteLine(e);
                Console.WriteLine(e.InnerException);
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all EntryFeeParameters: {e.Message}");
                return ServiceResponse<List<EntryFeeParameterDto>>.Return500(e, "Failed to get all EntryFeeParameters");
            }
        }
    }
}
