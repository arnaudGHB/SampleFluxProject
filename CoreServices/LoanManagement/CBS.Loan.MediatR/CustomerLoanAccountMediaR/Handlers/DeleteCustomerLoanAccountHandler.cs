using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Commands;
using CBS.NLoan.Repository.CustomerLoanAccountP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteCustomerLoanAccountHandler : IRequestHandler<DeleteCustomerLoanAccountCommand, ServiceResponse<bool>>
    {
        private readonly ICustomerLoanAccountRepository _CustomerLoanAccountRepository; // Repository CustomerLoanAccount accessing CustomerLoanAccount data.
        private readonly ILogger<DeleteCustomerLoanAccountHandler> _logger; // Logger CustomerLoanAccount logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper CustomerLoanAccount object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor CustomerLoanAccount initializing the DeleteCustomerLoanAccountCommandHandler.
        /// </summary>
        /// <param name="CustomerLoanAccountRepository">Repository CustomerLoanAccount CustomerLoanAccount data access.</param>
        /// <param name="logger">Logger CustomerLoanAccount logging handler actions and errors.</param>
        public DeleteCustomerLoanAccountHandler(
            ICustomerLoanAccountRepository CustomerLoanAccountRepository, IMapper mapper,
            ILogger<DeleteCustomerLoanAccountHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _CustomerLoanAccountRepository = CustomerLoanAccountRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteCustomerLoanAccountCommand to delete a CustomerLoanAccount.
        /// </summary>
        /// <param name="request">The DeleteCustomerLoanAccountCommand containing CustomerLoanAccount ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteCustomerLoanAccountCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the CustomerLoanAccount entity with the specified ID exists
                var existingCustomerLoanAccount = await _CustomerLoanAccountRepository.FindAsync(request.Id);
                if (existingCustomerLoanAccount == null)
                {
                    errorMessage = $"CustomerLoanAccount with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingCustomerLoanAccount.IsDeleted = true;
                existingCustomerLoanAccount.DeletedBy = request.UserId;
                _CustomerLoanAccountRepository.Update(existingCustomerLoanAccount);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting CustomerLoanAccount: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
