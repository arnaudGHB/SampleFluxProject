using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using FluentValidation.Validators;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a OperationEventName based on UpdateOperationEventNameCommand.
    /// </summary>
    public class AddFinancialReportConfigurationCommandHandler : IRequestHandler<AddFinancialReportConfigurationCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentReferenceCodeRepository _documentReferenceCodeRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly ICorrespondingMappingExceptionRepository _correspondingMappingExceptionRepository;
        private readonly ICorrespondingMappingRepository _correspondingMappingRepository;
        private readonly ILogger<AddFinancialReportConfigurationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IMediator _mediator;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly ICashMovementTrackingConfigurationRepository? _chartOfAccountRepository;

        /// <summary>
        /// Constructor for initializing the UpdateOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public AddFinancialReportConfigurationCommandHandler(

            ILogger<AddFinancialReportConfigurationCommandHandler> logger,
            IMapper mapper,
            IMediator madiator,
            IDocumentReferenceCodeRepository? documentReferenceCodeRepository,
            IDocumentRepository? documentRepository,
                        ICashMovementTrackingConfigurationRepository? chartOfAccountRepository,
            ICorrespondingMappingExceptionRepository? correspondingMappingExceptionRepository,
            IDocumentTypeRepository? documentTypeRepository,
            ICorrespondingMappingRepository? correspondingMappingRepository,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _documentReferenceCodeRepository = documentReferenceCodeRepository;
            _documentTypeRepository = documentTypeRepository;
            _documentRepository = documentRepository;
            _correspondingMappingExceptionRepository = correspondingMappingExceptionRepository;
            _correspondingMappingRepository = correspondingMappingRepository;
            _logger = logger;
            _mapper = mapper;
            _mediator = madiator;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _chartOfAccountRepository = chartOfAccountRepository;
        }

        public async Task<ServiceResponse<bool>> Handle(AddFinancialReportConfigurationCommand request, CancellationToken cancellationToken)
        {

            List<AddDocumentReferenceCodeCommand> AssetCollection = new List<AddDocumentReferenceCodeCommand>();



            List<AddDocumentReferenceCodeCommand> LiabilityCollection = new List<AddDocumentReferenceCodeCommand>();
            List<AddDocumentReferenceCodeCommand> IncomeCollection = new List<AddDocumentReferenceCodeCommand>();
            List<AddDocumentReferenceCodeCommand> ExpenseCollection = new List<AddDocumentReferenceCodeCommand>();
            List<AddDocumentReferenceCodeCommand> CommandCollection = new List<AddDocumentReferenceCodeCommand>();
            List<CorrespondingMappingException> CorrespondingMappingExceptions = new List<CorrespondingMappingException>();
            DocumentReferenceCode documentReferenceCode = new DocumentReferenceCode();
            string errorMessage = string.Empty;
            //check  if document  exist if not throw exception 
            //check  if documentType  exist if not throw exception 
            try
            {
                foreach (var item in request.FinancialReportConfigurations.BalanceSheetAssets)
                {
                    var Model = new AddDocumentReferenceCodeCommand
                    {
                        Interface = "UPLOAD",
                        Document = item.DocumentId,
                        DocumentType = item.DocumentTypeId,
                        ReferenceCode = item.Reference,
                        Description = item.HeadingEN,
                        GrossCorrespondingAccount = item.ListGrossDr.Split("/").ToList(),
                        ProvCorrespondingAccount = item.ListProvCr.Split("/").ToList(),
                    };

                    AssetCollection.Add(Model);
                }
                foreach (var item in AssetCollection)
                {

                    var resultStatus = await _mediator.Send(item);
                    if (resultStatus.Data == false)
                    {

                    }
                }
                foreach (var item in request.FinancialReportConfigurations.BalanceSheetLiabilities)
                {
                    var Model = new AddDocumentReferenceCodeCommand
                    {
                        Interface = "UPLOAD",
                        Document = item.DocumentId,
                        DocumentType = item.DocumentTypeId,
                        ReferenceCode = item.Reference,
                        Description = item.HeadingEn,
                        GrossCorrespondingAccount = item.selectedList.Split("/").ToList(),
                        GrossCorrespondingExceptionAccount = (item.selectedException.Count() > 0) ? item.selectedException.Split("/").ToList() : null  
                    };
                    LiabilityCollection.Add(Model);
                }
                foreach (var item in LiabilityCollection)
                {

                    var resultStatus = await _mediator.Send(item);
                    if (resultStatus.Data == false)
                    {

                    }
                }
                foreach (var item in request.FinancialReportConfigurations.Incomes)
                {
                    var Model = new AddDocumentReferenceCodeCommand
                    {
                        Interface = "UPLOAD",
                        Document = item.DocumentId,
                        DocumentType = item.DocumentTypeId,
                        ReferenceCode = item.Reference,
                        Description = item.HeadingEN,
                        GrossCorrespondingAccount = item.AccountList.Split("/").ToList(),
                        GrossCorrespondingExceptionAccount = (item.AccountExceptionList.Count() > 0) ? item.AccountExceptionList.Split("/").ToList() : null
                    };
                    IncomeCollection.Add(Model);
                }
                foreach (var item in IncomeCollection)
                {

                    var resultStatus = await _mediator.Send(item);
                    if (resultStatus.Data == false)
                    {

                    }
                }
                foreach (var item in request.FinancialReportConfigurations.Expenses)
                {
                    var Model = new AddDocumentReferenceCodeCommand
                    {
                        Interface = "UPLOAD",
                        Document = item.DocumentId,
                        DocumentType = item.DocumentTypeId,
                        ReferenceCode = item.Reference,
                        Description = item.HeadingEN,
                        GrossCorrespondingAccount = item.AccountList.Split("/").ToList(),
                        GrossCorrespondingExceptionAccount = (item.AccountExceptionList.Count() > 0) ? item.AccountExceptionList.Split("/").ToList() : null
                    };
                    ExpenseCollection.Add(Model);
                }
            
                foreach (var item in ExpenseCollection)
                {
                  
                  var resultStatus=  await _mediator.Send(item);
                    if (resultStatus.Data==false)
                    {

                    }
                }
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true, $"Reference code:{request.FinancialReportConfigurations} has been created successfully for report:{request.FinancialReportConfigurations}");
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                errorMessage = $"Error occurred while saving DocumentReferenceCOde: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddEntryTempDataCommand",
                  request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }


    }


}