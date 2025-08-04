using AutoMapper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Data;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Currency.
    /// </summary>
    public class AddCurrencyCommandHandler : IRequestHandler<AddCurrencyCommand, ServiceResponse<CurrencyDto>>
    {
        private readonly ICurrencyRepository _CurrencyRepository; // Repository for accessing Currency data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCurrencyCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddCurrencyCommandHandler.
        /// </summary>
        /// <param name="CurrencyRepository">Repository for Currency data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCurrencyCommandHandler(
            ICurrencyRepository CurrencyRepository,
            IMapper mapper,
            ILogger<AddCurrencyCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _CurrencyRepository = CurrencyRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddCurrencyCommand to add a new Currency.
        /// </summary>
        /// <param name="request">The AddCurrencyCommand containing Currency data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CurrencyDto>> Handle(AddCurrencyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Currency with the same name already exists (case-insensitive)
                var existingCurrency = await _CurrencyRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();

                // If a Currency with the same name already exists, return a conflict response
                if (existingCurrency!=null)
                {
                    var errorMessage = $"Currency {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CurrencyDto>.Return409(errorMessage);
                }
                var existingCurrencyCode = await _CurrencyRepository.FindBy(c => c.Code == request.Code).FirstOrDefaultAsync();

                // If a Currency with the same code already exists, return a conflict response
                if (existingCurrencyCode != null)
                {
                    var errorMessage = $"Currency code {request.Code} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CurrencyDto>.Return409(errorMessage);
                }
                // Map the AddCurrencyCommand to a Currency entity
                var CurrencyEntity = _mapper.Map<Currency>(request);
                // Convert UTC to local time and set it in the entity
                CurrencyEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                CurrencyEntity.Id = BaseUtilities.GenerateUniqueNumber();
                // Add the new Currency entity to the repository
                _CurrencyRepository.Add(CurrencyEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CurrencyDto>.Return500();
                }
                // Map the Currency entity to CurrencyDto and return it with a success response
                var CurrencyDto = _mapper.Map<CurrencyDto>(CurrencyEntity);
                return ServiceResponse<CurrencyDto>.ReturnResultWith200(CurrencyDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Currency: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CurrencyDto>.Return500(e);
            }
        }
    }

}
