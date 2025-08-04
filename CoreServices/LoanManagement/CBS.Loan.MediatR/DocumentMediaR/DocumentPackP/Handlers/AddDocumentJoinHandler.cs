using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Data.Entity.DocumentP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Commands;
using CBS.NLoan.Repository.DocumentP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddDocumentToPackHandler : IRequestHandler<AddDocumentToPackCommand, ServiceResponse<DocumentJoinDto>>
    {
        private readonly IDocumentJoinRepository _DocumentJoinRepository; // Repository for accessing DocumentPack data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddDocumentPackHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddDocumentPackCommandHandler.
        /// </summary>
        /// <param name="DocumentPackRepository">Repository for DocumentPack data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddDocumentToPackHandler(
            IDocumentJoinRepository DocumentJoinRepository,
            IMapper mapper,
            ILogger<AddDocumentPackHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _DocumentJoinRepository = DocumentJoinRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddDocumentPackCommand to add a new DocumentPack.
        /// </summary>
        /// <param name="request">The AddDocumentPackCommand containing DocumentPack data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentJoinDto>> Handle(AddDocumentToPackCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a DocumentPack with the same name already exists (case-insensitive)
                var existingDocumentPack = await _DocumentJoinRepository.FindBy(c => c.DocumentPackId == request.DocumentPackId && c.DocumentId == request.DocumentId).FirstOrDefaultAsync();

                // If a DocumentPack with the same name already exists, return a conflict response
                if (existingDocumentPack != null)
                {
                    var errorMessage = $"DocumentToPack {request.DocumentPackId} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<DocumentJoinDto>.Return409(errorMessage);
                }


                // Map the AddDocumentPackCommand to a DocumentPack entity
                var DocumentPackEntity = _mapper.Map<DocumentJoin>(request);
                // Add id
                DocumentPackEntity.Id = BaseUtilities.GenerateUniqueNumber();
                // Convert UTC to local time and set it in the entity
                DocumentPackEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);

                // Add the new DocumentPack entity to the repository
                _DocumentJoinRepository.Add(DocumentPackEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<DocumentJoinDto>.Return500();
                }

                // Map the DocumentPack entity to DocumentPackDto and return it with a success response
                var DocumentPackDto = _mapper.Map<DocumentJoinDto>(DocumentPackEntity);
                return ServiceResponse<DocumentJoinDto>.ReturnResultWith200(DocumentPackDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving DocumentPack: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentJoinDto>.Return500(e);
            }
        }
    }

}
