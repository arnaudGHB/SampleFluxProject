using AutoMapper;
using CBS.NLoan.Data.Dto.WriteOffLoanP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.WriteOffLoanMediaR.Queries;
using CBS.NLoan.Repository.WriteOffLoanP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.WriteOffLoanMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetWriteOffLoanHandler : IRequestHandler<GetWriteOffLoanQuery, ServiceResponse<WriteOffLoanDto>>
    {
        private readonly IWriteOffLoanRepository _WriteOffLoanRepository; // Repository for accessing WriteOffLoan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetWriteOffLoanHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetWriteOffLoanQueryHandler.
        /// </summary>
        /// <param name="WriteOffLoanRepository">Repository for WriteOffLoan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetWriteOffLoanHandler(
            IWriteOffLoanRepository WriteOffLoanRepository,
            IMapper mapper,
            ILogger<GetWriteOffLoanHandler> logger)
        {
            _WriteOffLoanRepository = WriteOffLoanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetWriteOffLoanQuery to retrieve a specific WriteOffLoan.
        /// </summary>
        /// <param name="request">The GetWriteOffLoanQuery containing WriteOffLoan ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<WriteOffLoanDto>> Handle(GetWriteOffLoanQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the WriteOffLoan entity with the specified ID from the repository
                var entity = await _WriteOffLoanRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the WriteOffLoan entity to WriteOffLoanDto and return it with a success response
                    var WriteOffLoanDto = _mapper.Map<WriteOffLoanDto>(entity);
                    return ServiceResponse<WriteOffLoanDto>.ReturnResultWith200(WriteOffLoanDto);
                }
                else
                {
                    // If the WriteOffLoan entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("WriteOffLoan not found.");
                    return ServiceResponse<WriteOffLoanDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting WriteOffLoan: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<WriteOffLoanDto>.Return500(e);
            }
        }
    }

}
