using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.CashCeilingMovement;
using CBS.TransactionManagement.Data.Dto.CashCeilingMovement;
using CBS.TransactionManagement.MediatR.CashCeilingMovement.Queries;

namespace CBS.TransactionManagement.CashCeilingMovement.Handlers
{



    /// <summary>
    /// Handles the CashCeilingRequestRetrieval of all cash ceiling requests with optional filtering by branch ID and approval status.
    /// Logs and audits the process at key steps for traceability and debugging.
    /// </summary>
    /// <param name="request">The query containing optional filtering parameters (BranchId, Status).</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A ServiceResponse containing a list of CashCeilingRequestDto objects, or an appropriate error response.</returns>
    public class GetAllCashCeilingRequestsHandler : IRequestHandler<GetAllCashCeilingRequestsQuery, ServiceResponse<List<CashCeilingRequestDto>>>
    {
        private readonly ICashCeilingRequestRepository _cashCeilingRequestRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllCashCeilingRequestsHandler> _logger;

        public GetAllCashCeilingRequestsHandler(
            ICashCeilingRequestRepository cashCeilingRequestRepository,
            IMapper mapper,
            ILogger<GetAllCashCeilingRequestsHandler> logger)
        {
            _cashCeilingRequestRepository = cashCeilingRequestRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all cash ceiling requests, optionally filtered by branch ID and approval status.
        /// </summary>
        /// <param name="request">The query containing optional filtering parameters.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A ServiceResponse containing the list of filtered cash ceiling requests, or an error message.</returns>
        public async Task<ServiceResponse<List<CashCeilingRequestDto>>> Handle(GetAllCashCeilingRequestsQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // Build a filtered query dynamically based on provided parameters
                var cashCeilingRequestsQuery = _cashCeilingRequestRepository.All.Where(x => x.IsDeleted == false);

                if (!string.IsNullOrWhiteSpace(request.BranchId) || !string.IsNullOrWhiteSpace(request.Status) || !string.IsNullOrWhiteSpace(request.UserId))
                {
                    cashCeilingRequestsQuery = cashCeilingRequestsQuery.Where(c =>
                        (string.IsNullOrEmpty(request.BranchId) || c.BranchId == request.BranchId) &&
                        (string.IsNullOrEmpty(request.RequestType) || c.RequestType == request.RequestType) &&
                        (string.IsNullOrEmpty(request.Status) || c.ApprovedStatus == request.Status) &&
                        (string.IsNullOrEmpty(request.UserId) || c.RequestedByUserId == request.UserId));
                }


                // Fetch the filtered data as a list
                var cashCeilingRequests = await cashCeilingRequestsQuery.ToListAsync(cancellationToken);

                // Map the entities to DTOs
                var cashCeilingRequestDtos = _mapper.Map<List<CashCeilingRequestDto>>(cashCeilingRequests);

                // Log and audit successful CashCeilingRequestRetrieval
                string successMessage = $"Successfully retrieved {cashCeilingRequestDtos.Count} cash ceiling requests.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.CashCeilingRequestRetrieval, LogLevelInfo.Information);
                _logger.LogInformation(successMessage);

                // Return the result
                return ServiceResponse<List<CashCeilingRequestDto>>.ReturnResultWith200(cashCeilingRequestDtos);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                string errorMessage = "An error occurred while retrieving cash ceiling requests.";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CashCeilingRequestRetrieval, LogLevelInfo.Error);
                return ServiceResponse<List<CashCeilingRequestDto>>.Return500(ex);
            }
        }
    }
}
