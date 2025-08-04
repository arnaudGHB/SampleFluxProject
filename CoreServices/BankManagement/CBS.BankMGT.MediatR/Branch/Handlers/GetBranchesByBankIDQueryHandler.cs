using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Data.Dto;
using Microsoft.EntityFrameworkCore;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Branch based on its unique identifier.
    /// </summary>
    public class GetBranchesByBankIDQueryHandler : IRequestHandler<GetBranchesByBankIDQuery, ServiceResponse<List<BranchDto>>>
    {
        private readonly IBranchRepository _BranchRepository; // Repository for accessing Branch data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBranchQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetBranchQueryHandler.
        /// </summary>
        /// <param name="BranchRepository">Repository for Branch data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetBranchesByBankIDQueryHandler(
            IBranchRepository BranchRepository,
            IMapper mapper,
            ILogger<GetBranchQueryHandler> logger)
        {
            _BranchRepository = BranchRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetBranchQuery to retrieve a specific Branch.
        /// </summary>
        /// <param name="request">The GetBranchQuery containing Branch ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BranchDto>>> Handle(GetBranchesByBankIDQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Branch entity with the specified ID from the repository
                var existingBranch = await _BranchRepository.AllIncluding(c => c.Towns, x => x.Bank).FirstOrDefaultAsync(c => c.BankId == request.BankID);
                if (existingBranch != null)
                {
                    // Map the Branch entity to BranchDto and return it with a success response
                    var BranchDto = _mapper.Map<List<BranchDto>>(existingBranch);
                    return ServiceResponse<List<BranchDto>>.ReturnResultWith200(BranchDto);
                }
                else
                {
                    // If the Branch entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Branch not found.");
                    return ServiceResponse<List<BranchDto>>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Branch: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<BranchDto>>.Return500(e, errorMessage);
            }
        }
    }

}
