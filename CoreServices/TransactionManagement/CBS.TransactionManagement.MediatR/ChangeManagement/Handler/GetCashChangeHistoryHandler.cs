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
    /// Handles the retrieval of filtered cash change history.
    /// </summary>
    public class GetCashChangeHistoryHandler : IRequestHandler<GetCashChangeHistoryQuery, ServiceResponse<List<CashChangeHistoryDto>>>
    {
        private readonly ICashChangeHistoryRepository _cashChangeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCashChangeHistoryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCashChangeHistoryHandler"/> class.
        /// </summary>
        /// <param name="cashChangeRepository">Repository to access cash change data.</param>
        /// <param name="mapper">AutoMapper instance for mapping entities to DTOs.</param>
        /// <param name="logger">Logger for activity tracking.</param>
        public GetCashChangeHistoryHandler(ICashChangeHistoryRepository cashChangeRepository, IMapper mapper, ILogger<GetCashChangeHistoryHandler> logger)
        {
            _cashChangeRepository = cashChangeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the retrieval of filtered cash change history.
        /// </summary>
        /// <param name="request">Query object containing filter parameters.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>Service response containing a list of cash change history entries.</returns>
        public async Task<ServiceResponse<List<CashChangeHistoryDto>>> Handle(GetCashChangeHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Log the start of the history retrieval process.
                string startMessage = "Starting retrieval of filtered cash change history.";
                _logger.LogInformation(startMessage);
                await BaseUtilities.LogAndAuditAsync(startMessage, request, HttpStatusCodeEnum.OK, LogAction.RetrieveCashChangeHistory, LogLevelInfo.Information);

                // Retrieve all records from the repository.
                var queryable = _cashChangeRepository.All.Where(x => x.IsDeleted == false);

                // Apply filters if specified.
                if (!string.IsNullOrEmpty(request.VaultId))
                    queryable = queryable.Where(record => record.VaultId == request.VaultId);

                if (!string.IsNullOrEmpty(request.SubTellerId))
                    queryable = queryable.Where(record => record.SubTellerId == request.SubTellerId);

                if (!string.IsNullOrEmpty(request.PrimaryTellerId))
                    queryable = queryable.Where(record => record.PrimaryTellerId == request.PrimaryTellerId);

                if (!string.IsNullOrEmpty(request.BranchId))
                    queryable = queryable.Where(record => record.BranchId == request.BranchId);

                if (request.StartDate.HasValue)
                    queryable = queryable.Where(record => record.ChangeDate >= request.StartDate.Value);

                if (request.EndDate.HasValue)
                    queryable = queryable.Where(record => record.ChangeDate <= request.EndDate.Value);

                if (!string.IsNullOrEmpty(request.UserId))
                    queryable = queryable.Where(record => record.CreatedBy == request.UserId);

                // Execute query and project results.
                var filteredRecords = await queryable.ToListAsync(cancellationToken);

                if (!filteredRecords.Any())
                {
                    string noRecordsMessage = "No cash change history records match the specified filters.";
                    _logger.LogInformation(noRecordsMessage);
                    await BaseUtilities.LogAndAuditAsync(noRecordsMessage, request, HttpStatusCodeEnum.NotFound, LogAction.RetrieveCashChangeHistory, LogLevelInfo.Warning);
                    return ServiceResponse<List<CashChangeHistoryDto>>.Return404(noRecordsMessage);
                }

                // Map results to DTOs using AutoMapper.
                var historyDtos = _mapper.Map<List<CashChangeHistoryDto>>(filteredRecords);

                // Log successful operation.
                string successMessage = "Successfully retrieved filtered cash change history.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, historyDtos, HttpStatusCodeEnum.OK, LogAction.RetrieveCashChangeHistory, LogLevelInfo.Information);

                return ServiceResponse<List<CashChangeHistoryDto>>.ReturnResultWith200(historyDtos, successMessage);
            }
            catch (Exception ex)
            {
                // Log and audit unexpected errors.
                string errorMessage = $"An error occurred while retrieving cash change history: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.RetrieveCashChangeHistory, LogLevelInfo.Error);
                return ServiceResponse<List<CashChangeHistoryDto>>.Return500(errorMessage);
            }
        }
    }
}
