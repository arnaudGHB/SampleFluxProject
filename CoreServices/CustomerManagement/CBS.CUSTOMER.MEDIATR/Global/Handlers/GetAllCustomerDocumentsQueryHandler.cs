using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.REPOSITORY.DocumentRepo;
using CBS.CUSTOMER.DATA.Dto;
using Microsoft.IdentityModel.Tokens;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the retrieval of all CustomerDocuments based on the GetAllCustomerDocumentQuery.
    /// </summary>
    public class GetAllCustomerDocumentsQueryHandler : IRequestHandler<GetAllCustomerDocumentsQuery, ServiceResponse<List<GetCustomerDocument>>>
    {
        private readonly IDocumentRepository _CustomerDocumentRepository; // Repository for accessing CustomerDocuments data.
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository; // Repository for accessing CustomerDocumentsBase  Url data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCustomerDocumentsQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllCustomerDocumentQueryHandler.
        /// </summary>
        /// <param name="CustomerDocumentRepository">Repository for CustomerDocuments data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCustomerDocumentsQueryHandler(
            IDocumentRepository CustomerDocumentRepository,
             IDocumentBaseUrlRepository DocumentBaseUrlRepository,
       
            IMapper mapper, ILogger<GetAllCustomerDocumentsQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _CustomerDocumentRepository = CustomerDocumentRepository;
            _DocumentBaseUrlRepository = DocumentBaseUrlRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCustomerDocumentQuery to retrieve all CustomerDocuments.
        /// </summary>
        /// <param name="request">The GetAllCustomerDocumentQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<GetCustomerDocument>>> Handle(GetAllCustomerDocumentsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var documentBaseUrl = "";
                var baseDocumentUrl = _DocumentBaseUrlRepository.Find("0");

                if (baseDocumentUrl != null)
                {
                    documentBaseUrl = baseDocumentUrl.baseURL;
                }

                // Retrieve all CustomerDocuments entities from the repository
                var entities = _mapper.Map< List<GetCustomerDocument>> (await _CustomerDocumentRepository.All.Where(x=>x.IsDeleted==false).ToListAsync());

                entities.ForEach(x =>
                {
                    if (!x.UrlPath.IsNullOrEmpty())
                    {
                        x.UrlPath = $"{documentBaseUrl}/{x.UrlPath}";
                    }

                
                });

                return ServiceResponse<List<GetCustomerDocument>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all CustomerDocuments: {e.Message}");
                return ServiceResponse<List<GetCustomerDocument>>.Return500(e, "Failed to get all CustomerDocuments");
            }
        }
    }
}
