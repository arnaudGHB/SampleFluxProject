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
    /// Handles the request to retrieve a specific Bank based on its unique identifier.
    /// </summary>
    public class GetBankQueryHandler : IRequestHandler<GetBankQuery, ServiceResponse<BankDto>>
    {
        private readonly IBankRepository _BankRepository; // Repository for accessing Bank data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBankQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetBankQueryHandler.
        /// </summary>
        /// <param name="BankRepository">Repository for Bank data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetBankQueryHandler(
            IBankRepository BankRepository,
            IMapper mapper,
            ILogger<GetBankQueryHandler> logger)
        {
            _BankRepository = BankRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetBankQuery to retrieve a specific Bank.
        /// </summary>
        /// <param name="request">The GetBankQuery containing Bank ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BankDto>> Handle(GetBankQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Bank entity with the specified ID from the repository
                var existingBank = await _BankRepository.AllIncluding(c => c.Branches, x => x.Organization, y => y.Towns, s => s.Subdivisions).FirstOrDefaultAsync(c => c.Id == request.Id);
                if (existingBank != null)
                {
                    // Map the Bank entity to BankDto and return it with a success response
                    var BankDto = _mapper.Map<BankDto>(existingBank);
                    return ServiceResponse<BankDto>.ReturnResultWith200(BankDto);
                }
                else
                {
                    // If the Bank entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Bank not found.");
                    return ServiceResponse<BankDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Bank: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<BankDto>.Return500(e);
            }
        }
    }

}
