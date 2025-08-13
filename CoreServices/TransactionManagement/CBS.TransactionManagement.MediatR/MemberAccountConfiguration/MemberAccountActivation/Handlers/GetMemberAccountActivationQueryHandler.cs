using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.MemberAccountConfiguration.Queries;
using CBS.TransactionManagement.Repository.MemberAccountConfiguration;
using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;

namespace CBS.TransactionManagement.MemberAccountConfiguration.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific MemberAccountActivation based on its unique identifier.
    /// </summary>
    public class GetMemberAccountActivationQueryHandler : IRequestHandler<GetMemberAccountActivationQuery, ServiceResponse<MemberAccountActivationDto>>
    {
        private readonly IMemberAccountActivationRepository _MemberAccountActivationRepository; // Repository for accessing MemberAccountActivation data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetMemberAccountActivationQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetMemberAccountActivationQueryHandler.
        /// </summary>
        /// <param name="MemberAccountActivationRepository">Repository for MemberAccountActivation data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetMemberAccountActivationQueryHandler(
            IMemberAccountActivationRepository MemberAccountActivationRepository,
            IMapper mapper,
            ILogger<GetMemberAccountActivationQueryHandler> logger)
        {
            _MemberAccountActivationRepository = MemberAccountActivationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetMemberAccountActivationQuery to retrieve a specific MemberAccountActivation.
        /// </summary>
        /// <param name="request">The GetMemberAccountActivationQuery containing MemberAccountActivation ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<MemberAccountActivationDto>> Handle(GetMemberAccountActivationQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the MemberAccountActivation entity with the specified ID from the repository
                var entity = await _MemberAccountActivationRepository.FindBy(a => a.Id == request.Id).Include(a => a.Fee).FirstAsync();
                if (entity != null)
                {
                    // Map the MemberAccountActivation entity to MemberAccountActivationDto and return it with a success response
                    var MemberAccountActivationDto = _mapper.Map<MemberAccountActivationDto>(entity);
                    return ServiceResponse<MemberAccountActivationDto>.ReturnResultWith200(MemberAccountActivationDto);
                }
                else
                {
                    // If the MemberAccountActivation entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("MemberAccountActivation not found.");
                    return ServiceResponse<MemberAccountActivationDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting MemberAccountActivation: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<MemberAccountActivationDto>.Return500(e);
            }
        }

    }

}
