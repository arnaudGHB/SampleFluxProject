using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Account.Queries;
 
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.BankingOperation.Handlers
{
    public class CheckIfReferenceIdExistQueryHandler : IRequestHandler<CheckIfReferenceIdExistQuery, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<CheckIfReferenceIdExistQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;


        /// <summary>
        /// Constructor for initializing the GetAccountByReferenceQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public CheckIfReferenceIdExistQueryHandler(
            IAccountingEntryRepository accountingEntryRepository,
            IMapper mapper,
            ILogger<CheckIfReferenceIdExistQueryHandler> logger, UserInfoToken userInfoToken)
        {
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAccountQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(CheckIfReferenceIdExistQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Account entity with the specified ID from the repository
                var entitiesAccounts = _accountingEntryRepository.FindBy(x=>x.ReferenceID.Equals(request.ReferenceId)&&x.IsDeleted.Equals(false));
                if (entitiesAccounts.Any())
                {
                    errorMessage = $"No Accounting entry was found with ReferenceId: {request.ReferenceId}";

                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.CheckIfReferenceIdExistQuery, LogLevelInfo.Warning);



                    return ServiceResponse<bool>.ReturnResultWith200(false);
                }
                else
                {
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.CheckIfReferenceIdExistQuery, LogLevelInfo.Information);


                    return ServiceResponse<bool>.ReturnResultWith200(true) ;
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "CheckIfReferenceIdExistQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}
