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
    /// Handles the command to add a new DepositLimit.
    /// </summary>
    public class AddDepositLimitCommandHandler : IRequestHandler<AddDepositLimitCommand, ServiceResponse<CashDepositParameterDto>>
    {
        private readonly IDepositLimitRepository _DepositLimitRepository;
        private readonly ISavingProductRepository _SavingProductRepository;// Repository for accessing DepositLimit data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddDepositLimitCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddDepositLimitCommandHandler.
        /// </summary>
        /// <param name="DepositLimitRepository">Repository for DepositLimit data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddDepositLimitCommandHandler(
            IDepositLimitRepository DepositLimitRepository,
            ISavingProductRepository SavingProductRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddDepositLimitCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null)
        {
            _DepositLimitRepository = DepositLimitRepository;
            _SavingProductRepository = SavingProductRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddDepositLimitCommand to add a new DepositLimit.
        /// </summary>
        /// <param name="request">The AddDepositLimitCommand containing DepositLimit data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CashDepositParameterDto>> Handle(AddDepositLimitCommand request, CancellationToken cancellationToken)
        {
            try
            {  // Validate the sum of shares using the helper class
                ShareValidator.Validate(SharingWithPartner.TRUST_SOFT_CREDIT_SHARING.ToString(),request.CamCCULShare, request.FluxAndPTMShare, request.HeadOfficeShare, request.SourceBrachOfficeShare, request.DestinationBranchOfficeShare);
                var savingProductEntity = await GetSavingProduct(request.ProductId);

                if (savingProductEntity == null)
                {
                    var errorMessage = $"Unknown Saving product.";
                    await LogAndAuditError(request, errorMessage, 409);
                    return ServiceResponse<CashDepositParameterDto>.Return409(errorMessage);
                }

                var existingDepositLimit = await _DepositLimitRepository
                    .FindBy(c => c.ProductId == request.ProductId && c.DepositType == request.DepositType && c.IsDeleted==false)
                    .FirstOrDefaultAsync();

                if (existingDepositLimit != null)
                {
                    var errorMessage = $"DepositLimit {request.DepositType} already exists.";
                    await LogAndAuditError(request, errorMessage, 409);
                    return ServiceResponse<CashDepositParameterDto>.Return409(errorMessage);
                }
                var depositLimitEntity = _mapper.Map<CashDepositParameter>(request);
                depositLimitEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                depositLimitEntity.Id = BaseUtilities.GenerateUniqueNumber();
                _DepositLimitRepository.Add(depositLimitEntity);

                if (await _uow.SaveAsync() <= 0)
                {
                    await LogAndAuditError(request, "An error occurred creating a deposit limit", 500);
                    return ServiceResponse<CashDepositParameterDto>.Return500();
                }

                depositLimitEntity.Product = savingProductEntity;
                var depositLimitDto = _mapper.Map<CashDepositParameterDto>(depositLimitEntity);

                await LogAndAuditInfo(request, "Deposit limit created successfully", 409);
                return ServiceResponse<CashDepositParameterDto>.ReturnResultWith200(depositLimitDto);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving DepositLimit: {e.Message}";
                await LogAndAuditError(request, errorMessage, 500);
                return ServiceResponse<CashDepositParameterDto>.Return500(e);
            }
        }

        private async Task<SavingProduct> GetSavingProduct(string productId)
        {
            return await _SavingProductRepository.FindAsync(productId);
        }

        private async Task LogAndAuditError(AddDepositLimitCommand request, string errorMessage, int statusCode)
        {
            _logger.LogError(errorMessage);
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

        private async Task LogAndAuditInfo(AddDepositLimitCommand request, string message, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

    }

}
