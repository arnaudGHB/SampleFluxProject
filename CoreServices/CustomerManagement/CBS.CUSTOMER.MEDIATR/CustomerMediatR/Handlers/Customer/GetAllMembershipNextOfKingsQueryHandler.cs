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

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the retrieval of all MembershipNextOfKings based on the GetAllMembershipNextOfKingsQuery.
    /// </summary>
    public class GetAllMembershipNextOfKingsQueryHandler : IRequestHandler<GetAllMembershipNextOfKingsQuery, ServiceResponse<List<GetMembershipNextOfKings>>>
    {
        private readonly IMembershipNextOfKingRepository _MembershipNextOfKingsRepository; // Repository for accessing MembershipNextOfKings data.
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository; // Repository for accessing DocumentBaseUrl data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllMembershipNextOfKingsQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllMembershipNextOfKingsQueryHandler.
        /// </summary>
        /// <param name="MembershipNextOfKingsRepository">Repository for MembershipNextOfKings data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllMembershipNextOfKingsQueryHandler(
            IMembershipNextOfKingRepository MembershipNextOfKingsRepository,

            IMapper mapper, ILogger<GetAllMembershipNextOfKingsQueryHandler> logger, IDocumentBaseUrlRepository documentBaseUrlRepository)
        {
            // Assign provided dependencies to local variables.
            _MembershipNextOfKingsRepository = MembershipNextOfKingsRepository;
            _mapper = mapper;
            _logger = logger;
            _DocumentBaseUrlRepository = documentBaseUrlRepository;
        }

        /// <summary>
        /// Handles the GetAllMembershipNextOfKingsQuery to retrieve all MembershipNextOfKings.
        /// </summary>
        /// <param name="request">The GetAllMembershipNextOfKingsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<GetMembershipNextOfKings>>> Handle(GetAllMembershipNextOfKingsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var documentBaseUrl = "";
                var baseDocumentUrl = _DocumentBaseUrlRepository.Find("0");

                if (baseDocumentUrl != null)
                {
                    documentBaseUrl = baseDocumentUrl.baseURL;
                }

                // Retrieve all MembershipNextOfKings entities from the repository
                var entities = _mapper.Map< List<GetMembershipNextOfKings>> (await _MembershipNextOfKingsRepository.All.Where(x=>x.IsDeleted==false).ToListAsync());

                entities.ForEach(x =>
                {
                    if (!x.PhotoUrl.IsNullOrEmpty())
                    {
                        x.PhotoUrl = $"{documentBaseUrl}/{x.PhotoUrl}";
                    }

                    if (!x.SignatureUrl.IsNullOrEmpty())
                    {
                        x.SignatureUrl = $"{documentBaseUrl}/{x.SignatureUrl}";
                    }
                });

                return ServiceResponse<List<GetMembershipNextOfKings>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all MembershipNextOfKings: {e.Message}");
                return ServiceResponse<List<GetMembershipNextOfKings>>.Return500(e, "Failed to get all MembershipNextOfKings");
            }
        }
    }
}
