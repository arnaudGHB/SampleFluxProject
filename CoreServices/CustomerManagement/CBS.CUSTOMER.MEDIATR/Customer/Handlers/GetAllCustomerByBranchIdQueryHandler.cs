using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the retrieval of all Customers based on the GetAllCustomerByBranchIdQuery.
    /// </summary>
    public class GetAllCustomerByBranchIdQueryHandler : IRequestHandler<GetAllCustomerByBranchIdQuery, ServiceResponse<List<GetAllCustomers>>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customers data.
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCustomerByBranchIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllCustomerByBranchIdQueryHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customers data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCustomerByBranchIdQueryHandler(
            ICustomerRepository CustomerRepository,
       IDocumentBaseUrlRepository DocumentBaseUrlRepository,
            IMapper mapper, ILogger<GetAllCustomerByBranchIdQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _DocumentBaseUrlRepository = DocumentBaseUrlRepository;
            _CustomerRepository = CustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCustomerByBranchIdQuery to retrieve all Customers.
        /// </summary>
        /// <param name="request">The GetAllCustomerByBranchIdQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<GetAllCustomers>>> Handle(GetAllCustomerByBranchIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Initialize the base document URL as an empty string or retrieve from the repository
                var documentBaseUrl = _DocumentBaseUrlRepository.Find("0")?.baseURL ?? "";

                // Query to retrieve all non-deleted customers for the given branch ID
                var query = _CustomerRepository.All.Where(x => x.BranchId == request.BranchId && x.IsDeleted == false);

                // Execute the query and map the result to the DTO
                var entities = _mapper.Map<List<GetAllCustomers>>(await query.ToListAsync(cancellationToken));

                // Update URLs for photos and signatures if they are not null or empty
                //entities.ForEach(x =>
                //{
                //    if (!string.IsNullOrEmpty(x.PhotoUrl))
                //    {
                //        x.PhotoUrl = $"{documentBaseUrl}/{x.PhotoUrl}";
                //    }

                //    if (!string.IsNullOrEmpty(x.SignatureUrl))
                //    {
                //        x.SignatureUrl = $"{documentBaseUrl}/{x.SignatureUrl}";
                //    }
                //});

                // Return the result with a 200 status code
                return ServiceResponse<List<GetAllCustomers>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError($"Failed to get all Customers: {e.Message}");
                return ServiceResponse<List<GetAllCustomers>>.Return500(e, "Failed to get all Customers");
            }
        }
    }
}
