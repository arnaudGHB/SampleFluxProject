using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.WriteOffLoanP;
using CBS.NLoan.Data.Entity.WriteOffLoanP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.WriteOffLoanMediaR.Commands;
using CBS.NLoan.Repository.WriteOffLoanP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.WriteOffLoanMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateWriteOffLoanHandler : IRequestHandler<UpdateWriteOffLoanCommand, ServiceResponse<WriteOffLoanDto>>
    {
        private readonly IWriteOffLoanRepository _WriteOffLoanRepository; // Repository for accessing WriteOffLoan data.
        private readonly ILogger<UpdateWriteOffLoanHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateWriteOffLoanCommandHandler.
        /// </summary>
        /// <param name="WriteOffLoanRepository">Repository for WriteOffLoan data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateWriteOffLoanHandler(
            IWriteOffLoanRepository WriteOffLoanRepository,
            ILogger<UpdateWriteOffLoanHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _WriteOffLoanRepository = WriteOffLoanRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateWriteOffLoanCommand to update a WriteOffLoan.
        /// </summary>
        /// <param name="request">The UpdateWriteOffLoanCommand containing updated WriteOffLoan data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<WriteOffLoanDto>> Handle(UpdateWriteOffLoanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the WriteOffLoan entity to be updated from the repository
                var existingWriteOffLoan = await _WriteOffLoanRepository.FindAsync(request.Id);

                // Check if the WriteOffLoan entity exists
                if (existingWriteOffLoan != null)
                {
                    // Update WriteOffLoan entity properties with values from the request
                    var WriteOffLoanToUpdate = _mapper.Map<WriteOffLoan>(request);
                    // Use the repository to update the existing WriteOffLoan entity
                    _WriteOffLoanRepository.Update(WriteOffLoanToUpdate);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<WriteOffLoanDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<WriteOffLoanDto>.ReturnResultWith200(_mapper.Map<WriteOffLoanDto>(existingWriteOffLoan));
                    _logger.LogInformation($"WriteOffLoan {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the WriteOffLoan entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<WriteOffLoanDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating WriteOffLoan: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<WriteOffLoanDto>.Return500(e);
            }
        }
    }

}
