using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanCommentryMediaR.Commands;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanCommentryMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanCommentryHandler : IRequestHandler<UpdateLoanCommentryCommand, ServiceResponse<LoanCommentryDto>>
    {
        private readonly ILoanCommentryRepository _LoanCommentryRepository; // Repository for accessing LoanCommentry data.
        private readonly ILogger<UpdateLoanCommentryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLoanCommentryCommandHandler.
        /// </summary>
        /// <param name="LoanCommentryRepository">Repository for LoanCommentry data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanCommentryHandler(
            ILoanCommentryRepository LoanCommentryRepository,
            ILogger<UpdateLoanCommentryHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _LoanCommentryRepository = LoanCommentryRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLoanCommentryCommand to update a LoanCommentry.
        /// </summary>
        /// <param name="request">The UpdateLoanCommentryCommand containing updated LoanCommentry data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanCommentryDto>> Handle(UpdateLoanCommentryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanCommentry entity to be updated from the repository
                var existingLoanCommentry = await _LoanCommentryRepository.FindAsync(request.Id);

                // Check if the LoanCommentry entity exists
                if (existingLoanCommentry != null)
                {
                    // Update LoanCommentry entity properties with values from the request
                    var LoanCommentryToUpdate = _mapper.Map<LoanCommentry>(request);
                    // Use the repository to update the existing LoanCommentry entity
                    _LoanCommentryRepository.Update(LoanCommentryToUpdate);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<LoanCommentryDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanCommentryDto>.ReturnResultWith200(_mapper.Map<LoanCommentryDto>(existingLoanCommentry));
                    _logger.LogInformation($"LoanCommentry {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the LoanCommentry entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanCommentryDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LoanCommentry: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanCommentryDto>.Return500(e);
            }
        }
    }

}
