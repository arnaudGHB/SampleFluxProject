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
    /// Handler for retrieving a transaction by its reference.
    /// </summary>
    public class GetTransactionByReferenceHandler : IRequestHandler<GetTransactionByReferenceQuery, ServiceResponse<TransactionTrackerAccountingDto>>
    {
        private readonly ILogger<GetTransactionByReferenceHandler> _logger;
        private readonly IMongoUnitOfWork _mongoUnitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for initializing GetTransactionByReferenceHandler.
        /// </summary>
        /// <param name="logger">Logger for logging actions and errors.</param>
        /// <param name="mongoUnitOfWork">MongoDB Unit of Work for database operations.</param>
        /// <param name="mapper">Mapper for mapping objects.</param>
        public GetTransactionByReferenceHandler(
            ILogger<GetTransactionByReferenceHandler> logger,
            IMongoUnitOfWork mongoUnitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _mongoUnitOfWork = mongoUnitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the query to retrieve a transaction by its reference.
        /// </summary>
        /// <param name="request">The query request containing the transaction reference.</param>
        /// <param name="cancellationToken">Token to signal cancellation of the operation.</param>
        /// <returns>The transaction matching the specified reference.</returns>
        public async Task<ServiceResponse<TransactionTrackerAccountingDto>> Handle(GetTransactionByReferenceQuery request, CancellationToken cancellationToken)
        {
            var logAction = LogAction.TransactionTrackerAccounting; // Define log action
            try
            {
                _logger.LogInformation("Retrieving transaction with reference: {TransactionReference}", request.TransactionReference);

                // Get repository and apply filter
                var repository = _mongoUnitOfWork.GetRepository<TransactionTrackerAccounting>();
                var filter = Builders<TransactionTrackerAccounting>.Filter.Eq("TransactionReference", request.TransactionReference);

                var entities = await repository.GetWithAdvancedFilterAsync(filter: filter, limit: 1, cancellationToken: cancellationToken);

                if (entities == null || !entities.Any())
                {
                    var notFoundMessage = $"Transaction with reference {request.TransactionReference} not found.";
                    _logger.LogWarning(notFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(
                        notFoundMessage,
                        request,
                        HttpStatusCodeEnum.NotFound,
                        logAction,
                        LogLevelInfo.Warning);

                    return ServiceResponse<TransactionTrackerAccountingDto>.Return404(notFoundMessage);
                }

                var entity = entities.First();
                var dto = _mapper.Map<TransactionTrackerAccountingDto>(entity);

                var successMessage = $"Transaction with reference {request.TransactionReference} retrieved successfully.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    logAction,
                    LogLevelInfo.Information);

                return ServiceResponse<TransactionTrackerAccountingDto>.ReturnResultWith200(dto, successMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error occurred while retrieving transaction with reference {request.TransactionReference}: {ex.Message}";
                _logger.LogError(ex, errorMessage);

                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    logAction,
                    LogLevelInfo.Error);

                return ServiceResponse<TransactionTrackerAccountingDto>.Return500(errorMessage);
            }
        }
    }






}
