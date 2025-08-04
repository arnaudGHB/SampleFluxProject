using AutoMapper;
using CBS.TransactionManagement.Common.Repository.Uow;
using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.Data.Entity.MongoDBObjects;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Handlers
{
    /// <summary>
    /// Handles the command to update an existing TransactionTrackerAccounting.
    /// </summary>
    public class UpdateTransactionTrackerAccountingCommandHandler : IRequestHandler<UpdateTransactionTrackerAccountingCommand, ServiceResponse<TransactionTrackerAccountingDto>>
    {
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateTransactionTrackerAccountingCommandHandler> _logger; // Logger for logging actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly UserInfoToken _userInfoToken; // User information for tracking.

        /// <summary>
        /// Constructor for initializing the UpdateTransactionTrackerAccountingCommandHandler.
        /// </summary>
        /// <param name="userInfoToken">User information for tracking.</param>
        /// <param name="mapper">Mapper for object-to-object mapping.</param>
        /// <param name="logger">Logger for tracking actions and errors.</param>
        /// <param name="mongoUnitOfWork">MongoDB unit of work for database operations.</param>
        public UpdateTransactionTrackerAccountingCommandHandler(
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<UpdateTransactionTrackerAccountingCommandHandler> logger,
            IMongoUnitOfWork mongoUnitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _mongoUnitOfWork = mongoUnitOfWork;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the UpdateTransactionTrackerAccountingCommand to update an existing TransactionTrackerAccounting.
        /// </summary>
        /// <param name="request">The command request containing the details to be updated.</param>
        /// <param name="cancellationToken">Token to signal cancellation of the operation.</param>
        public async Task<ServiceResponse<TransactionTrackerAccountingDto>> Handle(UpdateTransactionTrackerAccountingCommand request, CancellationToken cancellationToken)
        {
            var logAction = LogAction.TransactionTrackerAccounting; // Define the type of log action
            try
            {
                // Log the update operation details
                _logger.LogInformation("Updating TransactionTrackerAccounting with ID: {Id}", request.Id);

                // Get the MongoDB repository for TransactionTrackerAccounting
                var transactionTrackerAccountingRepository = _mongoUnitOfWork.GetRepository<TransactionTrackerAccounting>();

                // Fetch the existing entity from the database
                var existingEntity = await transactionTrackerAccountingRepository.GetByIdAsync(request.Id);

                if (existingEntity == null)
                {
                    var notFoundMessage = $"TransactionTrackerAccounting with ID: {request.Id} not found.";
                    _logger.LogWarning(notFoundMessage);
                    return ServiceResponse<TransactionTrackerAccountingDto>.Return404(notFoundMessage);
                }

                // Map the updated values from the request to the existing entity
                _mapper.Map(request, existingEntity);

                // Update the ModifiedDate
                existingEntity.ModifiedDate = BaseUtilities.UtcNowToDoualaTime();

                // Update the entity in the database
                await transactionTrackerAccountingRepository.UpdateAsync(existingEntity.Id, existingEntity);

                // Map the updated entity to DTO
                var transactionTrackerAccountingDto = _mapper.Map<TransactionTrackerAccountingDto>(existingEntity);
                string message = $"Successfully updated TransactionTrackerAccounting. ID: {existingEntity.Id}, Reference: {existingEntity.TransactionReferenceId}";
                // Log and audit the successful update operation
                await BaseUtilities.LogAndAuditAsync(
                    message,
                    request,
                    HttpStatusCodeEnum.OK,
                    logAction,
                    LogLevelInfo.Information,
                    existingEntity.TransactionReferenceId);

                // Return success response
                return ServiceResponse<TransactionTrackerAccountingDto>.ReturnResultWith200(transactionTrackerAccountingDto, message);
            }
            catch (Exception e)
            {
                // Log the error with context information
                var errorMessage = $"Error occurred while updating TransactionTrackerAccounting with ID: {request.Id}. Error: {e.Message}";
                _logger.LogError(e, errorMessage);

                // Log and audit the failed update operation
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    logAction,
                    LogLevelInfo.Error,
                    request.Id);

                // Return a 500 Internal Server Error response with the error details
                return ServiceResponse<TransactionTrackerAccountingDto>.Return500(errorMessage);
            }
        }
    }

}
