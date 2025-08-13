using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.CashReplenishmentPrimaryTellerP.Commands;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentPrimaryTellerPrimaryTellerP.Handlers
{
    /// <summary>
    /// Handles the command to delete a CashReplenishmentPrimaryTeller based on DeleteCashReplenishmentPrimaryTellerCommand.
    /// </summary>
    public class DeleteCashReplenishmentPrimaryTellerPrimaryTellerHandler : IRequestHandler<DeleteCashReplenishmentPrimaryTellerCommand, ServiceResponse<bool>>
    {
        private readonly ICashReplenishmentPrimaryTellerRepository _CashReplenishmentPrimaryTellerRepository; // Repository for accessing CashReplenishmentPrimaryTeller data.
        private readonly ILogger<DeleteCashReplenishmentPrimaryTellerPrimaryTellerHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteCashReplenishmentPrimaryTellerCommandHandler.
        /// </summary>
        /// <param name="CashReplenishmentPrimaryTellerRepository">Repository for CashReplenishmentPrimaryTeller data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteCashReplenishmentPrimaryTellerPrimaryTellerHandler(
            ICashReplenishmentPrimaryTellerRepository CashReplenishmentPrimaryTellerRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteCashReplenishmentPrimaryTellerPrimaryTellerHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _CashReplenishmentPrimaryTellerRepository = CashReplenishmentPrimaryTellerRepository;
            _logger = logger;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteCashReplenishmentPrimaryTellerCommand to delete a CashReplenishmentPrimaryTeller.
        /// </summary>
        /// <param name="request">The DeleteCashReplenishmentPrimaryTellerCommand containing CashReplenishmentPrimaryTeller ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteCashReplenishmentPrimaryTellerCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the CashReplenishmentPrimaryTeller entity with the specified ID exists
                var existingCashReplenishmentPrimaryTeller = await _CashReplenishmentPrimaryTellerRepository.FindAsync(request.Id);
                if (existingCashReplenishmentPrimaryTeller == null)
                {
                    errorMessage = $"CashReplenishmentPrimaryTeller with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingCashReplenishmentPrimaryTeller.IsDeleted = true;
                _CashReplenishmentPrimaryTellerRepository.Update(existingCashReplenishmentPrimaryTeller);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting CashReplenishmentPrimaryTeller: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
