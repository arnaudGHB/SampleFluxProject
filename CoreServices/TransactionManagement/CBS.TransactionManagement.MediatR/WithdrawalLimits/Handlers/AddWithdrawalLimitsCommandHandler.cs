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
    /// Handles the command to add a new WithdrawalLimits.
    /// </summary>
    public class AddWithdrawalLimitsCommandHandler : IRequestHandler<AddWithdrawalLimitsCommand, ServiceResponse<WithdrawalParameterDto>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly ISavingProductRepository _SavingProductRepository;
        private readonly IWithdrawalLimitsRepository _WithdrawalLimitsRepository; // Repository for accessing WithdrawalLimits data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddWithdrawalLimitsCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddWithdrawalLimitsCommandHandler.
        /// </summary>
        /// <param name="WithdrawalLimitsRepository">Repository for WithdrawalLimits data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddWithdrawalLimitsCommandHandler(
            IWithdrawalLimitsRepository WithdrawalLimitsRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ISavingProductRepository SavingProductRepository,
            ILogger<AddWithdrawalLimitsCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null)
        {
            _WithdrawalLimitsRepository = WithdrawalLimitsRepository;
            _SavingProductRepository = SavingProductRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddWithdrawalLimitsCommand to add a new WithdrawalLimits.
        /// </summary>
        /// <param name="request">The AddWithdrawalLimitsCommand containing WithdrawalLimits data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        public async Task<ServiceResponse<WithdrawalParameterDto>> Handle(AddWithdrawalLimitsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate the sum of shares using the helper class
                ShareValidator.Validate(SharingWithPartner.TRUST_SOFT_CREDIT_SHARING.ToString(),request.CamCCULShare, request.FluxAndPTMShare, request.HeadOfficeShare, request.SourceBrachOfficeShare, request.DestinationBranchOfficeShare);
                var savingProductEntity = await GetSavingProduct(request.ProductId);
                if (savingProductEntity == null)
                {
                    return HandleUnknownSavingProduct(request);
                }

                var existingWithdrawalLimits = await _WithdrawalLimitsRepository
                    .FindBy(c => c.ProductId == request.ProductId && c.WithdrawalType == request.WithdrawalType)
                    .FirstOrDefaultAsync();

                if (existingWithdrawalLimits != null)
                {
                    return HandleExistingWithdrawalLimits(request.WithdrawalType);
                }
                var withdrawalLimitsEntity = MapToWithdrawalLimitsEntity(request, savingProductEntity);
                await AddWithdrawalLimitsAsync(withdrawalLimitsEntity);

                withdrawalLimitsEntity.Product = savingProductEntity;
                var withdrawalLimitsDto = _mapper.Map<WithdrawalParameterDto>(withdrawalLimitsEntity);
                string message = "Operation was successful.";
                return ServiceResponse<WithdrawalParameterDto>.ReturnResultWith200(withdrawalLimitsDto, message);
            }
            catch (Exception e)
            {
                return HandleErrorSavingWithdrawalLimits(e);
            }
        }



        private ServiceResponse<WithdrawalParameterDto> HandleUnknownSavingProduct(AddWithdrawalLimitsCommand request)
        {
            var errorMessage = "Unknown Saving product.";
            _logger.LogError(errorMessage);
            LogAndAuditError(request, errorMessage, 409).Wait();
            return ServiceResponse<WithdrawalParameterDto>.Return409(errorMessage);
        }

        private ServiceResponse<WithdrawalParameterDto> HandleExistingWithdrawalLimits(string withdrawalType)
        {
            var errorMessage = $"WithdrawalLimits {withdrawalType} already exists.";
            _logger.LogError(errorMessage);
            return ServiceResponse<WithdrawalParameterDto>.Return409(errorMessage);
        }

        private WithdrawalParameter MapToWithdrawalLimitsEntity(AddWithdrawalLimitsCommand request, SavingProduct savingProductEntity)
        {
            var withdrawalLimitsEntity = _mapper.Map<WithdrawalParameter>(request);
            withdrawalLimitsEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            withdrawalLimitsEntity.Id = BaseUtilities.GenerateUniqueNumber();
            return withdrawalLimitsEntity;
        }

        private async Task AddWithdrawalLimitsAsync(WithdrawalParameter withdrawalLimitsEntity)
        {
            _WithdrawalLimitsRepository.Add(withdrawalLimitsEntity);
            await _uow.SaveAsync();
        }

        private async Task LogAndAuditError(AddWithdrawalLimitsCommand request, string errorMessage, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

        private ServiceResponse<WithdrawalParameterDto> HandleErrorSavingWithdrawalLimits(Exception e)
        {
            var errorMessage = $"Error occurred while saving WithdrawalLimits: {e.Message}";
            _logger.LogError(errorMessage);
            return ServiceResponse<WithdrawalParameterDto>.Return500(e);
        }

        private async Task<SavingProduct> GetSavingProduct(string productId)
        {
            return await _SavingProductRepository.FindAsync(productId);
        }






    }

}
