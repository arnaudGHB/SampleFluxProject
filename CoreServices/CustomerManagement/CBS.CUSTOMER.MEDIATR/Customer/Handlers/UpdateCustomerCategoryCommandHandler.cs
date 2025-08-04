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
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.DATA;

namespace CBS.CUSTOMER.MEDIATR
{

    /// <summary>
    /// Handles the command to update a Customer based on UpdateCustomerCategoryCommand.
    /// </summary>
    public class UpdateCustomerCategoryCommandHandler : IRequestHandler<UpdateCustomerCategoryCommand, ServiceResponse<CreateCustomerCategory>>
    {
        private readonly ICustomerCategoryRepository _CustomerCategoryRepository; // Repository for accessing Customer data.
        private readonly ILogger<UpdateCustomerCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateCustomerCategoryCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateCustomerCategoryCommandHandler(
            ICustomerCategoryRepository CustomerRepository,
            ILogger<UpdateCustomerCategoryCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _CustomerCategoryRepository = CustomerRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateCustomerCategoryCommand to update a Customer.
        /// </summary>
        /// <param name="request">The UpdateCustomerCategoryCommand containing updated Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateCustomerCategory>> Handle(UpdateCustomerCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the CustomerCategory entity to be updated from the repository
                var existingCategory = await _CustomerCategoryRepository.FindAsync(request.Id);

                // Check if the CustomerCategory entity exists
                if (existingCategory != null)
                {
                    // Update the properties of the existing CustomerCategory entity with values from the request
                    _mapper.Map(request, existingCategory);

                    // Update the CustomerCategory entity in the repository
                    _CustomerCategoryRepository.Update(existingCategory);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<CreateCustomerCategory>.ReturnResultWith200(_mapper.Map<CreateCustomerCategory>(existingCategory));
                    _logger.LogInformation($"Customer category {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the CustomerCategory entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CreateCustomerCategory>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating CustomerCategory: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateCustomerCategory>.Return500(e);
            }
        }
    }

}
