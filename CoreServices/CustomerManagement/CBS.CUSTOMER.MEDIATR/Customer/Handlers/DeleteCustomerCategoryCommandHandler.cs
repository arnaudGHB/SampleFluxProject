using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to delete a Customer based on DeleteCustomerCategoryCommand.
    /// </summary>
    public class DeleteCustomerCategoryCommandHandler : IRequestHandler<DeleteCustomerCategoryCommand, ServiceResponse<bool>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly ILogger<DeleteCustomerCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteCustomerCategoryCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteCustomerCategoryCommandHandler(
            ICustomerRepository CustomerRepository, IMapper mapper,
            ILogger<DeleteCustomerCategoryCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _CustomerRepository = CustomerRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteCustomerCategoryCommand to delete a Customer.
        /// </summary>
        /// <param name="request">The DeleteCustomerCategoryCommand containing Customer ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteCustomerCategoryCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Customer entity with the specified ID exists
                var existingCustomer = await _CustomerRepository.FindAsync(request.Id);
                if (existingCustomer == null)
                {
                    errorMessage = $"Customer category with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingCustomer.IsDeleted = true;
                _CustomerRepository.Update(existingCustomer);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Customer: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
