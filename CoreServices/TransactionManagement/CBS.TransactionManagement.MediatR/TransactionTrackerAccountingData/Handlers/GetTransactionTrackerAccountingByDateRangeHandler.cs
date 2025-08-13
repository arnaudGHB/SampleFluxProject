using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Queries;
using MongoDB.Driver;
using CBS.TransactionManagement.Common.Repository.Uow;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Data.Entity.MongoDBObjects;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Handlers
{
    /// <summary>
    /// Handler for retrieving TransactionTrackerAccounting records between specified dates.
    /// </summary>
    public class GetTransactionTrackerAccountingByDateRangeHandler : IRequestHandler<GetTransactionTrackerAccountingByDateRangeQuery, ServiceResponse<List<TransactionTrackerAccountingDto>>>
    {
        private readonly ILogger<GetTransactionTrackerAccountingByDateRangeHandler> _logger;
        private readonly IMongoUnitOfWork _mongoUnitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for initializing GetTransactionTrackerAccountingByDateRangeHandler.
        /// </summary>
        /// <param name="logger">Logger for logging actions and errors.</param>
        /// <param name="mongoUnitOfWork">MongoDB Unit of Work for database operations.</param>
        /// <param name="mapper">Mapper for mapping objects.</param>
        public GetTransactionTrackerAccountingByDateRangeHandler(
            ILogger<GetTransactionTrackerAccountingByDateRangeHandler> logger,
            IMongoUnitOfWork mongoUnitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _mongoUnitOfWork = mongoUnitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the query to retrieve TransactionTrackerAccounting records between dates.
        /// </summary>
        /// <param name="request">The query request containing the date range.</param>
        /// <param name="cancellationToken">Token to signal cancellation of the operation.</param>
        /// <returns>A list of TransactionTrackerAccounting records between the specified dates.</returns>
        public async Task<ServiceResponse<List<TransactionTrackerAccountingDto>>> Handle(GetTransactionTrackerAccountingByDateRangeQuery request, CancellationToken cancellationToken)
        {
            var logAction = LogAction.TransactionTrackerAccounting;
            try
            {
                // Log the query details
                _logger.LogInformation(
                    "Retrieving TransactionTrackerAccounting records between {StartDate} and {EndDate}",
                    request.StartDate,
                    request.EndDate);

                // Retrieve the repository
                var repository = _mongoUnitOfWork.GetRepository<TransactionTrackerAccounting>();

                // Build the filter for MongoDB
                var filter = Builders<TransactionTrackerAccounting>.Filter.And(
                    Builders<TransactionTrackerAccounting>.Filter.Gte("CreatedDate", request.StartDate),
                    Builders<TransactionTrackerAccounting>.Filter.Lte("CreatedDate", request.EndDate));

                // Execute the query
                var entities = await repository.FindByAsync(filter);

                // Map entities to DTOs
                var dtos = _mapper.Map<List<TransactionTrackerAccountingDto>>(entities);

                string message = $"Successfully retrieved TransactionTrackerAccounting records between {request.StartDate} and {request.EndDate}";
                // Log and audit the successful operation
                await BaseUtilities.LogAndAuditAsync(
                    message,
                    request,
                    HttpStatusCodeEnum.OK,
                    logAction,
                    LogLevelInfo.Information);

                // Return success response
                return ServiceResponse<List<TransactionTrackerAccountingDto>>.ReturnResultWith200(dtos, message);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error occurred while retrieving TransactionTrackerAccounting records between {request.StartDate} and {request.EndDate}: {ex.Message}";

                // Log the error
                _logger.LogError(ex, errorMessage);

                // Log and audit the failed operation
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    logAction,
                    LogLevelInfo.Error);

                // Return a 500 Internal Server Error response with error details
                return ServiceResponse<List<TransactionTrackerAccountingDto>>.Return500(errorMessage);
            }
        }
    }
}
