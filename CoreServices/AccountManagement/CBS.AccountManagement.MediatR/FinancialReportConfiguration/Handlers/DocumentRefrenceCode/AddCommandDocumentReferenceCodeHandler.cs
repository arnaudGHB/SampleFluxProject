using AutoMapper;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Domain;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.FinancialReportConfiguration.Handlers
{


    /// <summary>
    /// Handles the command to update a OperationEventName based on UpdateOperationEventNameCommand.
    /// </summary>
    public class AddCommandDocumentReferenceCodeHandler : IRequestHandler<AddCommandDocumentReferenceCode, ServiceResponse<bool>>
    {
        private readonly IDocumentReferenceCodeRepository _documentReferenceCodeRepository;
        private readonly ILogger<AddCommandDocumentReferenceCodeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;


        /// <summary>
        /// Constructor for initializing the UpdateOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public AddCommandDocumentReferenceCodeHandler(
            IDocumentReferenceCodeRepository documentReferenceCodeRepository,
            ILogger<AddCommandDocumentReferenceCodeHandler> logger,
            IMapper mapper,
                      IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _documentReferenceCodeRepository = documentReferenceCodeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        public async Task<ServiceResponse<bool>> Handle(AddCommandDocumentReferenceCode request, CancellationToken cancellationToken)
        {

            string errorMessage = string.Empty;
            //check  if document  exist if not throw exception 
            //check  if documentType  exist if not throw exception 
            try
            {
                var model = _documentReferenceCodeRepository.FindBy(x => x.ReferenceCode.Equals(request.ReferenceCode) && x.DocumentTypeId.Equals(request.DocumentTypeId) && x.DocumentId.Equals(request.DocumentId));
                if (model == null)
                {

                    errorMessage = $"The referenceCode:{request.ReferenceCode} already exist in the document you are creating.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(),
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                DocumentReferenceCode referenceCode = _mapper.Map<DocumentReferenceCode>(request);
                referenceCode.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "Ref");
                _documentReferenceCodeRepository.Add(referenceCode);
                 await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true, $"Reference code:{request.ReferenceCode} has been created successfully ");
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

    }
}
