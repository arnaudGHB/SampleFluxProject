using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using FluentValidation.Validators;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a OperationEventName based on UpdateOperationEventNameCommand.
    /// </summary>
    public class UpdateDocumentReferenceCodeCommandHandler : IRequestHandler<UpdateDocumentReferenceCodeCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentReferenceCodeRepository _documentReferenceCodeRepository;
              private readonly ILogger<AddDocumentReferenceCodeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IChartOfAccountRepository? _chartOfAccountRepository;
        private readonly ICorrespondingMappingRepository? _correspondingMappingRepository;
        private readonly ICorrespondingMappingExceptionRepository? _correspondingMappingExceptionRepository;

        /// <summary>
        /// Constructor for initializing the UpdateOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateDocumentReferenceCodeCommandHandler(

            ILogger<AddDocumentReferenceCodeCommandHandler> logger,
            IMapper mapper,
            IDocumentReferenceCodeRepository? documentReferenceCodeRepository,

                        IChartOfAccountRepository? chartOfAccountRepository,
   
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IDocumentRepository? documentRepository, IDocumentTypeRepository? documentTypeRepository, ICorrespondingMappingRepository? correspondingMappingRepository, ICorrespondingMappingExceptionRepository? correspondingMappingExceptionRepository)
        {
            _documentReferenceCodeRepository = documentReferenceCodeRepository;
            _documentRepository = documentRepository;
            _documentTypeRepository = documentTypeRepository;
            _logger = logger;
            _mapper = mapper;
            _correspondingMappingRepository = correspondingMappingRepository;
            _correspondingMappingExceptionRepository = correspondingMappingExceptionRepository;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _chartOfAccountRepository = chartOfAccountRepository;
        }

        public async Task<ServiceResponse<bool>> Handle(UpdateDocumentReferenceCodeCommand request, CancellationToken cancellationToken)
        {

            List<CorrespondingMapping> CorrespondingMappings = new List<CorrespondingMapping>();
            List<CorrespondingMappingException> CorrespondingMappingExceptions = new List<CorrespondingMappingException>();
            DocumentReferenceCode documentReferenceCode = new DocumentReferenceCode();
            string errorMessage = string.Empty;
            //check  if document  exist if not throw exception 
            //check  if documentType  exist if not throw exception 
            try
            {
                var existingDocument = await _documentRepository.FindAsync(request.Document);
                if (existingDocument == null)
                {
                    errorMessage = $"There is no document create with Id: {request.Document}. ";

                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "CREATE",
                    request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                var existingDocumentType = await _documentTypeRepository.FindAsync(request.DocumentType);
                if (existingDocumentType == null)
                {
                    errorMessage = $"There is no document create with Id: {request.Document}. ";

                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "CREATE",
                    request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                var existingCorrespondingMapping  =   _correspondingMappingRepository.FindBy(x=>x.DocumentReferenceCodeId.Equals(request.Id));
                if (existingCorrespondingMapping.Any())
                {
                    var existingCorrespondingExceptionMapping = _correspondingMappingExceptionRepository.FindBy(x => x.DocumentReferenceCodeId.Equals(request.Id));

                    if (existingCorrespondingExceptionMapping.Any())
                    {
                        _correspondingMappingExceptionRepository.RemoveRange(existingCorrespondingExceptionMapping.ToList());
                    }
                    _correspondingMappingRepository.RemoveRange(existingCorrespondingMapping.ToList());
                }
                documentReferenceCode = new DocumentReferenceCode
                {
                    BankId = _userInfoToken.BankId,
                    BranchId= _userInfoToken.BranchId,
                    HasException = false,
                    DocumentId = request.Document,
                    DocumentTypeId = request.DocumentType,
                    ReferenceCode = request.ReferenceCode,
                    Description = request.Description,
                    Id = request.Id

                };
                if (!request.GrossCorrespondingAccount.Contains("NONE"))
                {
                    int ii = 0;
                    foreach (var itemc in request.GrossCorrespondingAccount)
                    {
                        var accountc = await _chartOfAccountRepository.FindAsync(itemc);
                        var CorrespondingMappingx = new CorrespondingMapping
                        {

                            DocumentReferenceCodeId = documentReferenceCode.Id,
                            AccountNumber = accountc.AccountNumber,
                            IsActive = (accountc.LabelEn != "NOT_FOUND"),
                            Cartegory = BalanceSheetCartegory.GROSS,
                            ChartOfAccountId = accountc.Id,
                            Id =  BaseUtilities.GenerateInsuranceUniqueNumber(18, existingDocumentType.Name.Substring(0, 4) + "GR" + (++ii).ToString() + documentReferenceCode.ReferenceCode),


                        };


                        CorrespondingMappings.Add(CorrespondingMappingx);
                    }
                }
                if (!request.GrossCorrespondingExceptionAccount.Contains("NONE"))
                {
                    documentReferenceCode.HasException = true;

                    foreach (var itemGrossException in request.GrossCorrespondingExceptionAccount)
                    {
                        var accountc = await _chartOfAccountRepository.FindAsync(itemGrossException);

                        int ii = 0;
                        var CorrespondingMappingEx = new CorrespondingMappingException
                        {

                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(18, existingDocumentType.Name.Substring(0, 4) + "GR" + (++ii).ToString() + documentReferenceCode.ReferenceCode),
                            DocumentReferenceCodeId = documentReferenceCode.Id,
                            Category = BalanceSheetCartegory.GROSS,
                            AccountNumber = accountc.AccountNumber,

                            ChartOfAccountId = accountc.Id

                        };
                        CorrespondingMappingExceptions.Add(CorrespondingMappingEx);
                    }
                }
                if (!request.ProvCorrespondingExceptionAccount.Contains("NONE"))
                {
                    foreach (var itemc in request.ProvCorrespondingAccount)
                    {
                        int ii = 0;
                        var accountc = await _chartOfAccountRepository.FindAsync(itemc);
                        var CorrespondingMappingx = new CorrespondingMapping
                        {

                            DocumentReferenceCodeId = documentReferenceCode.Id,
                            AccountNumber = accountc.AccountNumber,
                            IsActive = (accountc.LabelEn != "NOT_FOUND"),
                            Cartegory = BalanceSheetCartegory.PROVISION,
                            ChartOfAccountId = accountc.Id,
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(18, existingDocumentType.Name.Substring(0, 4) + "GR" + (++ii).ToString() + documentReferenceCode.ReferenceCode),

                        };


                        CorrespondingMappings.Add(CorrespondingMappingx);
                    }
                }
                if (!request.ProvCorrespondingExceptionAccount.Contains("NONE"))
                {
                    documentReferenceCode.HasException = true;

                    foreach (var itemGrossException in request.ProvCorrespondingExceptionAccount)
                    {
                        var accountc = await _chartOfAccountRepository.FindAsync(itemGrossException);
                        int ii = 0;
                        var CorrespondingMappingEx = new CorrespondingMappingException
                        {

                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(18, existingDocumentType.Name.Substring(0, 4) + "GR" + (++ii).ToString() + documentReferenceCode.ReferenceCode),
                            DocumentReferenceCodeId = documentReferenceCode.Id,
                            Category = BalanceSheetCartegory.PROVISION,
                            AccountNumber = accountc.AccountNumber,

                            ChartOfAccountId = accountc.Id

                        };
                        CorrespondingMappingExceptions.Add(CorrespondingMappingEx);
                    }
                }
                _documentReferenceCodeRepository.Update(documentReferenceCode);
                _correspondingMappingRepository.AddRange(CorrespondingMappings);
                if (CorrespondingMappingExceptions.Count > 0)
                {
                    _correspondingMappingExceptionRepository.AddRange(CorrespondingMappingExceptions);
                }
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true, $"Reference code:{request.ReferenceCode} has been created successfully for report:{existingDocument.Name}");
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