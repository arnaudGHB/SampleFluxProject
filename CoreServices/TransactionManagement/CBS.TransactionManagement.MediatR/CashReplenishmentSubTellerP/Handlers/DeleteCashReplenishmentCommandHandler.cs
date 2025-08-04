using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Handlers
{
    /// <summary>
    /// Handles the command to delete a CashReplenishment based on DeleteCashReplenishmentCommand.
    /// </summary>
    public class DeleteCashReplenishmentCommandHandler : IRequestHandler<DeleteCashReplenishmentCommand, ServiceResponse<bool>>
    {
        private readonly ICashReplenishmentRepository _CashReplenishmentRepository; // Repository for accessing CashReplenishment data.
        private readonly ILogger<DeleteCashReplenishmentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteCashReplenishmentCommandHandler.
        /// </summary>
        /// <param name="CashReplenishmentRepository">Repository for CashReplenishment data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteCashReplenishmentCommandHandler(
            ICashReplenishmentRepository CashReplenishmentRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteCashReplenishmentCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _CashReplenishmentRepository = CashReplenishmentRepository;
            _logger = logger;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteCashReplenishmentCommand to delete a CashReplenishment.
        /// </summary>
        /// <param name="request">The DeleteCashReplenishmentCommand containing CashReplenishment ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteCashReplenishmentCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the CashReplenishment entity with the specified ID exists
                var existingCashReplenishment = await _CashReplenishmentRepository.FindAsync(request.Id);
                if (existingCashReplenishment == null)
                {
                    errorMessage = $"CashReplenishment with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingCashReplenishment.IsDeleted = true;
                _CashReplenishmentRepository.Update(existingCashReplenishment);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting CashReplenishment: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
