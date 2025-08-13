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
    /// Handles the command to add a new EntryFeeParameter.
    /// </summary>
    public class AddEntryFeeParameterCommandHandler : IRequestHandler<AddEntryFeeParameterCommand, ServiceResponse<EntryFeeParameterDto>>
    {
        private readonly IEntryFeeParameterRepository _EntryFeeParameterRepository;
        private readonly ISavingProductRepository _SavingProductRepository;// Repository for accessing EntryFeeParameter data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddEntryFeeParameterCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddEntryFeeParameterCommandHandler.
        /// </summary>
        /// <param name="EntryFeeParameterRepository">Repository for EntryFeeParameter data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddEntryFeeParameterCommandHandler(
            IEntryFeeParameterRepository EntryFeeParameterRepository,
            ISavingProductRepository SavingProductRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddEntryFeeParameterCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null)
        {
            _EntryFeeParameterRepository = EntryFeeParameterRepository;
            _SavingProductRepository = SavingProductRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddEntryFeeParameterCommand to add a new EntryFeeParameter.
        /// </summary>
        /// <param name="request">The AddEntryFeeParameterCommand containing EntryFeeParameter data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<EntryFeeParameterDto>> Handle(AddEntryFeeParameterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var savingProductEntity = await GetSavingProduct(request.ProductId);

                if (savingProductEntity == null)
                {
                    var errorMessage = $"Unknown Saving product.";
                    await LogAndAuditError(request, errorMessage, 409);
                    return ServiceResponse<EntryFeeParameterDto>.Return409(errorMessage);
                }

                var existingEntryFeeParameter = await _EntryFeeParameterRepository.FindBy(x=>x.ProductId==request.ProductId).ToListAsync();

                if (existingEntryFeeParameter.Any())
                {
                    var errorMessage = $"EntryFeeParameter already exists. You can update.";
                    await LogAndAuditError(request, errorMessage, 409);
                    return ServiceResponse<EntryFeeParameterDto>.Return409(errorMessage);
                }

                var EntryFeeParameterEntity = _mapper.Map<EntryFeeParameter>(request);
                EntryFeeParameterEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                EntryFeeParameterEntity.Id = BaseUtilities.GenerateUniqueNumber();
                _EntryFeeParameterRepository.Add(EntryFeeParameterEntity);

                if (await _uow.SaveAsync() <= 0)
                {
                    await LogAndAuditError(request, "An error occurred creating a deposit limit", 500);
                    return ServiceResponse<EntryFeeParameterDto>.Return500();
                }

                EntryFeeParameterEntity.Product = savingProductEntity;
                var EntryFeeParameterDto = _mapper.Map<EntryFeeParameterDto>(EntryFeeParameterEntity);

                await LogAndAuditInfo(request, "Deposit limit created successfully", 409);
                return ServiceResponse<EntryFeeParameterDto>.ReturnResultWith200(EntryFeeParameterDto);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving EntryFeeParameter: {e.Message}";
                await LogAndAuditError(request, errorMessage, 500);
                return ServiceResponse<EntryFeeParameterDto>.Return500(e);
            }
        }

        private async Task<SavingProduct> GetSavingProduct(string productId)
        {
            return await _SavingProductRepository.FindAsync(productId);
        }

        private async Task LogAndAuditError(AddEntryFeeParameterCommand request, string errorMessage, int statusCode)
        {
            _logger.LogError(errorMessage);
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

        private async Task LogAndAuditInfo(AddEntryFeeParameterCommand request, string message, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

       
       

    }

}
