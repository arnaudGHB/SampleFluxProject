using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Queries;
using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.Helper.Helper.Pagging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Common.Repository.Uow;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CBS.TransactionManagement.Data.Entity.MongoDBObjects;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Handlers
{




    /// <summary>
    /// Handles the command to retrieve TransactionTrackerAccounting data with pagination and filtering.
    /// </summary>
    public class GetTransactionTrackerAccountingPaginationHandler : IRequestHandler<GetTransactionTrackerAccountingPaginationQuery, ServiceResponse<PaginatedList<TransactionTrackerAccountingDto>>>
    {
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTransactionTrackerAccountingPaginationHandler> _logger; // Logger for logging actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.

        /// <summary>
        /// Constructor for initializing the GetTransactionTrackerAccountingPaginationHandler.
        /// </summary>
        public GetTransactionTrackerAccountingPaginationHandler(
            IMapper mapper,
            ILogger<GetTransactionTrackerAccountingPaginationHandler> logger,
            IMongoUnitOfWork mongoUnitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _mongoUnitOfWork = mongoUnitOfWork;
        }

        /// <summary>
        /// Handles the query to retrieve TransactionTrackerAccounting data with pagination and filtering.
        /// </summary>
        public async Task<ServiceResponse<PaginatedList<TransactionTrackerAccountingDto>>> Handle(GetTransactionTrackerAccountingPaginationQuery request, CancellationToken cancellationToken)
        {
            var logAction = LogAction.PaginatedTransactionTrackerAccounting;
            try
            {
                _logger.LogInformation("Retrieving paginated TransactionTrackerAccounting data. Page: {PageNumber}, PageSize: {PageSize}", request.PageNumber, request.PageSize);

                // Get the repository
                var transactionTrackerAccountingRepository = _mongoUnitOfWork.GetRepository<TransactionTrackerAccounting>();

                // Build dynamic filters
                var filterBuilder = Builders<TransactionTrackerAccounting>.Filter;
                var filters = request.Filters?.Select(f => filterBuilder.Eq(f.Key, f.Value)).ToList();
                var combinedFilter = filters != null && filters.Any()
                    ? filterBuilder.And(filters)
                    : FilterDefinition<TransactionTrackerAccounting>.Empty;

                // Apply sorting and pagination
                var sort = Builders<TransactionTrackerAccounting>.Sort.Descending("CreatedDate");

                var paginatedList = await transactionTrackerAccountingRepository.GetPaginatedAsync(
                    request.PageNumber,
                    request.PageSize,
                    combinedFilter,
                    sort,
                    cancellationToken);

                // Map to DTOs
                var dtoList = _mapper.Map<List<TransactionTrackerAccountingDto>>(paginatedList.Items);

                // Create a paginated DTO list
                var paginatedDtoList = new PaginatedList<TransactionTrackerAccountingDto>(
                    dtoList,
                    paginatedList.TotalCount,
                    paginatedList.PageNumber,
                    paginatedList.PageSize);
                string message = $"Successfully retrieved paginated TransactionTrackerAccounting data for Page: {request.PageNumber}, PageSize: {request.PageSize}.";
                // Log and audit successful retrieval
                await BaseUtilities.LogAndAuditAsync(
                    message,
                    request,
                    HttpStatusCodeEnum.OK,
                    logAction,
                    LogLevelInfo.Information,
                    null);

                return ServiceResponse<PaginatedList<TransactionTrackerAccountingDto>>.ReturnResultWith200(paginatedDtoList, message);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while retrieving paginated TransactionTrackerAccounting data. Page: {request.PageNumber}, PageSize: {request.PageSize}. Error: {e.Message}";
                _logger.LogError(e, errorMessage);

                // Log and audit the error
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    logAction,
                    LogLevelInfo.Error,
                    null);

                return ServiceResponse<PaginatedList<TransactionTrackerAccountingDto>>.Return500(errorMessage);
            }
        }
    }



    ///// <summary>
    ///// Handles the command to retrieve TransactionTrackerAccounting data with pagination.
    ///// </summary>
    //public class GetTransactionTrackerAccountingPaginationHandler : IRequestHandler<GetTransactionTrackerAccountingPaginationQuery, ServiceResponse<PaginatedList<TransactionTrackerAccountingDto>>>
    //{
    //    private readonly IMapper _mapper; // AutoMapper for object mapping.
    //    private readonly ILogger<GetTransactionTrackerAccountingPaginationHandler> _logger; // Logger for logging actions and errors.
    //    private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.

    //    /// <summary>
    //    /// Constructor for initializing the GetTransactionTrackerAccountingPaginationHandler.
    //    /// </summary>
    //    /// <param name="mapper">Mapper for object-to-object mapping.</param>
    //    /// <param name="logger">Logger for tracking actions and errors.</param>
    //    /// <param name="mongoUnitOfWork">MongoDB unit of work for database operations.</param>
    //    public GetTransactionTrackerAccountingPaginationHandler(
    //        IMapper mapper,
    //        ILogger<GetTransactionTrackerAccountingPaginationHandler> logger,
    //        IMongoUnitOfWork mongoUnitOfWork)
    //    {
    //        _mapper = mapper;
    //        _logger = logger;
    //        _mongoUnitOfWork = mongoUnitOfWork;
    //    }

    //    /// <summary>
    //    /// Handles the query to retrieve TransactionTrackerAccounting data with pagination.
    //    /// </summary>
    //    /// <param name="request">The query request containing pagination details.</param>
    //    /// <param name="cancellationToken">Token to signal cancellation of the operation.</param>
    //    public async Task<ServiceResponse<PaginatedList<TransactionTrackerAccountingDto>>> Handle(GetTransactionTrackerAccountingPaginationQuery request, CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            _logger.LogInformation("Retrieving TransactionTrackerAccounting data for Page: {PageNumber}, PageSize: {PageSize}", request.PageNumber, request.PageSize);

    //            // Get the repository
    //            var transactionTrackerAccountingRepository = _mongoUnitOfWork.GetRepository<TransactionTrackerAccounting>();

    //            // Apply filters, sorting, and pagination
    //            var filter = Builders<TransactionTrackerAccounting>.Filter.Empty; // Modify filter as needed
    //            var sort = Builders<TransactionTrackerAccounting>.Sort.Descending("CreatedDate"); // Example sort by CreatedDate

    //            var paginatedList = await transactionTrackerAccountingRepository.GetPaginatedAsync(
    //            request.PageNumber,
    //            request.PageSize,
    //            filter,
    //            sort,
    //            cancellationToken);

    //            // Map to DTOs
    //            var dtoList = _mapper.Map<List<TransactionTrackerAccountingDto>>(paginatedList.Items);

    //            // Create a paginated DTO list
    //            var paginatedDtoList = new PaginatedList<TransactionTrackerAccountingDto>(
    //                dtoList,
    //                paginatedList.TotalCount,
    //                paginatedList.PageNumber,
    //                paginatedList.PageSize);

    //            // Return response
    //            return ServiceResponse<PaginatedList<TransactionTrackerAccountingDto>>.ReturnResultWith200(paginatedDtoList);
    //        }
    //        catch (Exception e)
    //        {
    //            var errorMessage = $"Error occurred while retrieving paginated TransactionTrackerAccounting data. Error: {e.Message}";
    //            _logger.LogError(e, errorMessage);
    //            return ServiceResponse<PaginatedList<TransactionTrackerAccountingDto>>.Return500(errorMessage);
    //        }
    //    }
    //}
}
