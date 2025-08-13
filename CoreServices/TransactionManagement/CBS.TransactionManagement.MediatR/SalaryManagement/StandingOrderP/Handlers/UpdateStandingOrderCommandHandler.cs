using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Repository.SalaryManagement.StandingOrderP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Handlers
{


    public class UpdateStandingOrderCommandHandler : IRequestHandler<UpdateStandingOrderCommand, ServiceResponse<StandingOrderDto>>
    {
        private readonly IStandingOrderRepository _standingOrderRepository; // Repository for managing standing order entities
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for transactional operations
        private readonly ILogger<UpdateStandingOrderCommandHandler> _logger; // Logger for logging actions and errors
        private readonly IMapper _mapper; // Mapper for mapping between entities and DTOs
        private readonly IAccountRepository _accountRepository;

        /// <summary>
        /// Constructor for the UpdateStandingOrderCommandHandler.
        /// </summary>
        /// <param name="standingOrderRepository">Repository for managing standing orders.</param>
        /// <param name="uow">Unit of work for managing transactions.</param>
        /// <param name="logger">Logger instance for logging operations.</param>
        /// <param name="mapper">Mapper instance for mapping entities and DTOs.</param>
        public UpdateStandingOrderCommandHandler(
            IStandingOrderRepository standingOrderRepository,
            IUnitOfWork<TransactionContext> uow,
            ILogger<UpdateStandingOrderCommandHandler> logger,
            IMapper mapper,
            IAccountRepository accountRepository)
        {
            _standingOrderRepository = standingOrderRepository; // Assign repository
            _uow = uow; // Assign unit of work
            _logger = logger; // Assign logger
            _mapper = mapper; // Assign mapper
            _accountRepository=accountRepository;
        }

        /// <summary>
        /// Handles the UpdateStandingOrderCommand to update an existing standing order.
        /// </summary>
        /// <param name="request">Command containing updated standing order details.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>ServiceResponse containing the updated standing order.</returns>
        public async Task<ServiceResponse<StandingOrderDto>> Handle(UpdateStandingOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate input: Ensure the StandingOrder ID is provided
                if (string.IsNullOrEmpty(request.Id))
                {
                    string errorMessage = "StandingOrderId is required for updating a standing order.";

                    // Log and audit the missing ID error
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.StandingOrder, LogLevelInfo.Error, null);
                    _logger.LogError(errorMessage);

                    // Return a 422 Unprocessable Entity response
                    return ServiceResponse<StandingOrderDto>.Return422(errorMessage);
                }
                var account = new Account();
                if (request.ExternalAccount)
                {
                    account=await _accountRepository.GetAccountByAccountNumber(request.ExternalAccountNumber);
                    request.ExternalAccountHolderName=account.CustomerName;
                    request.DestinationAccountType="None";
                }
                // Fetch the existing standing order by ID from the repository
                var standingOrder = await _standingOrderRepository.FindAsync(request.Id);

                // Check if the standing order exists
                if (standingOrder == null)
                {
                    string errorMessage = $"Standing order with ID {request.Id} not found.";

                    // Log and audit the "not found" error
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.StandingOrder, LogLevelInfo.Warning, null);
                    _logger.LogError(errorMessage);

                    // Return a 404 Not Found response
                    return ServiceResponse<StandingOrderDto>.Return404(errorMessage);
                }

                // Map the updated details from the request to the existing standing order entity
                _mapper.Map(request, standingOrder);

                // Update the standing order in the repository
                _standingOrderRepository.Update(standingOrder);

                // Commit the transaction
                await _uow.SaveAsync();

                // Prepare a success message for logging and auditing
                string successMessage = $"Standing order updated successfully for Member: {request.MemberName}.";

                // Log and audit the successful operation
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.StandingOrder, LogLevelInfo.Information);
                _logger.LogInformation(successMessage);

                // Map the updated standing order entity back to a DTO
                var updatedStandingOrderDto = _mapper.Map<StandingOrderDto>(standingOrder);

                // Return a 200 OK response with the updated DTO
                return ServiceResponse<StandingOrderDto>.ReturnResultWith200(updatedStandingOrderDto, successMessage);
            }
            catch (Exception ex)
            {
                // Prepare an error message for logging and auditing
                string errorMessage = $"Error occurred while updating standing order for Member: {request.MemberName}. Error: {ex.Message}";

                // Log and audit the exception details
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.StandingOrder, LogLevelInfo.Error);
                _logger.LogError(errorMessage);

                // Return a 500 Internal Server Error response
                return ServiceResponse<StandingOrderDto>.Return500(errorMessage);
            }
        }
    }
}
