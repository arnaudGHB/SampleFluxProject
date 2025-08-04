using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the retrieval of all CustomerCategory based on the GetAllCustomerCategoryQuery.
    /// </summary>
    public class GetAllCustomerCategoryQueryHandler : IRequestHandler<GetAllCustomerCategoryQuery, ServiceResponse<List<CreateCustomerCategory>>>
    {
        private readonly ICustomerCategoryRepository _CustomerCategoryRepository; // Repository for accessing CustomerCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCustomerCategoryQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllCustomerCategoryQueryHandler.
        /// </summary>
        /// <param name="CustomerCategoryRepository">Repository for CustomerCategory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCustomerCategoryQueryHandler(
            ICustomerCategoryRepository CustomerCategoryRepository,
       
            IMapper mapper, ILogger<GetAllCustomerCategoryQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _CustomerCategoryRepository = CustomerCategoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCustomerCategoryQuery to retrieve all CustomerCategory.
        /// </summary>
        /// <param name="request">The GetAllCustomerCategoryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CreateCustomerCategory>>> Handle(GetAllCustomerCategoryQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // Retrieve all CustomerCategory entities from the repository
                var entities = _mapper.Map< List<CreateCustomerCategory>> (await _CustomerCategoryRepository.All.Where(x=>x.IsDeleted==false).ToListAsync());
                return ServiceResponse<List<CreateCustomerCategory>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all CustomerCategory: {e.Message}");
                return ServiceResponse<List<CreateCustomerCategory>>.Return500(e, "Failed to get all CustomerCategory");
            }
        }
    }
}
