using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.RemittanceP.Commands;
using CBS.TransactionManagement.Repository.RemittanceP;

namespace CBS.TransactionManagement.MediatR.RemittanceP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Remittance based on DeleteRemittanceCommand.
    /// </summary>
    public class DeleteRemittanceHandler : IRequestHandler<DeleteRemittanceCommand, ServiceResponse<bool>>
    {
        private readonly IRemittanceRepository _RemittanceRepository; // Repository for accessing Remittance data.
        private readonly ILogger<DeleteRemittanceHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteRemittanceCommandHandler.
        /// </summary>
        /// <param name="RemittanceRepository">Repository for Remittance data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteRemittanceHandler(
            IRemittanceRepository RemittanceRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteRemittanceHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _RemittanceRepository = RemittanceRepository;
            _logger = logger;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteRemittanceCommand to delete a Remittance.
        /// </summary>
        /// <param name="request">The DeleteRemittanceCommand containing Remittance ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteRemittanceCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Remittance entity with the specified ID exists
                var existingRemittance = await _RemittanceRepository.FindAsync(request.Id);
                if (existingRemittance == null)
                {
                    errorMessage = $"Remittance with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(
                   errorMessage,
                   request,
                   HttpStatusCodeEnum.OK,
                   LogAction.Remittance,
                   LogLevelInfo.Information);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingRemittance.IsDeleted = true;
                _RemittanceRepository.Update(existingRemittance);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Remittance: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                     errorMessage,
                     request,
                     HttpStatusCodeEnum.InternalServerError,
                     LogAction.Remittance,
                     LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
