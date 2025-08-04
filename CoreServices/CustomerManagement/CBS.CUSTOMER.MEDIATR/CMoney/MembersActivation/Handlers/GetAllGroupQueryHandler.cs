using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Queries;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Handlers
{
    /// <summary>
    /// Handles the retrieval of all CustomerSmsConfigurations based on the GetAllCustomerSmsConfigurationsQuery.
    /// </summary>
    public class GetAllCustomerSmsConfigurationsQueryHandler : IRequestHandler<GetAllCustomerSmsConfigurationsQuery, ServiceResponse<List<CreateCustomerSmsConfigurations>>>
    {
        private readonly ICustomerSmsConfigurationRepository _CustomerSmsConfigurationsRepository; // Repository for accessing CustomerSmsConfigurationss data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCustomerSmsConfigurationsQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllCustomerSmsConfigurationsQueryHandler.
        /// </summary>
        /// <param name="CustomerSmsConfigurationsRepository">Repository for CustomerSmsConfigurationss data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCustomerSmsConfigurationsQueryHandler(
            ICustomerSmsConfigurationRepository CustomerSmsConfigurationsRepository,
             IMapper mapper, ILogger<GetAllCustomerSmsConfigurationsQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _CustomerSmsConfigurationsRepository = CustomerSmsConfigurationsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCustomerSmsConfigurationsQuery to retrieve all CustomerSmsConfigurationss.
        /// </summary>
        /// <param name="request">The GetAllCustomerSmsConfigurationsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CreateCustomerSmsConfigurations>>> Handle(GetAllCustomerSmsConfigurationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //   await _CustomerSmsConfigurationsRepository.AllIncluding(x => x.CustomerSmsConfigurationsCustomers, s => s.ProfileRequirements).ToListAsync();
                // Retrieve all CustomerSmsConfigurationss entities from the repository
                var entities = _mapper.Map<List<CreateCustomerSmsConfigurations>>(await _CustomerSmsConfigurationsRepository.All.ToListAsync());




                return ServiceResponse<List<CreateCustomerSmsConfigurations>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all CustomerSmsConfigurationss: {e.Message}");
                return ServiceResponse<List<CreateCustomerSmsConfigurations>>.Return500(e, "Failed to get all CustomerSmsConfigurationss");
            }
        }
    }
}
