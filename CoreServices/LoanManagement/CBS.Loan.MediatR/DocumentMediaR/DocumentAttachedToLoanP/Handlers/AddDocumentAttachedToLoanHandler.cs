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
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddDocumentAttachedToLoanHandler : IRequestHandler<DocumentAttachedToLoanCommand, ServiceResponse<DocumentAttachedToLoanDto>>
    {
        private readonly IDocumentAttachedToLoanRepository _DocumentAttachedToLoanRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        public readonly PathHelper _pathHelper;
        private readonly ILogger<AddDocumentAttachedToLoanHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddDocumentAttachedToLoanCommandHandler.
        /// </summary>
        /// <param name="DocumentAttachedToLoanRepository">Repository for DocumentAttachedToLoan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddDocumentAttachedToLoanHandler(
            IDocumentAttachedToLoanRepository DocumentAttachedToLoanRepository,
            IMapper mapper,
            ILogger<AddDocumentAttachedToLoanHandler> logger,
            IUnitOfWork<LoanContext> uow,
            PathHelper pathHelper = null)
        {
            _DocumentAttachedToLoanRepository = DocumentAttachedToLoanRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the AddDocumentAttachedToLoanCommand to add a new DocumentAttachedToLoan.
        /// </summary>
        /// <param name="request">The AddDocumentAttachedToLoanCommand containing DocumentAttachedToLoan data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentAttachedToLoanDto>> Handle(DocumentAttachedToLoanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string fullPath = string.Empty;
                var filePath = $"{request.RootPath}/{_pathHelper.LoanDocumentPath}";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                if (request.AttachedFiles.Any())
                {
                    var profileFile = request.AttachedFiles[0];
                    var fileExtension = Path.GetExtension(profileFile.FileName);
                    var newfilepath = $"{Guid.NewGuid()}{fileExtension}";
                    fullPath = Path.Combine(filePath, newfilepath);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        profileFile.CopyTo(stream);
                    }

                    var FilePath = $"{request.BaseURL}/Loans/{newfilepath}";

                    var documentAttachedToLoan = new DocumentAttachedToLoan
                    {
                        Id = BaseUtilities.GenerateUniqueNumber(),
                        FilePath = FilePath,
                        LoanApplicationId = request.LoanApplicationId,
                        DocumentId = request.DocumentId,
                        FileExtension = fileExtension,
                        FileName = profileFile.FileName,
                        Date=DateTime.Now
                    };
                    _DocumentAttachedToLoanRepository.Add(documentAttachedToLoan);
                    await _uow.SaveAsync();
                    var documentAttachedToLoanDto = new DocumentAttachedToLoanDto
                    {
                        Id = documentAttachedToLoan.Id,
                        FilePath = FilePath,
                        LoanApplicationId = request.LoanApplicationId,
                        DocumentId = request.DocumentId,
                        FileExtension = fileExtension,
                        FileName = profileFile.FileName,
                        Date= documentAttachedToLoan.Date,
                    };
                    return ServiceResponse<DocumentAttachedToLoanDto>.ReturnResultWith200(documentAttachedToLoanDto, "DocumentAttachedToLoan added successfully");
                }
                return ServiceResponse<DocumentAttachedToLoanDto>.Return403("No file was submited.");

            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving DocumentAttachedToLoan: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentAttachedToLoanDto>.Return500(errorMessage);
            }
        }
    }

}
