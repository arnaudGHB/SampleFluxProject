using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Data.Entity.DocumentP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Commands;
using CBS.NLoan.Repository.DocumentP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateDocumentPackCommandHandler : IRequestHandler<UpdateDocumentPackCommand, ServiceResponse<DocumentPackDto>>
    {
        private readonly IDocumentPackRepository _DocumentPackRepository; // Repository for accessing DocumentPack data.
        private readonly ILogger<UpdateDocumentPackCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateDocumentPackCommandHandler.
        /// </summary>
        /// <param name="DocumentPackRepository">Repository for DocumentPack data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateDocumentPackCommandHandler(
            IDocumentPackRepository DocumentPackRepository,
            ILogger<UpdateDocumentPackCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _DocumentPackRepository = DocumentPackRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateDocumentPackCommand to update a DocumentPack.
        /// </summary>
        /// <param name="request">The UpdateDocumentPackCommand containing updated DocumentPack data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentPackDto>> Handle(UpdateDocumentPackCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the DocumentPack entity to be updated from the repository
                var existingDocumentPack = await _DocumentPackRepository.FindAsync(request.Id);

                // Check if the DocumentPack entity exists
                if (existingDocumentPack != null)
                {
                    // Update DocumentPack entity properties with values from the request
                    var documentPackToUpdate = _mapper.Map<DocumentPack>(request);
                    // Use the repository to update the existing DocumentPack entity
                    _DocumentPackRepository.Update(documentPackToUpdate);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<DocumentPackDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<DocumentPackDto>.ReturnResultWith200(_mapper.Map<DocumentPackDto>(existingDocumentPack));
                    _logger.LogInformation($"DocumentPack {request.Name} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the DocumentPack entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Name} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<DocumentPackDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating DocumentPack: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentPackDto>.Return500(e);
            }
        }
    }

}
