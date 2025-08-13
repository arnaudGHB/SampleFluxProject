using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.Common.Repository.Uow;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Data.Entity.MongoDBObjects;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Handlers
{
    /// <summary>
    /// Handles the command to add a new TransactionTrackerAccounting.
    /// </summary>
    public class AddTransactionTrackerAccountingCommandHandler : IRequestHandler<AddTransactionTrackerAccountingCommand, ServiceResponse<TransactionTrackerAccountingDto>>
    {
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddTransactionTrackerAccountingCommandHandler> _logger; // Logger for logging actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly UserInfoToken _userInfoToken; // User information for tracking.
        private readonly PathHelper _pathHelper;

        /// <summary>
        /// Constructor for initializing the AddTransactionTrackerAccountingCommandHandler.
        /// </summary>
        /// <param name="userInfoToken">User information for tracking.</param>
        /// <param name="mapper">Mapper for object-to-object mapping.</param>
        /// <param name="logger">Logger for tracking actions and errors.</param>
        /// <param name="mongoUnitOfWork">MongoDB unit of work for database operations.</param>
        public AddTransactionTrackerAccountingCommandHandler(
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<AddTransactionTrackerAccountingCommandHandler> logger,
            IMongoUnitOfWork mongoUnitOfWork,
            PathHelper pathHelper)
        {
            _mapper = mapper;
            _logger = logger;
            _mongoUnitOfWork = mongoUnitOfWork;
            _userInfoToken = userInfoToken;
            _pathHelper=pathHelper;
        }

        /// <summary>
        /// Handles the AddTransactionTrackerAccountingCommand to add a new TransactionTrackerAccounting.
        /// </summary>
        /// <param name="request">The command request containing the details to be added.</param>
        /// <param name="cancellationToken">Token to signal cancellation of the operation.</param>
        public async Task<ServiceResponse<TransactionTrackerAccountingDto>> Handle(AddTransactionTrackerAccountingCommand request, CancellationToken cancellationToken)
        {
            var logAction = LogAction.TransactionTrackerAccounting; // Define the type of log action
            try
            {
                // Map request to TransactionTrackerAccounting entity
                var transactionTrackerAccountingEntity = _mapper.Map<TransactionTrackerAccounting>(request);
                transactionTrackerAccountingEntity.Id = BaseUtilities.GenerateUniqueNumber(15);
                transactionTrackerAccountingEntity.CreatedDate = BaseUtilities.UtcNowToDoualaTime();
                transactionTrackerAccountingEntity.DatePassed=DateTime.MinValue;
                transactionTrackerAccountingEntity.ModifiedDate=DateTime.MinValue;
                transactionTrackerAccountingEntity.UserFullName=_userInfoToken.FullName;
                transactionTrackerAccountingEntity.BranchId=_userInfoToken.BranchID;
                transactionTrackerAccountingEntity.BranchCode=_userInfoToken.BranchCode;
                transactionTrackerAccountingEntity.SourceUrl=$"{_pathHelper.SourceBaseURL}{_pathHelper.SourceUrl}";
                // Log SourceUrl and DestinationUrl for transparency
                _logger.LogInformation("Processing TransactionTrackerAccounting with SourceUrl: {SourceUrl} and DestinationUrl: {DestinationUrl}",
                    request.SourceUrl ?? "N/A",
                    request.DestinationUrl ?? "N/A");
                // Log operation details
                _logger.LogInformation("Adding new TransactionTrackerAccounting with Reference: {Reference}", transactionTrackerAccountingEntity.TransactionReferenceId);

                // Get the MongoDB repository for TransactionTrackerAccounting
                var transactionTrackerAccountingRepository = _mongoUnitOfWork.GetRepository<TransactionTrackerAccounting>();

                // Insert entity into MongoDB
                await transactionTrackerAccountingRepository.InsertAsync(transactionTrackerAccountingEntity);

                // Map to DTO
                var transactionTrackerAccountingDto = _mapper.Map<TransactionTrackerAccountingDto>(transactionTrackerAccountingEntity);
                if (!request.IsBG)
                {
                    // Log and audit the successful operation
                    await BaseUtilities.LogAndAuditAsync(
                        $"Successfully added TransactionTrackerAccounting. Reference: {transactionTrackerAccountingEntity.TransactionReferenceId}, " +
                        $"SourceUrl: {request.SourceUrl ?? "N/A"}, DestinationUrl: {request.DestinationUrl ?? "N/A"}",
                        request,
                        HttpStatusCodeEnum.OK,
                        logAction,
                        LogLevelInfo.Information,
                        transactionTrackerAccountingEntity.TransactionReferenceId);
                }
                else
                {
                    // Log and audit the successful operation
                    await BaseUtilities.LogAndAuditAsync(
                        $"Successfully added TransactionTrackerAccounting. Reference: {transactionTrackerAccountingEntity.TransactionReferenceId}, " +
                        $"SourceUrl: {request.SourceUrl ?? "N/A"}, DestinationUrl: {request.DestinationUrl ?? "N/A"}",
                        request,
                        HttpStatusCodeEnum.OK,
                        logAction,
                        LogLevelInfo.Information, request.UserInfoToken.FullName, request.UserInfoToken.Token,
                        transactionTrackerAccountingEntity.TransactionReferenceId);
                }
                // Return success response
                return ServiceResponse<TransactionTrackerAccountingDto>.ReturnResultWith200(transactionTrackerAccountingDto);
            }
            catch (Exception e)
            {
                // Log the error with context information
                var errorMessage = $"Error occurred while saving TransactionTrackerAccounting. SourceUrl: {request.SourceUrl ?? "N/A"}, " +
                                   $"DestinationUrl: {request.DestinationUrl ?? "N/A"}. Error: {e.Message}";
                _logger.LogError(e, errorMessage);


                if (!request.IsBG)
                {
                    // Log and audit the successful operation
                    await BaseUtilities.LogAndAuditAsync(
                        $"Successfully added TransactionTrackerAccounting. Reference: {request.TransactionReferenceId}, " +
                        $"SourceUrl: {request.SourceUrl ?? "N/A"}, DestinationUrl: {request.DestinationUrl ?? "N/A"}",
                        request,
                        HttpStatusCodeEnum.OK,
                        logAction,
                        LogLevelInfo.Information,
                        request.TransactionReferenceId);
                }
                else
                {
                    // Log and audit the failed operation
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.InternalServerError,
                        logAction,
                        LogLevelInfo.Error, request.UserInfoToken.FullName, request.UserInfoToken.Token,
                        request.TransactionReferenceId);
                }

                

                // Return a 500 Internal Server Error response with the error details
                return ServiceResponse<TransactionTrackerAccountingDto>.Return500(errorMessage);
            }
        }
    }
}
