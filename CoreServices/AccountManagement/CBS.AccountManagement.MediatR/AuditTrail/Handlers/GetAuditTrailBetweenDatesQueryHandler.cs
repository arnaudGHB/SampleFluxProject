using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Common.Repository.Uow;
using CBS.BankMGT.Data;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all AuditTrails based on the GetAllAuditTrailQuery.
    /// </summary>
    public class GetAuditTrailBetweenDatesQueryHandler : IRequestHandler<GetAuditTrailBetweenDatesQuery, ServiceResponse<List<AuditTrailDto>>>
    {
        private readonly ILogger<GetAuditTrailBetweenDatesQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly IMapper _mapper; // AutoMapper for object mapping.

        /// <summary>
        /// Constructor for initializing the GetAuditTrailBetweenDatesQueryHandler.
        /// </summary>
        /// <param name="mongoUnitOfWork">MongoDB Unit of Work.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAuditTrailBetweenDatesQueryHandler(
            IMongoUnitOfWork mongoUnitOfWork,
            IMapper mapper,
            ILogger<GetAuditTrailBetweenDatesQueryHandler> logger)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAuditTrailBetweenDatesQuery to retrieve AuditTrails within the specified date range.
        /// </summary>
        /// <param name="request">The GetAuditTrailBetweenDatesQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AuditTrailDto>>> Handle(GetAuditTrailBetweenDatesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get the MongoDB repository for AuditTrail
                var auditTrailRepository = _mongoUnitOfWork.GetRepository<AuditTrail>();

                // Convert input dates from strings to DateTime
                var startDate = BaseUtilities.ConvertStringToDateTime(request.DateFrom_dd_MM_yyyy).Date;
                var endDate = BaseUtilities.ConvertStringToDateTime(request.DateTo_dd_MM_yyyy).Date.AddDays(1).AddTicks(-1); // Include the entire end date

                // Retrieve AuditTrails within the specified date range
                var entities = await auditTrailRepository.FindBy(x => x.Timestamp >= startDate && x.Timestamp <= endDate).ToListAsync();

                // Map entities to DTOs and return success response
                var auditTrailDtos = _mapper.Map<List<AuditTrailDto>>(entities);
                return ServiceResponse<List<AuditTrailDto>>.ReturnResultWith200(auditTrailDtos);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                var errorMessage = $"Failed to get AuditTrails between dates: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<List<AuditTrailDto>>.Return500(e, "Failed to get AuditTrails between dates");
            }
        }
    }
}
