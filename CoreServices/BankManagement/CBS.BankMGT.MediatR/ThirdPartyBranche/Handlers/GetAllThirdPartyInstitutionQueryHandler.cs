using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data.Entity;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all ThirdPartyBranche based on the GetAllThirdPartyInstitutionQuery.
    /// </summary>
    public class GetAllThirdPartyInstitutionQueryHandler : IRequestHandler<GetAllThirdPartyInstitutionQuery, ServiceResponse<List<CorrespondingBankDto>>>
    {
        private readonly IThirdPartyInstitutionRepository _thirdPartyInstitutionRepository; // Repository for accessing Banks data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBankQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllBankQueryHandler.
        /// </summary>
        /// <param name="BankRepository">Repository for Banks data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllThirdPartyInstitutionQueryHandler(
            IThirdPartyInstitutionRepository thirdPartyInstitutionRepository,
            IMapper mapper, ILogger<GetAllBankQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _thirdPartyInstitutionRepository = thirdPartyInstitutionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllBankQuery to retrieve all Banks.
        /// </summary>
        /// <param name="request">The GetAllBankQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CorrespondingBankDto>>> Handle(GetAllThirdPartyInstitutionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Banks entities from the repository
                var entities = await _thirdPartyInstitutionRepository.All.ToListAsync();
                var model = _mapper.Map<List<CorrespondingBankDto>>(entities);
                return ServiceResponse<List<CorrespondingBankDto>>.ReturnResultWith200(model);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Banks: {e.Message}");
                return ServiceResponse<List<CorrespondingBankDto>>.Return500(e, "Failed to get all Banks");
            }
        }
    }
}
