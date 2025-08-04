using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanGuarantorMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanGuarantorP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanGuarantorMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanGuarantorHandler : IRequestHandler<GetLoanGuarantorQuery, ServiceResponse<LoanGuarantorDto>>
    {
        private readonly ILoanGuarantorRepository _LoanGuarantorRepository; // Repository for accessing LoanGuarantor data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanGuarantorHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanGuarantorQueryHandler.
        /// </summary>
        /// <param name="LoanGuarantorRepository">Repository for LoanGuarantor data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanGuarantorHandler(
            ILoanGuarantorRepository LoanGuarantorRepository,
            IMapper mapper,
            ILogger<GetLoanGuarantorHandler> logger)
        {
            _LoanGuarantorRepository = LoanGuarantorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanGuarantorQuery to retrieve a specific LoanGuarantor.
        /// </summary>
        /// <param name="request">The GetLoanGuarantorQuery containing LoanGuarantor ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanGuarantorDto>> Handle(GetLoanGuarantorQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanGuarantor entity with the specified ID from the repository
                var entity = await _LoanGuarantorRepository.FindBy(x=>x.Id==request.Id).Include(x=>x.LoanApplication).Include(x=>x.LoanApplication.LoanProduct).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the LoanGuarantor entity to LoanGuarantorDto and return it with a success response
                    var LoanGuarantorDto = _mapper.Map<LoanGuarantorDto>(entity);
                    return ServiceResponse<LoanGuarantorDto>.ReturnResultWith200(LoanGuarantorDto);
                }
                else
                {
                    // If the LoanGuarantor entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanGuarantor not found.");
                    return ServiceResponse<LoanGuarantorDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanGuarantor: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanGuarantorDto>.Return500(e);
            }
        }
    }

}
