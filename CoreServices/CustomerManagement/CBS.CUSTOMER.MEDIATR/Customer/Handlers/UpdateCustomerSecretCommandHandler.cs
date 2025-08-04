using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using Microsoft.EntityFrameworkCore;

namespace CBS.CUSTOMER.MEDIATR
{

    /// <summary>
    /// Handles the command to update a Customer based on UpdateCustomerSecretCommand.
    /// </summary>
    public class UpdateCustomerSecretCommandHandler : IRequestHandler<UpdateCustomerSecretCommand, ServiceResponse<UpdateCustomer>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly ILogger<UpdateCustomerSecretCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateCustomerSecretCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateCustomerSecretCommandHandler(
            ICustomerRepository CustomerRepository,
            ILogger<UpdateCustomerSecretCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _CustomerRepository = CustomerRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateCustomerSecretCommand to update a Customer.
        /// </summary>
        /// <param name="request">The UpdateCustomerSecretCommand containing updated Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UpdateCustomer>> Handle(UpdateCustomerSecretCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                // Retrieve the Customer entity to be updated from the repository
                var existingCustomer = await _CustomerRepository.FindAsync(request.CustomerId);
       
                // Check if the Customer entity exists
                if (existingCustomer != null)
                {
               
                    existingCustomer.SecretAnswer= request.SecretAnswer;
                    existingCustomer.SecretQuestion = request.SecretQuestion;

                    _CustomerRepository.Update(existingCustomer);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<UpdateCustomer>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<UpdateCustomer>.ReturnResultWith200(_mapper.Map<UpdateCustomer >(existingCustomer));
                    _logger.LogInformation($"Customer {request.CustomerId} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Customer entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.CustomerId} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<UpdateCustomer>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Customer: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<UpdateCustomer>.Return500(e);
            }
        }
    }

}
