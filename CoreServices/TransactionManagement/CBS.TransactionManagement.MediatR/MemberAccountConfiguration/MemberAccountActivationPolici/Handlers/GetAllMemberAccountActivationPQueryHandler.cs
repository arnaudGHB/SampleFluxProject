using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository.MemberAccountConfiguration;
using CBS.TransactionManagement.MemberAccountConfiguration.Queries;
using CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Queries;
using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;

namespace CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolicyPolici.Handlers
{
    /// <summary>
    /// Handles the retrieval of all TempAccount based on the GetAllTempAccountQuery.
    /// </summary>
    public class GetAllMemberAccountActivationPolicyQueryHandler : IRequestHandler<GetAllMemberAccountActivationPolicyQuery, ServiceResponse<List<MemberAccountActivationPolicyDto>>>
    {
        private readonly IMemberAccountActivationPolicyRepository _MemberAccountActivationPolicyRepository; // Repository for accessing MemberAccountActivationPolicy data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllMemberAccountActivationPolicyQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllTempAccountQueryHandler.
        /// </summary>
        /// <param name="TempAccountRepository">Repository for MemberAccountActivationPolicy data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllMemberAccountActivationPolicyQueryHandler(
            IMemberAccountActivationPolicyRepository MemberAccountActivationPolicyRepository,
            IMapper mapper, ILogger<GetAllMemberAccountActivationPolicyQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _MemberAccountActivationPolicyRepository = MemberAccountActivationPolicyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllTempAccountQuery to retrieve all MemberAccountActivationPolicy.
        /// </summary>
        /// <param name="request">The GetAllTempAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<MemberAccountActivationPolicyDto>>> Handle(GetAllMemberAccountActivationPolicyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all MemberAccountActivationPolicy entities from the repository
                var entities = await _MemberAccountActivationPolicyRepository.All.Include(a => a.MemberAccountActivations).Where(x=>!x.IsDeleted).ToListAsync();
                return ServiceResponse<List<MemberAccountActivationPolicyDto>>.ReturnResultWith200(_mapper.Map<List<MemberAccountActivationPolicyDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all MemberAccountActivationPolicy: {e.Message}");
                return ServiceResponse<List<MemberAccountActivationPolicyDto>>.Return500(e, "Failed to get all MemberAccountActivationPolicy");
            }
        }

    }
}
