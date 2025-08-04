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
    /// Handles the command to add a new TransferLimits.
    /// </summary>
    public class AddTransferLimitsCommandHandler : IRequestHandler<AddTransferLimitsCommand, ServiceResponse<TransferParameterDto>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly ISavingProductRepository _SavingProductRepository; // Repository for accessing TransferLimits data.
        private readonly ITransferLimitsRepository _TransferLimitsRepository; // Repository for accessing TransferLimits data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddTransferLimitsCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddTransferLimitsCommandHandler.
        /// </summary>
        /// <param name="TransferLimitsRepository">Repository for TransferLimits data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddTransferLimitsCommandHandler(
            ITransferLimitsRepository TransferLimitsRepository,
            ISavingProductRepository SavingProductRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddTransferLimitsCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null)
        {
            _TransferLimitsRepository = TransferLimitsRepository;
            _SavingProductRepository = SavingProductRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddTransferLimitsCommand to add a new TransferLimits.
        /// </summary>
        /// <param name="request">The AddTransferLimitsCommand containing TransferLimits data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TransferParameterDto>> Handle(AddTransferLimitsCommand request, CancellationToken cancellationToken)
        {
            try
            {  // Validate the sum of shares using the helper class
                ShareValidator.Validate(SharingWithPartner.TRUST_SOFT_CREDIT_SHARING.ToString(),request.CamCCULShare, request.FluxAndPTMShare, request.HeadOfficeShare, request.SourceBrachOfficeShare, request.DestinationBranchOfficeShare);

                var savingProductEntity = _SavingProductRepository.Find(request.ProductId);
                if (savingProductEntity == null)
                {
                    return HandleUnknownSavingProduct(request);
                }

                var existingTransferLimits = await _TransferLimitsRepository.FindBy(c => c.ProductId == request.ProductId && c.TransferType == request.TransferType).FirstOrDefaultAsync();
                if (existingTransferLimits != null)
                {
                    return HandleExistingTransferLimits(request.TransferType);
                }

                var transferLimitsEntity = MapToTransferLimitsEntity(request, savingProductEntity);

                _TransferLimitsRepository.Add(transferLimitsEntity);
                await _uow.SaveAsync();

                transferLimitsEntity.Product = savingProductEntity;

                string message = "Operation created successfully.";

                var transferLimitsDto = _mapper.Map<TransferParameterDto>(transferLimitsEntity);
                return ServiceResponse<TransferParameterDto>.ReturnResultWith200(transferLimitsDto, message);
            }
            catch (Exception e)
            {
                return HandleErrorSavingTransferLimits(e);
            }
        }

        private ServiceResponse<TransferParameterDto> HandleUnknownSavingProduct(AddTransferLimitsCommand request)
        {
            var errorMessage = "Unknown Saving product.";
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
            return ServiceResponse<TransferParameterDto>.Return409(errorMessage);
        }

        private ServiceResponse<TransferParameterDto> HandleExistingTransferLimits(string transferType)
        {
            var errorMessage = $"TransferLimits {transferType} already exists.";
            _logger.LogError(errorMessage);
            return ServiceResponse<TransferParameterDto>.Return409(errorMessage);
        }

        private TransferParameter MapToTransferLimitsEntity(AddTransferLimitsCommand request, SavingProduct savingProductEntity)
        {
            var transferLimitsEntity = _mapper.Map<TransferParameter>(request);
            transferLimitsEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            transferLimitsEntity.Id = BaseUtilities.GenerateUniqueNumber();
            return transferLimitsEntity;
        }

        private ServiceResponse<TransferParameterDto> HandleErrorSavingTransferLimits(Exception e)
        {
            var errorMessage = $"Error occurred while saving TransferLimits: {e.Message}";
            _logger.LogError(errorMessage);
            return ServiceResponse<TransferParameterDto>.Return500(e, errorMessage);
        }
       
       


    }

}
