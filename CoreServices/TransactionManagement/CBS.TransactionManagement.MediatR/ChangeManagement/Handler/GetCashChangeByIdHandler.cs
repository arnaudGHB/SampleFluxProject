using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.DailyTellerP.Queries;

namespace CBS.TransactionManagement.MediatR.ChangeManagement.Handler
{


    /// <summary>
    /// Handles the retrieval of a specific cash change operation by its ID.
    /// </summary>
    public class GetCashChangeByIdHandler : IRequestHandler<GetCashChangeByIdQuery, ServiceResponse<CashChangeHistoryDto>>
    {
        private readonly ICashChangeHistoryRepository _cashChangeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCashChangeByIdHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCashChangeByIdHandler"/> class.
        /// </summary>
        /// <param name="cashChangeRepository">Repository for accessing cash change operations.</param>
        /// <param name="mapper">AutoMapper instance for mapping entities to DTOs.</param>
        /// <param name="logger">Logger for activity tracking.</param>
        public GetCashChangeByIdHandler(ICashChangeHistoryRepository cashChangeRepository, IMapper mapper, ILogger<GetCashChangeByIdHandler> logger)
        {
            _cashChangeRepository = cashChangeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the retrieval of a specific cash change operation by its ID.
        /// </summary>
        /// <param name="request">Query containing the unique ID of the operation.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Service response containing the cash change operation details.</returns>
        public async Task<ServiceResponse<CashChangeHistoryDto>> Handle(GetCashChangeByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Log the start of the operation.
                string startMessage = $"Starting retrieval of cash change operation with ID: {request.Id}.";
                _logger.LogInformation(startMessage);
                // Validate the ID.
                if (string.IsNullOrEmpty(request.Id))
                {
                    string invalidIdMessage = "Cash change ID is null or empty.";
                    _logger.LogWarning(invalidIdMessage);
                    await BaseUtilities.LogAndAuditAsync(invalidIdMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.RetrieveCashChangeById, LogLevelInfo.Warning);
                    return ServiceResponse<CashChangeHistoryDto>.Return400("Invalid ID provided.");
                }

                // Retrieve the record from the repository.
                var record = await _cashChangeRepository.FindAsync(request.Id);
                if (record == null)
                {
                    string notFoundMessage = $"No cash change operation found with ID: {request.Id}.";
                    _logger.LogInformation(notFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.RetrieveCashChangeById, LogLevelInfo.Warning);
                    return ServiceResponse<CashChangeHistoryDto>.Return404("Cash change operation not found.");
                }

                // Map the record to a DTO.
                var historyDto = _mapper.Map<CashChangeHistoryDto>(record);

                // Log and audit the success of the operation.
                string successMessage = $"Successfully retrieved cash change operation with ID: {request.Id}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, historyDto, HttpStatusCodeEnum.OK, LogAction.RetrieveCashChangeById, LogLevelInfo.Information);

                return ServiceResponse<CashChangeHistoryDto>.ReturnResultWith200(historyDto, "Cash change operation retrieved successfully.");
            }
            catch (Exception ex)
            {
                // Log and audit the exception.
                string errorMessage = $"Error retrieving cash change operation with ID {request.Id}: {ex.Message}.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.RetrieveCashChangeById, LogLevelInfo.Error);
                return ServiceResponse<CashChangeHistoryDto>.Return500("An error occurred while retrieving the cash change operation.");
            }
        }
    }
}
