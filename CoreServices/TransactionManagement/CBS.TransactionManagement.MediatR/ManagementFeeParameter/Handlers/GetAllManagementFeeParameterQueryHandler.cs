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
    /// Handles the retrieval of all ManagementFeeParameter based on the GetAllManagementFeeParameterQuery.
    /// </summary>
    public class GetAllManagementFeeParameterQueryHandler : IRequestHandler<GetAllManagementFeeParameterQuery, ServiceResponse<List<ManagementFeeParameterDto>>>
    {
        private readonly IManagementFeeParameterRepository _ManagementFeeParameterRepository; // Repository for accessing ManagementFeeParameters data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllManagementFeeParameterQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllManagementFeeParameterQueryHandler.
        /// </summary>
        /// <param name="ManagementFeeParameterRepository">Repository for ManagementFeeParameters data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllManagementFeeParameterQueryHandler(
            IManagementFeeParameterRepository ManagementFeeParameterRepository,
            IMapper mapper, ILogger<GetAllManagementFeeParameterQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _ManagementFeeParameterRepository = ManagementFeeParameterRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllManagementFeeParameterQuery to retrieve all ManagementFeeParameters.
        /// </summary>
        /// <param name="request">The GetAllManagementFeeParameterQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ManagementFeeParameterDto>>> Handle(GetAllManagementFeeParameterQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all ManagementFeeParameters entities from the repository
                var entities = await _ManagementFeeParameterRepository.All.Include(a => a.Product).ToListAsync();
                return ServiceResponse<List<ManagementFeeParameterDto>>.ReturnResultWith200(_mapper.Map<List<ManagementFeeParameterDto>>(entities));
            }
            catch (Exception e)
            {

                Console.WriteLine("error");
                Console.WriteLine(e);
                Console.WriteLine(e.InnerException);
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all ManagementFeeParameters: {e.Message}");
                return ServiceResponse<List<ManagementFeeParameterDto>>.Return500(e, "Failed to get all ManagementFeeParameters");
            }
        }
    }
}
