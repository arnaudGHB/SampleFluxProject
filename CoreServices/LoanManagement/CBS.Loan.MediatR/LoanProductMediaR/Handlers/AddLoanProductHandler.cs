using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.AccountingRuleP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.PenaltyP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Queries;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Validations;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;
using CBS.NLoan.Repository.AccountingRuleP;
using CBS.NLoan.Repository.CustomerProfileP;
using CBS.NLoan.Repository.FeeP.FeeP;
using CBS.NLoan.Repository.LoanProductP;
using CBS.NLoan.Repository.PenaltyP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanProductCommandHandler : IRequestHandler<AddLoanProductCommand, ServiceResponse<LoanProductDto>>
    {
        private readonly ILoanProductRepository _LoanProductRepository; // Repository for accessing LoanProduct data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanProductCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        private readonly IMediator _mediator; // Repository for accessing LoanProduct data.

        /// <summary>
        /// Constructor for initializing the AddLoanProductCommandHandler.
        /// </summary>
        /// <param name="LoanProductRepository">Repository for LoanProduct data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanProductCommandHandler(
            ILoanProductRepository LoanProductRepository,
            IMapper mapper,
            ILogger<AddLoanProductCommandHandler> logger,
            IUnitOfWork<LoanContext> uow,
            IMediator mediator = null)
        {
            _LoanProductRepository = LoanProductRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddLoanProductCommand to add a new LoanProduct.
        /// </summary>
        /// <param name="request">The AddLoanProductCommand containing LoanProduct data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductDto>> Handle(AddLoanProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a LoanProduct with the same name already exists (case-insensitive)
                var existingLoanProduct = await _LoanProductRepository.FindBy(c => c.ProductCode == request.ProductCode && c.IsDeleted == false).FirstOrDefaultAsync();

                // If a LoanProduct with the same name already exists, return a conflict response
                if (existingLoanProduct != null)
                {
                    var errorMessage = $"{request.ProductCode} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanProductDto>.Return409(errorMessage);
                }
                // Check if a LoanProduct with the same name already exists (case-insensitive)
                var nameExists = await _LoanProductRepository.FindBy(c => c.ProductName == request.ProductName && c.IsDeleted==false).FirstOrDefaultAsync();

                // If a LoanProduct with the same name already exists, return a conflict response
                if (nameExists != null)
                {
                    var errorMessage = $"{request.ProductName} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanProductDto>.Return409(errorMessage);
                }

                //var validator = LoanValidation.AddLoanProductValidation(request);

                //if (!validator.Equals(""))
                //{
                //    return ServiceResponse<LoanProductDto>.Return403(validator);
                //}

                // Map the AddLoanProductCommand to a LoanProduct entity
                var LoanProductEntity = _mapper.Map<LoanProduct>(request);
                LoanProductEntity.Id = BaseUtilities.GenerateUniqueNumber();
                // Add the new LoanProduct entity to the repository
                _LoanProductRepository.Add(LoanProductEntity);

                //var addLoanProductRepaymentCycle = new AddLoanProductRepaymentCycleCommand { LoanProductId = LoanProductEntity.Id, RepaymentCycles = request.RepaymentCycles};
                //var resultCycle = await _mediator.Send(addLoanProductRepaymentCycle, cancellationToken);

                //if (resultCycle.StatusCode!=200)
                //{
                //    var errorMessage = $"{resultCycle.Message}";
                //    _logger.LogError(errorMessage);
                //    return ServiceResponse<LoanProductDto>.Return409(errorMessage);
                //}
                //var repaymentOrderCommand = new AddLoanProductRepaymentOrderCommand { LoanProductId = LoanProductEntity.Id, RepaymentReceives = request.RefundOrders };
                //var resultOrder = await _mediator.Send(repaymentOrderCommand, cancellationToken);

                //if (resultOrder.StatusCode != 200)
                //{
                //    var errorMessage = $"{resultOrder.Message}";
                //    _logger.LogError(errorMessage);
                //    return ServiceResponse<LoanProductDto>.Return409(errorMessage);

                //}
                await _uow.SaveAsync();
                // Map the LoanProduct entity to LoanProductDto and return it with a success response
                var LoanProductDto = _mapper.Map<LoanProductDto>(LoanProductEntity);
                return ServiceResponse<LoanProductDto>.ReturnResultWith200(LoanProductDto,$"{request.ProductName} was successfully created.");
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving LoanProduct: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductDto>.Return500(e);
            }
        }
    }

}
