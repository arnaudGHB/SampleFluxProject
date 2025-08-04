using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Data.Entity.DocumentP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Commands;
using CBS.NLoan.Repository.DocumentP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateDocumentAttachedToLoanHandler : IRequestHandler<UpdateDocumentAttachedToLoanCommand, ServiceResponse<DocumentAttachedToLoanDto>>
    {
        private readonly IDocumentAttachedToLoanRepository _DocumentAttachedToLoanRepository; // Repository for accessing DocumentAttachedToLoan data.
        private readonly ILogger<UpdateDocumentAttachedToLoanHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateDocumentAttachedToLoanCommandHandler.
        /// </summary>
        /// <param name="DocumentAttachedToLoanRepository">Repository for DocumentAttachedToLoan data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateDocumentAttachedToLoanHandler(
            IDocumentAttachedToLoanRepository DocumentAttachedToLoanRepository,
            ILogger<UpdateDocumentAttachedToLoanHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _DocumentAttachedToLoanRepository = DocumentAttachedToLoanRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateDocumentAttachedToLoanCommand to update a DocumentAttachedToLoan.
        /// </summary>
        /// <param name="request">The UpdateDocumentAttachedToLoanCommand containing updated DocumentAttachedToLoan data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentAttachedToLoanDto>> Handle(UpdateDocumentAttachedToLoanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the DocumentAttachedToLoan entity to be updated from the repository
                var existingDocumentAttachedToLoan = await _DocumentAttachedToLoanRepository.FindAsync(request.Id);

                // Check if the DocumentAttachedToLoan entity exists
                if (existingDocumentAttachedToLoan != null)
                {
                    // Update DocumentAttachedToLoan entity properties with values from the request
                    var DocumentAttachedToLoanToUpdate = _mapper.Map<DocumentAttachedToLoan>(request);
                    // Use the repository to update the existing DocumentAttachedToLoan entity
                    _DocumentAttachedToLoanRepository.Update(DocumentAttachedToLoanToUpdate);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<DocumentAttachedToLoanDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<DocumentAttachedToLoanDto>.ReturnResultWith200(_mapper.Map<DocumentAttachedToLoanDto>(existingDocumentAttachedToLoan));
                    _logger.LogInformation($"DocumentAttachedToLoan {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the DocumentAttachedToLoan entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<DocumentAttachedToLoanDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating DocumentAttachedToLoan: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentAttachedToLoanDto>.Return500(e);
            }
        }
    }

}
