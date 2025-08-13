using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using System.Linq;
using CBS.TransactionManagement.DailyTellerP.Queries;

namespace CBS.TransactionManagement.DailyTellerP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all DailyTeller based on the GetAllDailyTellerQuery.
    /// </summary>
    public class GetAllDailyTellerQueryHandler : IRequestHandler<GetAllDailyTellerQuery, ServiceResponse<List<DailyTellerDto>>>
    {
        private readonly IDailyTellerRepository _DailyTellerRepository; // Repository for accessing DailyTellers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDailyTellerQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllDailyTellerQueryHandler.
        /// </summary>
        /// <param name="DailyTellerRepository">Repository for DailyTellers data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDailyTellerQueryHandler(
            IDailyTellerRepository DailyTellerRepository,
            IMapper mapper, ILogger<GetAllDailyTellerQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _DailyTellerRepository = DailyTellerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllDailyTellerQuery to retrieve all DailyTellers.
        /// </summary>
        /// <param name="request">The GetAllDailyTellerQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DailyTellerDto>>> Handle(GetAllDailyTellerQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all DailyTellers entities from the repository
                var entities = await _DailyTellerRepository.FindBy(x=>!x.IsDeleted && x.CreatedDate.Date>=request.DateFrom.Date && x.CreatedDate.Date<=request.DateTo.Date).ToListAsync();
                return ServiceResponse<List<DailyTellerDto>>.ReturnResultWith200(_mapper.Map<List<DailyTellerDto>>(entities));
            }
            catch (Exception e)
            {

                Console.WriteLine("error");
                Console.WriteLine(e);
                Console.WriteLine(e.InnerException);
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all DailyTellers: {e.Message}");
                return ServiceResponse<List<DailyTellerDto>>.Return500(e, "Failed to get all DailyTellers");
            }
        }
    }
}
