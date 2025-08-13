using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using System.Linq;
using CBS.TransactionManagement.otherCashIn.Queries;
using CBS.TransactionManagement.Repository.OtherCashIn;

namespace CBS.TransactionManagement.otherCashIn.Handlers
{
    /// <summary>
    /// Handles the retrieval of all OtherTransaction based on the GetAllOtherTransactionQuery.
    /// </summary>
    public class GetAllOtherTransactionQueryHandler : IRequestHandler<GetAllOtherTransactionQuery, ServiceResponse<List<CashDepositParameterDto>>>
    {
        private readonly IOtherTransactionRepository _OtherTransactionRepository; // Repository for accessing OtherTransactions data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllOtherTransactionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOtherTransactionQueryHandler.
        /// </summary>
        /// <param name="OtherTransactionRepository">Repository for OtherTransactions data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllOtherTransactionQueryHandler(
            IOtherTransactionRepository OtherTransactionRepository,
            IMapper mapper, ILogger<GetAllOtherTransactionQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _OtherTransactionRepository = OtherTransactionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllOtherTransactionQuery to retrieve all OtherTransactions.
        /// </summary>
        /// <param name="request">The GetAllOtherTransactionQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CashDepositParameterDto>>> Handle(GetAllOtherTransactionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OtherTransactions entities from the repository
                var entities = await _OtherTransactionRepository.All.Where(x=>!x.IsDeleted).Include(a => a.Teller).ToListAsync();
                return ServiceResponse<List<CashDepositParameterDto>>.ReturnResultWith200(_mapper.Map<List<CashDepositParameterDto>>(entities));
            }
            catch (Exception e)
            {

                Console.WriteLine("error");
                Console.WriteLine(e);
                Console.WriteLine(e.InnerException);
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all OtherTransactions: {e.Message}");
                return ServiceResponse<List<CashDepositParameterDto>>.Return500(e, "Failed to get all OtherTransactions");
            }
        }
    }
}
