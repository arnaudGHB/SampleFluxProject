using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using Newtonsoft.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Helper.DataModel;

namespace CBS.AccountManagement.MediatR.Handlers
{
    ///// <summary>
    ///// Handles the command to update a AccountType based on UpdateAccountRubriqueCommand.
    ///// </summary>
    //public class UpdateAccountRubriqueCommandHandler : IRequestHandler<UpdateAccountRubriqueCommand, ServiceResponse<AccountRubricResponseDto>>
    //{
    //    private readonly IAccountTypeRepository _AccountTypeRepository; // Repository for accessing AccountType data.
    //    private readonly ILogger<UpdateAccountRubriqueCommandHandler> _logger; // Logger for logging handler actions and errors.
    //    private readonly IMapper _mapper;  // AutoMapper for object mapping.
    //    private readonly UserInfoToken _userInfoToken;
    //    private readonly IUnitOfWork<POSContext> _uow;
    //    private readonly IAccountRubriqueRepository _accountRubriqueRepository;
    //    /// <summary>
    //    /// Constructor for initializing the UpdateAccountRubriqueCommandHandler.
    //    /// </summary>
    //    /// <param name="AccountTypeRepository">Repository for AccountType data access.</param>
    //    /// <param name="logger">Logger for logging handler actions and errors.</param>
    //    /// <param name="mapper">AutoMapper for object mapping.</param>
    //    public UpdateAccountRubriqueCommandHandler(
    //        UserInfoToken userInfoToken,
    //       IAccountRubriqueRepository AccountRubricRepository,
    //        ILogger<UpdateAccountRubriqueCommandHandler> logger,
    //        IMapper mapper,
           
    //        IUnitOfWork<POSContext> uow = null)
    //    {
    //        _accountRubriqueRepository = AccountRubricRepository;
    //        _logger = logger;
    //        _mapper = mapper;
    //        _userInfoToken = userInfoToken;
    //        _uow = uow;
    //    }

    //    /// <summary>
    //    /// Handles the UpdateAccountRubriqueCommand to update a AccountType.
    //    /// </summary>
    //    /// <param name="request">The UpdateAccountRubriqueCommand containing updated AccountType data.</param>
    //    /// <param name="cancellationToken">A cancellation token.</param>
    //    public async Task<ServiceResponse<AccountRubricResponseDto>> Handle(UpdateAccountRubriqueCommand request, CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //          List<AccountRubrique> accountRubriques = new List<AccountRubrique>();
               
    //                foreach (var item in request.AccountRubriques)
    //                {
    //                    var  existingOperationAccounts = _accountRubriqueRepository.FindBy(f => f.OperationEventAttributeId.Equals(item.OperationEventAttributeId));
    //                    if (existingOperationAccounts!=null)
    //                    {
    //                        AccountRubrique model = existingOperationAccounts.FirstOrDefault();
    //                        model.ChartOfAccountId = item.ChartOfAccountId;
    //                        accountRubriques.Add(model);
    //                    }
                       
    //                }
    //                _accountRubriqueRepository.UpdateRange(accountRubriques);
                    
              
    //            // Retrieve the AccountType entity to be updated from the repository
    //             await _uow.SaveAsync();
    //            var result = _mapper.Map<AccountRubricResponseDto>(accountRubriques);
    //            return ServiceResponse<AccountRubricResponseDto>.ReturnResultWith200(result);
    //        }
    //        catch (Exception e)
    //        {
    //            // Log error and return 500 Internal Server Error response with an error message
    //            string errorMessage = $"Error occurred while updating AccountType: {e.Message}";
    //            LogAndAuditError(request, errorMessage, LogLevelInfo.Information, 500);
    //            return ServiceResponse<AccountRubricResponseDto>.Return500(e);
    //        }

    //    }

    //    private bool GetAccountType(string operationAccountType)
    //    {
    //        return operationAccountType=="LOAN" || operationAccountType == "SAVING";
    //    }

     

    //    private void LogAndAuditError(UpdateAccountRubriqueCommand request, string errorMessage, LogLevelInfo logLevel, int statusCode)
    //    {
    //        _logger.LogError(errorMessage);
    //        APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateAccountRubriqueCommand",
    //            JsonConvert.SerializeObject(request), errorMessage, logLevel.ToString(), statusCode, _userInfoToken.Token).Wait();
    //    }

    //    private void LogAuditSuccess(UpdateAccountRubriqueCommand request)
    //    {
    //        string successMessage = "AccountType update successfully.";
    //        APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateAccountRubriqueCommand",
    //            JsonConvert.SerializeObject(request), successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token).Wait();
    //    }
    //}
}