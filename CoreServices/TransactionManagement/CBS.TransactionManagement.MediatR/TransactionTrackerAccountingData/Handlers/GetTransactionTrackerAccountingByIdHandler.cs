using AutoMapper;
using CBS.TransactionManagement.Common.Repository.Uow;
using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.Data.Entity.MongoDBObjects;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Handlers
{
    /// <summary>
    /// Handler for retrieving a TransactionTrackerAccounting record by ID.
    /// </summary>
    public class GetTransactionTrackerAccountingByIdHandler : IRequestHandler<GetTransactionTrackerAccountingByIdQuery, ServiceResponse<TransactionTrackerAccountingDto>>
    {
        private readonly ILogger<GetTransactionTrackerAccountingByIdHandler> _logger;
        private readonly IMongoUnitOfWork _mongoUnitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for initializing GetTransactionTrackerAccountingByIdHandler.
        /// </summary>
        /// <param name="logger">Logger for logging actions and errors.</param>
        /// <param name="mongoUnitOfWork">MongoDB Unit of Work for database operations.</param>
        /// <param name="mapper">Mapper for mapping objects.</param>
        public GetTransactionTrackerAccountingByIdHandler(
            ILogger<GetTransactionTrackerAccountingByIdHandler> logger,
            IMongoUnitOfWork mongoUnitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _mongoUnitOfWork = mongoUnitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the query to retrieve a TransactionTrackerAccounting record by ID.
        /// </summary>
        /// <param name="request">The query request containing the ID.</param>
        /// <param name="cancellationToken">Token to signal cancellation of the operation.</param>
        /// <returns>The TransactionTrackerAccounting record.</returns>
        public async Task<ServiceResponse<TransactionTrackerAccountingDto>> Handle(GetTransactionTrackerAccountingByIdQuery request, CancellationToken cancellationToken)
        {
            var logAction = LogAction.TransactionTrackerAccounting;

            try
            {
                _logger.LogInformation("Attempting to retrieve TransactionTrackerAccounting record with ID: {Id}", request.Id);

                // Retrieve the repository
                var repository = _mongoUnitOfWork.GetRepository<TransactionTrackerAccounting>();

                // Fetch the record by ID
                var entity = await repository.GetByIdAsync(request.Id);

                if (entity == null)
                {
                    var notFoundMessage = $"TransactionTrackerAccounting record with ID: {request.Id} not found.";
                    _logger.LogWarning(notFoundMessage);

                    // Log and audit not-found scenario
                    await BaseUtilities.LogAndAuditAsync(
                        notFoundMessage,
                        request,
                        HttpStatusCodeEnum.NotFound,
                        logAction,
                        LogLevelInfo.Warning,
                        request.Id);

                    return ServiceResponse<TransactionTrackerAccountingDto>.Return404(notFoundMessage);
                }

                // Map entity to DTO
                var dto = _mapper.Map<TransactionTrackerAccountingDto>(entity);
                string message = $"Successfully retrieved TransactionTrackerAccounting record with ID: {request.Id}";
                // Log and audit successful operation
                await BaseUtilities.LogAndAuditAsync(
                    message,
                    request,
                    HttpStatusCodeEnum.OK,
                    logAction,
                    LogLevelInfo.Information,
                    request.Id);

                return ServiceResponse<TransactionTrackerAccountingDto>.ReturnResultWith200(dto, message);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error occurred while retrieving TransactionTrackerAccounting record with ID: {request.Id}: {ex.Message}";

                // Log the error
                _logger.LogError(ex, errorMessage);

                // Log and audit failed operation
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    logAction,
                    LogLevelInfo.Error,
                    request.Id);

                // Return error response
                return ServiceResponse<TransactionTrackerAccountingDto>.Return500(errorMessage);
            }
        }
    }
}
