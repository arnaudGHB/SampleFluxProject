using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands.Global;
using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.REPOSITORY.OrganisationRepo;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Configuration;
using CBS.CUSTOMER.DATA.Entity.Document;
using CBS.CUSTOMER.REPOSITORY.DocumentRepo;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Entity;
using Microsoft.IdentityModel.Tokens;
using CBS.CUSTOMER.DATA.Entity.Config;
using CBS.CUSTOMER.DATA.Entity.Groups;

namespace CBS.GroupCustomer.MEDIATR.GroupCustomerMediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific GroupCustomer based on its unique identifier.
    /// </summary>
    public class AddDocumentHandler : IRequestHandler<AddDocumentCommand, ServiceResponse<bool>>
    {

        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly IGroupDocumentRepository _GroupDocumentRepository; // Repository for accessing Group Document data.
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _UserInfoToken;
        private readonly IDocumentRepository _documentRepository;
        private readonly IMembershipNextOfKingRepository _MembershipNextOfKingRepository;
        private readonly ICardSignatureSpecimenRepository _CardSignatureSpecimenRepository;
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ILogger<AddDocumentHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCustomerByIDAndProfileQueryHandler.
        /// </summary>
        /// <param name="GroupCustomerRepository">Repository for GroupCustomer data access.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddDocumentHandler(
             ILogger<AddDocumentHandler> logger,
            ICustomerRepository customerRepository,
            IDocumentRepository documentRepository,
            IMembershipNextOfKingRepository MembershipNextOfKingRepository,
            ICardSignatureSpecimenRepository CardSignatureSpecimenRepository,
             IConfiguration configuration,
             IDocumentBaseUrlRepository DocumentBaseUrlRepository,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken,
            IGroupDocumentRepository groupDocumentRepository)
        {
            _logger = logger;
            _documentRepository = documentRepository;
            _pathHelper = new PathHelper(configuration);
            _CustomerRepository = customerRepository;
            _DocumentBaseUrlRepository = DocumentBaseUrlRepository;
            _CardSignatureSpecimenRepository = CardSignatureSpecimenRepository;
            _uow = uow;
            _MembershipNextOfKingRepository = MembershipNextOfKingRepository;
            _UserInfoToken = userInfoToken;
            _GroupDocumentRepository = groupDocumentRepository;
        }


        private List<string> EnumToList<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Select(enumValue => enumValue.ToString()).ToList();
        }

        public async Task<ServiceResponse<bool>> Handle(AddDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {

                if (request.BaseUrl.IsNullOrEmpty() || request.UrlPath.IsNullOrEmpty() || request.Id.IsNullOrEmpty())
                {
                    var errorMessage = $"Either BaseUrl Or UrlPath Or CustomerId is Null or Empty";
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _UserInfoToken.Token);
                    return ServiceResponse<bool>.Return500(errorMessage);
                }


                var baseDocumentUrl = _DocumentBaseUrlRepository.Find("0");

                if (baseDocumentUrl == null)
                {

                    var documentUrl = new DocumentBaseUrl()
                    {
                        id = "0",
                        baseURL = request.BaseUrl,
                    };

                    _DocumentBaseUrlRepository.Add(documentUrl);


                }
                else
                {
                    baseDocumentUrl.baseURL = request.BaseUrl;
                    _DocumentBaseUrlRepository.Update(baseDocumentUrl);
                }



                var documentTypes = EnumToList<DocumentTypes>();


                var verifyDocumentType = documentTypes.FirstOrDefault(x => x == request.DocumentType);

                if (verifyDocumentType != null)
                {

                    switch (verifyDocumentType)
                    {

                        case "NextofkingsPhoto":
                            {
                                var membershipNextOfKing = await _MembershipNextOfKingRepository.FindAsync(request.Id);
                                if (membershipNextOfKing != null)
                                {
                                    membershipNextOfKing.PhotoUrl = request.UrlPath;
                                    _MembershipNextOfKingRepository.Update(membershipNextOfKing);
                                }
                            }
                            break;

                        case "NextofkingsSignature":
                            {
                                var membershipNextOfKing = await _MembershipNextOfKingRepository.FindAsync(request.Id);
                                if (membershipNextOfKing != null)
                                {
                                    membershipNextOfKing.SignatureUrl = request.UrlPath;
                                    _MembershipNextOfKingRepository.Update(membershipNextOfKing);
                                }
                            }
                            break;

                        case "CardSignatureSpecimenPhoto":
                            {
                                var cardSignatureSpecimenDetail = await _CardSignatureSpecimenRepository.FindAsync(request.Id);
                                if (cardSignatureSpecimenDetail != null)
                                {
                                    cardSignatureSpecimenDetail.PhotoUrl1 = request.UrlPath;
                                    _CardSignatureSpecimenRepository.Update(cardSignatureSpecimenDetail);
                                }
                            }
                            break;

                        case "CardSignatureSpecimenSignature1":
                            {
                                var cardSignatureSpecimenDetail = await _CardSignatureSpecimenRepository.FindAsync(request.Id);
                                if (cardSignatureSpecimenDetail != null)
                                {
                                    cardSignatureSpecimenDetail.SignatureUrl1 = request.UrlPath;
                                    _CardSignatureSpecimenRepository.Update(cardSignatureSpecimenDetail);
                                }
                            }
                            break;

                        case "CardSignatureSpecimenSignature2":
                            {
                                var cardSignatureSpecimenDetail = await _CardSignatureSpecimenRepository.FindAsync(request.Id);
                                if (cardSignatureSpecimenDetail != null)
                                {
                                    cardSignatureSpecimenDetail.SignatureUrl2 = request.UrlPath;
                                    _CardSignatureSpecimenRepository.Update(cardSignatureSpecimenDetail);
                                }
                            }
                            break;


                        case "CustomerPhoto":
                            {
                                var customer = await _CustomerRepository.FindAsync(request.Id);
                                if (customer != null)
                                {
                                    customer.PhotoUrl = request.UrlPath;
                                    _CustomerRepository.Update(customer);
                                }
                            }
                            break;

                        case "CustomerSignature":
                            {
                                var customer = await _CustomerRepository.FindAsync(request.Id);
                                if (customer != null)
                                {
                                    customer.SignatureUrl = request.UrlPath;
                                    _CustomerRepository.Update(customer);
                                }
                            }
                            break;

                        case "CustomerOtherDocument":
                            {
                                var customer = await _CustomerRepository.FindAsync(request.Id);
                                if (customer != null)
                                {

                                    var document = new CustomerDocument()
                                    {
                                        DocumentId = BaseUtilities.GenerateUniqueNumber(),
                                        CustomerId = request.Id,
                                        Extension = request.Extension,
                                        DocumentName = request.DocumentName,
                                        UrlPath = request.UrlPath,
                                        BaseUrl = request.BaseUrl,
                                        DocumentType = request.DocumentType,

                                    };
                                    _documentRepository.Add(document);

                                }


                            }
                            break;
                        case "GroupDocument":
                            {
                                var customer = await _GroupDocumentRepository.FindAsync(request.Id);
                                if (customer != null)
                                {

                                    var document = new GroupDocument()
                                    {
                                        GroupDocumentId = BaseUtilities.GenerateUniqueNumber(),
                                        GroupId = request.Id,
                                        Extension = request.Extension,
                                        DocumentName = request.DocumentName,
                                        UrlPath = request.UrlPath,
                                        BaseUrl = request.BaseUrl,
                                        DocumentType = request.DocumentType,

                                    };
                                    _GroupDocumentRepository.Add(document);

                                }


                            }
                            break;




                    }
                    if (await _uow.SaveAsync() <= 0)
                    {
                        await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, $"Internal Server Error Occurred While Persisting DocumentPath into the database", LogLevelInfo.Information.ToString(), 500, _UserInfoToken.Token);
                        return ServiceResponse<bool>.Return500();
                    }

                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, $"Adding New  Document  Successful", LogLevelInfo.Information.ToString(), 200, _UserInfoToken.Token);
                    return ServiceResponse<bool>.ReturnResultWith200(true);
                }
                else
                {
                    var errorMessage = $"DocumentType : {request.DocumentType} Doest Not Exist";
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _UserInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

            }
            catch (Exception ex)
            {
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, ex.Message, LogLevelInfo.Information.ToString(), 500, _UserInfoToken.Token);
                return ServiceResponse<bool>.Return500();
            }


        }

    }

}
