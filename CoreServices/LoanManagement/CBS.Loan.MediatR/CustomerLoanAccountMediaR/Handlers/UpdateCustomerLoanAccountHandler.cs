using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.CustomerLoanAccountP;
using CBS.NLoan.Data.Entity.CustomerLoanAccountP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Commands;
using CBS.NLoan.Repository.CustomerLoanAccountP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateCustomerLoanAccountCommandHandler : IRequestHandler<UpdateCustomerLoanAccountCommand, ServiceResponse<CustomerLoanAccountDto>>
    {
        private readonly ICustomerLoanAccountRepository _CustomerLoanAccountRepository; // Repository for accessing CustomerLoanAccount data.
        private readonly ILogger<UpdateCustomerLoanAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateCustomerLoanAccountCommandHandler.
        /// </summary>
        /// <param name="CustomerLoanAccountRepository">Repository for CustomerLoanAccount data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateCustomerLoanAccountCommandHandler(
            ICustomerLoanAccountRepository CustomerLoanAccountRepository,
            ILogger<UpdateCustomerLoanAccountCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _CustomerLoanAccountRepository = CustomerLoanAccountRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateCustomerLoanAccountCommand to update a CustomerLoanAccount.
        /// </summary>
        /// <param name="request">The UpdateCustomerLoanAccountCommand containing updated CustomerLoanAccount data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomerLoanAccountDto>> Handle(UpdateCustomerLoanAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the CustomerLoanAccount entity to be updated from the repository
                var existingCustomerLoanAccount = await _CustomerLoanAccountRepository.FindAsync(request.Id);

                // Check if the CustomerLoanAccount entity exists
                if (existingCustomerLoanAccount != null)
                {
                    // Update CustomerLoanAccount entity properties with values from the request

                    var customerLoanAccountToUpdate = _mapper.Map<CustomerLoanAccount>(request);
                    // Use the repository to update the existing CustomerLoanAccount entity
                    _CustomerLoanAccountRepository.Update(customerLoanAccountToUpdate);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<CustomerLoanAccountDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<CustomerLoanAccountDto>.ReturnResultWith200(_mapper.Map<CustomerLoanAccountDto>(existingCustomerLoanAccount));
                    _logger.LogInformation($"CustomerLoanAccount {request.CustomerId} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the CustomerLoanAccount entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.CustomerId} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CustomerLoanAccountDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating CustomerLoanAccount: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CustomerLoanAccountDto>.Return500(e);
            }
        }
    }

}
