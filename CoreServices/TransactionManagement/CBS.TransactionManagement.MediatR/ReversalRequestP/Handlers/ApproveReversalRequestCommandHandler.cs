using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data.Dto.ReversalRequestP;
using CBS.TransactionManagement.Data.Entity.ReversalRequestP;
using CBS.TransactionManagement.MediatR.Commands.ReversalRequestP;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Handlers.ReversalRequestP
{

    /// <summary>
    /// Handles the command to approve or reject a reversal request.
    /// </summary>
    public class ApproveReversalRequestCommandHandler : IRequestHandler<ApprovedReversalRequestCommand, ServiceResponse<ReversalRequestDto>>
    {
        private readonly IReversalRequestRepository _reversalRequestRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ApproveReversalRequestCommandHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApproveReversalRequestCommandHandler"/> class.
        /// </summary>
        public ApproveReversalRequestCommandHandler(
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<ApproveReversalRequestCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            IReversalRequestRepository reversalRequestRepository = null)
        {
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _reversalRequestRepository = reversalRequestRepository;
        }

        /// <summary>
        /// Handles the approval or rejection of a reversal request.
        /// </summary>
        /// <param name="request">The command containing approval details.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ReversalRequestDto>> Handle(ApprovedReversalRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the reversal request by ID
                var reversalRequest = await _reversalRequestRepository.FindAsync(request.Id);
                if (reversalRequest == null)
                {
                    string errorMessage = $"Reversal request with ID {request.Id} was not found.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.ApprovedReversalRequest, LogLevelInfo.Error);
                    return ServiceResponse<ReversalRequestDto>.Return404(errorMessage);
                }

                // Step 2: Check if the request has already been approved
                if (reversalRequest.IsAppoved)
                {
                    string errorMessage = $"Reversal request ID {request.Id} has already been approved.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Conflict, LogAction.ApprovedReversalRequest, LogLevelInfo.Warning);
                    return ServiceResponse<ReversalRequestDto>.Return409(errorMessage);
                }

                // Step 3: Process the approval or rejection
                if (request.Status == Status.Approved.ToString())
                {
                    reversalRequest.IsAppoved = true;
                    reversalRequest.RequestStatus = true;
                }

                reversalRequest.Status = request.Status;
                reversalRequest.ApprovedBy = _userInfoToken.FullName;
                reversalRequest.ApprovedDate = BaseUtilities.UtcNowToDoualaTime();
                reversalRequest.ApprovedComment = request.ApprovedComment;

                // Step 4: Update the request and save changes
                _reversalRequestRepository.Update(reversalRequest);
                await _uow.SaveAsync();

                // Step 5: Log and audit successful request approval
                string successMessage = $"Reversal request ID {request.Id} was successfully {request.Status.ToLower()} by {_userInfoToken.FullName}.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.ApprovedReversalRequest, LogLevelInfo.Information);

                // Step 6: Map the entity to a DTO and return success response
                var reversalRequestDto = _mapper.Map<ReversalRequestDto>(reversalRequest);
                return ServiceResponse<ReversalRequestDto>.ReturnResultWith200(reversalRequestDto, successMessage);
            }
            catch (Exception e)
            {
                // Step 7: Log and audit error if any exception occurs
                string errorMessage = $"An error occurred while processing the reversal request approval: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.ApprovedReversalRequest, LogLevelInfo.Error);
                return ServiceResponse<ReversalRequestDto>.Return500(e, errorMessage);
            }
        }
    }

}
