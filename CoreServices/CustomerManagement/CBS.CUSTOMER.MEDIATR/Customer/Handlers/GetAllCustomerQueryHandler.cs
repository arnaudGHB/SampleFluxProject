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
    /// Handles the retrieval of all Customers based on the GetAllCustomerQuery.
    /// </summary>
    public class GetAllCustomerQueryHandler : IRequestHandler<GetAllCustomerQuery, ServiceResponse<List<GetAllCustomers>>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customers data.
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCustomerQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllCustomerQueryHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customers data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCustomerQueryHandler(
            ICustomerRepository CustomerRepository,
       IDocumentBaseUrlRepository DocumentBaseUrlRepository,
            IMapper mapper, ILogger<GetAllCustomerQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _DocumentBaseUrlRepository = DocumentBaseUrlRepository;
            _CustomerRepository = CustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCustomerQuery to retrieve all Customers.
        /// </summary>
        /// <param name="request">The GetAllCustomerQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<GetAllCustomers>>> Handle(GetAllCustomerQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var documentBaseUrl = "";
                var baseDocumentUrl = _DocumentBaseUrlRepository.Find("0");

                if(baseDocumentUrl != null)
                {
                    documentBaseUrl = baseDocumentUrl.baseURL;
                }

                // Retrieve all Customers entities from the repository
                var entities = _mapper.Map< List< GetAllCustomers >> (await _CustomerRepository.All.Where(x=>x.IsDeleted==false).ToListAsync());

                //entities.ForEach(x =>
                //{
                //    if (!x.PhotoUrl.IsNullOrEmpty())
                //    {
                //       x.PhotoUrl= $"{documentBaseUrl}/{x.PhotoUrl}";
                //    }

                //    if(!x.SignatureUrl.IsNullOrEmpty())
                //    {
                //        x.SignatureUrl = $"{documentBaseUrl}/{x.SignatureUrl}";
                //    }
                //});
       


                return ServiceResponse<List<GetAllCustomers>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Customers: {e.Message}");
                return ServiceResponse<List<GetAllCustomers>>.Return500(e, "Failed to get all Customers");
            }
        }
    }
}
