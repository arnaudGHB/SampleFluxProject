using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Banks based on the GetAllBankQuery.
    /// </summary>
    public class GetAllBankQueryHandler : IRequestHandler<GetAllBankQuery, ServiceResponse<List<BankDto>>>
    {
        private readonly IBankRepository _BankRepository; // Repository for accessing Banks data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBankQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllBankQueryHandler.
        /// </summary>
        /// <param name="BankRepository">Repository for Banks data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllBankQueryHandler(
            IBankRepository BankRepository,
            IMapper mapper, ILogger<GetAllBankQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _BankRepository = BankRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllBankQuery to retrieve all Banks.
        /// </summary>
        /// <param name="request">The GetAllBankQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BankDto>>> Handle(GetAllBankQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Banks entities from the repository
                var entities = await _BankRepository.AllIncluding(c => c.Branches, x=>x.Organization, y => y.Towns).ToListAsync();
                return ServiceResponse<List<BankDto>>.ReturnResultWith200(_mapper.Map<List<BankDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Banks: {e.Message}");
                return ServiceResponse<List<BankDto>>.Return500(e, "Failed to get all Banks");
            }
        }
    }
}
