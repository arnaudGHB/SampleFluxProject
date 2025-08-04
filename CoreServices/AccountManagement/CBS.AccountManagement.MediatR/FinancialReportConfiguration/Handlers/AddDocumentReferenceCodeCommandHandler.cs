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
using System.Linq;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a OperationEventName based on UpdateOperationEventNameCommand.
    /// </summary>
    public class AddDocumentReferenceCodeCommandHandler : IRequestHandler<AddDocumentReferenceCodeCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentReferenceCodeRepository _documentReferenceCodeRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly ICorrespondingMappingExceptionRepository _correspondingMappingExceptionRepository;
        private readonly ICorrespondingMappingRepository _correspondingMappingRepository;
        private readonly ILogger<AddDocumentReferenceCodeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IChartOfAccountRepository? _chartOfAccountRepository;
        private readonly IConditionalAccountReferenceFinancialReportRepository _conditionalAccountReferenceFinancialReportRepository;


        /// <summary>
        /// Constructor for initializing the UpdateOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public AddDocumentReferenceCodeCommandHandler(

            ILogger<AddDocumentReferenceCodeCommandHandler> logger,
            IMapper mapper,
            IDocumentReferenceCodeRepository? documentReferenceCodeRepository,
            IDocumentRepository? documentRepository,
                        IChartOfAccountRepository? chartOfAccountRepository,
            ICorrespondingMappingExceptionRepository? correspondingMappingExceptionRepository,
            IDocumentTypeRepository? documentTypeRepository,
            ICorrespondingMappingRepository? correspondingMappingRepository,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IConditionalAccountReferenceFinancialReportRepository? conditionalAccountReferenceFinancialReportRepository)
        {
            _documentReferenceCodeRepository = documentReferenceCodeRepository;
            _documentTypeRepository = documentTypeRepository;
            _documentRepository = documentRepository;
            _correspondingMappingExceptionRepository = correspondingMappingExceptionRepository;
            _correspondingMappingRepository = correspondingMappingRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _chartOfAccountRepository = chartOfAccountRepository;
            _conditionalAccountReferenceFinancialReportRepository = conditionalAccountReferenceFinancialReportRepository;
        }

        public async Task<ServiceResponse<bool>> Handle(AddDocumentReferenceCodeCommand request, CancellationToken cancellationToken)
        {

            List<CorrespondingMapping> CorrespondingMappings = new List<CorrespondingMapping>();
            List<CorrespondingMappingException> CorrespondingMappingExceptions = new List<CorrespondingMappingException>();
            DocumentReferenceCode documentReferenceCode = new DocumentReferenceCode();
            string errorMessage = string.Empty;
            //check  if document  exist if not throw exception 
            //check  if documentType  exist if not throw exception 
            try
            {
                if (request.ReferenceCode== "RP34")
                {

                }
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
                documentReferenceCode = new DocumentReferenceCode
                {
                    HasException = false,
                    DocumentId = request.Document,
                    DocumentTypeId = request.DocumentType,
                    ReferenceCode = request.ReferenceCode,
                    Description = request.Description,
                    Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, existingDocumentType.Name.Substring(0, 6))

                };
                int ii = 0;

                if (request.Interface == "UPLOAD")
                {
                    foreach (var reference in request.GrossCorrespondingAccount)
                    {
                        if (reference.Contains("ou") || reference.Contains("or"))
                        {
                            List<string> modelistv = new List<string>();

                            documentReferenceCode.IsConditional = true;
                            if (reference.Contains("ou"))
                            {
                                modelistv = reference.Split("ou").ToList();

                            }
                            else
                            {
                                modelistv = reference.Split("or").ToList();
                            }
                            foreach (var itemc in modelistv)
                            {

                                if (IsNumber(itemc) == false)
                                {
                                    continue;
                                }

                                var accountc = request.Interface == "UPLOAD" ? GetAccountByReference(RemoveSpecialCharacters(itemc)) : await _chartOfAccountRepository.FindAsync(itemc.Trim());
                                var CorrespondingMappingx = new CorrespondingMapping
                                {

                                    DocumentReferenceCodeId = documentReferenceCode.Id,
                                    AccountNumber = accountc.AccountNumber,
                                    IsActive = (accountc.LabelEn != "NOT_FOUND"),
                                    Cartegory = BalanceSheetCartegory.GROSS,
                                    ChartOfAccountId = accountc.Id,
                                    Id = BaseUtilities.GenerateInsuranceUniqueNumber(18, existingDocumentType.Name.Substring(0, 4) + "GR" + (++ii).ToString() + documentReferenceCode.ReferenceCode),

                                };
                                if (request.GrossCorrespondingExceptionAccount != null)
                                {
                                    documentReferenceCode.HasException = true;
                                    foreach (var item in request.GrossCorrespondingExceptionAccount)
                                    {
                                        var CorrespondingMappingEx = new CorrespondingMappingException
                                        {

                                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(18, existingDocumentType.Name.Substring(0, 4) + "GR" + (++ii).ToString() + documentReferenceCode.ReferenceCode),



                                            DocumentReferenceCodeId = reference,
                                            AccountNumber = accountc.AccountNumber,



                                        };
                                        CorrespondingMappingExceptions.Add(CorrespondingMappingEx);
                                    }
                                }

                                CorrespondingMappings.Add(CorrespondingMappingx);
                            }
                            continue;
                        }
                        var modelist = reference.Split(",");
                        foreach (var itemc in modelist)
                        {

                            if (IsNumber(itemc) == false && request.Interface == "UPLOAD")
                            {
                                continue;
                            }

                            var accountc = request.Interface == "UPLOAD" ? GetAccountByReference(RemoveSpecialCharacters(itemc)) : await _chartOfAccountRepository.FindAsync(itemc.Trim());
                            var CorrespondingMappingx = new CorrespondingMapping
                            {

                                DocumentReferenceCodeId = documentReferenceCode.Id,
                                AccountNumber = accountc.AccountNumber,
                                IsActive = (accountc.LabelEn != "NOT_FOUND"),
                                Cartegory = BalanceSheetCartegory.GROSS,
                                ChartOfAccountId = accountc.Id,
                                Id = BaseUtilities.GenerateInsuranceUniqueNumber(18, existingDocumentType.Name.Substring(0, 4) + "GR" + (++ii).ToString() + documentReferenceCode.ReferenceCode),

                            };
                            if (request.GrossCorrespondingExceptionAccount != null)
                            {
                                documentReferenceCode.HasException = true;
                                foreach (var item in request.GrossCorrespondingExceptionAccount)
                                {
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

                            CorrespondingMappings.Add(CorrespondingMappingx);
                        }
                    }
                    if (request.ProvCorrespondingAccount != null)
                    {
                        foreach (var reference in request.ProvCorrespondingAccount)
                        {
                            var modelist = reference.Split("/");
                            foreach (var itemc in modelist)
                            {

                                if (IsNumber(itemc) == false)
                                {
                                    continue;
                                }

                                var accountc = request.Interface == "UPLOAD" ? GetAccountByReference(RemoveSpecialCharacters(itemc)) : await _chartOfAccountRepository.FindAsync(itemc);
                                var CorrespondingMappingx = new CorrespondingMapping
                                {

                                    DocumentReferenceCodeId = documentReferenceCode.Id,
                                    AccountNumber = accountc.AccountNumber,
                                    IsActive = (accountc.LabelEn != "NOT_FOUND"),
                                    Cartegory = BalanceSheetCartegory.PROVISION,
                                    ChartOfAccountId = accountc.Id,
                                    Id = BaseUtilities.GenerateInsuranceUniqueNumber(18, existingDocumentType.Name.Substring(0, 4) + "GR" + (++ii).ToString() + documentReferenceCode.ReferenceCode),

                                };
                                if (request.ProvCorrespondingExceptionAccount != null)
                                {
                                    documentReferenceCode.HasException = true;
                                    foreach (var item in request.ProvCorrespondingExceptionAccount)
                                    {
                                        var CorrespondingMappingEx = new CorrespondingMappingException
                                        {

                                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(18, existingDocumentType.Name.Substring(0, 4) + "GR" + (++ii).ToString() + documentReferenceCode.ReferenceCode),


                                            AccountNumber = accountc.AccountNumber,
                                            DocumentReferenceCodeId = documentReferenceCode.Id,
                                            Category = BalanceSheetCartegory.PROVISION,
                                            ChartOfAccountId = accountc.Id

                                        };
                                        CorrespondingMappingExceptions.Add(CorrespondingMappingEx);
                                    }
                                }

                                CorrespondingMappings.Add(CorrespondingMappingx);
                            }

                        }
                    }
                }
                else
                {
                    if (!request.GrossCorrespondingAccount.Contains("NONE"))
                    {
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
                                Id = BaseUtilities.GenerateInsuranceUniqueNumber(18, existingDocumentType.Name.Substring(0, 4) + "GR" + (++ii).ToString() + documentReferenceCode.ReferenceCode),

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

                }
                _documentReferenceCodeRepository.Add(documentReferenceCode);
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

        public static bool IsNumber(string input)
        {
            return double.TryParse(input, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out _);
        }
        public string RemoveSpecialCharacters(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }
        /// <summary>
        /// Retrieves a ChartOfAccount by its reference number, or creates a new one if not found.
        /// </summary>
        /// <param name="reference">The account reference number to search for.</param>
        /// <returns>An existing ChartOfAccount if found, otherwise a new ChartOfAccount with default values.</returns>
        private Data.ChartOfAccount GetAccountByReference(string reference)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(reference))
            {
                throw new ArgumentException("Reference cannot be null or empty.", nameof(reference));
            }

            // Trim the reference to remove any leading or trailing whitespace
            string trimmedReference = reference.Trim();

            // Query the repository for an account with the matching reference number
            var result = _chartOfAccountRepository.FindBy(x => x.AccountNumber.Equals(trimmedReference))
                                                  .FirstOrDefault();

            // If an account is found, return it
            if (result != null)
            {
                return result;
            }

            // If no account is found, create and return a new ChartOfAccount with default values
            return new Data.ChartOfAccount
            {
                AccountNumber = trimmedReference,
                LabelEn = "NOT_FOUND",
                // You might want to set other default properties here
                Id = "CA719425748885",
                CreatedDate = DateTime.UtcNow  // Set creation date
            };
        }
    }
}