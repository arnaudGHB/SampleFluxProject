using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.OldLoanConfiguration.Commands;
using CBS.TransactionManagement.Repository.OldLoanConfiguration;

namespace CBS.TransactionManagement.OldLoanConfiguration.Handlers
{
    /// <summary>
    /// Handles the command to delete a OldLoanAccountingMaping based on DeleteOldLoanAccountingMapingCommand.
    /// </summary>
    public class DeleteOldLoanAccountingMapingCommandHandler : IRequestHandler<DeleteOldLoanAccountingMapingCommand, ServiceResponse<bool>>
    {
        private readonly IOldLoanAccountingMapingRepository _OldLoanAccountingMapingRepository; // Repository for accessing OldLoanAccountingMaping data.
        private readonly ILogger<DeleteOldLoanAccountingMapingCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteOldLoanAccountingMapingCommandHandler.
        /// </summary>
        /// <param name="OldLoanAccountingMapingRepository">Repository for OldLoanAccountingMaping data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteOldLoanAccountingMapingCommandHandler(
            IOldLoanAccountingMapingRepository OldLoanAccountingMapingRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteOldLoanAccountingMapingCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _OldLoanAccountingMapingRepository = OldLoanAccountingMapingRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteOldLoanAccountingMapingCommand to delete a OldLoanAccountingMaping.
        /// </summary>
        /// <param name="request">The DeleteOldLoanAccountingMapingCommand containing OldLoanAccountingMaping ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteOldLoanAccountingMapingCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the OldLoanAccountingMaping entity with the specified ID exists
                var existingOldLoanAccountingMaping = await _OldLoanAccountingMapingRepository.FindAsync(request.Id);
                if (existingOldLoanAccountingMaping == null)
                {
                    errorMessage = $"OldLoanAccountingMaping with ID {request.Id} not found.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.AccountingMapping, LogLevelInfo.Error);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Mark the entity as deleted
                existingOldLoanAccountingMaping.IsDeleted = true;
                _OldLoanAccountingMapingRepository.Update(existingOldLoanAccountingMaping);

                // Save changes
                await _uow.SaveAsync();

                // Log success and return a successful response
                string successMessage = $"OldLoanAccountingMaping {existingOldLoanAccountingMaping.LoanTypeName} successfully deleted.";
                await BaseUtilities.LogAndAuditAsync(successMessage, existingOldLoanAccountingMaping, HttpStatusCodeEnum.OK, LogAction.AccountingMapping, LogLevelInfo.Information);
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting OldLoanAccountingMaping: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingMapping, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }

}
