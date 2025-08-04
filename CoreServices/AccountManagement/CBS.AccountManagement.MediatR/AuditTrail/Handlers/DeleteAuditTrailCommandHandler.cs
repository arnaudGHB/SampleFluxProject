using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Common.Repository.Uow;
using CBS.BankMGT.Data;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a AuditTrail based on DeleteAuditTrailCommand.
    /// </summary>
    public class DeleteAuditTrailCommandHandler : IRequestHandler<DeleteAuditTrailCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<DeleteAuditTrailCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.

        /// <summary>
        /// Constructor for initializing the DeleteAuditTrailCommandHandler.
        /// </summary>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mongoUnitOfWork">MongoDB Unit of Work.</param>
        public DeleteAuditTrailCommandHandler(
            ILogger<DeleteAuditTrailCommandHandler> logger,
            IMongoUnitOfWork mongoUnitOfWork)
        {
            _logger = logger;
            _mongoUnitOfWork = mongoUnitOfWork;
        }

        /// <summary>
        /// Handles the DeleteAuditTrailCommand to delete an AuditTrail.
        /// </summary>
        /// <param name="request">The DeleteAuditTrailCommand containing AuditTrail ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteAuditTrailCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Get the MongoDB repository for AuditTrail
                var auditTrailRepository = _mongoUnitOfWork.GetRepository<AuditTrail>();

                // Check if the AuditTrail entity with the specified ID exists
                var existingAuditTrail = await auditTrailRepository.GetByIdAsync(request.Id);
                if (existingAuditTrail == null)
                {
                    errorMessage = $"AuditTrail with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                await auditTrailRepository.DeleteAsync(existingAuditTrail.Id);
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting AuditTrail: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
