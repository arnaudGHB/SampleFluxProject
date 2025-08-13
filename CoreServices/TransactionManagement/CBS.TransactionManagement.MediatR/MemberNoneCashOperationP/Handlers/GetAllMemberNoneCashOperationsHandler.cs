using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.TellerP.Queries;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Queries;
using CBS.TransactionManagement.Data.Dto.MemberNoneCashOperationP;
using CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Queries;
using CBS.TransactionManagement.Repository.MemberNoneCashOperationP;

namespace CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Handlers
{
    /// <summary>
    /// Handles the query to retrieve Member None Cash Operations based on status and branch.
    /// </summary>
    public class GetAllMemberNoneCashOperationsHandler : IRequestHandler<GetAllMemberNoneCashOperationsQuery, ServiceResponse<List<MemberNoneCashOperationDto>>>
    {
        private readonly IMemberNoneCashOperationRepository _noneCashOperationRepository; // Repository for fetching none cash operations.
        private readonly IMapper _mapper; // Mapper for converting entities to DTOs.
        private readonly ILogger<GetAllMemberNoneCashOperationsHandler> _logger; // Logger for logging actions and errors.
        private readonly UserInfoToken _userInfoToken; // Repository for fetching none cash operations.

        public GetAllMemberNoneCashOperationsHandler(
            IMemberNoneCashOperationRepository noneCashOperationRepository,
            IMapper mapper,
            ILogger<GetAllMemberNoneCashOperationsHandler> logger,
            UserInfoToken userInfoToken)
        {
            _noneCashOperationRepository = noneCashOperationRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken=userInfoToken;
        }

        /// <summary>
        /// Handles the query to retrieve Member None Cash Operations based on status and branch filters.
        /// </summary>
        /// <param name="request">The query containing status and branch filters.</param>
        /// <param name="cancellationToken">Cancellation token for the request.</param>
        public async Task<ServiceResponse<List<MemberNoneCashOperationDto>>> Handle(GetAllMemberNoneCashOperationsQuery request, CancellationToken cancellationToken)
        {
            string logReference = BaseUtilities.GenerateUniqueNumber(); // Generate a unique reference for tracking.

            try
            {
                // Step 1: Initialize the base query for active (non-deleted) records
                var query = _noneCashOperationRepository.All
                    .Include(x => x.Account)
                    .AsNoTracking() // Optimizes for read-only operations
                    .Where(x => !x.IsDeleted);

                // Step 2: Apply optional status filter
                if (!string.IsNullOrWhiteSpace(request.Status) && request.Status.ToLower() != "all")
                {
                    var validStatuses = new HashSet<string> { Status.Pending.ToString(), Status.Approved.ToString(), Status.Rejected.ToString(), "my_requests" };

                    if (!validStatuses.Contains(request.Status))
                    {
                        string errorMessage = "Invalid status provided. Allowed values are: Pending, Approved, Rejected, my_requests, All.";
                        _logger.LogWarning(errorMessage);
                        await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.GetMemberNoneCashOperations, LogLevelInfo.Warning, logReference);
                        return ServiceResponse<List<MemberNoneCashOperationDto>>.Return400(errorMessage);
                    }
                    // Step 2a: Handle special "my_requests" case
                    if (request.Status.ToLower() == "my_requests")
                    {
                        string currentUserId = _userInfoToken.Id; // Assuming you have a service to get the current user
                        query = query.Where(op => op.InitiatedByUSerId == currentUserId);
                    }
                    else
                    {
                        // Filter by specific status
                        query = query.Where(op => op.ApprovalStatus == request.Status);
                    }
                }

                // Step 3: Apply optional branch filter
                if (!string.IsNullOrWhiteSpace(request.BranchId))
                {
                    query = query.Where(op => op.BranchId == request.BranchId);
                }

                // Step 4: Retrieve and map results efficiently
                var operations = await query
                    .OrderByDescending(op => op.RequestDate) // Optimization: order before materializing results
                    .ToListAsync(cancellationToken);

                var operationDtos = _mapper.Map<List<MemberNoneCashOperationDto>>(operations);

                // Step 5: Log and return the results
                string successMessage = "Member None Cash Operations retrieved successfully.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.GetMemberNoneCashOperations, LogLevelInfo.Information, logReference);

                return ServiceResponse<List<MemberNoneCashOperationDto>>.ReturnResultWith200(operationDtos, successMessage);
            }
            catch (Exception ex)
            {
                // Step 6: Handle and log any unexpected exceptions
                string errorMessage = $"An error occurred while retrieving Member None Cash Operations: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.GetMemberNoneCashOperations, LogLevelInfo.Error, logReference);
                return ServiceResponse<List<MemberNoneCashOperationDto>>.Return500(errorMessage);
            }
        }
    }
}
