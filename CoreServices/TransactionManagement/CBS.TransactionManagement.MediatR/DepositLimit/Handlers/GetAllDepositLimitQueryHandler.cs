using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using System.Linq;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all DepositLimit based on the GetAllDepositLimitQuery.
    /// </summary>
    public class GetAllDepositLimitQueryHandler : IRequestHandler<GetAllDepositLimitQuery, ServiceResponse<List<CashDepositParameterDto>>>
    {
        private readonly IDepositLimitRepository _DepositLimitRepository; // Repository for accessing DepositLimits data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDepositLimitQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllDepositLimitQueryHandler.
        /// </summary>
        /// <param name="DepositLimitRepository">Repository for DepositLimits data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDepositLimitQueryHandler(
            IDepositLimitRepository DepositLimitRepository,
            IMapper mapper, ILogger<GetAllDepositLimitQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _DepositLimitRepository = DepositLimitRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllDepositLimitQuery to retrieve all DepositLimits.
        /// </summary>
        /// <param name="request">The GetAllDepositLimitQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CashDepositParameterDto>>> Handle(GetAllDepositLimitQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all DepositLimits entities from the repository
                var entities = await _DepositLimitRepository.All.Where(x=>!x.IsDeleted).Include(a => a.Product).ToListAsync();
                return ServiceResponse<List<CashDepositParameterDto>>.ReturnResultWith200(_mapper.Map<List<CashDepositParameterDto>>(entities));
            }
            catch (Exception e)
            {

                Console.WriteLine("error");
                Console.WriteLine(e);
                Console.WriteLine(e.InnerException);
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all DepositLimits: {e.Message}");
                return ServiceResponse<List<CashDepositParameterDto>>.Return500(e, "Failed to get all DepositLimits");
            }
        }
    }
}
