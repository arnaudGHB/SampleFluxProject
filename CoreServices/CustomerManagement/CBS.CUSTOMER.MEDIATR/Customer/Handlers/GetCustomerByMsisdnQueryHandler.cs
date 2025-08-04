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
    public class GetCustomerByMsisdnQueryHandler : IRequestHandler<GetCustomerByMsisdnQuery, ServiceResponse<CustomerDto>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository;
        private readonly UserInfoToken _UserInfoToken;
        private readonly ILogger<GetCustomerByMsisdnQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCustomerByMsisdnQueryHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCustomerByMsisdnQueryHandler(
            ICustomerRepository CustomerRepository,
            IMapper mapper,
            ILogger<GetCustomerByMsisdnQueryHandler> logger,
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
        /// Handles the GetCustomerByMsisdnQuery to retrieve a specific Customer.
        /// </summary>
        /// <param name="request">The GetCustomerByMsisdnQuery containing Customer ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomerDto>> Handle(GetCustomerByMsisdnQuery request, CancellationToken cancellationToken)
        {
          
            string errorMessage = null;
            try
            {
                
                // Retrieve the Customer entity with the specified ID from the repository
                var entity = await _CustomerRepository.AllIncluding(x => x.MembershipNextOfKings, r => r.CustomerDocuments, t=> t.CardSignatureSpecimens,z=>z.CustomerCategory,t=>t.GroupCustomers).FirstOrDefaultAsync(  s => s.Phone == request.TelephoneNumber &&  s.IsDeleted == false);
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

                    if(entity.CustomerDocuments!=null && entity.CustomerDocuments.Count>0)
                    {
                        foreach (var customerDocument in entity.CustomerDocuments)
                        {
                            if (!customerDocument.UrlPath.IsNullOrEmpty())
                            {
                                customerDocument.UrlPath = $"{documentBaseUrl}/{customerDocument.UrlPath}";
                            }
                        }

                    }

                    if (entity.MembershipNextOfKings != null && entity.MembershipNextOfKings.Count > 0)
                    {
                        foreach (var membershipNextOfKing in entity.MembershipNextOfKings)
                        {
                            if (!membershipNextOfKing.PhotoUrl.IsNullOrEmpty())
                            {
                                membershipNextOfKing.PhotoUrl = $"{documentBaseUrl}/{membershipNextOfKing.PhotoUrl}";
                            } 
                            
                            
                            if (!membershipNextOfKing.SignatureUrl.IsNullOrEmpty())
                            {
                                membershipNextOfKing.SignatureUrl = $"{documentBaseUrl}/{membershipNextOfKing.SignatureUrl}";
                            }


                        }

                    } 

                    var Customer = _mapper.Map<CustomerDto>(entity);

                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), request, $"Get Customer with phone number : {request.TelephoneNumber}", LogLevelInfo.Information.ToString(), 200,_UserInfoToken.Token);
                    return ServiceResponse<CustomerDto>.ReturnResultWith200(Customer);
                }
                else
                {
                    // If the Customer entity was not found, log the error and return a 404 Not Found response
                    errorMessage = $"Customer with telephone number :  {request.TelephoneNumber} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _UserInfoToken.Token);
                    return ServiceResponse<CustomerDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Customer: {e.Message}";
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _UserInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CustomerDto>.Return500(e);
            }
        }
    }

}
