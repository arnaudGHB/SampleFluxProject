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
    public class GetThirdPartyBrancheQueryHandler : IRequestHandler<GetThirdPartyBrancheQuery, ServiceResponse<CorrespondingBankBranchDto>>
    {
        private readonly IThirdPartyBrancheRepository _thirdPartyInstitutionRepository; // Repository for accessing Bank data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBankQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetBankQueryHandler.
        /// </summary>
        /// <param name="thirdPartyInstitutionRepository">Repository for Bank data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
 
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetThirdPartyBrancheQueryHandler(
            IThirdPartyBrancheRepository thirdPartyInstitutionRepository,
            IMapper mapper,
            ILogger<GetBankQueryHandler> logger)
        {
            _thirdPartyInstitutionRepository = thirdPartyInstitutionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetThirdPartyBrancheQuery to retrieve a specific ThirdPartyBranche.
        /// </summary>
        /// <param name="request">The GetBankQuery containing ThirdPartyBranche ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CorrespondingBankBranchDto>> Handle(GetThirdPartyBrancheQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Bank entity with the specified ID from the repository
                var existingBank = await _thirdPartyInstitutionRepository.FindAsync(request.Id);
                if (existingBank != null)
                {
                    // Map the Bank entity to BankDto and return it with a success response
                    var BankDto = _mapper.Map<CorrespondingBankBranchDto>(existingBank);
                    return ServiceResponse<CorrespondingBankBranchDto>.ReturnResultWith200(BankDto);
                }
                else
                {
                    // If the ThirdPartyBranche entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("ThirdPartyBrancheDto not found.");
                    return ServiceResponse<CorrespondingBankBranchDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Bank: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CorrespondingBankBranchDto>.Return500(e);
            }
        }
    }

}
