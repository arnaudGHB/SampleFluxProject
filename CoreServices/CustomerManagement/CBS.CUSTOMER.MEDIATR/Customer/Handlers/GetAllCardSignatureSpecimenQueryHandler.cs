using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY.DocumentRepo;
using Microsoft.IdentityModel.Tokens;
using CBS.CUSTOMER.DATA;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the retrieval of all CardSignatureSpecimen based on the GetAllCardSignatureSpecimenQuery.
    /// </summary>
    public class GetAllCardSignatureSpecimenQueryHandler : IRequestHandler<GetAllCardSignatureSpecimenQuery, ServiceResponse<List<GetCardSignatureSpecimen>>>
    {
        private readonly ICardSignatureSpecimenRepository _CardSignatureSpecimenRepository; // Repository for accessing CardSignatureSpecimen data.
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository; // Repository for accessing DocumentBaseUrl data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCardSignatureSpecimenQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllCardSignatureSpecimenQueryHandler.
        /// </summary>
        /// <param name="CardSignatureSpecimenRepository">Repository for CardSignatureSpecimen data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCardSignatureSpecimenQueryHandler(
            ICardSignatureSpecimenRepository CardSignatureSpecimenRepository,

            IMapper mapper, ILogger<GetAllCardSignatureSpecimenQueryHandler> logger, IDocumentBaseUrlRepository documentBaseUrlRepository)
        {
            // Assign provided dependencies to local variables.
            _CardSignatureSpecimenRepository = CardSignatureSpecimenRepository;
            _mapper = mapper;
            _logger = logger;
            _DocumentBaseUrlRepository = documentBaseUrlRepository;
        }

        /// <summary>
        /// Handles the GetAllCardSignatureSpecimenQuery to retrieve all CardSignatureSpecimen.
        /// </summary>
        /// <param name="request">The GetAllCardSignatureSpecimenQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<GetCardSignatureSpecimen>>> Handle(GetAllCardSignatureSpecimenQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var documentBaseUrl = "";
                var baseDocumentUrl = _DocumentBaseUrlRepository.Find("0");

                if (baseDocumentUrl != null)
                {
                    documentBaseUrl = baseDocumentUrl.baseURL;
                }

                // Retrieve all CardSignatureSpecimen entities from the repository
                var entities = _mapper.Map< List<GetCardSignatureSpecimen>> (await _CardSignatureSpecimenRepository.All
                    .Where(x=>x.IsDeleted==false).ToListAsync());

                //entities.ForEach(x =>
                //{
                //    x.CardSignatureSpecimenDetails.ForEach(y =>
                //    {
                //        if (!y.PhotoUrl1.IsNullOrEmpty())
                //        {
                //            y.PhotoUrl1 = $"{documentBaseUrl}/{y.PhotoUrl1}";
                //        }

                //        if (!y.SignatureUrl1.IsNullOrEmpty())
                //        {
                //            y.SignatureUrl1 = $"{documentBaseUrl}/{y.SignatureUrl1}";
                //        }
                        
                //        if (!y.SignatureUrl2.IsNullOrEmpty())
                //        {
                //            y.SignatureUrl2 = $"{documentBaseUrl}/{y.SignatureUrl2}";
                //        }
                //    });
                  
                //});

                return ServiceResponse<List<GetCardSignatureSpecimen>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all CardSignatureSpecimen: {e.Message}");
                return ServiceResponse<List<GetCardSignatureSpecimen>>.Return500(e, "Failed to get all CardSignatureSpecimen");
            }
        }
    }
}
