using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;

namespace CBS.OrganizationCustomer.MEDIATR.OrganizationCustomerMediatR.Handlers
{

    /// <summary>
    /// Handles the command to update a OrganizationCustomer based on UpdateOrganizationCustomerCommand.
    /// </summary>
    public class UpdateOrganizationCustomerCommandHandler : IRequestHandler<UpdateOrganizationCustomerCommand, ServiceResponse<UpdateOrganizationCustomer>>
    {
        private readonly IOrganizationCustomerRepository _OrganizationCustomerRepository; // Repository for accessing OrganizationCustomer data.
        private readonly ILogger<UpdateOrganizationCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateOrganizationCustomerCommandHandler.
        /// </summary>
        /// <param name="OrganizationCustomerRepository">Repository for OrganizationCustomer data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateOrganizationCustomerCommandHandler(
            IOrganizationCustomerRepository OrganizationCustomerRepository,
            ILogger<UpdateOrganizationCustomerCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _OrganizationCustomerRepository = OrganizationCustomerRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateOrganizationCustomerCommand to update a OrganizationCustomer.
        /// </summary>
        /// <param name="request">The UpdateOrganizationCustomerCommand containing updated OrganizationCustomer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UpdateOrganizationCustomer>> Handle(UpdateOrganizationCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the OrganizationCustomer entity to be updated from the repository
                var existingOrganizationCustomer = await _OrganizationCustomerRepository.FindAsync(request.OrganizationCustomerId);

                // Check if the OrganizationCustomer entity exists
                if (existingOrganizationCustomer != null)
                {
                 
                    _OrganizationCustomerRepository.Update(existingOrganizationCustomer);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<UpdateOrganizationCustomer>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<UpdateOrganizationCustomer>.ReturnResultWith200(_mapper.Map<UpdateOrganizationCustomer>(existingOrganizationCustomer));
                    _logger.LogInformation($"OrganizationCustomer {request.OrganizationCustomerId} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the OrganizationCustomer entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.OrganizationCustomerId} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<UpdateOrganizationCustomer>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating OrganizationCustomer: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<UpdateOrganizationCustomer>.Return500(e);
            }
        }
    }

}
