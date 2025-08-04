using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountType based on its unique identifier.
    /// </summary>
    public class GetOperationEventAttributeByOperationAccountTypeIdQueryHandler : IRequestHandler<GetOperationEventAttributeByOperationAccountTypeIdQuery, ServiceResponse<List<OperationEventAttributesDto>>>
    {
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IOperationEventRepository _operationEventRepository; // Repository for accessing AccountType data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<GetOperationEventAttributeByOperationAccountTypeIdQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IProductAccountingBookRepository _productAccountingBookRepository;
        /// <summary>
        /// Constructor for initializing the GetAccountTypeQueryHandler.
        /// </summary>
        /// <param name="AccountTypeRepository">Repository for AccountType data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="userInfoToken">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetOperationEventAttributeByOperationAccountTypeIdQueryHandler(
            IProductAccountingBookRepository productAccountingBookRepository,
            IOperationEventRepository operationEventRepository,
             IOperationEventAttributeRepository operationEventAttributeRepository,
            IMapper mapper,
            ILogger<GetOperationEventAttributeByOperationAccountTypeIdQueryHandler> logger,UserInfoToken userInfoToken)
        {
            _operationEventRepository = operationEventRepository;
            _operationEventAttributeRepository= operationEventAttributeRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _productAccountingBookRepository = productAccountingBookRepository;
        }

        /// <summary>
        /// Handles the GetAccountTypeQuery to retrieve a specific AccountType.
        /// </summary>
        /// <param name="request">The GetAccountTypeQuery containing AccountType ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<OperationEventAttributesDto>>> Handle(GetOperationEventAttributeByOperationAccountTypeIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _productAccountingBookRepository
                    .FindBy(x => x.ProductAccountingBookId == request.OperationAccountTypeId)
                    .FirstOrDefault();

                if (entity == null)
                {
                    string errorMessage = "AccountType not found.";
                    _logger.LogError(errorMessage);
                    LogAndAuditError(request, errorMessage, LogLevelInfo.Error, 404);
                    return ServiceResponse<List<OperationEventAttributesDto>>.Return404(errorMessage);
                }

                if (entity.IsDeleted)
                {
                    string errorMessage = "AccountType has been deleted.";
                    _logger.LogError(errorMessage);
                    LogAndAuditError(request, errorMessage, LogLevelInfo.Information, 409);
                    return ServiceResponse<List<OperationEventAttributesDto>>.Return404(errorMessage);
                }

                var eventOperations = _operationEventRepository
                    .FindBy(x => x.AccountTypeId == entity.AccountTypeId)
                    .ToList();

                if (eventOperations.Count == 0)
                {
                    string errorMessage = "No operationEventAttributeId has been found.";
                    _logger.LogError(errorMessage);
                    LogAndAuditError(request, errorMessage, LogLevelInfo.Information, 409);
                    return ServiceResponse<List<OperationEventAttributesDto>>.Return404(errorMessage);
                }
                var eventOperation = eventOperations.FirstOrDefault();
                var listData = new List<OperationEventAttributesDto>();
                var listDb= new List<OperationEventAttributes>();
                foreach (var item in eventOperations)
                {
                    var modelist = _operationEventAttributeRepository.FindBy(x => x.OperationEventId == item.Id).ToList();
                    listDb.AddRange(modelist);
                }


                foreach (var item in listDb)
                {

                    if (item != null)
                    {
                        listData.Add(new OperationEventAttributesDto
                        {
                            Name = item.Name,
                            OperationEventId = item.OperationEventId,
                            Id = item.Id,
                            OperationEventAttributeCode = item.OperationEventAttributeCode
                        });
                    }
                }

                string successMessage = $"Gotten OperationEventAttributesId for productId {request.OperationAccountTypeId} successfully.";
                LogAuditSuccess(request, successMessage);

                return ServiceResponse<List<OperationEventAttributesDto>>.ReturnResultWith200(listData);
            }
            catch (Exception e)
            {
                string errorMessage = $"Error occurred while getting AccountType: {e.Message}";
                LogAndAuditError(request, errorMessage, LogLevelInfo.Error, 500);
                _logger.LogError(errorMessage);
                return ServiceResponse<List<OperationEventAttributesDto>>.Return500(errorMessage);
            }
        }

        private void LogAndAuditError(GetOperationEventAttributeByOperationAccountTypeIdQuery request, string errorMessage, LogLevelInfo logLevel, int statusCode)
        {
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountTypeQuery",
                JsonConvert.SerializeObject(request), errorMessage, logLevel.ToString(), statusCode,_userInfoToken.Token).Wait();
        }

        private void LogAuditSuccess(GetOperationEventAttributeByOperationAccountTypeIdQuery request, string successMessage)
        {
          
            APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountTypeQuery",
                JsonConvert.SerializeObject(request), successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token).Wait();
        }
    }
}