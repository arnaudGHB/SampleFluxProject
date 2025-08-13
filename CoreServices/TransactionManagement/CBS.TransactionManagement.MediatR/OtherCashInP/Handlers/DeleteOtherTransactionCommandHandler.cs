using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.otherCashIn.Commands;
using CBS.TransactionManagement.Repository.OtherCashIn;

namespace CBS.TransactionManagement.otherCashIn.Handlers
{
    /// <summary>
    /// Handles the command to delete a OtherTransaction based on DeleteOtherTransactionCommand.
    /// </summary>
    public class DeleteOtherTransactionCommandHandler : IRequestHandler<DeleteOtherTransactionCommand, ServiceResponse<bool>>
    {
        private readonly IOtherTransactionRepository _OtherTransactionRepository; // Repository for accessing OtherTransaction data.
        private readonly ILogger<DeleteOtherTransactionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteOtherTransactionCommandHandler.
        /// </summary>
        /// <param name="OtherTransactionRepository">Repository for OtherTransaction data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteOtherTransactionCommandHandler(
            IOtherTransactionRepository OtherTransactionRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteOtherTransactionCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _OtherTransactionRepository = OtherTransactionRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteOtherTransactionCommand to delete a OtherTransaction.
        /// </summary>
        /// <param name="request">The DeleteOtherTransactionCommand containing OtherTransaction ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteOtherTransactionCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the OtherTransaction entity with the specified ID exists
                var existingOtherTransaction = await _OtherTransactionRepository.FindAsync(request.Id);
                if (existingOtherTransaction == null)
                {
                    errorMessage = $"OtherTransaction with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingOtherTransaction.IsDeleted = true;
                _OtherTransactionRepository.Update(existingOtherTransaction);
                if (await _uow.SaveAsync() <= 0)
                {
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, "An error occurred deleting deposit limit", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return500();
                }
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, "Deposit limit deleted successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting OtherTransaction: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
