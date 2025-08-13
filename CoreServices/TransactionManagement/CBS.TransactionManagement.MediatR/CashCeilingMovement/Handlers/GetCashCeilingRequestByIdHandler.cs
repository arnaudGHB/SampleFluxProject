using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.CashCeilingMovement;
using CBS.TransactionManagement.Data.Dto.CashCeilingMovement;
using CBS.TransactionManagement.MediatR.CashCeilingMovement.Queries;

namespace CBS.TransactionManagement.CashCeilingMovement.Handlers
{

    /// <summary>
    /// Handles the retrieval of a specific cash ceiling request by its unique ID.
    /// Logs and audits the process for traceability and debugging purposes.
    /// </summary>
    public class GetCashCeilingRequestByIdHandler : IRequestHandler<GetCashCeilingRequestByIdQuery, ServiceResponse<CashCeilingRequestDto>>
    {
        private readonly ICashCeilingRequestRepository _cashCeilingRequestRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCashCeilingRequestByIdHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCashCeilingRequestByIdHandler"/> class.
        /// </summary>
        /// <param name="cashCeilingRequestRepository">Repository for cash ceiling request data access.</param>
        /// <param name="mapper">Mapper for converting entities to DTOs.</param>
        /// <param name="logger">Logger for logging and debugging purposes.</param>
        public GetCashCeilingRequestByIdHandler(
            ICashCeilingRequestRepository cashCeilingRequestRepository,
            IMapper mapper,
            ILogger<GetCashCeilingRequestByIdHandler> logger)
        {
            _cashCeilingRequestRepository = cashCeilingRequestRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the retrieval of a cash ceiling request by its ID.
        /// </summary>
        /// <param name="request">The query containing the ID of the cash ceiling request to retrieve.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>
        /// A ServiceResponse containing the requested CashCeilingRequestDto if found, or an appropriate error response.
        /// </returns>
        public async Task<ServiceResponse<CashCeilingRequestDto>> Handle(GetCashCeilingRequestByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // Retrieve the cash ceiling request by ID
                var cashCeilingRequest = await _cashCeilingRequestRepository.FindAsync(request.Id);

                // Check if the request exists
                if (cashCeilingRequest == null)
                {
                    string errorMessage = $"Cash ceiling request with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.CashCeilingRequestRetrieval, LogLevelInfo.Warning);
                    return ServiceResponse<CashCeilingRequestDto>.Return404(errorMessage);
                }

                // Map the entity to a DTO
                var cashCeilingRequestDto = _mapper.Map<CashCeilingRequestDto>(cashCeilingRequest);

                // Log and audit successful retrieval
                string successMessage = $"Successfully retrieved cash ceiling request with ID {request.Id}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.CashCeilingRequestRetrieval, LogLevelInfo.Information);

                // Return the result
                return ServiceResponse<CashCeilingRequestDto>.ReturnResultWith200(cashCeilingRequestDto);
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                string errorMessage = $"An error occurred while retrieving cash ceiling request with ID {request.Id}. Error: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CashCeilingRequestRetrieval, LogLevelInfo.Error);
                return ServiceResponse<CashCeilingRequestDto>.Return500(ex);
            }
        }
    }
}
