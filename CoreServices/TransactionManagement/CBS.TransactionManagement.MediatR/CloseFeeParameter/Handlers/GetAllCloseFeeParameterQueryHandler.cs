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
    /// Handles the retrieval of all CloseFeeParameter based on the GetAllCloseFeeParameterQuery.
    /// </summary>
    public class GetAllCloseFeeParameterQueryHandler : IRequestHandler<GetAllCloseFeeParameterQuery, ServiceResponse<List<CloseFeeParameterDto>>>
    {
        private readonly ICloseFeeParameterRepository _CloseFeeParameterRepository; // Repository for accessing CloseFeeParameters data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCloseFeeParameterQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllCloseFeeParameterQueryHandler.
        /// </summary>
        /// <param name="CloseFeeParameterRepository">Repository for CloseFeeParameters data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCloseFeeParameterQueryHandler(
            ICloseFeeParameterRepository CloseFeeParameterRepository,
            IMapper mapper, ILogger<GetAllCloseFeeParameterQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _CloseFeeParameterRepository = CloseFeeParameterRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCloseFeeParameterQuery to retrieve all CloseFeeParameters.
        /// </summary>
        /// <param name="request">The GetAllCloseFeeParameterQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CloseFeeParameterDto>>> Handle(GetAllCloseFeeParameterQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all CloseFeeParameters entities from the repository
                var entities = await _CloseFeeParameterRepository.All.Include(a => a.Product).ToListAsync();
                return ServiceResponse<List<CloseFeeParameterDto>>.ReturnResultWith200(_mapper.Map<List<CloseFeeParameterDto>>(entities));
            }
            catch (Exception e)
            {

                Console.WriteLine("error");
                Console.WriteLine(e);
                Console.WriteLine(e.InnerException);
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all CloseFeeParameters: {e.Message}");
                return ServiceResponse<List<CloseFeeParameterDto>>.Return500(e, "Failed to get all CloseFeeParameters");
            }
        }
    }
}
