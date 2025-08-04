using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.CustomerLoanAccountP;
using CBS.NLoan.Data.Entity.CustomerLoanAccountP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Commands;
using CBS.NLoan.Repository.CustomerLoanAccountP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddCustomerLoanAccountHandler : IRequestHandler<AddCustomerLoanAccountCommand, ServiceResponse<CustomerLoanAccountDto>>
    {
        private readonly ICustomerLoanAccountRepository _CustomerLoanAccountRepository; // Repository for accessing CustomerLoanAccount data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCustomerLoanAccountHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddCustomerLoanAccountCommandHandler.
        /// </summary>
        /// <param name="CustomerLoanAccountRepository">Repository for CustomerLoanAccount data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCustomerLoanAccountHandler(
            ICustomerLoanAccountRepository CustomerLoanAccountRepository,
            IMapper mapper,
            ILogger<AddCustomerLoanAccountHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _CustomerLoanAccountRepository = CustomerLoanAccountRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddCustomerLoanAccountCommand to add a new CustomerLoanAccount.
        /// </summary>
        /// <param name="request">The AddCustomerLoanAccountCommand containing CustomerLoanAccount data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomerLoanAccountDto>> Handle(AddCustomerLoanAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a CustomerLoanAccount with the same name already exists (case-insensitive)
                var existingCustomerLoanAccount = await _CustomerLoanAccountRepository.FindBy(c => c.CustomerId == request.CustomerId).FirstOrDefaultAsync();

                // If a CustomerLoanAccount with the same name already exists, return a conflict response
                if (existingCustomerLoanAccount != null)
                {
                    var errorMessage = $"CustomerLoanAccount {request.CustomerId} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CustomerLoanAccountDto>.Return409(errorMessage);
                }


                // Map the AddCustomerLoanAccountCommand to a CustomerLoanAccount entity
                var CustomerLoanAccountEntity = _mapper.Map<CustomerLoanAccount>(request);
                // Convert UTC to local time and set it in the entity
                CustomerLoanAccountEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                CustomerLoanAccountEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new CustomerLoanAccount entity to the repository
                _CustomerLoanAccountRepository.Add(CustomerLoanAccountEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CustomerLoanAccountDto>.Return500();
                }

                // Map the CustomerLoanAccount entity to CustomerLoanAccountDto and return it with a success response
                var CustomerLoanAccountDto = _mapper.Map<CustomerLoanAccountDto>(CustomerLoanAccountEntity);
                return ServiceResponse<CustomerLoanAccountDto>.ReturnResultWith200(CustomerLoanAccountDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving CustomerLoanAccount: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CustomerLoanAccountDto>.Return500(e);
            }
        }
    }

}
