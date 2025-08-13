using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.MobileMoney.Commands;
using CBS.TransactionManagement.Repository.MobileMoney;

namespace CBS.TransactionManagement.MediatR.MobileMoney.Handlers
{
    /// <summary>
    /// Handles the command to delete a MobileMoneyCashTopup based on DeleteMobileMoneyCashTopupCommand.
    /// </summary>
    public class DeleteMobileMoneyCashTopupHandler : IRequestHandler<DeleteMobileMoneyCashTopupCommand, ServiceResponse<bool>>
    {
        private readonly IMobileMoneyCashTopupRepository _MobileMoneyCashTopupRepository; // Repository for accessing MobileMoneyCashTopup data.
        private readonly ILogger<DeleteMobileMoneyCashTopupHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteMobileMoneyCashTopupCommandHandler.
        /// </summary>
        /// <param name="MobileMoneyCashTopupRepository">Repository for MobileMoneyCashTopup data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteMobileMoneyCashTopupHandler(
            IMobileMoneyCashTopupRepository MobileMoneyCashTopupRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteMobileMoneyCashTopupHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _MobileMoneyCashTopupRepository = MobileMoneyCashTopupRepository;
            _logger = logger;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteMobileMoneyCashTopupCommand to delete a MobileMoneyCashTopup.
        /// </summary>
        /// <param name="request">The DeleteMobileMoneyCashTopupCommand containing MobileMoneyCashTopup ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteMobileMoneyCashTopupCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the MobileMoneyCashTopup entity with the specified ID exists
                var existingMobileMoneyCashTopup = await _MobileMoneyCashTopupRepository.FindAsync(request.Id);
                if (existingMobileMoneyCashTopup == null)
                {
                    errorMessage = $"MobileMoneyCashTopup with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                if (existingMobileMoneyCashTopup.RequestApprovalStatus==Status.Approved.ToString())
                {
                    errorMessage = $"Can not delete approved request.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                existingMobileMoneyCashTopup.IsDeleted = true;
                _MobileMoneyCashTopupRepository.Update(existingMobileMoneyCashTopup);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting MobileMoneyCashTopup: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }

}
