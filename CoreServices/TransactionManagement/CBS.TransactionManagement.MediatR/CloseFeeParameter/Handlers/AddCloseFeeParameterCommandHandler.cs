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
    /// Handles the command to add a new CloseFeeParameter.
    /// </summary>
    public class AddCloseFeeParameterCommandHandler : IRequestHandler<AddCloseFeeParameterCommand, ServiceResponse<CloseFeeParameterDto>>
    {
        private readonly ICloseFeeParameterRepository _CloseFeeParameterRepository;
        private readonly ISavingProductRepository _SavingProductRepository;// Repository for accessing CloseFeeParameter data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCloseFeeParameterCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddCloseFeeParameterCommandHandler.
        /// </summary>
        /// <param name="CloseFeeParameterRepository">Repository for CloseFeeParameter data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCloseFeeParameterCommandHandler(
            ICloseFeeParameterRepository CloseFeeParameterRepository,
            ISavingProductRepository SavingProductRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddCloseFeeParameterCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null)
        {
            _CloseFeeParameterRepository = CloseFeeParameterRepository;
            _SavingProductRepository = SavingProductRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddCloseFeeParameterCommand to add a new CloseFeeParameter.
        /// </summary>
        /// <param name="request">The AddCloseFeeParameterCommand containing CloseFeeParameter data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CloseFeeParameterDto>> Handle(AddCloseFeeParameterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var savingProductEntity = await GetSavingProduct(request.ProductId);

                if (savingProductEntity == null)
                {
                    var errorMessage = $"Unknown Saving product.";
                    await LogAndAuditError(request, errorMessage, 409);
                    return ServiceResponse<CloseFeeParameterDto>.Return409(errorMessage);
                }

                var existingCloseFeeParameter = await _CloseFeeParameterRepository.All.ToListAsync();

                if (existingCloseFeeParameter.Any())
                {
                    var errorMessage = $"CloseFeeParameter already exists. You can update.";
                    await LogAndAuditError(request, errorMessage, 409);
                    return ServiceResponse<CloseFeeParameterDto>.Return409(errorMessage);
                }

                var CloseFeeParameterEntity = _mapper.Map<CloseFeeParameter>(request);
                CloseFeeParameterEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                CloseFeeParameterEntity.Id = BaseUtilities.GenerateUniqueNumber();
                CloseFeeParameterEntity.OperationEventAttributeName = OperationEventRubbriqueName.Fee.ToString();
                _CloseFeeParameterRepository.Add(CloseFeeParameterEntity);

                await _uow.SaveAsync();

                CloseFeeParameterEntity.Product = savingProductEntity;
                var CloseFeeParameterDto = _mapper.Map<CloseFeeParameterDto>(CloseFeeParameterEntity);

                await LogAndAuditInfo(request, "Deposit limit created successfully", 409);
                return ServiceResponse<CloseFeeParameterDto>.ReturnResultWith200(CloseFeeParameterDto);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving CloseFeeParameter: {e.Message}";
                await LogAndAuditError(request, errorMessage, 500);
                return ServiceResponse<CloseFeeParameterDto>.Return500(e);
            }
        }

        private async Task<SavingProduct> GetSavingProduct(string productId)
        {
            return await _SavingProductRepository.FindAsync(productId);
        }

        private async Task LogAndAuditError(AddCloseFeeParameterCommand request, string errorMessage, int statusCode)
        {
            _logger.LogError(errorMessage);
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

        private async Task LogAndAuditInfo(AddCloseFeeParameterCommand request, string message, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

       
    }

}
