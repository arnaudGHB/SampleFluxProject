using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Validations;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{

    public class AddLoanProductRepaymentOrderCommandHandler : IRequestHandler<AddLoanProductRepaymentOrderCommand, ServiceResponse<bool>>
    {
        private readonly ILoanProductRepaymentOrderRepository _LoanProductRepaymentOrderRepository; // Repository for accessing LoanProductRepaymentOrder data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanProductRepaymentOrderCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the AddLoanProductRepaymentOrderCommandHandler.
        /// </summary>
        /// <param name="LoanProductRepaymentOrderRepository">Repository for LoanProductRepaymentOrder data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanProductRepaymentOrderCommandHandler(
            ILoanProductRepaymentOrderRepository LoanProductRepaymentOrderRepository,
            IMapper mapper,
            ILogger<AddLoanProductRepaymentOrderCommandHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _LoanProductRepaymentOrderRepository = LoanProductRepaymentOrderRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddLoanProductRepaymentOrderCommand to add a new LoanProductRepaymentOrder.
        /// </summary>
        /// <param name="request">The AddLoanProductRepaymentOrderCommand containing LoanProductRepaymentOrder data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(AddLoanProductRepaymentOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                List<string> repaymentOrderList = new List<string>(Enum.GetNames(typeof(LoanProductRepaymentOrderType)));
                if (!repaymentOrderList.Contains(request.LoanProductRepaymentOrderType))
                {
                    var errorMessage = $"Loan Product Repayment Order Type {request.LoanProductRepaymentOrderType} does not exist.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                var loanProductRepaymentOrders = await _LoanProductRepaymentOrderRepository.FindBy(x => x.LoanProductRepaymentOrderType==request.LoanProductRepaymentOrderType).ToListAsync();
                if (loanProductRepaymentOrders.Any())
                {
                    var errorMessage = $"Repayment order already exist for {request.LoanDeliquencyPeriod}.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                var LoanProductEntity = _mapper.Map<LoanProductRepaymentOrder>(request);
                LoanProductEntity.Id = BaseUtilities.GenerateUniqueNumber();
                _LoanProductRepaymentOrderRepository.Add(LoanProductEntity);
                await _uow.SaveAsync();

                return ServiceResponse<bool>.ReturnResultWith200(true,$"Loan repayment order for {request.LoanDeliquencyPeriod} was success fully created.");
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving LoanProductRepaymentOrder: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }

}
