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
    public class GetAllAuditTrailQueryHandler : IRequestHandler<GetAllAuditTrailQuery, ServiceResponse<List<AuditTrailDto>>>
    {
        private readonly ILogger<GetAllAuditTrailQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly IMapper _mapper; // AutoMapper for object mapping.

        /// <summary>
        /// Constructor for initializing the GetAllAuditTrailQueryHandler.
        /// </summary>
        /// <param name="mongoUnitOfWork">MongoDB Unit of Work.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAuditTrailQueryHandler(
            IMongoUnitOfWork mongoUnitOfWork,
            IMapper mapper,
            ILogger<GetAllAuditTrailQueryHandler> logger)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAuditTrailQuery to retrieve all AuditTrails.
        /// </summary>
        /// <param name="request">The GetAllAuditTrailQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AuditTrailDto>>> Handle(GetAllAuditTrailQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get the MongoDB repository for AuditTrail
                var auditTrailRepository = _mongoUnitOfWork.GetRepository<AuditTrail>();

                // Retrieve all non-deleted AuditTrail entities
                var entities = await auditTrailRepository.GetAllAsync();

                // Map the entities to AuditTrailDto and return a success response
                var auditTrailDtos = _mapper.Map<List<AuditTrailDto>>(entities);
                return ServiceResponse<List<AuditTrailDto>>.ReturnResultWith200(auditTrailDtos);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                var errorMessage = $"Failed to get all AuditTrails: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<List<AuditTrailDto>>.Return500(e, "Failed to get all AuditTrails");
            }
        }
    }
}
