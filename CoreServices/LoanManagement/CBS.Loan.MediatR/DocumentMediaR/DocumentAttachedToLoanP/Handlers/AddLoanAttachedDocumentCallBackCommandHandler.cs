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
    public class AddLoanAttachedDocumentCallBackCommandHandler : IRequestHandler<AddLoanAttachedDocumentCallBackCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentAttachedToLoanRepository _DocumentAttachedToLoanRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        public readonly PathHelper _pathHelper;
        private readonly ILogger<AddLoanAttachedDocumentCallBackCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddAddLoanAttachedDocumentCallBackCommandHandler.
        /// </summary>
        /// <param name="DocumentAttachedToLoanRepository">Repository for DocumentAttachedToLoan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanAttachedDocumentCallBackCommandHandler(
            IDocumentAttachedToLoanRepository DocumentAttachedToLoanRepository,
            IMapper mapper,
            ILogger<AddLoanAttachedDocumentCallBackCommandHandler> logger,
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
        /// Handles the AddAddLoanAttachedDocumentCallBackCommand to add a new DocumentAttachedToLoan.
        /// </summary>
        /// <param name="request">The AddAddLoanAttachedDocumentCallBackCommand containing DocumentAttachedToLoan data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(AddLoanAttachedDocumentCallBackCommand request, CancellationToken cancellationToken)
        {
            try
            {
               
                var documentAttachedToLoan = new DocumentAttachedToLoan
                {
                    Id = BaseUtilities.GenerateUniqueNumber(),
                    FilePath = request.UrlPath,
                    LoanApplicationId = request.Id,
                    DocumentId = request.DocumentId,
                    FileExtension = request.Extension,
                    FileName = request.DocumentName,
                    Date = DateTime.Now
                };
                _DocumentAttachedToLoanRepository.Add(documentAttachedToLoan);
                await _uow.SaveAsync();

                return ServiceResponse<bool>.ReturnResultWith200(true, "DocumentAttachedToLoan added successfully");

            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving DocumentAttachedToLoan: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }

}
