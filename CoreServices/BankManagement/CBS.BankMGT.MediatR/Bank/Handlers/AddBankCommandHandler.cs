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
    /// Handles the command to add a new Bank.
    /// </summary>
    public class AddBankCommandHandler : IRequestHandler<AddBankCommand, ServiceResponse<BankDto>>
    {
        private readonly IBankRepository _BankRepository; // Repository for accessing Bank data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddBankCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddBankCommandHandler.
        /// </summary>
        /// <param name="BankRepository">Repository for Bank data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddBankCommandHandler(
            IBankRepository BankRepository,
            IMapper mapper,
            ILogger<AddBankCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _BankRepository = BankRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddBankCommand to add a new Bank.
        /// </summary>
        /// <param name="request">The AddBankCommand containing Bank data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BankDto>> Handle(AddBankCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Bank with the same name already exists (case-insensitive)
                var existingBank = await _BankRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();

                // If a Bank with the same name already exists, return a conflict response
                if (existingBank != null)
                {
                    var errorMessage = $"Bank {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<BankDto>.Return409(errorMessage);
                }

                // Map the AddBankCommand to a Bank entity
                var BankEntity = _mapper.Map<Bank>(request);

                // Convert UTC to local time and set it in the entity
                BankEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                BankEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new Bank entity to the repository
                _BankRepository.Add(BankEntity);
                await _uow.SaveAsync();

                // Map the Bank entity to BankDto and return it with a success response
                var BankDto = _mapper.Map<BankDto>(BankEntity);
                return ServiceResponse<BankDto>.ReturnResultWith200(BankDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Bank: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<BankDto>.Return500(e, errorMessage);
            }
        }
    }

}
