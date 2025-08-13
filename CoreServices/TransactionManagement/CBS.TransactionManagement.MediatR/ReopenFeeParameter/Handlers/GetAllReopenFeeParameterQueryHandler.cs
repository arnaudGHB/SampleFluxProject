using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using System.Linq;
using CBS.TransactionManagement.Repository.ReopenFeeParameterP;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all ReopenFeeParameter based on the GetAllReopenFeeParameterQuery.
    /// </summary>
    public class GetAllReopenFeeParameterQueryHandler : IRequestHandler<GetAllReopenFeeParameterQuery, ServiceResponse<List<ReopenFeeParameterDto>>>
    {
        private readonly IReopenFeeParameterRepository _ReopenFeeParameterRepository; // Repository for accessing ReopenFeeParameters data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllReopenFeeParameterQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllReopenFeeParameterQueryHandler.
        /// </summary>
        /// <param name="ReopenFeeParameterRepository">Repository for ReopenFeeParameters data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllReopenFeeParameterQueryHandler(
            IReopenFeeParameterRepository ReopenFeeParameterRepository,
            IMapper mapper, ILogger<GetAllReopenFeeParameterQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _ReopenFeeParameterRepository = ReopenFeeParameterRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllReopenFeeParameterQuery to retrieve all ReopenFeeParameters.
        /// </summary>
        /// <param name="request">The GetAllReopenFeeParameterQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ReopenFeeParameterDto>>> Handle(GetAllReopenFeeParameterQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all ReopenFeeParameters entities from the repository
                var entities = await _ReopenFeeParameterRepository.All.Include(a => a.Product).ToListAsync();
                return ServiceResponse<List<ReopenFeeParameterDto>>.ReturnResultWith200(_mapper.Map<List<ReopenFeeParameterDto>>(entities));
            }
            catch (Exception e)
            {

                Console.WriteLine("error");
                Console.WriteLine(e);
                Console.WriteLine(e.InnerException);
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all ReopenFeeParameters: {e.Message}");
                return ServiceResponse<List<ReopenFeeParameterDto>>.Return500(e, "Failed to get all ReopenFeeParameters");
            }
        }
    }
}
