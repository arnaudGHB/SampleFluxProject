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
    public class GetAuditTrailByUserQueryHandler : IRequestHandler<GetAuditTrailByUserQuery, ServiceResponse<List<AuditTrailDto>>>
    {
        private readonly ILogger<GetAuditTrailByUserQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly IMapper _mapper; // AutoMapper for object mapping.

        /// <summary>
        /// Constructor for initializing the GetAuditTrailByUserQueryHandler.
        /// </summary>
        /// <param name="mongoUnitOfWork">MongoDB Unit of Work.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAuditTrailByUserQueryHandler(
            IMongoUnitOfWork mongoUnitOfWork,
            IMapper mapper,
            ILogger<GetAuditTrailByUserQueryHandler> logger)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAuditTrailByUserQuery to retrieve AuditTrails by a specific user.
        /// </summary>
        /// <param name="request">The GetAuditTrailByUserQuery containing the username parameter.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AuditTrailDto>>> Handle(GetAuditTrailByUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.UserName))
                {
                    const string errorMessage = "UserName cannot be null or empty.";
                    _logger.LogWarning(errorMessage);
                    return ServiceResponse<List<AuditTrailDto>>.Return400(errorMessage);
                }

                // Access MongoDB repository
                var auditTrailRepository = _mongoUnitOfWork.GetRepository<AuditTrail>();

                // Query MongoDB collection for the user's audit trails
                var entities = await auditTrailRepository.FindBy(x => x.UserName == request.UserName).ToListAsync(cancellationToken);

                // Check for empty results
                if (entities == null || !entities.Any())
                {
                    var infoMessage = $"No audit trails found for user: {request.UserName}.";
                    _logger.LogInformation(infoMessage);
                    return ServiceResponse<List<AuditTrailDto>>.ReturnResultWith200(new List<AuditTrailDto>(), infoMessage);
                }

                // Map entities to DTOs and return
                var auditTrailDtos = _mapper.Map<List<AuditTrailDto>>(entities);
                return ServiceResponse<List<AuditTrailDto>>.ReturnResultWith200(auditTrailDtos);
            }
            catch (Exception e)
            {
                // Log error and return a 500 response
                var errorMessage = $"Failed to get audit trails for user: {request.UserName}. Error: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<List<AuditTrailDto>>.Return500(e, errorMessage);
            }
        }
    }
}
