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
    /// Handles the retrieval of all Customers based on the GetAllCustomerListingsQuery.
    /// </summary>
    public class GetAllCustomerListingsQueryHandler : IRequestHandler<GetAllCustomerListingsQuery, ServiceResponse<List<CustomerListingDto>>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customers data.
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCustomerListingsQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllCustomerListingsQueryHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customers data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCustomerListingsQueryHandler(
            ICustomerRepository CustomerRepository,
       IDocumentBaseUrlRepository DocumentBaseUrlRepository,
            IMapper mapper, ILogger<GetAllCustomerListingsQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _DocumentBaseUrlRepository = DocumentBaseUrlRepository;
            _CustomerRepository = CustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCustomerListingsQuery to retrieve all Customers.
        /// </summary>
        /// <param name="request">The GetAllCustomerListingsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CustomerListingDto>>> Handle(GetAllCustomerListingsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Assign default dates if null
                DateTime dateFrom = request.DateFrom ?? DateTime.MinValue; // Default to the earliest date possible
                DateTime dateTo = request.DateTo ?? DateTime.UtcNow; // Default to today's date
                if (request.QueryParameter=="bymatricule")
                {
                    var entities = await _CustomerRepository.GetCustomerListingDto(
                                       request.QueryParameter,
                                       dateFrom,
                                       dateTo,
                                       request.BranchId,
                                       request.LegalFormStatus,
                                       request.MembersStatusType, request.PageNumber, request.PageSize
                                   );

                    return ServiceResponse<List<CustomerListingDto>>.ReturnResultWith200(entities);
                }
                var entitiesx = await _CustomerRepository.GetCustomerListingDto(
                    request.QueryParameter,
                    dateFrom,
                    dateTo,
                    request.BranchId,
                    request.LegalFormStatus,
                    request.MembersStatusType
                );

                return ServiceResponse<List<CustomerListingDto>>.ReturnResultWith200(entitiesx);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Customers: {e.Message}");
                return ServiceResponse<List<CustomerListingDto>>.Return500(e, "Failed to get all Customers");
            }
        }


    }
}
