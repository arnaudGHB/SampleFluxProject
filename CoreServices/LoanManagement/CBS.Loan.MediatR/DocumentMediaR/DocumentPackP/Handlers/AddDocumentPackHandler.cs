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
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddDocumentPackHandler : IRequestHandler<AddDocumentPackCommand, ServiceResponse<DocumentPackDto>>
    {
        private readonly IDocumentPackRepository _DocumentPackRepository; // Repository for accessing DocumentPack data.
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
        public AddDocumentPackHandler(
            IDocumentPackRepository DocumentPackRepository,
            IDocumentJoinRepository DocumentJoinRepository,
            IMapper mapper,
            ILogger<AddDocumentPackHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _DocumentPackRepository = DocumentPackRepository;
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
        public async Task<ServiceResponse<DocumentPackDto>> Handle(AddDocumentPackCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a DocumentPack with the same name already exists (case-insensitive)
                //var existingDocumentPack = await _DocumentPackRepository.FindBy(c => c.Id == request.Id).FirstOrDefaultAsync();

                // If a DocumentPack with the same name already exists, return a conflict response
                //if (existingDocumentPack != null)
                //{
                //    var errorMessage = $"DocumentPack {request.Id} already exists.";
                //    _logger.LogError(errorMessage);
                //    return ServiceResponse<DocumentPackDto>.Return409(errorMessage);
                //}


                // Map the AddDocumentPackCommand to a DocumentPack entity
                var DocumentPackEntity = _mapper.Map<DocumentPack>(request);
                // Convert UTC to local time and set it in the entity
                DocumentPackEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                DocumentPackEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new DocumentPack entity to the repository
                _DocumentPackRepository.Add(DocumentPackEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<DocumentPackDto>.Return500();
                }

                // Add to the join

                foreach (var documentId in request.DocumentIds)
                {
                    DocumentJoin DocumentJoinEntity = new DocumentJoin();
                    DocumentJoinEntity.Id = BaseUtilities.GenerateUniqueNumber();
                    DocumentJoinEntity.DocumentId = documentId;
                    DocumentJoinEntity.DocumentPackId = DocumentPackEntity.Id;
                    _DocumentJoinRepository.Add(DocumentJoinEntity);
                }

                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<DocumentPackDto>.Return500();
                }

                // Map the DocumentPack entity to DocumentPackDto and return it with a success response
                var DocumentPackDto = _mapper.Map<DocumentPackDto>(DocumentPackEntity);
                return ServiceResponse<DocumentPackDto>.ReturnResultWith200(DocumentPackDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving DocumentPack: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentPackDto>.Return500(e);
            }
        }
    }

}
