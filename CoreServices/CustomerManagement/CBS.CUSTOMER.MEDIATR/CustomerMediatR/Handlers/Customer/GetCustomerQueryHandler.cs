using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CBS.CUSTOMER.DATA.Dto;
using System.Collections;
using Microsoft.IdentityModel.Tokens;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the request to retrieve a specific Customer based on its unique identifier.
    /// </summary>
    public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, ServiceResponse<GetCustomer>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository;
        private readonly UserInfoToken _UserInfoToken;
        private readonly ILogger<GetCustomerQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCustomerQueryHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCustomerQueryHandler(
            ICustomerRepository CustomerRepository,
            IMapper mapper,
            ILogger<GetCustomerQueryHandler> logger,
            UserInfoToken userInfoToken,
            IDocumentBaseUrlRepository DocumentBaseUrlRepository)
        {
            _CustomerRepository = CustomerRepository;
            _mapper = mapper;
            _logger = logger;
            _UserInfoToken = userInfoToken;
            _DocumentBaseUrlRepository = DocumentBaseUrlRepository;
        }

        /// <summary>
        /// Handles the GetCustomerQuery to retrieve a specific Customer.
        /// </summary>
        /// <param name="request">The GetCustomerQuery containing Customer ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<GetCustomer>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        {
          
            string errorMessage = null;
            try
            {
                // Retrieve the Customer entity with the specified ID from the repository
                var entity = await _CustomerRepository.AllIncluding(x => x.MembershipNextOfKings, r => r.CustomerDocuments, t=> t.CardSignatureSpecimens,z=>z.CustomerCategory).FirstOrDefaultAsync(  s => s.CustomerId == request.Id &&  s.IsDeleted == false);
                if (entity != null)
                {
                    var documentBaseUrl = "";
                    var baseDocumentUrl = _DocumentBaseUrlRepository.Find("0");

                    if (baseDocumentUrl != null)
                    {
                        documentBaseUrl = baseDocumentUrl.baseURL;
                    }

                    if (!entity.PhotoUrl.IsNullOrEmpty())
                    {
                        entity.PhotoUrl = $"{documentBaseUrl}/{entity.PhotoUrl}";
                    }

                    if (!entity.SignatureUrl.IsNullOrEmpty())
                    {
                        entity.SignatureUrl = $"{documentBaseUrl}/{entity.SignatureUrl}";
                    }

                    var Customer = _mapper.Map<GetCustomer>(entity);
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), Customer, $"Get Customer with Id : {request.Id}", LogLevelInfo.Information.ToString(), 200,_UserInfoToken.Token);
                    return ServiceResponse<GetCustomer>.ReturnResultWith200(Customer);
                }
                else
                {
                    // If the Customer entity was not found, log the error and return a 404 Not Found response
                    errorMessage = $"Customer with Id :  {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _UserInfoToken.Token);
                    return ServiceResponse<GetCustomer>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Customer: {e.Message}";
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _UserInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<GetCustomer>.Return500(e);
            }
        }
    }

}
