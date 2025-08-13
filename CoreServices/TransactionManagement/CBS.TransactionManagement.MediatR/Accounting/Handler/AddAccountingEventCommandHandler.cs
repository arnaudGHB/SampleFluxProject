using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Command;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountingEvent.
    /// </summary>
    public class AddAccountingEventCommandHandler : IRequestHandler<AddAccountingEventCommand, ServiceResponse<AccountingEventDto>>
    {
        private readonly IAccountingEventRepository _accountingEventRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountingEventCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the AddAccountingEventCommandHandler.
        /// </summary>
        public AddAccountingEventCommandHandler(
            IAccountingEventRepository accountingEventRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddAccountingEventCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null)
        {
            _accountingEventRepository = accountingEventRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddAccountingEventCommand to add a new AccountingEvent.
        /// </summary>
        public async Task<ServiceResponse<AccountingEventDto>> Handle(AddAccountingEventCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Map the request to the entity
                var accountingEventEntity = _mapper.Map<AccountingEvent>(request);

                // Generate unique ID for the entity
                accountingEventEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add entity to the repository
                _accountingEventRepository.Add(accountingEventEntity);

                // Save changes to the database
                if (await _uow.SaveAsync() <= 0)
                {
                    string errorMsg = "An error occurred while creating the accounting event. No changes were saved.";
                    _logger.LogError(errorMsg);

                    await BaseUtilities.LogAndAuditAsync(
                        message: errorMsg,
                        request,
                        statusCode: HttpStatusCodeEnum.InternalServerError,
                        LogAction.Create,
                        LogLevelInfo.Error
                        
                    );

                    return ServiceResponse<AccountingEventDto>.Return500("Unable to save the accounting event.");
                }

                // Map the entity to DTO for the response
                var accountingEventDto = _mapper.Map<AccountingEventDto>(accountingEventEntity);

                string successMessage = $"Accounting event created successfully. Event ID: {accountingEventEntity.Id}.";
                _logger.LogInformation(successMessage);

                await BaseUtilities.LogAndAuditAsync(
                    message: successMessage,
                    request,
                    statusCode: HttpStatusCodeEnum.OK,
                    LogAction.Create,
                    LogLevelInfo.Information,
                    accountingEventEntity.Id
                );

                return ServiceResponse<AccountingEventDto>.ReturnResultWith200(accountingEventDto, successMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error occurred while saving AccountingEvent. Details: {ex.Message}";
                _logger.LogError(errorMessage);

                await BaseUtilities.LogAndAuditAsync(
                    message: errorMessage,
                    request,
                    statusCode: HttpStatusCodeEnum.InternalServerError,
                    LogAction.Create,
                    LogLevelInfo.Error
                    
                );

                return ServiceResponse<AccountingEventDto>.Return500(ex, errorMessage);
            }
        }
    }

}
