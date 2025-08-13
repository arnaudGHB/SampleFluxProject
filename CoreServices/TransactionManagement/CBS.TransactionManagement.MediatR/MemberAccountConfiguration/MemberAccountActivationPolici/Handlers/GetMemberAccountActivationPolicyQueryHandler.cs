using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository.MemberAccountConfiguration;
using CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Queries;
using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;

namespace CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific MemberAccountActivationPolicy based on its unique identifier.
    /// </summary>
    public class GetMemberAccountActivationPolicyQueryHandler : IRequestHandler<GetMemberAccountActivationPolicyQuery, ServiceResponse<MemberAccountActivationPolicyDto>>
    {
        private readonly IMemberAccountActivationPolicyRepository _MemberAccountActivationPolicyRepository; // Repository for accessing MemberAccountActivationPolicy data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetMemberAccountActivationPolicyQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetMemberAccountActivationPolicyQueryHandler.
        /// </summary>
        /// <param name="MemberAccountActivationPolicyRepository">Repository for MemberAccountActivationPolicy data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetMemberAccountActivationPolicyQueryHandler(
            IMemberAccountActivationPolicyRepository MemberAccountActivationPolicyRepository,
            IMapper mapper,
            ILogger<GetMemberAccountActivationPolicyQueryHandler> logger)
        {
            _MemberAccountActivationPolicyRepository = MemberAccountActivationPolicyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetMemberAccountActivationPolicyQuery to retrieve a specific MemberAccountActivationPolicy.
        /// </summary>
        /// <param name="request">The GetMemberAccountActivationPolicyQuery containing MemberAccountActivationPolicy ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<MemberAccountActivationPolicyDto>> Handle(GetMemberAccountActivationPolicyQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the MemberAccountActivationPolicy entity with the specified ID from the repository
                var entity = await _MemberAccountActivationPolicyRepository.FindBy(a => a.Id == request.Id).Include(a => a.MemberAccountActivations).FirstAsync();
                if (entity != null)
                {
                    // Map the MemberAccountActivationPolicy entity to MemberAccountActivationPolicyDto and return it with a success response
                    var MemberAccountActivationPolicyDto = _mapper.Map<MemberAccountActivationPolicyDto>(entity);
                    return ServiceResponse<MemberAccountActivationPolicyDto>.ReturnResultWith200(MemberAccountActivationPolicyDto);
                }
                else
                {
                    // If the MemberAccountActivationPolicy entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("MemberAccountActivationPolicy not found.");
                    return ServiceResponse<MemberAccountActivationPolicyDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting MemberAccountActivationPolicy: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<MemberAccountActivationPolicyDto>.Return500(e);
            }
        }

    }

}
