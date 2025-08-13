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
using CBS.TransactionManagement.Repository.ReopenFeeParameterP;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new ReopenFeeParameter.
    /// </summary>
    public class AddReopenFeeParameterCommandHandler : IRequestHandler<AddReopenFeeParameterCommand, ServiceResponse<ReopenFeeParameterDto>>
    {
        private readonly IReopenFeeParameterRepository _ReopenFeeParameterRepository;
        private readonly ISavingProductRepository _SavingProductRepository;// Repository for accessing ReopenFeeParameter data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddReopenFeeParameterCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddReopenFeeParameterCommandHandler.
        /// </summary>
        /// <param name="ReopenFeeParameterRepository">Repository for ReopenFeeParameter data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddReopenFeeParameterCommandHandler(
            IReopenFeeParameterRepository ReopenFeeParameterRepository,
            ISavingProductRepository SavingProductRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddReopenFeeParameterCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null)
        {
            _ReopenFeeParameterRepository = ReopenFeeParameterRepository;
            _SavingProductRepository = SavingProductRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddReopenFeeParameterCommand to add a new ReopenFeeParameter.
        /// </summary>
        /// <param name="request">The AddReopenFeeParameterCommand containing ReopenFeeParameter data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ReopenFeeParameterDto>> Handle(AddReopenFeeParameterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var savingProductEntity = await GetSavingProduct(request.ProductId);

                if (savingProductEntity == null)
                {
                    var errorMessage = $"Unknown Saving product.";
                    await LogAndAuditError(request, errorMessage, 409);
                    return ServiceResponse<ReopenFeeParameterDto>.Return409(errorMessage);
                }

                var existingReopenFeeParameter = await _ReopenFeeParameterRepository.All.ToListAsync();

                if (existingReopenFeeParameter.Any())
                {
                    var errorMessage = $"ReopenFeeParameter already exists. You can update.";
                    await LogAndAuditError(request, errorMessage, 409);
                    return ServiceResponse<ReopenFeeParameterDto>.Return409(errorMessage);
                }

                var ReopenFeeParameterEntity = _mapper.Map<ReopenFeeParameter>(request);
                ReopenFeeParameterEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                ReopenFeeParameterEntity.Id = BaseUtilities.GenerateUniqueNumber();
                _ReopenFeeParameterRepository.Add(ReopenFeeParameterEntity);
                await _uow.SaveAsync(); 
                
                await LogAndAuditInfo(request, "Deposit limit created successfully", 409);
                var ReopenFeeParameterDto = _mapper.Map<ReopenFeeParameterDto>(ReopenFeeParameterEntity);
                    return ServiceResponse<ReopenFeeParameterDto>.ReturnResultWith200(ReopenFeeParameterDto);

              
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving ReopenFeeParameter: {e.Message}";
                await LogAndAuditError(request, errorMessage, 500);
                return ServiceResponse<ReopenFeeParameterDto>.Return500(e);
            }
        }

        private async Task<SavingProduct> GetSavingProduct(string productId)
        {
            return await _SavingProductRepository.FindAsync(productId);
        }

        private async Task LogAndAuditError(AddReopenFeeParameterCommand request, string errorMessage, int statusCode)
        {
            _logger.LogError(errorMessage);
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

        private async Task LogAndAuditInfo(AddReopenFeeParameterCommand request, string message, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

    }

}
