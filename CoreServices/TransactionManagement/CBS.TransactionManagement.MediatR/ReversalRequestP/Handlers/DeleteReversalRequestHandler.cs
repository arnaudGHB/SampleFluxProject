using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.Commands.ReversalRequestP;

namespace CBS.TransactionManagement.MediatR.Handlers.ReversalRequestP
{
    /// <summary>
    /// Handles the command to delete a ReversalRequest based on DeleteReversalRequestCommand.
    /// </summary>
    public class DeleteReversalRequestHandler : IRequestHandler<DeleteReversalRequestCommand, ServiceResponse<bool>>
    {
        private readonly IReversalRequestRepository _ReversalRequestRepository; // Repository for accessing ReversalRequest data.
        private readonly ILogger<DeleteReversalRequestHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteReversalRequestCommandHandler.
        /// </summary>
        /// <param name="ReversalRequestRepository">Repository for ReversalRequest data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteReversalRequestHandler(
            IReversalRequestRepository ReversalRequestRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteReversalRequestHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _ReversalRequestRepository = ReversalRequestRepository;
            _logger = logger;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteReversalRequestCommand to delete a ReversalRequest.
        /// </summary>
        /// <param name="request">The DeleteReversalRequestCommand containing ReversalRequest ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteReversalRequestCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the ReversalRequest entity with the specified ID exists
                var existingReversalRequest = await _ReversalRequestRepository.FindAsync(request.Id);
                if (existingReversalRequest == null)
                {
                    errorMessage = $"ReversalRequest with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingReversalRequest.IsDeleted = true;
                _ReversalRequestRepository.Update(existingReversalRequest);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting ReversalRequest: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
