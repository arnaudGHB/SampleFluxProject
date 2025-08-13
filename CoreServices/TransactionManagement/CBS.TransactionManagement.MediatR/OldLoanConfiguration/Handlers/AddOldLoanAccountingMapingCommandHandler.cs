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
using CBS.TransactionManagement.OldLoanConfiguration.Commands;
using CBS.TransactionManagement.Repository.OldLoanConfiguration;
using CBS.TransactionManagement.Data.Dto.OldLoanConfiguration;
using CBS.TransactionManagement.Data.Entity.OldLoanConfiguration;

namespace CBS.TransactionManagement.OldLoanConfiguration.Handlers
{
    /// <summary>
    /// Handles the command to add a new OldLoanAccountingMaping.
    /// </summary>
    public class AddOldLoanAccountingMapingCommandHandler : IRequestHandler<AddOldLoanAccountingMapingCommand, ServiceResponse<OldLoanAccountingMapingDto>>
    {
        private readonly IOldLoanAccountingMapingRepository _OldLoanAccountingMapingRepository;
        private readonly ISavingProductRepository _SavingProductRepository;// Repository for accessing OldLoanAccountingMaping data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddOldLoanAccountingMapingCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddOldLoanAccountingMapingCommandHandler.
        /// </summary>
        /// <param name="OldLoanAccountingMapingRepository">Repository for OldLoanAccountingMaping data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddOldLoanAccountingMapingCommandHandler(
            IOldLoanAccountingMapingRepository OldLoanAccountingMapingRepository,
            ISavingProductRepository SavingProductRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddOldLoanAccountingMapingCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null)
        {
            _OldLoanAccountingMapingRepository = OldLoanAccountingMapingRepository;
            _SavingProductRepository = SavingProductRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddOldLoanAccountingMapingCommand to add a new OldLoanAccountingMaping.
        /// </summary>
        /// <param name="request">The AddOldLoanAccountingMapingCommand containing OldLoanAccountingMaping data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OldLoanAccountingMapingDto>> Handle(AddOldLoanAccountingMapingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if an OldLoanAccountingMaping already exists with the same parameters
                var existingMapping = await _OldLoanAccountingMapingRepository
                    .FindBy(x => x.LoanTypeName == request.LoanTypeName).ToListAsync();

                if (existingMapping.Any())
                {
                    var errorMessage = "An OldLoanAccountingMaping with the same loan type and chart of account mapping already exists.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Conflict, LogAction.AccountingMapping, LogLevelInfo.Warning);
                    return ServiceResponse<OldLoanAccountingMapingDto>.Return409(errorMessage); // HTTP 409 Conflict
                }

                // Explicit message: Mapping request to entity
                string message = "Mapping AddOldLoanAccountingMapingCommand request to OldLoanAccountingMaping entity.";
                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.AccountingMapping, LogLevelInfo.Information);

                // Map the request to the entity
                var OldLoanAccountingMapingEntity = _mapper.Map<OldLoanAccountingMaping>(request);
                OldLoanAccountingMapingEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                OldLoanAccountingMapingEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Explicit message: Adding new OldLoanAccountingMaping to repository
                message = "Adding new OldLoanAccountingMaping to repository.";
                await BaseUtilities.LogAndAuditAsync(message, OldLoanAccountingMapingEntity, HttpStatusCodeEnum.OK, LogAction.AccountingMapping, LogLevelInfo.Information);

                // Add to repository and save changes
                _OldLoanAccountingMapingRepository.Add(OldLoanAccountingMapingEntity);
                await _uow.SaveAsync();

                // Map the entity to DTO
                var OldLoanAccountingMapingDto = _mapper.Map<OldLoanAccountingMapingDto>(OldLoanAccountingMapingEntity);

                // Explicit message: Successfully added OldLoanAccountingMaping
                message = "OldLoanAccountingMaping successfully added and saved.";
                await BaseUtilities.LogAndAuditAsync(message, OldLoanAccountingMapingEntity, HttpStatusCodeEnum.OK, LogAction.AccountingMapping, LogLevelInfo.Information);

                // Return success response
                return ServiceResponse<OldLoanAccountingMapingDto>.ReturnResultWith200(OldLoanAccountingMapingDto, message);
            }
            catch (Exception e)
            {
                // Handle exception, log the error, and return a 500 response
                var errorMessage = $"Error occurred while saving OldLoanAccountingMaping: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingMapping, LogLevelInfo.Error);
                return ServiceResponse<OldLoanAccountingMapingDto>.Return500(errorMessage);
            }
        }



    }

}
