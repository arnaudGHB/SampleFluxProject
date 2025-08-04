using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all OperationEvent based on the GetAllOperationEventNameQuery.
    /// </summary>
    public class GetDocumentRefQueryHandler : IRequestHandler<GetDocumentRefQuery, ServiceResponse<DocumentRef>>
    {
        private readonly IDocumentTypeRepository _documentTypeRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDocumentRefQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDocumentRefQueryHandler(
            IDocumentTypeRepository documentTypeRepository,
            IMapper mapper, ILogger<GetDocumentRefQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _documentTypeRepository = documentTypeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllDocumentTypeQuery to retrieve all  DocumentType.
        /// </summary>
        /// <param name="request">The GetAllDocumentTypeQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentRef>> Handle(GetDocumentRefQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var model = new DocumentRef();
                // Retrieve all OperationEvent entities from the repository
                var entities = await _documentTypeRepository.All.Where(x=>x.IsDeleted.Equals(false)).ToListAsync();
                
                model.AssetId = entities.Where(x => x.Name.ToLower() == "assets").First().Id;
                model.LiabilityId= entities.Where(x => x.Name.ToLower() == "liability").First().Id;
                model.BalanceSheetId = entities.Where(x => x.Name.ToLower() == "assets").First().DocumentId;
                model.IncomeId = entities.Where(x => x.Name.ToLower() == "income").First().Id;
                model.ExpenseId = entities.Where(x => x.Name.ToLower() == "expense").First().Id;
                model.ProfitAndLossId = entities.Where(x => x.Name.ToLower() == "expense").First().DocumentId;

                return ServiceResponse<DocumentRef>.ReturnResultWith200(model);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all DocumentTypeDto: {e.Message}");
                return ServiceResponse<DocumentRef>.Return500(e, "Failed to get all DocumentTypeDto");
            }
        }
    }
}