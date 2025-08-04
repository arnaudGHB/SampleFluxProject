using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    ///// <summary>
    ///// Handles the retrieval of all AccountRubriques based on the GetAllAccountRubriqueQuery.
    ///// </summary>
    //public class GetAllAccountRubriqueQueryHandler : IRequestHandler<GetAllAccountRubriqueQuery, ServiceResponse<List<AccountRubricResponseDto>>>
    //{
    //    private readonly IAccountRubriqueRepository _AccountRubriqueRepository; // Repository for accessing AccountRubriques data.
    //    private readonly IMapper _mapper; // AutoMapper for object mapping.
    //    private readonly ILogger<GetAllAccountRubriqueQueryHandler> _logger; // Logger for logging handler actions and errors.
    //    private readonly UserInfoToken _userInfoToken;

    //    /// <summary>
    //    /// Constructor for initializing the GetAllAccountRubriqueQueryHandler.
    //    /// </summary>
    //    /// <param name="AccountRubriqueRepository">Repository for AccountRubriques data access.</param>
    //    /// <param name="mapper">AutoMapper for object mapping.</param>
    //    /// <param name="logger">Logger for logging handler actions and errors.</param>
    //    public GetAllAccountRubriqueQueryHandler(
    //        IAccountRubriqueRepository AccountRubriqueRepository,
    //        IMapper mapper, ILogger<GetAllAccountRubriqueQueryHandler> logger,UserInfoToken userInfoToken)
    //    {
    //        // Assign provided dependencies to local variables.
    //        _AccountRubriqueRepository = AccountRubriqueRepository;
    //        _mapper = mapper;
    //        _logger = logger;
    //        _userInfoToken = userInfoToken;
    //    }

    //    ///// <summary>
    //    ///// Handles the GetAllAccountRubriqueQuery to retrieve all AccountRubriques.
    //    ///// </summary>
    //    ///// <param name="request">The GetAllAccountRubriqueQuery containing query parameters.</param>
    //    ///// <param name="cancellationToken">A cancellation token.</param>
    //    //public async Task<ServiceResponse<List<AccountRubricResponseDto>>> Handle(GetAllAccountRubriqueQuery request, CancellationToken cancellationToken)
    //    //{
    //    //    try
    //    //    {
    //    //        // Retrieve all AccountRubriques entities from the repository
    //    //        var entities = await _AccountRubriqueRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
    //    //        var errorMessag = $" GetAllAccountRubriqueQuery executed Successfully.";
    //    //        await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllAccountRubriqueQuery",
    //    //            JsonConvert.SerializeObject(request), errorMessag, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

    //    //        return ServiceResponse<List<AccountRubricResponseDto>>.ReturnResultWith200(_mapper.Map<List<AccountRubricResponseDto>>(entities));
    //    //    }
    //    //    catch (Exception e)
    //    //    {
               
    //    //        // Log error and return 500 Internal Server Error response with error message
    //    //        var errorMessage = $"Failed to get all AccountRubriques: {e.Message}";
    //    //        _logger.LogError(errorMessage);
    //    //        await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllAccountRubriqueQuery",
    //    //            JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
 
    //    //        return ServiceResponse<List<AccountRubricResponseDto>>.Return500(e, errorMessage);
    //    //    }
    //    //}
    //}
}