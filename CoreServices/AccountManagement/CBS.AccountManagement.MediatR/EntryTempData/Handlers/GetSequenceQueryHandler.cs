using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;

using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all AccountingEntryRecordSequence based on the GetAllAccountingEntryRecordSequenceQuery.
    /// </summary>
    //public class GetSequenceQueryHandler : IRequestHandler<GetSequenceQuery, ServiceResponse<string>>
    //{

    //    private readonly IAccountingEntryRecordSequenceRepository _accountingEntryRecordSequenceRepository; // Repository for accessing AccountingEntryRecordSequence data.
    //    private readonly IMapper _mapper; // AutoMapper for object mapping.
    //    private readonly ILogger<GetSequenceQueryHandler> _logger; // Logger for logging handler actions and errors.
    //    private readonly IUnitOfWork<POSContext> _uow;
    //    private readonly UserInfoToken _userInfoToken;
    //    /// <summary>
    //    /// Constructor for initializing the AddAccountingEntryRecordSequenceCommandHandler.
    //    /// </summary>
    //    /// <param name="AccountingEntryRecordSequenceRepository">Repository for AccountingEntryRecordSequence data access.</param>
    //    /// <param name="mapper">AutoMapper for object mapping.</param>
    //    /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
    //    /// <param name="logger">Logger for logging handler actions and errors.</param>
    //    public GetSequenceQueryHandler(
    //        IAccountingEntryRecordSequenceRepository AccountingEntryRecordSequenceRepository,
    //        IMapper mapper,
    //        ILogger<GetSequenceQueryHandler> logger,
    //        IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
    //    {
    //        _accountingEntryRecordSequenceRepository = AccountingEntryRecordSequenceRepository;
    //        _mapper = mapper;
    //        _logger = logger;
    //        _uow = uow;
    //        _userInfoToken = userInfoToken;
    //    }
    //    /// <summary>
    //    /// Handles the GetAllPostedEntriesQuery to retrieve all PostedEntry.
    //    /// </summary>
    //    /// <param name="request">The GetAllPostedEntriesQuery containing query parameters.</param>
    //    /// <param name="cancellationToken">A cancellation token.</param>
    //    public async Task<ServiceResponse<string>> Handle(GetSequenceQuery request, CancellationToken cancellationToken)
    //    {
    //        try
    //        {
               
    //            var entities = await _accountingEntryRecordSequenceRepository.GetNextSequenceValueAsync("AccountingEntryReference", _userInfoToken.BranchCodeX);
    //            string errorMessage = $"Retrieve accounting entry record next sequence entities from the repository successfully with refereceId:{_userInfoToken.BranchCodeX}";
    //            await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllPostedEntriesQuery",
    //             request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
    //            return ServiceResponse<string>.ReturnResultWith200(entities);
    //        }
    //        catch (Exception e)
    //        {
    //            var errorMessage = $"Failed to get all PostedEntries:{e.Message}";
    //            // Log error and return a 500 Internal Server Error response with error message
    //            _logger.LogError($"Failed to get all PostedEntries: {e.Message}");
    //            await APICallHelper.AuditLogger(_userInfoToken.Email, "GetSequenceQuery",
    //            request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
    //            return ServiceResponse<string>.Return500(e, "Failed to get all PostedEntries");
    //        }
    //    }

      
    //}
}