using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Data.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Common.Repository.Uow;
using CBS.BankMGT.Data;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AuditTrail based on its unique identifier.
    /// </summary>
    public class GetAuditTrailQueryHandler : IRequestHandler<GetAuditTrailQuery, ServiceResponse<AuditTrailDto>>
    {
        private readonly ILogger<GetAuditTrailQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly IMapper _mapper; // AutoMapper for object mapping.

        /// <summary>
        /// Constructor for initializing the GetAuditTrailQueryHandler.
        /// </summary>
        /// <param name="mongoUnitOfWork">MongoDB Unit of Work.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAuditTrailQueryHandler(
            IMongoUnitOfWork mongoUnitOfWork,
            IMapper mapper,
            ILogger<GetAuditTrailQueryHandler> logger)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAuditTrailQuery to retrieve a specific AuditTrail by its ID.
        /// </summary>
        /// <param name="request">The GetAuditTrailQuery containing the AuditTrail ID to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AuditTrailDto>> Handle(GetAuditTrailQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    errorMessage = "AuditTrail ID cannot be null or empty.";
                    _logger.LogWarning(errorMessage);
                    return ServiceResponse<AuditTrailDto>.Return400(errorMessage);
                }

                // Access MongoDB repository
                var auditTrailRepository = _mongoUnitOfWork.GetRepository<AuditTrail>();

                // Retrieve the AuditTrail entity by its ID
                var entity = await auditTrailRepository.GetByIdAsync(request.Id);

                if (entity != null)
                {
                    // Map the entity to AuditTrailDto and return success response
                    var auditTrailDto = _mapper.Map<AuditTrailDto>(entity);
                    return ServiceResponse<AuditTrailDto>.ReturnResultWith200(auditTrailDto);
                }
                else
                {
                    // Log not found error and return 404 Not Found response
                    var notFoundMessage = $"AuditTrail with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    return ServiceResponse<AuditTrailDto>.Return404(notFoundMessage);
                }
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response
                errorMessage = $"Error occurred while retrieving AuditTrail with ID '{request.Id}': {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AuditTrailDto>.Return500(e, errorMessage);
            }
        }
    }

}
