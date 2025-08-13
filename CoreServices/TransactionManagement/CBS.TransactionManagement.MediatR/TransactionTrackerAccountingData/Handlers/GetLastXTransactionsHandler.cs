using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.Common.Repository.Uow;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Queries;
using MongoDB.Driver;
using CBS.TransactionManagement.Data.Entity.MongoDBObjects;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Handlers
{
    /// <summary>
    /// Handler for retrieving the last X transactions from the database.
    /// </summary>
    public class GetLastXTransactionsHandler : IRequestHandler<GetLastXTransactionsQuery, ServiceResponse<List<TransactionTrackerAccountingDto>>>
    {
        private readonly ILogger<GetLastXTransactionsHandler> _logger;
        private readonly IMongoUnitOfWork _mongoUnitOfWork;
        private readonly IMapper _mapper;
        private const int MaxTransactionLimit = 100; // Maximum allowed transactions to retrieve

        /// <summary>
        /// Constructor for initializing GetLastXTransactionsHandler.
        /// </summary>
        /// <param name="logger">Logger for logging actions and errors.</param>
        /// <param name="mongoUnitOfWork">MongoDB Unit of Work for database operations.</param>
        /// <param name="mapper">Mapper for mapping objects.</param>
        public GetLastXTransactionsHandler(
            ILogger<GetLastXTransactionsHandler> logger,
            IMongoUnitOfWork mongoUnitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _mongoUnitOfWork = mongoUnitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the query to retrieve the last X transactions.
        /// </summary>
        public async Task<ServiceResponse<List<TransactionTrackerAccountingDto>>> Handle(GetLastXTransactionsQuery request, CancellationToken cancellationToken)
        {
            var logAction = LogAction.GetLastXTransactionTrackerAccounting;
            try
            {
                // Validate the transaction count
                if (request.TransactionCount > MaxTransactionLimit)
                {
                    var warningMessage = $"Requested TransactionTrackerAccounting count {request.TransactionCount} exceeds the maximum limit of {MaxTransactionLimit}.";
                    _logger.LogWarning(warningMessage);
                    await BaseUtilities.LogAndAuditAsync(warningMessage, request, HttpStatusCodeEnum.BadRequest, logAction, LogLevelInfo.Warning);
                    return ServiceResponse<List<TransactionTrackerAccountingDto>>.Return400(warningMessage);
                }

                _logger.LogInformation("Retrieving the last {TransactionCount} TransactionTrackerAccounting.", request.TransactionCount);

                // Retrieve transactions from the repository
                var repository = _mongoUnitOfWork.GetRepository<TransactionTrackerAccounting>();
                var sort = Builders<TransactionTrackerAccounting>.Sort.Descending("CreatedDate");

                var entities = await repository.GetWithAdvancedFilterAsync(
                    filter: FilterDefinition<TransactionTrackerAccounting>.Empty,
                    sort: sort,
                    limit: request.TransactionCount,
                    cancellationToken: cancellationToken);

                // Map entities to DTOs
                var dtos = _mapper.Map<List<TransactionTrackerAccountingDto>>(entities);

                var successMessage = $"Successfully retrieved the last {request.TransactionCount} TransactionTrackerAccounting.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, logAction, LogLevelInfo.Information);

                return ServiceResponse<List<TransactionTrackerAccountingDto>>.ReturnResultWith200(dtos, successMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error occurred while retrieving the last {request.TransactionCount} TransactionTrackerAccounting: {ex.Message}";
                _logger.LogError(ex, errorMessage);

                // Log and audit the error
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, logAction, LogLevelInfo.Error);
                return ServiceResponse<List<TransactionTrackerAccountingDto>>.Return500(errorMessage);
            }
        }
    }






}
