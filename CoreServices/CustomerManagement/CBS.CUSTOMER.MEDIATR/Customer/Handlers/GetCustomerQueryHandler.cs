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
using CBS.CUSTOMER.MEDIATR.Customer.Queries;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using System.Text.RegularExpressions;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the request to retrieve a specific Customer based on its unique identifier.
    /// </summary>
    public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, ServiceResponse<CustomerDto>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository;
        private readonly UserInfoToken _UserInfoToken;
        private readonly ILogger<GetCustomerQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMediator _mediator;
        private readonly IUnitOfWork<POSContext> _uow;

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
            IMediator mediator,
            UserInfoToken userInfoToken,
            IDocumentBaseUrlRepository DocumentBaseUrlRepository,
            IUnitOfWork<POSContext> uow)
        {
            _CustomerRepository = CustomerRepository;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
            _UserInfoToken = userInfoToken;
            _DocumentBaseUrlRepository = DocumentBaseUrlRepository;
            _uow=uow;
        }

        /// <summary>
        /// Handles the retrieval of a customer by their Reference.
        /// </summary>
        /// <param name="request">The query containing the Reference.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A service response containing the customer details.</returns>
        public async Task<ServiceResponse<CustomerDto>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Step 1: Validate the provided Reference
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    errorMessage = "[ERROR] Customer retrieval failed: The provided Reference is empty or invalid.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.RetrieveMember, LogLevelInfo.Error);
                    return ServiceResponse<CustomerDto>.Return409(errorMessage);
                }

                // Step 2: Retrieve the Customer entity along with its related data
                var entity = await _CustomerRepository
                    .AllIncluding(
                        x => x.MembershipNextOfKings,
                        r => r.CustomerDocuments,
                        t => t.CardSignatureSpecimens,
                        z => z.CustomerCategory,
                        t => t.GroupCustomers
                    )
                    .FirstOrDefaultAsync(s => s.CustomerId == request.Id && !s.IsDeleted);

                // Step 3: Handle customer not found scenario
                if (entity == null)
                {
                    errorMessage = $"[ERROR] Customer retrieval failed: No customer found with Reference: {request.Id}.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.RetrieveMember, LogLevelInfo.Error);
                    return ServiceResponse<CustomerDto>.Return404(errorMessage);
                }

                // Step 4: Retrieve Document Base URL (for processing file paths)
                var documentBaseUrl = _DocumentBaseUrlRepository.Find("0")?.baseURL ?? "";

                // Step 5: Process URLs for customer profile image and signature
                entity.PhotoUrl = BaseUtilities.PrependBaseUrl(entity.PhotoUrl, documentBaseUrl);
                entity.SignatureUrl = BaseUtilities.PrependBaseUrl(entity.SignatureUrl, documentBaseUrl);

                // Step 6: Process document URLs for Customer Documents (if available)
                if (entity.CustomerDocuments?.Count > 0)
                {
                    foreach (var customerDocument in entity.CustomerDocuments)
                    {
                        customerDocument.UrlPath = BaseUtilities.PrependBaseUrl(customerDocument.UrlPath, documentBaseUrl);
                    }
                }

                // Step 7: Process profile and signature URLs for Next of Kin (if available)
                if (entity.MembershipNextOfKings?.Count > 0)
                {
                    foreach (var membershipNextOfKing in entity.MembershipNextOfKings)
                    {
                        membershipNextOfKing.PhotoUrl = BaseUtilities.PrependBaseUrl(membershipNextOfKing.PhotoUrl, documentBaseUrl);
                        membershipNextOfKing.SignatureUrl = BaseUtilities.PrependBaseUrl(membershipNextOfKing.SignatureUrl, documentBaseUrl);
                    }
                }

                // Step 8: Retrieve all customer age categories from the system
                var getAllCustomerQuery = new GetAllCustomerAgeCategoriesQuery();
                var result = await _mediator.Send(getAllCustomerQuery);

                // Step 9: Handle failure in fetching customer age categories
                if (!result.Success || result.Data == null)
                {
                    errorMessage = "[ERROR] Failed to retrieve customer age categories. Unable to determine customer's age category.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.RetrieveMember, LogLevelInfo.Error);
                    return ServiceResponse<CustomerDto>.Return404(errorMessage);
                }

                // Step 10: Calculate Age Category for the customer
                int ageDifference = DateTime.Now.Year - entity.DateOfBirth.Year;
                var ageCategory = result.Data.FirstOrDefault(x => x.from <= ageDifference && ageDifference <= x.to)?.Name ?? "No Status";

                // Step 11: Update Age Category if it has changed
                if (entity.AgeCategoryStatus != ageCategory)
                {
                    entity.AgeCategoryStatus = ageCategory;
                    _CustomerRepository.Update(entity);

                    // Step 12: Save changes and handle database update failures
                    await _uow.SaveAsync();
                    errorMessage = $"[SUCCESS] Customer [Reference: {entity.CustomerId}, Name: {entity.FirstName} {entity.LastName}.] age category was update successfull. from [{entity.AgeCategoryStatus}] to [{ageCategory}] ";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.UpdateMemberAgeStatusOnMemberRetrival, LogLevelInfo.Information);
                    
                }

                // Step 13: Map customer entity to DTO
                var customerDto = _mapper.Map<CustomerDto>(entity);

                // Step 14: Log and audit successful retrieval
                string successMessage = $"[SUCCESS] Customer retrieval successful. Reference: {entity.CustomerId}, Name: {entity.FirstName} {entity.LastName}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.RetrieveMember, LogLevelInfo.Information);

                return ServiceResponse<CustomerDto>.ReturnResultWith200(customerDto);
            }
            catch (Exception e)
            {
                // Step 15: Handle unexpected errors with detailed logging and auditing
                errorMessage = $"[ERROR] An unexpected error occurred while retrieving customer '{request.Id}'. Error Details: {e.Message}. Please contact support if the issue persists.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.RetrieveMember, LogLevelInfo.Error);
                return ServiceResponse<CustomerDto>.Return500(e);
            }
        }






        public async Task<ServiceResponse<CustomerDto>> Handlex(GetCustomerQuery request, CancellationToken cancellationToken)
        {

            string errorMessage = null;
            try
            {


                // Retrieve the Customer entity with the specified ID from the repository
                var entity = await _CustomerRepository.AllIncluding(x => x.MembershipNextOfKings, r => r.CustomerDocuments, t => t.CardSignatureSpecimens, z => z.CustomerCategory, t => t.GroupCustomers).FirstOrDefaultAsync(s => s.CustomerId == request.Id &&  s.IsDeleted == false);
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

                    if (entity.CustomerDocuments!=null && entity.CustomerDocuments.Count>0)
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



                    var getAllCustomerQuery = new GetAllCustomerAgeCategoriesQuery { };
                    var result = await _mediator.Send(getAllCustomerQuery);

                    if (!result.Success)
                    {

                        // If the All Customer Age Categories failed to be retrieved log the error and return a 404 Not Found response
                        errorMessage = $"failed to be retrieve All Customer Age Categories.";
                        _logger.LogError(errorMessage);
                        await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _UserInfoToken.Token);
                        return ServiceResponse<CustomerDto>.Return404();
                    }

                    int ageDifference = DateTime.Now.Year - entity.DateOfBirth.Year;
                    String getCategoryStatus = "No Status";

                    result.Data.ForEach(x =>
                    {
                        if (x.from <= ageDifference && ageDifference <= x.to)
                        {
                            getCategoryStatus = x.Name;
                        }

                    });
                    if (entity.AgeCategoryStatus!=getCategoryStatus)
                    {
                        entity.AgeCategoryStatus = getCategoryStatus;
                        _CustomerRepository.Update(entity);
                        if (await _uow.SaveAsync() <= 0)
                        {
                            return ServiceResponse<CustomerDto>.Return500();
                        }
                    }





                    var Customer = _mapper.Map<CustomerDto>(entity);

                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), request, $"Get Customer with CustomerId : {request.Id}", LogLevelInfo.Information.ToString(), 200, _UserInfoToken.Token);
                    return ServiceResponse<CustomerDto>.ReturnResultWith200(Customer);
                }
                else
                {
                    // If the Customer entity was not found, log the error and return a 404 Not Found response
                    errorMessage = $"Customer with CustomerId :  {request.Id} not found.";
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
