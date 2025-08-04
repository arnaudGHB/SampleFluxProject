using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to delete a DepositLimit based on DeleteDepositLimitCommand.
    /// </summary>
    public class DeleteDepositLimitCommandHandler : IRequestHandler<DeleteDepositLimitCommand, ServiceResponse<bool>>
    {
        private readonly IDepositLimitRepository _DepositLimitRepository; // Repository for accessing DepositLimit data.
        private readonly ILogger<DeleteDepositLimitCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteDepositLimitCommandHandler.
        /// </summary>
        /// <param name="DepositLimitRepository">Repository for DepositLimit data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteDepositLimitCommandHandler(
            IDepositLimitRepository DepositLimitRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteDepositLimitCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _DepositLimitRepository = DepositLimitRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteDepositLimitCommand to delete a DepositLimit.
        /// </summary>
        /// <param name="request">The DeleteDepositLimitCommand containing DepositLimit ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteDepositLimitCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the DepositLimit entity with the specified ID exists
                var existingDepositLimit = await _DepositLimitRepository.FindAsync(request.Id);
                if (existingDepositLimit == null)
                {
                    errorMessage = $"DepositLimit with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingDepositLimit.IsDeleted = true;
                _DepositLimitRepository.Update(existingDepositLimit);
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
                errorMessage = $"Error occurred while deleting DepositLimit: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
