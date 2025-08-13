using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.SavingProductFeeP;
using CBS.TransactionManagement.Data.Entity.SavingProductFeeP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.SavingProductFeeP.Commands;
using CBS.TransactionManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.SavingProductFeeP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddSavingProductFeeHandler : IRequestHandler<AddSavingProductFeeCommand, ServiceResponse<SavingProductFeeDto>>
    {
        private readonly ISavingProductFeeRepository _SavingProductFeeRepository; // Repository for accessing SavingProductFee data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddSavingProductFeeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the AddSavingProductFeeHandler.
        /// </summary>
        /// <param name="SavingProductFeeRepository">Repository for SavingProductFee data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for database operations.</param>
        public AddSavingProductFeeHandler(
            ISavingProductFeeRepository SavingProductFeeRepository,
            IMapper mapper,
            ILogger<AddSavingProductFeeHandler> logger,
            IUnitOfWork<TransactionContext> uow)
        {
            _SavingProductFeeRepository = SavingProductFeeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddSavingProductFeeCommand to add a new SavingProductFee.
        /// </summary>
        /// <param name="request">The AddSavingProductFeeCommand containing SavingProductFee data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SavingProductFeeDto>> Handle(AddSavingProductFeeCommand request, CancellationToken cancellationToken)
        {
            try
            {

                // Check if a CashReplenishment exists by name or code for BranchId and bankId
                var savingProductFee = await _SavingProductFeeRepository.FindBy(c => c.SavingProductId == request.SavingProductId && c.FeeId == request.FeeId && !c.IsDeleted && c.FeeType==request.FeeType && c.FeePolicyType==request.FeePolicyType).Include(x => x.SavingProduct).Include(x=>x.Fee).FirstOrDefaultAsync();

                if (savingProductFee != null)
                {
                    var errorMessage = $"Mapping already exist for {savingProductFee.Fee.Name} & {savingProductFee.SavingProduct.Name}.";
                    return ServiceResponse<SavingProductFeeDto>.Return409(errorMessage);
                }

                // Map the AddSavingProductFeeCommand to a SavingProductFee entity
                var SavingProductFeeEntity = _mapper.Map<SavingProductFee>(request);
                // Convert UTC to local time and set it in the entity
                SavingProductFeeEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new SavingProductFee entity to the repository
                _SavingProductFeeRepository.Add(SavingProductFeeEntity);
                await _uow.SaveAsync();

                // Map the SavingProductFee entity to SavingProductFeeDto and return it with a success response
                var SavingProductFeeDto = _mapper.Map<SavingProductFeeDto>(SavingProductFeeEntity);
                return ServiceResponse<SavingProductFeeDto>.ReturnResultWith200(SavingProductFeeDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving SavingProductFee: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<SavingProductFeeDto>.Return500(e);
            }
        }
    }

}
