using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Validations;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{
    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanProductRepaymentOrderCommandHandler : IRequestHandler<UpdateLoanProductRepaymentOrderCommand, ServiceResponse<LoanProductRepaymentOrderDto>>
    {
        private readonly ILoanProductRepaymentOrderRepository _LoanProductRepaymentOrderRepository; // Repository for accessing LoanProductRepaymentOrder data.
        private readonly ILogger<UpdateLoanProductRepaymentOrderCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLoanProductRepaymentOrderCommandHandler.
        /// </summary>
        /// <param name="LoanProductRepaymentOrderRepository">Repository for LoanProductRepaymentOrder data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanProductRepaymentOrderCommandHandler(
            ILoanProductRepaymentOrderRepository LoanProductRepaymentOrderRepository,
            ILogger<UpdateLoanProductRepaymentOrderCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _LoanProductRepaymentOrderRepository = LoanProductRepaymentOrderRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLoanProductRepaymentOrderCommand to update a LoanProductRepaymentOrder.
        /// </summary>
        /// <param name="request">The UpdateLoanProductRepaymentOrderCommand containing updated LoanProductRepaymentOrder data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductRepaymentOrderDto>> Handle(UpdateLoanProductRepaymentOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanProductRepaymentOrder entity to be updated from the repository
                var existingLoanProductRepaymentOrder = await _LoanProductRepaymentOrderRepository.FindAsync(request.Id);

                // Check if the LoanProductRepaymentOrder entity exists
                if (existingLoanProductRepaymentOrder != null)
                {
                    // Map properties from existingLoanProductRepaymentOrder to request
                    _mapper.Map(request, existingLoanProductRepaymentOrder);

                    // Use the repository to update the existing LoanProductRepaymentOrder entity
                    _LoanProductRepaymentOrderRepository.Update(existingLoanProductRepaymentOrder);
                    await _uow.SaveAsync();

                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanProductRepaymentOrderDto>.ReturnResultWith200(_mapper.Map<LoanProductRepaymentOrderDto>(existingLoanProductRepaymentOrder),$"Loan repayment order from {request.LoanDeliquencyPeriod} was successfully updated.");
                    _logger.LogInformation($"Loan repayment order from {request.LoanDeliquencyPeriod} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the LoanProductRepaymentOrder entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanProductRepaymentOrderDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LoanProductRepaymentOrder: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductRepaymentOrderDto>.Return500(e);
            }
        }
    }

}
