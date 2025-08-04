using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository.OtherCashIn;
using CBS.TransactionManagement.otherCashIn.Queries;
using CBS.TransactionManagement.Data.Dto.OtherCashInP;

namespace CBS.TransactionManagement.otherCashIn.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific OtherTransaction based on its unique identifier.
    /// </summary>
    public class GetOtherTransactionQueryHandler : IRequestHandler<GetOtherTransactionQuery, ServiceResponse<OtherTransactionDto>>
    {
        private readonly IOtherTransactionRepository _OtherTransactionRepository; // Repository for accessing OtherTransaction data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetOtherTransactionQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetOtherTransactionQueryHandler.
        /// </summary>
        /// <param name="OtherTransactionRepository">Repository for OtherTransaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetOtherTransactionQueryHandler(
            IOtherTransactionRepository OtherTransactionRepository,
            IMapper mapper,
            ILogger<GetOtherTransactionQueryHandler> logger)
        {
            _OtherTransactionRepository = OtherTransactionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetOtherTransactionQuery to retrieve a specific OtherTransaction.
        /// </summary>
        /// <param name="request">The GetOtherTransactionQuery containing OtherTransaction ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
       public async Task<ServiceResponse<OtherTransactionDto>> Handle(GetOtherTransactionQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                //request.Id = "017085183837349";
                // Retrieve the OtherTransaction entity with the specified ID from the repository
                var entity = await _OtherTransactionRepository.FindBy(x=>x.Id==request.Id).Include(x=>x.Teller).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the OtherTransaction entity to OtherTransactionDto and return it with a success response
                    var OtherTransactionDto = _mapper.Map<OtherTransactionDto>(entity);
                    return ServiceResponse<OtherTransactionDto>.ReturnResultWith200(OtherTransactionDto);
                }
                else
                {
                    // If the OtherTransaction entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("OtherTransaction not found.");
                    return ServiceResponse<OtherTransactionDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting OtherTransaction: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<OtherTransactionDto>.Return500(e);
            }
        }

    }

}
