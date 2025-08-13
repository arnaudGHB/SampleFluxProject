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
    /// Handles the command to add a new ManagementFeeParameter.
    /// </summary>
    public class AddManagementFeeParameterCommandHandler : IRequestHandler<AddManagementFeeParameterCommand, ServiceResponse<ManagementFeeParameterDto>>
    {
        private readonly IManagementFeeParameterRepository _ManagementFeeParameterRepository;
        private readonly ISavingProductRepository _SavingProductRepository;// Repository for accessing ManagementFeeParameter data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddManagementFeeParameterCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddManagementFeeParameterCommandHandler.
        /// </summary>
        /// <param name="ManagementFeeParameterRepository">Repository for ManagementFeeParameter data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddManagementFeeParameterCommandHandler(
            IManagementFeeParameterRepository ManagementFeeParameterRepository,
            ISavingProductRepository SavingProductRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddManagementFeeParameterCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null)
        {
            _ManagementFeeParameterRepository = ManagementFeeParameterRepository;
            _SavingProductRepository = SavingProductRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddManagementFeeParameterCommand to add a new ManagementFeeParameter.
        /// </summary>
        /// <param name="request">The AddManagementFeeParameterCommand containing ManagementFeeParameter data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ManagementFeeParameterDto>> Handle(AddManagementFeeParameterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var savingProductEntity = await GetSavingProduct(request.ProductId);

                if (savingProductEntity == null)
                {
                    var errorMessage = $"Unknown Saving product.";
                    await LogAndAuditError(request, errorMessage, 409);
                    return ServiceResponse<ManagementFeeParameterDto>.Return409(errorMessage);
                }

                var existingManagementFeeParameter = await _ManagementFeeParameterRepository.All.ToListAsync();

                if (existingManagementFeeParameter.Any())
                {
                    var errorMessage = $"ManagementFeeParameter already exists. You can update.";
                    await LogAndAuditError(request, errorMessage, 409);
                    return ServiceResponse<ManagementFeeParameterDto>.Return409(errorMessage);
                }

                var ManagementFeeParameterEntity = _mapper.Map<ManagementFeeParameter>(request);
                ManagementFeeParameterEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                ManagementFeeParameterEntity.Id = BaseUtilities.GenerateUniqueNumber();
                await _uow.SaveAsync();

                ManagementFeeParameterEntity.Product = savingProductEntity;
                var ManagementFeeParameterDto = _mapper.Map<ManagementFeeParameterDto>(ManagementFeeParameterEntity);

                await LogAndAuditInfo(request, "Deposit limit created successfully", 409);
                return ServiceResponse<ManagementFeeParameterDto>.ReturnResultWith200(ManagementFeeParameterDto);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving ManagementFeeParameter: {e.Message}";
                await LogAndAuditError(request, errorMessage, 500);
                return ServiceResponse<ManagementFeeParameterDto>.Return500(e);
            }
        }

        private async Task<SavingProduct> GetSavingProduct(string productId)
        {
            return await _SavingProductRepository.FindAsync(productId);
        }

        private async Task LogAndAuditError(AddManagementFeeParameterCommand request, string errorMessage, int statusCode)
        {
            _logger.LogError(errorMessage);
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

        private async Task LogAndAuditInfo(AddManagementFeeParameterCommand request, string message, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

     
    }

}
