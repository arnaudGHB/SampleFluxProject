using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Data.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Data.Entity;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Bank based on its unique identifier.
    /// </summary>
    public class GetThirdPartyInstitutionQueryHandler : IRequestHandler<GetThirdPartyInstitutionQuery, ServiceResponse<CorrespondingBankDto>>
    {
        private readonly IThirdPartyInstitutionRepository _thirdPartyInstitutionRepository; // Repository for accessing Bank data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBankQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetBankQueryHandler.
        /// </summary>
        /// <param name="thirdPartyInstitutionRepository">Repository for Bank data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
 
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetThirdPartyInstitutionQueryHandler(
            IThirdPartyInstitutionRepository thirdPartyInstitutionRepository,
            IMapper mapper,
            ILogger<GetBankQueryHandler> logger)
        {
            _thirdPartyInstitutionRepository = thirdPartyInstitutionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetThirdPartyInstitutionQuery to retrieve a specific ThirdPartyInstitution.
        /// </summary>
        /// <param name="request">The GetBankQuery containing ThirdPartyInstitution ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CorrespondingBankDto>> Handle(GetThirdPartyInstitutionQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Bank entity with the specified ID from the repository
                var existingBank = await _thirdPartyInstitutionRepository.FindAsync(request.Id);
                if (existingBank != null)
                {
                    // Map the Bank entity to BankDto and return it with a success response
                    var BankDto = _mapper.Map<CorrespondingBankDto>(existingBank);
                    return ServiceResponse<CorrespondingBankDto>.ReturnResultWith200(BankDto);
                }
                else
                {
                    // If the ThirdPartyInstitution entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("ThirdPartyInstitutionDto not found.");
                    return ServiceResponse<CorrespondingBankDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Bank: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CorrespondingBankDto>.Return500(e);
            }
        }
    }

}
