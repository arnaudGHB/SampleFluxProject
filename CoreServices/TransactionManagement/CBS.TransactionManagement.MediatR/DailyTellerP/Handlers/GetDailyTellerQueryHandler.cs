using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.DailyTellerP.Queries;

namespace CBS.TransactionManagement.DailyTellerP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific DailyTeller based on its unique identifier.
    /// </summary>
    public class GetDailyTellerQueryHandler : IRequestHandler<GetDailyTellerQuery, ServiceResponse<DailyTellerDto>>
    {
        private readonly IDailyTellerRepository _DailyTellerRepository; // Repository for accessing DailyTeller data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDailyTellerQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetDailyTellerQueryHandler.
        /// </summary>
        /// <param name="DailyTellerRepository">Repository for DailyTeller data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDailyTellerQueryHandler(
            IDailyTellerRepository DailyTellerRepository,
            IMapper mapper,
            ILogger<GetDailyTellerQueryHandler> logger)
        {
            _DailyTellerRepository = DailyTellerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetDailyTellerQuery to retrieve a specific DailyTeller.
        /// </summary>
        /// <param name="request">The GetDailyTellerQuery containing DailyTeller ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
       public async Task<ServiceResponse<DailyTellerDto>> Handle(GetDailyTellerQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                //request.Id = "017085183837349";
                // Retrieve the DailyTeller entity with the specified ID from the repository
                var entity = await _DailyTellerRepository.FindBy(x=>x.Id==request.Id).Include(x=>x.Teller).Include(x=>x.SubTellerProvioningHistories).Include(x=>x.PrimaryTellerProvisioningHistories).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the DailyTeller entity to DailyTellerDto and return it with a success response
                    var DailyTellerDto = _mapper.Map<DailyTellerDto>(entity);
                    return ServiceResponse<DailyTellerDto>.ReturnResultWith200(DailyTellerDto);
                }
                else
                {
                    // If the DailyTeller entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("DailyTeller not found.");
                    return ServiceResponse<DailyTellerDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting DailyTeller: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<DailyTellerDto>.Return500(e);
            }
        }

    }

}
