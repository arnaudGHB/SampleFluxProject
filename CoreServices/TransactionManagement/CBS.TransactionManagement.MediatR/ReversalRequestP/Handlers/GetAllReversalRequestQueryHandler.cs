using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.TellerP.Queries;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries.ReversalRequestP;
using CBS.TransactionManagement.Data.Dto.ReversalRequestP;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Handlers.ReversalRequestP
{
    /// <summary>
    /// Handles the retrieval of all ReversalRequest based on the GetAllReversalRequestQuery.
    /// </summary>
    public class GetAllReversalRequestQueryHandler : IRequestHandler<GetAllReversalRequestQuery, ServiceResponse<List<ReversalRequestDto>>>
    {
        private readonly IReversalRequestRepository _ReversalRequestRepository; // Repository for accessing ReversalRequest data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllReversalRequestQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllReversalRequestQueryHandler.
        /// </summary>
        /// <param name="ReversalRequestRepository">Repository for ReversalRequest data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllReversalRequestQueryHandler(
            IReversalRequestRepository ReversalRequestRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllReversalRequestQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _ReversalRequestRepository = ReversalRequestRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllReversalRequestQuery to retrieve all ReversalRequest.
        /// </summary>
        /// <param name="request">The GetAllReversalRequestQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ReversalRequestDto>>> Handle(GetAllReversalRequestQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _ReversalRequestRepository.FindBy(x => x.IsDeleted == false);

                // Filter by Branch if needed
                if (request.IsBranch)
                {
                    query = query.Where(x => x.BranchId == request.BranchId);
                }

                // Filter by Date if needed
                if (request.IsByDate)
                {
                    DateTime dateFrom = Convert.ToDateTime(request.DateFrom).Date;
                    DateTime dateTo = Convert.ToDateTime(request.DateTo).Date;
                    query = query.Where(x => x.CreatedDate.Date >= dateFrom && x.CreatedDate.Date <= dateTo);
                }
                
                // Filter by Status based on QueryString
                if (!string.IsNullOrEmpty(request.QueryString))
                {
                    switch (request.QueryString.ToLower())
                    {
                        case "pending_validated_approved":
                            query = query.Where(x => x.Status == "Pending" || x.Status == "Approved" || x.Status == "Validated");
                            break;
                        case "pending":
                            query = query.Where(x => x.Status == "Pending");
                            break;
                        case "approved":
                            query = query.Where(x => x.Status == "Approved");
                            break;
                        case "treated":
                            query = query.Where(x => x.Status == "Treated");
                            break;
                        case "rejected":
                            query = query.Where(x => x.Status == "Rejected");
                            break;
                        default:
                            // If QueryString does not match any specific status, include all statuses
                            break;
                    }
                }

                var entities = await query.ToListAsync(cancellationToken);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "System ReversalRequests returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<ReversalRequestDto>>.ReturnResultWith200(_mapper.Map<List<ReversalRequestDto>>(entities));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get all ReversalRequest: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all ReversalRequest: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<ReversalRequestDto>>.Return500(e, "Failed to get all ReversalRequest");
            }
        }

    }
}
