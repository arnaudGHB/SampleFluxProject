using AutoMapper;
using CBS.TransactionManagement.MediatR.FeePolicyP.Commands;
using CBS.NLoan.Repository.FeePolicyP;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Data.Entity.FeeP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository.FeeP;

namespace CBS.TransactionManagement.MediatR.FeePolicyP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddFeePolicyHandler : IRequestHandler<AddFeePolicyCommand, ServiceResponse<FeePolicyDto>>
    {
        private readonly IFeePolicyRepository _FeePolicyRepository; // Repository for accessing Fee data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddFeePolicyHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _unitOfWork;
        private readonly IFeeRepository _feeRepository; // Repository for accessing Fee data.

        /// <summary>
        /// Constructor for initializing the AddFeePolicyHandler.
        /// </summary>
        /// <param name="FeePolicyRepository">Repository for Fee data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="unitOfWork">Unit of Work for database operations.</param>
        public AddFeePolicyHandler(
            IFeePolicyRepository FeePolicyRepository,
            IMapper mapper,
            ILogger<AddFeePolicyHandler> logger,
            IUnitOfWork<TransactionContext> unitOfWork,
            IFeeRepository feeRepository)
        {
            _FeePolicyRepository = FeePolicyRepository;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _feeRepository = feeRepository;
        }

        /// <summary>
        /// Handles the AddFeePolicyCommand to add a new Fee Range.
        /// </summary>
        /// <param name="request">The AddFeePolicyCommand containing Fee Range data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<FeePolicyDto>> Handle(AddFeePolicyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.BranchId=="0")
                {
                    request.IsCentralised=true;
                }
                // Check if a Fee with the same details already exists
                var existingFee = await _FeePolicyRepository.FindBy(c =>
                    c.FeeId == request.FeeId &&
                    c.AmountFrom == request.AmountFrom &&
                    c.AmountTo == request.AmountTo && c.BranchId == request.BranchId && c.BankId == request.BankId && c.IsDeleted == false)
                    .FirstOrDefaultAsync();

                // If a Fee with the same details already exists, return a conflict response
                if (existingFee != null)
                {
                    var errorMessage = $"Fee range already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<FeePolicyDto>.Return409(errorMessage);
                }


                // Map the AddFeePolicyCommand to a Fee Range entity
                var FeePolicyEntity = _mapper.Map<FeePolicy>(request);
                FeePolicyEntity.Id = BaseUtilities.GenerateUniqueNumber();
                FeePolicyEntity.BranchId = FeePolicyEntity.BranchId;
                FeePolicyEntity.BankId = FeePolicyEntity.BankId;
                // Add the new Fee Range entity to the repository
                _FeePolicyRepository.Add(FeePolicyEntity);
                await _unitOfWork.SaveAsync();
                // Map the Fee Range entity to FeePolicyDto and return it with a success response
                var FeePolicyDto = _mapper.Map<FeePolicyDto>(FeePolicyEntity);
                return ServiceResponse<FeePolicyDto>.ReturnResultWith200(FeePolicyDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Fee Range: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<FeePolicyDto>.Return500(e);
            }
        }
    }

}
